using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SUADATOS;
using SUAMVC.Helpers;
using DocumentFormat.OpenXml.Spreadsheet;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using CrystalDecisions.Web;

namespace SUAMVC.Controllers
{
    public class PanelSolicitudController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: PanelSolicitud
        public ActionResult Index(string clientesId, String folioId)
        {

            ToolsHelper cp = new ToolsHelper();
            Concepto concepto = cp.obtenerConceptoPorGrupo("ESTASOL", "Apertura");
            Usuario usuario = Session["UsuarioData"] as Usuario;

            //Buscamos las solicitudes que puede ver ese usuario
            //de acuerdo a sus clientes permitidos
            var solicituds = (from s in db.Solicituds
                              join top in db.TopicosUsuarios on s.clienteId equals top.topicoId
                              where top.tipo.Trim().Equals("C") && top.usuarioId.Equals(usuario.Id)
                              orderby s.fechaSolicitud
                              select s).ToList();


            if (!String.IsNullOrEmpty(clientesId))
            {
                int clienteId = int.Parse(clientesId);
                Cliente cliente = db.Clientes.Find(clienteId);
                if (!cliente.descripcion.ToLower().Contains("seleccion"))
                {
                    solicituds = solicituds.Where(s => s.clienteId.Equals(clienteId)).ToList();
                }
            }// Se va a filtrar por cliente ?
            if (!String.IsNullOrEmpty(folioId))
            {
                solicituds = solicituds.Where(s => s.folioSolicitud.Contains(folioId)).ToList();
            }//Se va a filtrar por folio?
            solicituds = solicituds.Where(s => !s.estatusSolicitud.Equals(concepto.id)).ToList();
            
            return View(solicituds.ToList());
        }


        [HttpGet]
        public void crearExcelAfiliacion(int solicitudId)
        {
            FileStream fileStream = null;
            MemoryStream mem = new MemoryStream();
            try
            {

                List<Empleado> empleadosList = new List<Empleado>();

                empleadosList = (from s in db.SolicitudEmpleadoes
                                 where s.estatus.Equals("A")
                                   && s.solicitudId.Equals(solicitudId)
                                 orderby s.id
                                 select s.Empleado).ToList();

                DateTime date = DateTime.Now;
                String path = @"C:\\SUA\\Exceles\\";
                String fileName = @"Afiliacion-" + date.ToString("ddMMyyyyHHmm") + ".xlsx";
                String fullName = path + fileName;

                if (empleadosList.Count() > 0)
                {

                    ExcelHelper eh = new ExcelHelper();
                    //Creamos el objeto del workbook
                    SpreadsheetDocument xl = SpreadsheetDocument.Create(fullName, SpreadsheetDocumentType.Workbook);
                    
                    WorkbookPart wbp = xl.AddWorkbookPart();
                    WorksheetPart wsp = wbp.AddNewPart<WorksheetPart>();
                    Workbook wb = new Workbook();
                    FileVersion fv = new FileVersion();
                    fv.ApplicationName = "Microsoft Office Excel";

                    Worksheet ws = new Worksheet();
                    WorkbookStylesPart wbsp = wbp.AddNewPart<WorkbookStylesPart>();
                    // add styles to sheet
                    wbsp.Stylesheet = eh.CreateStylesheet();
                    wbsp.Stylesheet.Save();
                    
                    SheetData sd = crearContenidoHojaAfiliacion(empleadosList, eh);
                    ws.Append(sd);
                    wsp.Worksheet = ws;
                    wsp.Worksheet.Save();

                    Sheets sheets = new Sheets();
                    Sheet sheet = new Sheet();
                    sheet.Name = "rptPanelAltaIMSS";
                    sheet.SheetId = 1;
                    sheet.Id = wbp.GetIdOfPart(wsp);

                    sheets.Append(sheet);
                    wb.Append(fv);
                    wb.Append(sheets);

                    xl.WorkbookPart.Workbook = wb;
                    xl.WorkbookPart.Workbook.Save();
                    xl.Close();

                    fileStream = new FileStream(fullName, FileMode.Open);
                    fileStream.Position = 0;
                    mem = new MemoryStream();
                    fileStream.CopyTo(mem);

                    mem.Position = 0;
                    Response.ClearContent();
                    Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
                    ToolsHelper th = new ToolsHelper();
                    Response.ContentType = th.getMimeType(fullName);
                    Response.BinaryWrite(mem.ToArray());

                    Response.End();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());

            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Flush();
                    fileStream.Close();
                }
                mem.Flush();
                mem.Close();
            }

        }


        string[] headerColumns = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O" };
        public SheetData crearContenidoHojaAfiliacion(List<Empleado> empleados, ExcelHelper eh)
        {
            
            SheetData sheetData = new SheetData();
            int index = 1;

            //Creamos el Header
            Row row = new Row();
            row = eh.addNewCellToRow(index, row, "Titulo del Excel", headerColumns[0] + index, 0U, CellValues.String);
            sheetData.AppendChild(row);

            index = index + 1;
            row = eh.addNewCellToRow(index, row, "ALTA DE PERSONAL IMSS", headerColumns[0] + index, 0U, CellValues.String);
            sheetData.AppendChild(row);

            index = index + 2;
            row = eh.addNewCellToRow(index, row, "Folio:", headerColumns[0] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            index = index + 2;
            row = eh.addNewCellToRow(index, row, "Número de Seguro Social", headerColumns[0] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "RFC", headerColumns[1] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "CURP", headerColumns[2] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Primer Apellido", headerColumns[3] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            index++;
            //Creamos las celdas que contienen los datos
            foreach (Empleado dp in empleados)
            {
                int i = 0;

                row = eh.addNewCellToRow(index, row, dp.nss, headerColumns[i] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.rfc, headerColumns[i + 1] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.curp, headerColumns[i + 2] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.apellidoMaterno, headerColumns[i + 3] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                index++;
            }

            return sheetData;
        }

        public void launchReport() {

            ConnectionInfo oConn = new ConnectionInfo();
            List<Empleado> empleados = new List<Empleado>();
            empleados = db.Empleados.ToList();
            CrystalReports cr = new CrystalReports("test.rpt");
            ReportDocument rp = cr.launchReport();

            String serverName = "Driver={SQL Server Native Client 10.0};Server=MXQRMN-PC025WWD\\SQLEXPRESS";
            rp.SetDatabaseLogon("root", "jeargaqu", serverName, "sua", false);
            rp.VerifyDatabase();
            rp.Refresh();

            CrystalReportViewer crystalReportViewer = new CrystalReportViewer();
            crystalReportViewer.ReportSource = rp;
            crystalReportViewer.DisplayToolbar = true;

            //Response.Buffer = false;
            //Response.ClearContent();
            //Response.ClearHeaders();

            //try
            //{
            //    Stream stream = rp.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            //    return File(stream, "application/rpt", "miReporte.pdf");
            //}
            //catch (Exception e) {
            //    throw;
            //}
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
