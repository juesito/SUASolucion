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
using SUAMVC.Models;

namespace SUAMVC.Controllers
{
    public class PanelSolicitudController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: PanelSolicitud
        public ActionResult Index(String tipoId)
        {

            ToolsHelper cp = new ToolsHelper();
            Concepto concepto = cp.obtenerConceptoPorGrupo("ESTASOL", "Apertura");
            Concepto concepto2 = cp.obtenerConceptoPorGrupo("ESTASOL", "Cerrado");
            Usuario usuario = Session["UsuarioData"] as Usuario;
            SecurityUserModel.llenarPermisos(usuario.roleId);

            //Buscamos las solicitudes que puede ver ese usuario
            //de acuerdo a sus clientes permitidos
            var solicituds = (from s in db.Solicituds
                              join top in db.TopicosUsuarios on s.clienteId equals top.topicoId
                              where top.tipo.Trim().Equals("C") && top.usuarioId.Equals(usuario.Id)
                              orderby s.fechaEnvio
                              select s).ToList();


            if (!String.IsNullOrEmpty(tipoId))
            {
                ViewBag.tipoId = tipoId;
                int tipo = int.Parse(tipoId);
                Concepto conceptoTipo = db.Conceptos.Find(tipo);
                ViewBag.tipo = conceptoTipo.descripcion.Trim().ToLower();
                solicituds = solicituds.Where(s => s.tipoSolicitud.Equals(int.Parse(tipoId))).ToList();

            }

            solicituds = solicituds.Where(s => !s.estatusSolicitud.Equals(concepto.id)).ToList();
            solicituds = solicituds.Where(s => !s.estatusSolicitud.Equals(concepto2.id)).ToList();

            return View(solicituds.ToList());
        }

        public ActionResult IndexCerradas(String tipoId)
        {

            ToolsHelper cp = new ToolsHelper();
            Concepto concepto = cp.obtenerConceptoPorGrupo("ESTASOL", "Apertura");
            Concepto concepto2 = cp.obtenerConceptoPorGrupo("ESTASOL", "Cerrado");
            Usuario usuario = Session["UsuarioData"] as Usuario;
            SecurityUserModel.llenarPermisos(usuario.roleId);

            //Buscamos las solicitudes que puede ver ese usuario
            //de acuerdo a sus clientes permitidos
            var solicituds = (from s in db.Solicituds
                              join top in db.TopicosUsuarios on s.clienteId equals top.topicoId
                              where top.tipo.Trim().Equals("C") && top.usuarioId.Equals(usuario.Id)
                              orderby s.fechaEnvio
                              select s).ToList();


            if (!String.IsNullOrEmpty(tipoId))
            {
                ViewBag.tipoId = tipoId;
                int tipo = int.Parse(tipoId);
                Concepto conceptoTipo = db.Conceptos.Find(tipo);
                ViewBag.tipo = conceptoTipo.descripcion.Trim().ToLower();
                solicituds = solicituds.Where(s => s.tipoSolicitud.Equals(int.Parse(tipoId))).ToList();

            }

            solicituds = solicituds.Where(s => !s.estatusSolicitud.Equals(concepto.id)).ToList();
            solicituds = solicituds.Where(s => s.estatusSolicitud.Equals(concepto2.id)).ToList();

            return View(solicituds.ToList());
        }
        //Lay Out Afiliacion
        [HttpGet]
        public void crearExcelAfiliacion(int solicitudId)
        {
            FileStream fileStream = null;
            MemoryStream mem = new MemoryStream();
            try
            {
                Solicitud sol = db.Solicituds.Find(solicitudId);

                List<Empleado> empleadosList = new List<Empleado>();

                empleadosList = (from s in db.SolicitudEmpleadoes
                                 //                                 where s.estatus.Equals("A")
                                 where s.solicitudId.Equals(solicitudId)
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

                    SheetData sd = crearContenidoHojaAfiliacion(empleadosList, eh, sol);
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


        string[] headerColumns = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH", "AI", "AJ", "AK", "AL", "AM", "AN", "AO", "AP", "AQ", "AR", "AS", "AT", "AU", "AV", "AW", "AX", "AY", "AZ" };
        public SheetData crearContenidoHojaAfiliacion(List<Empleado> empleados, ExcelHelper eh, Solicitud sol)
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

            row = eh.addNewCellToRow(index, row, sol.folioSolicitud, headerColumns[1] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            index = index + 2;
            row = eh.addNewCellToRow(index, row, "#", headerColumns[0] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Número de Seguro Social", headerColumns[1] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "RFC", headerColumns[2] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "CURP", headerColumns[3] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Primer Apellido", headerColumns[4] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Segundo Apellido", headerColumns[5] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Nombre", headerColumns[6] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Fecha Alta", headerColumns[7] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Salario Diario Integrado", headerColumns[8] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Tipo de Trabajador", headerColumns[9] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Tipo de Salario", headerColumns[10] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Tipo de Jornada", headerColumns[11] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Unidad Médica Familiar", headerColumns[12] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);


            row = eh.addNewCellToRow(index, row, "Clave del Trabajador", headerColumns[13] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);


            row = eh.addNewCellToRow(index, row, "Credito Infonavit", headerColumns[14] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);


            row = eh.addNewCellToRow(index, row, "Código Postal", headerColumns[15] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);


            row = eh.addNewCellToRow(index, row, "Observaciones", headerColumns[16] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);


            row = eh.addNewCellToRow(index, row, "Movimiento", headerColumns[17] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);


            row = eh.addNewCellToRow(index, row, "Fecha SUA", headerColumns[18] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);


            row = eh.addNewCellToRow(index, row, "Registro Patronal", headerColumns[19] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            //Creamos las celdas que contienen los datos
            int consecutivo = 0;
            foreach (Empleado dp in empleados)
            {
                int i = 0;
                index = index + 1;
                consecutivo = consecutivo + 1;

                row = eh.addNewCellToRow(index, row, consecutivo.ToString(), headerColumns[i] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                if (dp.nss != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.nss, headerColumns[i + 1] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 1] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.rfc != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.rfc, headerColumns[i + 2] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 2] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.curp != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.curp, headerColumns[i + 3] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 3] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.apellidoMaterno != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.apellidoMaterno, headerColumns[i + 4] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 4] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                row = eh.addNewCellToRow(index, row, dp.apellidoPaterno, headerColumns[i + 5] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.nombre, headerColumns[i + 6] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                if (dp.fechaAltaImss != null)
                {
                    DateTime fechaAltaImss = (DateTime)dp.fechaAltaImss;
                    row = eh.addNewCellToRow(index, row, fechaAltaImss.ToShortDateString(), headerColumns[i + 7] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 7] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.SDI != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.SDI.descripcion, headerColumns[i + 8] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 8] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                //row = eh.addNewCellToRow(index, row, dp.categoria, headerColumns[i + 8] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, "1", headerColumns[i + 9] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                if (dp.EsquemasPago != null)
                {
                    //row = eh.addNewCellToRow(index, row, dp.EsquemasPago.descripcion, headerColumns[i + 9] + index, 3U, CellValues.String);
                    row = eh.addNewCellToRow(index, row, "0", headerColumns[i + 10] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, "0", headerColumns[i + 10] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.Asegurado != null)
                {
                    row = eh.addNewCellToRow(index, row, "0", headerColumns[i + 11] + index, 3U, CellValues.String);
                    //row = eh.addNewCellToRow(index, row, dp.Asegurado.tipoTrabajo, headerColumns[i + 10] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, "0", headerColumns[i + 11] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.Asegurado != null)
                {

                    if (!String.IsNullOrEmpty(dp.Asegurado.Patrone.unidadMedica))
                    {
                        row = eh.addNewCellToRow(index, row, dp.Asegurado.Patrone.unidadMedica.ToString(), headerColumns[i + 12] + index, 3U, CellValues.String);
                        sheetData.AppendChild(row);
                    }
                    else
                    {
                        row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 12] + index, 3U, CellValues.String);
                        sheetData.AppendChild(row);
                    }
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 12] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.Asegurado != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.Asegurado.Cliente.claveSua, headerColumns[i + 13] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 13] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                if (dp.creditoInfonavit != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.creditoInfonavit, headerColumns[i + 14] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 14] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.codigoPostal != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.codigoPostal, headerColumns[i + 15] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 15] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.observaciones != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.observaciones, headerColumns[i + 16] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 16] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                row = eh.addNewCellToRow(index, row, dp.estatus, headerColumns[i + 17] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                if (dp.fechaCreacion != null)
                {
                    DateTime fechaCreacion = (DateTime)dp.fechaCreacion;
                    row = eh.addNewCellToRow(index, row, fechaCreacion.ToShortDateString(), headerColumns[i + 18] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 18] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.Asegurado != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.Asegurado.Patrone.registro, headerColumns[i + 19] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 19] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                //                index++;
            }

            return sheetData;
        }

        //Lay out Nomina
        [HttpGet]
        public void crearExcelNomina(int solicitudId)
        {
            FileStream fileStream = null;
            MemoryStream mem = new MemoryStream();
            try
            {

                Solicitud sol = db.Solicituds.Find(solicitudId);

                List<Empleado> empleadosList = new List<Empleado>();

                empleadosList = (from s in db.SolicitudEmpleadoes
                                 //                                 where s.estatus.Equals("A")
                                 where s.solicitudId.Equals(solicitudId)
                                 orderby s.id
                                 select s.Empleado).ToList();

                DateTime date = DateTime.Now;
                String path = @"C:\\SUA\\Exceles\\";
                String fileName = @"Nomina-" + date.ToString("ddMMyyyyHHmm") + ".xlsx";
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

                    SheetData sd = crearContenidoHojaNomina(empleadosList, eh, sol);
                    ws.Append(sd);
                    wsp.Worksheet = ws;
                    wsp.Worksheet.Save();

                    Sheets sheets = new Sheets();
                    Sheet sheet = new Sheet();
                    sheet.Name = "rptPanelAltaNomina";
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

        public SheetData crearContenidoHojaNomina(List<Empleado> empleados, ExcelHelper eh, Solicitud sol)
        {

            SheetData sheetData = new SheetData();
            int index = 1;

            //Creamos el Header
            Row row = new Row();
            row = eh.addNewCellToRow(index, row, "Titulo del Excel", headerColumns[0] + index, 0U, CellValues.String);
            sheetData.AppendChild(row);

            index = index + 1;
            row = eh.addNewCellToRow(index, row, "ALTA DE PERSONAL NÓMINA", headerColumns[0] + index, 0U, CellValues.String);
            sheetData.AppendChild(row);

            index = index + 2;
            row = eh.addNewCellToRow(index, row, "Folio:", headerColumns[0] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, sol.folioSolicitud, headerColumns[1] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            index = index + 2;
            row = eh.addNewCellToRow(index, row, "#", headerColumns[0] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "APELLIDO PATERNO", headerColumns[1] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "APELLIDO MATERNO", headerColumns[2] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "NOMBRE", headerColumns[3] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "IMSS", headerColumns[4] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "CATEGORÍA", headerColumns[5] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "SEXO", headerColumns[6] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "FECHA INGRESO", headerColumns[7] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "RFC", headerColumns[8] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "HOMOCLAVE", headerColumns[9] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "CURP", headerColumns[10] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "FECHA NACIMIENTO", headerColumns[11] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "ESTADO NACIMIENTO", headerColumns[12] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);


            row = eh.addNewCellToRow(index, row, "MUNICIPIO NACIMIENTO", headerColumns[13] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);


            row = eh.addNewCellToRow(index, row, "TIPO NÓMINA", headerColumns[14] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);


            row = eh.addNewCellToRow(index, row, "BANCO", headerColumns[15] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);


            row = eh.addNewCellToRow(index, row, "CUENTA", headerColumns[16] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);


            row = eh.addNewCellToRow(index, row, "CLABE INTERBANCARIA", headerColumns[17] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);


            row = eh.addNewCellToRow(index, row, "PERIODO", headerColumns[18] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);


            row = eh.addNewCellToRow(index, row, "ESTADO CIVIL", headerColumns[19] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "INFONAVIT", headerColumns[20] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "DEPARTAMENTO CLIENTE", headerColumns[21] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "CALLE NUMERO", headerColumns[22] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "COLONIA", headerColumns[23] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "ESTADO MUNICIPIO", headerColumns[24] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "CÓDIGO POSTAL", headerColumns[25] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "ACTIVIDADES", headerColumns[26] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "DOMICILIO OFICINA", headerColumns[27] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "FECHA ANTIGÜEDAD", headerColumns[28] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "SALARIO VSM", headerColumns[29] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "JORNADA LABORAL", headerColumns[30] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "DIAS DE DESCANSO", headerColumns[31] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "SALARIO NOMINAL", headerColumns[32] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "DIAS DE VACACIONES", headerColumns[33] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "PORCENTAJE PRIMA", headerColumns[34] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "DIAS AGUINALDO", headerColumns[35] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "OTROS", headerColumns[36] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "CORREO", headerColumns[37] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "FECHA SUA", headerColumns[38] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "MOVIMIENTO", headerColumns[39] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "REGISTRO PATRONAL", headerColumns[40] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "ID EMPLEADO", headerColumns[41] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "ID CLIENTE", headerColumns[42] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "CLIENTE", headerColumns[43] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);


            //Creamos las celdas que contienen los datos
            int consecutivo = 0;
            foreach (Empleado dp in empleados)
            {
                int i = 0;
                index = index + 1;
                consecutivo = consecutivo + 1;

                row = eh.addNewCellToRow(index, row, consecutivo.ToString(), headerColumns[i] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.apellidoPaterno, headerColumns[i + 1] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                if (dp.apellidoMaterno != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.apellidoMaterno, headerColumns[i + 2] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 2] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                row = eh.addNewCellToRow(index, row, dp.nombre, headerColumns[i + 3] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                if (dp.nss != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.nss, headerColumns[i + 4] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 4] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.categoria != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.categoria, headerColumns[i + 5] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 5] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.Sexo != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.Sexo.descripcion, headerColumns[i + 6] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 6] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.Asegurado != null)
                {
                    DateTime fechaAlta = (DateTime)dp.Asegurado.fechaAlta;
                    row = eh.addNewCellToRow(index, row, fechaAlta.ToShortDateString(), headerColumns[i + 7] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 7] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.rfc != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.rfc, headerColumns[i + 8] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 8] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.homoclave != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.homoclave, headerColumns[i + 9] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 9] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.curp != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.curp, headerColumns[i + 10] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 10] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.fechaNacimiento != null)
                {
                    DateTime fechaNacimiento = (DateTime)dp.fechaNacimiento;
                    row = eh.addNewCellToRow(index, row, fechaNacimiento.ToShortDateString(), headerColumns[i + 11] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 11] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.estadoNacimientoId != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.Estado.descripcion, headerColumns[i + 12] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.municipioNacimientoId != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.Municipio.descripcion, headerColumns[i + 13] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 13] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (sol.EsquemasPago != null)
                {
                    row = eh.addNewCellToRow(index, row, sol.EsquemasPago.descripcion, headerColumns[i + 14] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 14] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.Banco != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.Banco.descripcion, headerColumns[i + 15] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 15] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }


                if (dp.cuentaBancaria != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.cuentaBancaria, headerColumns[i + 16] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 16] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.cuentaClabe != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.cuentaClabe, headerColumns[i + 17] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 17] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                //row = eh.addNewCellToRow(index, row, dp.periodo., headerColumns[i + 17] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 18] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                if (dp.EstadoCivil != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.EstadoCivil.descripcion, headerColumns[i + 19] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 19] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }


                //row = eh.addNewCellToRow(index, row, dp.DetallePago.infonavit, headerColumns[i + 20] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 20] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.departamentoCliente, headerColumns[i + 21] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 21] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                if (dp.calleNumero != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.calleNumero, headerColumns[i + 22] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 22] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.colonia != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.colonia, headerColumns[i + 23] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 23] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.edoMunicipio != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.edoMunicipio, headerColumns[i + 24] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 24] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.codigoPostal != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.codigoPostal, headerColumns[i + 25] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 25] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                //row = eh.addNewCellToRow(index, row, dp.actividades, headerColumns[i + 26] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 26] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.clientes.direccionOficina, headerColumns[i + 27] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 27] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.DocumentoEmpleados.fechaAntiguedad, headerColumns[i + 28] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 28] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //if ((dp.FamiliaresEmpleadoes.Count>0)dp.FamiliaresEmpleadoes fe = dp.FamiliaresEmpleadoes.FirstOrDefault())



                //row = eh.addNewCellToRow(index, row, dp.Acreditados.vsm, headerColumns[i + 29] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 29] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);


                if (dp.Asegurado != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.Asegurado.semanaJornada, headerColumns[i + 30] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 30] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                //row = eh.addNewCellToRow(index, row, dp.diasDescanso, headerColumns[i + 31] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 31] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.Asegurado.salarioNominal, headerColumns[i + 32] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 32] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.DocumentoEmpleado.diasVacaciones, headerColumns[i + 33] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 33] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.porcentajePrima, headerColumns[i + 34] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 34] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.DocumentoEmpleado.diasAguinaldo, headerColumns[i + 35] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 35] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.otros, headerColumns[i + 36] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 36] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                if (dp.email != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.email, headerColumns[i + 37] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 37] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.Asegurado != null)
                {

                    var movTemp = (from s in db.MovimientosAseguradoes
                                  .Where(s => s.aseguradoId.Equals(dp.Asegurado.id))
                                  .OrderByDescending(s => s.fechaInicio)
                                   select s).FirstOrDefault();

                    DateTime fechaSua = (DateTime)movTemp.fechaInicio;
                    row = eh.addNewCellToRow(index, row, fechaSua.ToShortDateString(), headerColumns[i + 38] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);

                    row = eh.addNewCellToRow(index, row, movTemp.CatalogoMovimiento.descripcion, headerColumns[i + 39] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);

                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 38] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);

                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 39] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.Asegurado != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.Asegurado.Patrone.registro, headerColumns[i + 40] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 40] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                //row = eh.addNewCellToRow(index, row, dp.id, headerColumns[i + 41] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 41] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, sol.Cliente.claveSua, headerColumns[i + 42] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, sol.Cliente.descripcion, headerColumns[i + 43] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //                index++;
            }

            return sheetData;
        }

        //LayOut Juridico

        [HttpGet]
        public void crearExcelJuridico(int solicitudId)
        {
            FileStream fileStream = null;
            MemoryStream mem = new MemoryStream();
            try
            {

                Solicitud sol = db.Solicituds.Find(solicitudId);

                List<Empleado> empleadosList = new List<Empleado>();

                empleadosList = (from s in db.SolicitudEmpleadoes
                                 //                                 where s.estatus.Equals("A")
                                 where s.solicitudId.Equals(solicitudId)
                                 orderby s.id
                                 select s.Empleado).ToList();

                DateTime date = DateTime.Now;
                String path = @"C:\\SUA\\Exceles\\";
                String fileName = @"Juridico-" + date.ToString("ddMMyyyyHHmm") + ".xlsx";
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

                    SheetData sd = crearContenidoHojaJuridico(empleadosList, eh, sol);
                    ws.Append(sd);
                    wsp.Worksheet = ws;
                    wsp.Worksheet.Save();

                    Sheets sheets = new Sheets();
                    Sheet sheet = new Sheet();
                    sheet.Name = "rptPanelAltaJuridico";
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

        public SheetData crearContenidoHojaJuridico(List<Empleado> empleados, ExcelHelper eh, Solicitud sol)
        {

            SheetData sheetData = new SheetData();
            int index = 1;

            //Creamos el Header
            Row row = new Row();
            row = eh.addNewCellToRow(index, row, "Titulo del Excel Juridico", headerColumns[0] + index, 0U, CellValues.String);
            sheetData.AppendChild(row);

            index = index + 1;
            row = eh.addNewCellToRow(index, row, "PANEL JURIDICO", headerColumns[0] + index, 0U, CellValues.String);
            sheetData.AppendChild(row);

            index = index + 2;
            row = eh.addNewCellToRow(index, row, "Folio:", headerColumns[0] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, sol.folioSolicitud, headerColumns[1] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            index = index + 2;
            row = eh.addNewCellToRow(index, row, "#", headerColumns[0] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Primer Apellido", headerColumns[1] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Segundo Apellido", headerColumns[2] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Nombre", headerColumns[3] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Categoria", headerColumns[4] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Salario Real", headerColumns[5] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "RFC", headerColumns[6] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Homoclave", headerColumns[7] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "CURP", headerColumns[8] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "NSS", headerColumns[9] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Credito Infonavit", headerColumns[10] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Sexo", headerColumns[11] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Estado Civil", headerColumns[12] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);


            row = eh.addNewCellToRow(index, row, "Fecha de Nacimiento", headerColumns[13] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);


            row = eh.addNewCellToRow(index, row, "Edad", headerColumns[14] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);


            row = eh.addNewCellToRow(index, row, "País de Nacimiento", headerColumns[15] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);


            row = eh.addNewCellToRow(index, row, "Estado de Nacimiento", headerColumns[16] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);


            row = eh.addNewCellToRow(index, row, "Municipio de Nacimiento", headerColumns[17] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);


            row = eh.addNewCellToRow(index, row, "Calle Número", headerColumns[18] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);


            row = eh.addNewCellToRow(index, row, "Colonia", headerColumns[19] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "CP", headerColumns[20] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Estado Municipio", headerColumns[21] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Banco", headerColumns[22] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Cuenta Bancaria", headerColumns[23] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Clave Interbancaria", headerColumns[24] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Correo Electrónico", headerColumns[25] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Fecha Inicio Contrato", headerColumns[26] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Fecha Final Contrato", headerColumns[27] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Duración de Contrato", headerColumns[28] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Telefono Particular", headerColumns[29] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Celular", headerColumns[30] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Licencia", headerColumns[31] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Tipo de Sangre", headerColumns[32] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "En Caso de Accidente Avisar a:", headerColumns[33] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Parentesco", headerColumns[34] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Domicilio", headerColumns[35] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Teléfono", headerColumns[36] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Se Identifica con (Tipo de Documento)", headerColumns[37] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Folio Identificación", headerColumns[38] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Escolaridad", headerColumns[39] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Observaciones", headerColumns[40] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            //Creamos las celdas que contienen los datos
            int consecutivo = 0;
            foreach (Empleado dp in empleados)
            {
                int i = 0;
                index = index + 1;
                consecutivo = consecutivo + 1;

                row = eh.addNewCellToRow(index, row, consecutivo.ToString(), headerColumns[i] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.apellidoPaterno, headerColumns[i + 1] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                if (dp.apellidoMaterno != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.apellidoMaterno, headerColumns[i + 2] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 2] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                row = eh.addNewCellToRow(index, row, dp.nombre, headerColumns[i + 3] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                if (dp.categoria != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.categoria, headerColumns[i + 4] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 4] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.SDI != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.SDI.descripcion, headerColumns[i + 5] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 5] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.rfc != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.rfc, headerColumns[i + 6] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 6] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.rfc != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.homoclave, headerColumns[i + 7] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 7] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.curp != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.curp, headerColumns[i + 8] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 8] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.nss != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.nss, headerColumns[i + 9] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 9] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.creditoInfonavit != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.creditoInfonavit, headerColumns[i + 10] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 10] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.Sexo != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.Sexo.descripcion, headerColumns[i + 11] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 11] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.EstadoCivil != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.EstadoCivil.descripcion, headerColumns[i + 12] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 12] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.fechaNacimiento != null)
                {
                    DateTime fechaNacimiento = (DateTime)dp.fechaNacimiento;
                    row = eh.addNewCellToRow(index, row, fechaNacimiento.ToShortDateString(), headerColumns[i + 13] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 13] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                //row = eh.addNewCellToRow(index, row, dp.edad, headerColumns[i + 13] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 14] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);


                if (dp.Pais != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.Pais.descripcion, headerColumns[i + 15] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 15] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.Estado != null)
                {

                    row = eh.addNewCellToRow(index, row, dp.Estado.descripcion, headerColumns[i + 16] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 16] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.Municipio != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.Municipio.descripcion, headerColumns[i + 17] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 17] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.calleNumero != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.calleNumero, headerColumns[i + 18] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 18] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.calleNumero != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.colonia, headerColumns[i + 19] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 19] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.codigoPostal != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.codigoPostal, headerColumns[i + 20] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 20] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.edoMunicipio != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.edoMunicipio, headerColumns[i + 21] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 20] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                row = eh.addNewCellToRow(index, row, dp.Banco.descripcion, headerColumns[i + 22] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                if (dp.cuentaBancaria != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.cuentaBancaria, headerColumns[i + 23] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 23] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.cuentaClabe != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.cuentaClabe, headerColumns[i + 24] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 24] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.email != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.email, headerColumns[i + 25] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 25] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                //row = eh.addNewCellToRow(index, row, dp.fechaInicio, headerColumns[i + 25] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 26] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.Solicitud.fechaTerminoContrato, headerColumns[i + 26] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 27] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.duracionContrato, headerColumns[i + 27] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 28] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);


                //row = eh.addNewCellToRow(index, row, dp.DocumentoEmpleadoes.telefono, headerColumns[i + 28] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 29] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.DocumentoEmpleadoes.celular, headerColumns[i + 29] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 30] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);


                //row = eh.addNewCellToRow(index, row, dp.DocumentoEmpleadoes.licencia, headerColumns[i + 30] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 31] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.DocumentoEmpleadoes.tipoSangre, headerColumns[i + 31] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 32] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.avisarA, headerColumns[i + 32] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 33] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);


                //row = eh.addNewCellToRow(index, row, dp.Parentesco.descripcion, headerColumns[i + 33] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 34] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.Parentesco.domicilio, headerColumns[i + 34] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 35] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.FamiliaresEmpleadoes.telefonoCasa, headerColumns[i + 35] + index, 3U, CellValues.String);
                if (dp.FamiliaresEmpleadoes.Count() > 0)
                {
                    FamiliaresEmpleado fe = dp.FamiliaresEmpleadoes.FirstOrDefault();
                    row = eh.addNewCellToRow(index, row, fe.telefonoCasa, headerColumns[i + 36] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 36] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }


                if (dp.DocumentoEmpleadoes.Count() > 0)
                {

                }
                else
                    //row = eh.addNewCellToRow(index, row, dp.DocumentoEmpleadoes.tipoDocumento, headerColumns[i + 36] + index, 3U, CellValues.String);
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 37] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.DocumentoEmpleadoes.folioIdentificacion, headerColumns[i + 37] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 38] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.escolaridad, headerColumns[i + 38] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 39] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                if (dp.observaciones != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.observaciones, headerColumns[i + 40] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 40] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                //                index++;
            }

            return sheetData;
        }





        // LayOut TarjetasSantander
        [HttpGet]
        public ActionResult crearExcelTarjetaSantander(int solicitudId, String banco, String clienteId, String folioId, String proyectoId, String tipoId)
        {

            Banco bank = db.Bancos.Where(b => b.descripcion.ToLower().Trim().Contains(banco.ToLower().Trim())).FirstOrDefault();

            if (bank != null)
            {
                List<Empleado> empleadosList = new List<Empleado>();

                empleadosList = (from s in db.SolicitudEmpleadoes
                                 //                                     where s.estatus.Equals("A")
                                 where s.Empleado.bancoId.Equals(bank.id)
                                   && s.solicitudId.Equals(solicitudId)
                                 orderby s.id
                                 select s.Empleado).ToList();

                if (empleadosList.Count() > 0)
                {

                    FileStream fileStream = null;
                    MemoryStream mem = new MemoryStream();
                    try
                    {
                        DateTime date = DateTime.Now;
                        String path = @"C:\\SUA\\Exceles\\";
                        String fileName = @"TarjetaSantander-" + date.ToString("ddMMyyyyHHmm") + ".xlsx";
                        String fullName = path + fileName;


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

                        SheetData sd = crearContenidoHojaTarjetaSantander(empleadosList, eh);
                        ws.Append(sd);
                        wsp.Worksheet = ws;
                        wsp.Worksheet.Save();

                        Sheets sheets = new Sheets();
                        Sheet sheet = new Sheet();
                        sheet.Name = "rptPanelAltaTarjetaSantander";
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
                else
                {
                    TempData["error"] = true;
                    TempData["viewMessage"] = "No existen registros para el Banco Santander...";
                }
            }
            else
            {
                TempData["error"] = true;
                TempData["viewMessage"] = "No existen registros para el Banco Santander...";
            }
            return RedirectToAction("Index", new { clienteId, folioId, proyectoId, tipoId });
        }

        public SheetData crearContenidoHojaTarjetaSantander(List<Empleado> empleados, ExcelHelper eh)
        {

            SheetData sheetData = new SheetData();
            int index = 1;

            //Creamos el Header
            Row row = new Row();
            row = eh.addNewCellToRow(index, row, "Titulo del Excel", headerColumns[0] + index, 0U, CellValues.String);
            sheetData.AppendChild(row);

            index = index + 1;
            row = eh.addNewCellToRow(index, row, "ALTA DE PERSONAL TARJETA SANTANDER", headerColumns[0] + index, 0U, CellValues.String);
            sheetData.AppendChild(row);

            index = index + 2;
            row = eh.addNewCellToRow(index, row, "Folio:", headerColumns[0] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            index = index + 2;
            row = eh.addNewCellToRow(index, row, "NUMERO EMPLEADO", headerColumns[0] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "NÚMERO DEPARTAMENTO", headerColumns[1] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "APELLIDO PATERNO", headerColumns[2] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "APELLIDO MATERNO", headerColumns[3] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "NOMBRE", headerColumns[4] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "RFC", headerColumns[5] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "HOMO", headerColumns[6] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "SEXO", headerColumns[7] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "CLAVE DE NACIONALIDAD", headerColumns[8] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "EDO. CIVIL", headerColumns[9] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "NOMBRE CORTO", headerColumns[10] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "CALLE Y NUMERO", headerColumns[11] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);


            row = eh.addNewCellToRow(index, row, "COLONIA", headerColumns[12] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);


            row = eh.addNewCellToRow(index, row, "DELEGACION O MUNICIPIO", headerColumns[13] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);


            row = eh.addNewCellToRow(index, row, "CUIDAD O POBLACION", headerColumns[14] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);


            row = eh.addNewCellToRow(index, row, "CODIGO POSTAL", headerColumns[15] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);


            row = eh.addNewCellToRow(index, row, "CLAVE DEL PAIS", headerColumns[16] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);


            row = eh.addNewCellToRow(index, row, "ESTATUS DE LA CASA", headerColumns[17] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);


            row = eh.addNewCellToRow(index, row, "FECHA DE RESIDENCIA", headerColumns[18] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "PREFIJO TEL", headerColumns[19] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "TEL CASA", headerColumns[20] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "FECHA DE INGRESO A LA EMPRESA", headerColumns[21] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "SUCURSAL", headerColumns[22] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "CLAVE DE ENVIO", headerColumns[23] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "DOMICILIO DE OFICINA", headerColumns[24] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "COLONIA", headerColumns[25] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "DELEGACION O MUNICIPIO", headerColumns[26] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "CIUDAD O POBLACIÓN", headerColumns[27] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "CODIGO POSTAL", headerColumns[28] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "PAIS DE OFICINA", headerColumns[29] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "PREFIJO TEL OFICINA", headerColumns[30] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "TEL OFICINA", headerColumns[31] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "EXT OFICINA", headerColumns[32] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "INGRESO MENSUAL BRUTO", headerColumns[33] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);


            //Creamos las celdas que contienen los datos
            foreach (Empleado dp in empleados)
            {
                int i = 0;
                index = index + 1;

                //row = eh.addNewCellToRow(index, row, dp.numeroEmpleado, headerColumns[i] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.numeroDepartamento, headerColumns[i + 1] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 1] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.apellidoPaterno, headerColumns[i + 2] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.apellidoMaterno, headerColumns[i + 3] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.nombre, headerColumns[i + 4] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.rfc, headerColumns[i + 5] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.homoclave, headerColumns[i + 6] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                if (dp.Sexo != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.Sexo.descripcion, headerColumns[i + 7] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 7] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                //row = eh.addNewCellToRow(index, row, dp.nacionalidadId, headerColumns[i + 8] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 8] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                if (dp.EstadoCivil != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.EstadoCivil.descripcion, headerColumns[i + 9] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);

                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 9] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                row = eh.addNewCellToRow(index, row, dp.nombre, headerColumns[i + 10] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.calleNumero, headerColumns[i + 11] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.colonia, headerColumns[i + 12] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                if (dp.Municipio != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.Municipio.descripcion, headerColumns[i + 13] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 13] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                //row = eh.addNewCellToRow(index, row, dp.cuidad, headerColumns[i + 14] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 14] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                if (dp.codigoPostal != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.codigoPostal, headerColumns[i + 15] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 15] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.Pais != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.Pais.clavePais, headerColumns[i + 16] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 16] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                //row = eh.addNewCellToRow(index, row, dp.estatusCasa, headerColumns[i + 17] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 17] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.fechaResidencia, headerColumns[i + 18] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 18] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.prefijoTel, headerColumns[i + 19] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 19] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.telefonoCasa, headerColumns[i + 20] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 20] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                if (dp.Asegurado != null)
                {
                    DateTime fechaAlta = (DateTime)dp.Asegurado.fechaAlta;
                    row = eh.addNewCellToRow(index, row, fechaAlta.ToShortDateString(), headerColumns[i + 21] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 21] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                //row = eh.addNewCellToRow(index, row, dp.sucursal, headerColumns[i + 22] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 22] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.clveEnvio, headerColumns[i + 23] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 23] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.Cliente.domicilioOficina, headerColumns[i + 24] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 24] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.colonia, headerColumns[i + 25] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                if (dp.Municipio != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.Municipio.descripcion, headerColumns[i + 26] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 26] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                //row = eh.addNewCellToRow(index, row, dp.cuidad.descripcion, headerColumns[i + 27] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 27] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                if (dp.codigoPostal != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.codigoPostal, headerColumns[i + 28] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 28] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.Pais != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.Pais.descripcion, headerColumns[i + 29] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 29] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                //row = eh.addNewCellToRow(index, row, dp.prefijoTelOficina, headerColumns[i + 30] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 30] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.telefonoOficina, headerColumns[i + 31] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 31] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.extencionOficina, headerColumns[i + 32] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 32] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.ingresoMensualBruto, headerColumns[i + 33] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 33] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //                index++;
            }

            return sheetData;
        }

        // LayOut TarjetaBanorte
        [HttpGet]
        public ActionResult crearExcelTarjetaBanorte(int solicitudId, String banco, String clienteId, String folioId, String proyectoId, String tipoId)
        {

            Banco bank = db.Bancos.Where(b => b.descripcion.ToLower().Trim().Contains(banco.ToLower().Trim())).FirstOrDefault();

            if (bank != null)
            {
                List<Empleado> empleadosList = new List<Empleado>();

                Solicitud sol = db.Solicituds.Find(solicitudId);

                empleadosList = (from s in db.SolicitudEmpleadoes
                                 //                               where s.estatus.Equals("A")
                                 where s.Empleado.bancoId.Equals(bank.id)
                                   && s.solicitudId.Equals(solicitudId)
                                 orderby s.id
                                 select s.Empleado).ToList();

                if (empleadosList.Count() > 0)
                {

                    FileStream fileStream = null;
                    MemoryStream mem = new MemoryStream();
                    try
                    {

                        DateTime date = DateTime.Now;
                        String path = @"C:\\SUA\\Exceles\\";
                        String fileName = @"TarjetaBanorte-" + date.ToString("ddMMyyyyHHmm") + ".xlsx";
                        String fullName = path + fileName;

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

                        SheetData sd = crearContenidoHojaTarjetaBanorte(empleadosList, eh, sol);
                        ws.Append(sd);
                        wsp.Worksheet = ws;
                        wsp.Worksheet.Save();

                        Sheets sheets = new Sheets();
                        Sheet sheet = new Sheet();
                        sheet.Name = "rptPanelAltaTarjetaBanorte";
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
                else
                {
                    TempData["error"] = true;
                    TempData["viewMessage"] = "No existen registros para el Banco Banorte...";
                }
            }
            else
            {
                TempData["error"] = true;
                TempData["viewMessage"] = "No existen registros para el Banco Banorte...";
            }
            return RedirectToAction("Index", new { clienteId, folioId, proyectoId, tipoId });
        }

        public SheetData crearContenidoHojaTarjetaBanorte(List<Empleado> empleados, ExcelHelper eh, Solicitud sol)
        {

            SheetData sheetData = new SheetData();
            int index = 1;

            //Creamos el Header
            Row row = new Row();
            row = eh.addNewCellToRow(index, row, "Titulo del Excel", headerColumns[0] + index, 0U, CellValues.String);
            sheetData.AppendChild(row);

            index = index + 1;
            row = eh.addNewCellToRow(index, row, "ALTA DE PERSONAL TARJETA BANORTE", headerColumns[0] + index, 0U, CellValues.String);
            sheetData.AppendChild(row);

            index = index + 2;
            row = eh.addNewCellToRow(index, row, "Folio:", headerColumns[0] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, sol.folioSolicitud, headerColumns[1] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            index = index + 2;
            row = eh.addNewCellToRow(index, row, "#", headerColumns[0] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "NOMBRE", headerColumns[1] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "APELLIDO PATERNO", headerColumns[2] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "APELLIDO MATERNO", headerColumns[3] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "NOMBRE COMPLETO", headerColumns[4] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "EMPRESA", headerColumns[5] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "NOMINA", headerColumns[6] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "BANCO", headerColumns[7] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "FECHA DE NACIMIENTO", headerColumns[8] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "ESTADO CIVIL", headerColumns[9] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "RFC", headerColumns[10] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "CURP DEL EMPLEADO", headerColumns[11] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "# SEG. SOCIAL", headerColumns[12] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "FECHA DE ALTA IMSS", headerColumns[13] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);


            row = eh.addNewCellToRow(index, row, "FECHA DE BAJA IMSS", headerColumns[14] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "SEXO", headerColumns[15] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "NACIONALIDAD", headerColumns[16] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "PAIS DE ORIGEN", headerColumns[17] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "OCUPACION", headerColumns[18] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "FECHA INGRESO", headerColumns[19] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "CALLE", headerColumns[20] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "NUMERO EXTERIOR", headerColumns[21] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "NUMERO INTERIOR", headerColumns[22] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "COLONIA", headerColumns[23] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "CÓDIGO POSTAL", headerColumns[24] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "POBLACIÓN", headerColumns[25] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "ESTADO", headerColumns[26] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "TEL. PARTICULAR", headerColumns[27] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "TEL OFICINA", headerColumns[28] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "EXTENSION OFICINA", headerColumns[29] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "NÚMERO CELULAR", headerColumns[30] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "EMAIL", headerColumns[31] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "NOMBRE", headerColumns[32] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "APELLIDO PATERNO", headerColumns[33] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "APELLIDO MATERNO", headerColumns[34] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "PARENTESCO", headerColumns[35] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "SEXO", headerColumns[36] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "FECHA DE NACIMIENTO", headerColumns[37] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);


            //Creamos las celdas que contienen los datos
            int consecutivo = 0;
            foreach (Empleado dp in empleados)
            {
                int i = 0;
                index = index + 1;
                consecutivo = consecutivo + 1;

                row = eh.addNewCellToRow(index, row, consecutivo.ToString(), headerColumns[i] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.nombre, headerColumns[i + 1] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.apellidoPaterno, headerColumns[i + 2] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.apellidoMaterno, headerColumns[i + 3] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.nombreCompleto, headerColumns[i + 4] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.empresa, headerColumns[i + 5] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 5] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.nomina, headerColumns[i + 6] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 6] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                if (dp.Banco != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.Banco.descripcion, headerColumns[i + 7] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 7] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.fechaNacimiento != null)
                {
                    DateTime fechaNacimiento = (DateTime)dp.fechaNacimiento;
                    row = eh.addNewCellToRow(index, row, fechaNacimiento.ToShortDateString(), headerColumns[i + 8] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 8] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.EstadoCivil != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.EstadoCivil.descripcion, headerColumns[i + 9] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 9] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                row = eh.addNewCellToRow(index, row, dp.rfc, headerColumns[i + 10] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                if (dp.curp != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.curp, headerColumns[i + 11] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 11] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.nss != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.nss, headerColumns[i + 12] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 12] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.fechaAltaImss != null)
                {
                    DateTime fechaAltaImss = (DateTime)dp.fechaAltaImss;
                    row = eh.addNewCellToRow(index, row, fechaAltaImss.ToShortDateString(), headerColumns[i + 13] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 13] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.fechaBaja != null)
                {
                    DateTime fechaBaja = (DateTime)dp.fechaBaja;
                    row = eh.addNewCellToRow(index, row, fechaBaja.ToShortDateString(), headerColumns[i + 14] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 14] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.Sexo != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.Sexo.descripcion, headerColumns[i + 15] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 15] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.Pais != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.Pais.naturalez, headerColumns[i + 16] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 16] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.Pais != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.Pais.descripcion, headerColumns[i + 17] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);

                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 17] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                //row = eh.addNewCellToRow(index, row, dp.ocupacion, headerColumns[i + 18] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 18] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                if (dp.fechaCreacion != null)
                {
                    DateTime fechaCreacion = (DateTime)dp.fechaCreacion;
                    row = eh.addNewCellToRow(index, row, fechaCreacion.ToShortDateString(), headerColumns[i + 19] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 19] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                row = eh.addNewCellToRow(index, row, dp.calleNumero, headerColumns[i + 20] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.numeroExterior, headerColumns[i + 21] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 21] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.numeroInterior, headerColumns[i + 22] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 22] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.colonia, headerColumns[i + 23] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                if (dp.codigoPostal != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.codigoPostal, headerColumns[i + 24] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 24] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.Municipio != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.Municipio.descripcion, headerColumns[i + 25] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 25] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.Estado != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.Estado.descripcion, headerColumns[i + 26] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 26] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                //row = eh.addNewCellToRow(index, row, dp.telefonoParticular, headerColumns[i + 27] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 27] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.telefonoOficina, headerColumns[i + 28] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 28] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.extOficina, headerColumns[i + 29] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 29] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.numeroCelular, headerColumns[i + 30] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 30] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.email, headerColumns[i + 31] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.nombre, headerColumns[i + 32] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.apellidoPaterno, headerColumns[i + 33] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                if (dp.apellidoMaterno != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.apellidoMaterno, headerColumns[i + 34] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 36] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                row = eh.addNewCellToRow(index, row, "HERMANO ", headerColumns[i + 35] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                if (dp.Sexo != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.Sexo.descripcion, headerColumns[i + 36] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 36] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.fechaNacimiento != null)
                {
                    DateTime fechaNac = (DateTime)dp.fechaNacimiento;
                    row = eh.addNewCellToRow(index, row, fechaNac.ToShortDateString(), headerColumns[i + 37] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 37] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

            }

            return sheetData;
        }

        // LayOut TarjetaBancomer
        [HttpGet]
        public ActionResult crearExcelTarjetaBancomer(int solicitudId, String banco, String clienteId, String folioId, String proyectoId, String tipoId)
        {

            Banco bank = db.Bancos.Where(b => b.descripcion.ToLower().Trim().Contains(banco.ToLower().Trim())).FirstOrDefault();

            if (bank != null)
            {
                List<Empleado> empleadosList = new List<Empleado>();

                empleadosList = (from s in db.SolicitudEmpleadoes
                                 //                               where s.estatus.Equals("A")
                                 where s.Empleado.bancoId.Equals(bank.id)
                                   && s.solicitudId.Equals(solicitudId)
                                 orderby s.id
                                 select s.Empleado).ToList();

                if (empleadosList.Count() > 0)
                {

                    FileStream fileStream = null;
                    MemoryStream mem = new MemoryStream();
                    try
                    {

                        DateTime date = DateTime.Now;
                        String path = @"C:\\SUA\\Exceles\\";
                        String fileName = @"TarjetaBancomer-" + date.ToString("ddMMyyyyHHmm") + ".xlsx";
                        String fullName = path + fileName;

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

                        SheetData sd = crearContenidoHojaTarjetaBancomer(empleadosList, eh);
                        ws.Append(sd);
                        wsp.Worksheet = ws;
                        wsp.Worksheet.Save();

                        Sheets sheets = new Sheets();
                        Sheet sheet = new Sheet();
                        sheet.Name = "rptPanelAltaTarjetaBancomer";
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
                else
                {
                    TempData["error"] = true;
                    TempData["viewMessage"] = "No existen registros para el Banco BBVA...";
                }
            }
            else
            {
                TempData["error"] = true;
                TempData["viewMessage"] = "No existen registros para el Banco BBVA...";
            }
            return RedirectToAction("Index", new { clienteId, folioId, proyectoId, tipoId });
        }

        public SheetData crearContenidoHojaTarjetaBancomer(List<Empleado> empleados, ExcelHelper eh)
        {

            SheetData sheetData = new SheetData();
            int index = 1;

            //Creamos el Header
            Row row = new Row();
            row = eh.addNewCellToRow(index, row, "Titulo del Excel", headerColumns[0] + index, 0U, CellValues.String);
            sheetData.AppendChild(row);

            index = index + 1;
            row = eh.addNewCellToRow(index, row, "ALTA DE PERSONAL TARJETA BANCOMER", headerColumns[0] + index, 0U, CellValues.String);
            sheetData.AppendChild(row);

            index = index + 2;
            row = eh.addNewCellToRow(index, row, "Folio:", headerColumns[0] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            index = index + 2;
            row = eh.addNewCellToRow(index, row, "NOMBRE", headerColumns[0] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "APELLIDO PATERNO", headerColumns[1] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "APELLIDO MATERNO", headerColumns[2] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "FECHA DE NACIMIENTO", headerColumns[3] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "ESTADO CIVIL", headerColumns[4] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "CURP", headerColumns[5] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "SEXO", headerColumns[6] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "NACIONALIDAD", headerColumns[7] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "PAÍS DE ORIGEN", headerColumns[8] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "OCUPACIÓN", headerColumns[9] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "FECHA INGRESO", headerColumns[10] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "CALLE", headerColumns[11] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "NÚMERO EXTERIOR", headerColumns[12] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "NÚMERO INTERIOR", headerColumns[13] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "COLONIA", headerColumns[14] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "CÓDIGO POSTAL", headerColumns[15] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "POBLACIÓN", headerColumns[16] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "ESTADO", headerColumns[17] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "TEL. PARTICULAR", headerColumns[18] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "TEL. OFICINA", headerColumns[19] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "EXTENCIÓN OFICINA", headerColumns[20] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "NÚMERO CELULAR", headerColumns[21] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "EMAIL", headerColumns[22] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "TIPO DE PRODUCTO", headerColumns[23] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "SUB PRODUCTO", headerColumns[24] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "TIPO DE CLIENTE", headerColumns[25] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "NÚMERO DE TARJETA", headerColumns[26] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "SUCURSAL GESTORA DE LA CUENTA", headerColumns[27] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "MONTO", headerColumns[28] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "NÚMERO DE DISPOSICIONES AL MES", headerColumns[29] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "SUCURSAL ENTREGA DE KIT", headerColumns[30] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "NOMBRE", headerColumns[31] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "APELLIDO PATERNO", headerColumns[32] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "APELLIDO MATERNO", headerColumns[33] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "PARENTESCO", headerColumns[34] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "SEXO", headerColumns[35] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "FECHA DE NACIMIENTO", headerColumns[36] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);


            //Creamos las celdas que contienen los datos
            foreach (Empleado dp in empleados)
            {
                int i = 0;
                index = index + 1;

                row = eh.addNewCellToRow(index, row, dp.nombre, headerColumns[i] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.apellidoPaterno, headerColumns[i + 1] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.apellidoMaterno, headerColumns[i + 2] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.fechaNacimiento.ToString(), headerColumns[i + 3] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                if (dp.EstadoCivil != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.EstadoCivil.descripcion, headerColumns[i + 4] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 4] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.curp != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.curp, headerColumns[i + 5] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 5] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.Sexo != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.Sexo.descripcion, headerColumns[i + 6] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 6] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.Pais != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.Pais.naturalez, headerColumns[i + 7] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 7] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.Pais != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.Pais.descripcion, headerColumns[i + 8] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);

                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 8] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }


                //row = eh.addNewCellToRow(index, row, dp.ocupacion, headerColumns[i + 9] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 9] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.fechaCreacion.ToString(), headerColumns[i + 10] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.calleNumero, headerColumns[i + 11] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.numeroExterior, headerColumns[i + 12] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 12] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.numeroInterior, headerColumns[i + 13] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 13] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.colonia, headerColumns[i + 14] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                if (dp.codigoPostal != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.codigoPostal, headerColumns[i + 15] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 15] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.Municipio != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.Municipio.descripcion, headerColumns[i + 16] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 16] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.Estado != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.Estado.descripcion, headerColumns[i + 17] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 17] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                //row = eh.addNewCellToRow(index, row, dp.telefonoParticular, headerColumns[i + 18] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 18] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.telefonoOficina, headerColumns[i + 19] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 19] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.extOficina, headerColumns[i + 20] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 20] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.numeroCelular, headerColumns[i + 21] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 21] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                if (dp.email != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.email, headerColumns[i + 22] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 22] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                //row = eh.addNewCellToRow(index, row, dp.tipoProducto, headerColumns[i + 23] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 23] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.subProducto, headerColumns[i + 24] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 24] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.tipoCliente, headerColumns[i + 25] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 25] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.numeroTarjeta, headerColumns[i + 26] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 26] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.sucursal, headerColumns[i + 27] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 27] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.monto, headerColumns[i + 28] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 28] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.numDisposiciones, headerColumns[i + 29] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 29] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.sucKit, headerColumns[i + 30] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 30] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.nombre, headerColumns[i + 31] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 31] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.apellidoPaterno, headerColumns[i + 32] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 32] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.apellidoMaterno, headerColumns[i + 33] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 33] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.parentesco, headerColumns[i + 34] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 34] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.sexo, headerColumns[i + 35] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 35] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.fechaNacimiento, headerColumns[i + 36] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 36] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //                index++;
            }

            return sheetData;
        }

        //Lay Out Detalle Empleado
        [HttpGet]
        public void crearExcelDetalleEmpleados(int solicitudId)
        {
            FileStream fileStream = null;
            MemoryStream mem = new MemoryStream();
            try
            {

                List<Empleado> empleadosList = new List<Empleado>();

                Solicitud sol = db.Solicituds.Find(solicitudId);

                empleadosList = (from s in db.SolicitudEmpleadoes
                                 //                                 where s.estatus.Equals("A")
                                 where s.solicitudId.Equals(solicitudId)
                                 orderby s.id
                                 select s.Empleado).ToList();

                DateTime date = DateTime.Now;
                String path = @"C:\\SUA\\Exceles\\";
                String fileName = @"DetalleEmpleados-" + date.ToString("ddMMyyyyHHmm") + ".xlsx";
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

                    SheetData sd = crearContenidoHojaDetalleEmpleados(empleadosList, eh, sol);
                    ws.Append(sd);
                    wsp.Worksheet = ws;
                    wsp.Worksheet.Save();

                    Sheets sheets = new Sheets();
                    Sheet sheet = new Sheet();
                    sheet.Name = "rptPanelDetalleEmpleados";
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


        public SheetData crearContenidoHojaDetalleEmpleados(List<Empleado> empleados, ExcelHelper eh, Solicitud sol)
        {

            SheetData sheetData = new SheetData();
            int index = 1;

            //Creamos el Header
            Row row = new Row();
            row = eh.addNewCellToRow(index, row, "Titulo del Excel", headerColumns[0] + index, 0U, CellValues.String);
            sheetData.AppendChild(row);

            index = index + 1;
            row = eh.addNewCellToRow(index, row, "DETALLE EMPLEADO", headerColumns[0] + index, 0U, CellValues.String);
            sheetData.AppendChild(row);

            index = index + 2;
            row = eh.addNewCellToRow(index, row, "Folio:", headerColumns[0] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, sol.folioSolicitud, headerColumns[1] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            index = index + 2;
            row = eh.addNewCellToRow(index, row, "#", headerColumns[0] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "APELLIDO PATERNO", headerColumns[1] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "APELLIDO MATERNO", headerColumns[2] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "NOMBRE", headerColumns[3] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "NSS", headerColumns[4] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "CODIGO CLIENTE", headerColumns[5] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "CATEGORIA", headerColumns[6] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "FECHA DE INGRESO", headerColumns[7] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "SEXO", headerColumns[8] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "RFC", headerColumns[9] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "HOMOCLAVE", headerColumns[10] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);


            row = eh.addNewCellToRow(index, row, "CURP", headerColumns[11] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);


            row = eh.addNewCellToRow(index, row, "SDI", headerColumns[12] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);


            row = eh.addNewCellToRow(index, row, "SALARIO REAL", headerColumns[13] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);


            row = eh.addNewCellToRow(index, row, "SALARIO MENSUAL", headerColumns[14] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);


            row = eh.addNewCellToRow(index, row, "BANCO CUENTA", headerColumns[15] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);


            row = eh.addNewCellToRow(index, row, "FECHA NACIMIENTO", headerColumns[16] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);


            row = eh.addNewCellToRow(index, row, "ESTADO DE NACIMIENTO", headerColumns[17] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "MUNICIPIO DE NACIMIENTO", headerColumns[18] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "NACIONALIDAD", headerColumns[19] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "CORREO", headerColumns[20] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "ESTADO CIVIL", headerColumns[21] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "CALLE NUMERO", headerColumns[22] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "COLONIA", headerColumns[23] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "C.P.", headerColumns[24] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "DOMICILIO OFICINA", headerColumns[25] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "FECHA DE ANTIGÜEDAD", headerColumns[26] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "SALARIO VSM", headerColumns[27] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "JORNADA LABORAL", headerColumns[28] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "DIAS DE DESCANSO", headerColumns[29] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "SALARIO NOMINAL", headerColumns[30] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "DIAS DE VACACIONES", headerColumns[31] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "PRIMA VACACIONAL", headerColumns[32] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "DIAS DE AGUINALDO", headerColumns[33] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "OTROS", headerColumns[34] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "TELEFONO", headerColumns[35] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "TIPO DE SANGRE", headerColumns[36] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "EN CASO DE ACCIDENTE LLAMAR A:", headerColumns[37] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "PARENTESO", headerColumns[38] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "TELEFONO FAMILIAR", headerColumns[39] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "SALARIO HORA EXTRA", headerColumns[40] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "CREDITO INFONAVIT", headerColumns[41] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "REGIMEN INFONAVIT", headerColumns[42] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "MONTO INFONAVIT", headerColumns[43] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "FONACOT", headerColumns[44] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "% PENSION", headerColumns[45] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "IMPORTE PENSION", headerColumns[46] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "PERIODO", headerColumns[47] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "DOCUMENTOS", headerColumns[48] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "FOTO", headerColumns[49] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);


            //Creamos las celdas que contienen los datos
            int consecutivo = 0;
            foreach (Empleado dp in empleados)
            {
                int i = 0;
                index = index + 1;
                consecutivo = consecutivo + 1;

                row = eh.addNewCellToRow(index, row, consecutivo.ToString(), headerColumns[i] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.apellidoPaterno, headerColumns[i + 1] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                if (dp.apellidoMaterno != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.apellidoMaterno, headerColumns[i + 2] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);

                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 2] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                row = eh.addNewCellToRow(index, row, dp.nombre, headerColumns[i + 3] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                if (dp.nss != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.nss, headerColumns[i + 4] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 4] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                row = eh.addNewCellToRow(index, row, sol.Cliente.claveCliente, headerColumns[i + 5] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                if (dp.categoria != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.categoria, headerColumns[i + 6] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 6] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.Asegurado != null)
                {
                    if (dp.Asegurado.fechaAlta != null)
                    {
                        DateTime fechaAlta = (DateTime)dp.Asegurado.fechaAlta;
                        row = eh.addNewCellToRow(index, row, fechaAlta.ToShortDateString(), headerColumns[i + 7] + index, 3U, CellValues.String);
                        sheetData.AppendChild(row);
                    }
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 7] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.Sexo != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.Sexo.descripcion, headerColumns[i + 8] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 8] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.rfc != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.rfc, headerColumns[i + 9] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 9] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.homoclave != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.homoclave, headerColumns[i + 10] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 10] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.curp != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.curp, headerColumns[i + 11] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 11] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.SDI != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.SDI.descripcion, headerColumns[i + 12] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 12] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                //Convertir a decimales dp.salarioReal
                if (dp.salarioReal != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.salarioReal.ToString(), headerColumns[i + 13] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 13] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                //row = eh.addNewCellToRow(index, row, dp.salarioMensual.ToString(), headerColumns[i + 15] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 14] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Banco.descripcion, headerColumns[i + 15] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);


                if (dp.fechaNacimiento != null)
                {
                    DateTime fechaNacimiento = (DateTime)dp.fechaNacimiento;
                    row = eh.addNewCellToRow(index, row, fechaNacimiento.ToShortDateString(), headerColumns[i + 16] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 16] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.Estado != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.Estado.descripcion, headerColumns[i + 17] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 17] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.Municipio != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.Municipio.descripcion, headerColumns[i + 18] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 18] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.Pais != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.Pais.descripcion, headerColumns[i + 19] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 19] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.email != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.email, headerColumns[i + 20] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 20] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.EstadoCivil != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.EstadoCivil.descripcion, headerColumns[i + 21] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 21] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.calleNumero != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.calleNumero, headerColumns[i + 22] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 22] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.colonia != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.colonia, headerColumns[i + 23] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 23] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.codigoPostal != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.codigoPostal, headerColumns[i + 24] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 24] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                //row = eh.addNewCellToRow(index, row, dp.domicilioOficina, headerColumns[i + 26] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 25] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.archivosEmpleado.fechaAntiguedad, headerColumns[i + 27] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 26] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.Acreditados.vsm, headerColumns[i + 28] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 27] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.Acreditados.jornada, headerColumns[i + 29] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 28] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.Acreditados.diasdescanso, headerColumns[i + 29] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 29] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.Asegurado.salarioNominal, headerColumns[i + 30] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 30] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.DocumentoEmpleado.diasVacaciones, headerColumns[i + 31] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 31] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.porcentajePrima, headerColumns[i + 32] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 32] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.DocumentoEmpleado.diasAguinaldo, headerColumns[i + 33] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 33] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.otros, headerColumns[i + 34] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 34] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.telefono, headerColumns[i + 35] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 35] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.tipoSangre, headerColumns[i + 36] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 36] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.enCasoAccidenteLlamara:, headerColumns[i + 37] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 37] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.parentesco, headerColumns[i + 38] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 38] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.FamiliaresEmpleado.telefonoCasa, headerColumns[i + 39] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 39] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.salarioHoraExtra, headerColumns[i + 40] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 40] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.creditoInfonavit, headerColumns[i + 41] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 41] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.regimenInfonavit, headerColumns[i + 42] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 42] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.montoInfonavit, headerColumns[i + 43] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 43] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.fonacot, headerColumns[i + 44] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 44] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.pension, headerColumns[i + 45] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 45] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.importePension, headerColumns[i + 46] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 46] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.periodo, headerColumns[i + 47] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 47] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.documentos, headerColumns[i + 48] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 48] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.foto, headerColumns[i + 49] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 49] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //                index++;
            }

            return sheetData;
        }

        public void launchReport()
        {

            Response.Redirect("~/Reports/ReportViewer.aspx");

            //String serverName = "Driver={SQL Server Native Client 10.0};Server=MXQRMN-PC025WWD\\SQLEXPRESS";
            //rp.SetDatabaseLogon("root", "jeargaqu", serverName, "sua", false);
            //rp.VerifyDatabase();
            //rp.Refresh();

            //CrystalReportViewer crystalReportViewer = new CrystalReportViewer();
            //crystalReportViewer.ReportSource = rp;
            //crystalReportViewer.DisplayToolbar = true;
            // rp.SetDatabaseLogon("root", "jeargaqu", serverName, "sua", false);
            // rp.VerifyDatabase();
            // rp.Refresh();

            CrystalReportViewer crystalReportViewer = new CrystalReportViewer();
            //crystalReportViewer.ReportSource = rp;
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

        //Lay Out Detalle Empleado
        [HttpGet]
        public void crearExcelDetalleEmpleadosBajas(int solicitudId)
        {
            FileStream fileStream = null;
            MemoryStream mem = new MemoryStream();
            try
            {

                List<Empleado> empleadosList = new List<Empleado>();

                Solicitud sol = db.Solicituds.Find(solicitudId);

                empleadosList = (from s in db.SolicitudEmpleadoes
                                 //                                 where s.estatus.Equals("A")
                                 where s.solicitudId.Equals(solicitudId)
                                 orderby s.id
                                 select s.Empleado).ToList();

                DateTime date = DateTime.Now;
                String path = @"C:\\SUA\\Exceles\\";
                String fileName = @"DetalleEmpleados-" + date.ToString("ddMMyyyyHHmm") + ".xlsx";
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

                    SheetData sd = crearContenidoHojaDetalleEmpleados(empleadosList, eh, sol);
                    ws.Append(sd);
                    wsp.Worksheet = ws;
                    wsp.Worksheet.Save();

                    Sheets sheets = new Sheets();
                    Sheet sheet = new Sheet();
                    sheet.Name = "rptPanelDetalleEmpleados";
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


        public SheetData crearContenidoHojaDetalleEmpleadosBaja(List<Empleado> empleados, ExcelHelper eh, Solicitud sol)
        {

            SheetData sheetData = new SheetData();
            int index = 1;

            //Creamos el Header
            Row row = new Row();
            row = eh.addNewCellToRow(index, row, "Titulo del Excel", headerColumns[0] + index, 0U, CellValues.String);
            sheetData.AppendChild(row);

            index = index + 1;
            row = eh.addNewCellToRow(index, row, "DETALLE EMPLEADO", headerColumns[0] + index, 0U, CellValues.String);
            sheetData.AppendChild(row);

            index = index + 2;
            row = eh.addNewCellToRow(index, row, "Folio:", headerColumns[0] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, sol.folioSolicitud, headerColumns[1] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            index = index + 2;
            row = eh.addNewCellToRow(index, row, "#", headerColumns[0] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "ID SAPYN", headerColumns[1] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "APELLIDO PATERNO", headerColumns[2] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "APELLIDO MATERNO", headerColumns[3] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "NOMBRE", headerColumns[4] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "NSS", headerColumns[5] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "CODIGO CLIENTE", headerColumns[6] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "CATEGORIA", headerColumns[7] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "FECHA DE INGRESO", headerColumns[8] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "FECHA DE BAJA", headerColumns[9] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "SEXO", headerColumns[10] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "RFC", headerColumns[11] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "HOMOCLAVE", headerColumns[12] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);


            row = eh.addNewCellToRow(index, row, "CURP", headerColumns[13] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);


            row = eh.addNewCellToRow(index, row, "SDI", headerColumns[14] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);


            row = eh.addNewCellToRow(index, row, "SALARIO REAL", headerColumns[15] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);


            row = eh.addNewCellToRow(index, row, "SALARIO MENSUAL", headerColumns[16] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);


            row = eh.addNewCellToRow(index, row, "BANCO CUENTA", headerColumns[17] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);


            row = eh.addNewCellToRow(index, row, "FECHA NACIMIENTO", headerColumns[18] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);


            row = eh.addNewCellToRow(index, row, "ESTADO DE NACIMIENTO", headerColumns[19] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "MUNICIPIO DE NACIMIENTO", headerColumns[20] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "NACIONALIDAD", headerColumns[21] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "CORREO", headerColumns[22] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "ESTADO CIVIL", headerColumns[23] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "CALLE NUMERO", headerColumns[24] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "COLONIA", headerColumns[25] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "C.P.", headerColumns[26] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "DOMICILIO OFICINA", headerColumns[27] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "FECHA DE ANTIGÜEDAD", headerColumns[28] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "SALARIO VSM", headerColumns[29] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "JORNADA LABORAL", headerColumns[30] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "DIAS DE DESCANSO", headerColumns[31] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "SALARIO NOMINAL", headerColumns[32] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "DIAS DE VACACIONES", headerColumns[33] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "PRIMA VACACIONAL", headerColumns[34] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "DIAS DE AGUINALDO", headerColumns[35] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "OTROS", headerColumns[36] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "TELEFONO", headerColumns[37] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "TIPO DE SANGRE", headerColumns[38] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "EN CASO DE ACCIDENTE LLAMAR A:", headerColumns[39] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "PARENTESO", headerColumns[40] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "TELEFONO FAMILIAR", headerColumns[41] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "SALARIO HORA EXTRA", headerColumns[42] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "CREDITO INFONAVIT", headerColumns[43] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "REGIMEN INFONAVIT", headerColumns[44] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "MONTO INFONAVIT", headerColumns[45] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "FONACOT", headerColumns[46] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "% PENSION", headerColumns[47] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "IMPORTE PENSION", headerColumns[48] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "PERIODO", headerColumns[49] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "DOCUMENTOS", headerColumns[50] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "FOTO", headerColumns[51] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);


            //Creamos las celdas que contienen los datos
            int consecutivo = 0;
            foreach (Empleado dp in empleados)
            {
                int i = 0;
                index = index + 1;
                consecutivo = consecutivo + 1;

                row = eh.addNewCellToRow(index, row, consecutivo.ToString(), headerColumns[i] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.apellidoPaterno, headerColumns[i + 1] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.apellidoMaterno, headerColumns[i + 2] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.nombre, headerColumns[i + 3] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                if (dp.nss != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.nss, headerColumns[i + 4] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 4] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                row = eh.addNewCellToRow(index, row, sol.Cliente.claveCliente, headerColumns[i + 5] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.categoria, headerColumns[i + 6] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                if (dp.Asegurado != null)
                {
                    if (dp.Asegurado.fechaAlta != null)
                    {
                        DateTime fechaAlta = (DateTime)dp.Asegurado.fechaAlta;
                        row = eh.addNewCellToRow(index, row, fechaAlta.ToShortDateString(), headerColumns[i + 7] + index, 3U, CellValues.String);
                        sheetData.AppendChild(row);
                    }
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 7] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.Asegurado != null)
                {
                    if (dp.Asegurado.fechaBaja != null)
                    {
                        DateTime fechaBaja = (DateTime)dp.Asegurado.fechaBaja;
                        row = eh.addNewCellToRow(index, row, fechaBaja.ToShortDateString(), headerColumns[i + 8] + index, 3U, CellValues.String);
                        sheetData.AppendChild(row);
                    }
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 8] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.Sexo != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.Sexo.descripcion, headerColumns[i + 9] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 9] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                row = eh.addNewCellToRow(index, row, dp.rfc, headerColumns[i + 10] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.homoclave, headerColumns[i + 11] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                if (dp.curp != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.curp, headerColumns[i + 12] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 12] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.SDI != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.SDI.descripcion, headerColumns[i + 13] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 13] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                //Convertir a decimales dp.salarioReal
                row = eh.addNewCellToRow(index, row, dp.salarioReal.ToString(), headerColumns[i + 14] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.salarioMensual.ToString(), headerColumns[i + 15] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 15] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Banco.descripcion, headerColumns[i + 16] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);


                if (dp.fechaNacimiento != null)
                {
                    DateTime fechaNacimiento = (DateTime)dp.fechaNacimiento;
                    row = eh.addNewCellToRow(index, row, fechaNacimiento.ToShortDateString(), headerColumns[i + 17] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 17] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.Estado != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.Estado.descripcion, headerColumns[i + 18] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 18] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.Municipio != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.Municipio.descripcion, headerColumns[i + 19] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 19] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.Pais != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.Pais.descripcion, headerColumns[i + 20] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 20] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.email != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.email, headerColumns[i + 21] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 21] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.EstadoCivil != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.EstadoCivil.descripcion, headerColumns[i + 22] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 22] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                row = eh.addNewCellToRow(index, row, dp.calleNumero, headerColumns[i + 23] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.colonia, headerColumns[i + 24] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                if (dp.codigoPostal != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.codigoPostal, headerColumns[i + 25] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 25] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                //row = eh.addNewCellToRow(index, row, dp.domicilioOficina, headerColumns[i + 26] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 26] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.archivosEmpleado.fechaAntiguedad, headerColumns[i + 27] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 27] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.Acreditados.vsm, headerColumns[i + 28] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 28] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.Acreditados.vsm, headerColumns[i + 29] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 29] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.Asegurado.salarioNominal, headerColumns[i + 30] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 30] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.DocumentoEmpleado.diasVacaciones, headerColumns[i + 31] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 31] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.porcentajePrima, headerColumns[i + 32] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 32] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.DocumentoEmpleado.diasAguinaldo, headerColumns[i + 33] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 33] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.otros, headerColumns[i + 34] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 34] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.telefono, headerColumns[i + 35] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 35] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.tipoSangre, headerColumns[i + 36] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 36] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.enCasoAccidenteLlamara:, headerColumns[i + 37] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 37] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.parentesco, headerColumns[i + 38] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 38] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.FamiliaresEmpleado.telefonoCasa, headerColumns[i + 39] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 39] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.salarioHoraExtra, headerColumns[i + 40] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 40] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.creditoInfonavit, headerColumns[i + 41] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 41] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.regimenInfonavit, headerColumns[i + 42] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 42] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.montoInfonavit, headerColumns[i + 43] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 43] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.fonacot, headerColumns[i + 44] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 44] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.pension, headerColumns[i + 45] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 45] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.importePension, headerColumns[i + 46] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 46] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.periodo, headerColumns[i + 47] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 47] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.documentos, headerColumns[i + 48] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 48] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //row = eh.addNewCellToRow(index, row, dp.foto, headerColumns[i + 49] + index, 3U, CellValues.String);
                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 49] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 50] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                //                index++;
            }

            return sheetData;
        }

        //Lay Out Afiliacion
        [HttpGet]
        public void crearExcelAfiliacionBaja(int solicitudId)
        {
            FileStream fileStream = null;
            MemoryStream mem = new MemoryStream();
            try
            {
                Solicitud sol = db.Solicituds.Find(solicitudId);

                List<Empleado> empleadosList = new List<Empleado>();

                empleadosList = (from s in db.SolicitudEmpleadoes
                                 //                                 where s.estatus.Equals("A")
                                 where s.solicitudId.Equals(solicitudId)
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

                    SheetData sd = crearContenidoHojaAfiliacionBaja(empleadosList, eh, sol);
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


        public SheetData crearContenidoHojaAfiliacionBaja(List<Empleado> empleados, ExcelHelper eh, Solicitud sol)
        {

            SheetData sheetData = new SheetData();
            int index = 1;

            //Creamos el Header
            Row row = new Row();
            row = eh.addNewCellToRow(index, row, "Titulo del Excel", headerColumns[0] + index, 0U, CellValues.String);
            sheetData.AppendChild(row);

            index = index + 1;
            row = eh.addNewCellToRow(index, row, "BAJA DE PERSONAL IMSS", headerColumns[0] + index, 0U, CellValues.String);
            sheetData.AppendChild(row);

            index = index + 2;
            row = eh.addNewCellToRow(index, row, "Folio:", headerColumns[0] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, sol.folioSolicitud, headerColumns[1] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            index = index + 2;
            row = eh.addNewCellToRow(index, row, "#", headerColumns[0] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Número de Seguro Social", headerColumns[1] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Primer Apellido", headerColumns[2] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Segundo Apellido", headerColumns[3] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Nombre(s)", headerColumns[4] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Fecha baja", headerColumns[5] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Causa baja", headerColumns[6] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Clave del trabajador", headerColumns[7] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            //Creamos las celdas que contienen los datos
            int consecutivo = 0;
            foreach (Empleado dp in empleados)
            {
                int i = 0;
                index = index + 1;
                consecutivo = consecutivo + 1;

                row = eh.addNewCellToRow(index, row, consecutivo.ToString(), headerColumns[i] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                if (dp.nss != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.nss, headerColumns[i + 1] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 1] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                row = eh.addNewCellToRow(index, row, dp.apellidoPaterno, headerColumns[i + 2] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                if (dp.apellidoMaterno != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.apellidoMaterno, headerColumns[i + 3] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 3] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                row = eh.addNewCellToRow(index, row, dp.nombre, headerColumns[i + 4] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                if (dp.fechaBaja != null)
                {
                    DateTime fechaBaja = (DateTime)dp.fechaBaja;
                    row = eh.addNewCellToRow(index, row, fechaBaja.ToShortDateString(), headerColumns[i + 5] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 5] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (sol.conceptoBaja != null)
                {
                    row = eh.addNewCellToRow(index, row, sol.conceptoBaja.ToString(), headerColumns[i + 6] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 6] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 7] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

            }

            return sheetData;
        }

        //Lay Out Afiliacion
        [HttpGet]
        public void crearExcelAfiliacionModif(int solicitudId)
        {
            FileStream fileStream = null;
            MemoryStream mem = new MemoryStream();
            try
            {
                Solicitud sol = db.Solicituds.Find(solicitudId);

                List<Empleado> empleadosList = new List<Empleado>();

                empleadosList = (from s in db.SolicitudEmpleadoes
                                 //                                 where s.estatus.Equals("A")
                                 where s.solicitudId.Equals(solicitudId)
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

                    SheetData sd = crearContenidoHojaAfiliacionModif(empleadosList, eh, sol);
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


        public SheetData crearContenidoHojaAfiliacionModif(List<Empleado> empleados, ExcelHelper eh, Solicitud sol)
        {

            SheetData sheetData = new SheetData();
            int index = 1;

            //Creamos el Header
            Row row = new Row();
            row = eh.addNewCellToRow(index, row, "Titulo del Excel", headerColumns[0] + index, 0U, CellValues.String);
            sheetData.AppendChild(row);

            index = index + 1;
            row = eh.addNewCellToRow(index, row, "BAJA DE PERSONAL IMSS", headerColumns[0] + index, 0U, CellValues.String);
            sheetData.AppendChild(row);

            index = index + 2;
            row = eh.addNewCellToRow(index, row, "Folio:", headerColumns[0] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, sol.folioSolicitud, headerColumns[1] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            index = index + 2;
            row = eh.addNewCellToRow(index, row, "#", headerColumns[0] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Número de Seguro Social", headerColumns[1] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "CURP", headerColumns[2] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Primer Apellido", headerColumns[3] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Segundo Apellido", headerColumns[4] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Nombre(s)", headerColumns[5] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Fecha modificación", headerColumns[6] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "SDI", headerColumns[7] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Tipo de trabajador", headerColumns[8] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Tipo de salario", headerColumns[9] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Tipo deJornada", headerColumns[10] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Clave del trabajador", headerColumns[11] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            //Creamos las celdas que contienen los datos
            int consecutivo = 0;
            foreach (Empleado dp in empleados)
            {
                int i = 0;
                index = index + 1;
                consecutivo = consecutivo + 1;

                row = eh.addNewCellToRow(index, row, consecutivo.ToString(), headerColumns[i] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                if (dp.nss != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.nss, headerColumns[i + 1] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 1] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.curp != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.curp, headerColumns[i + 2] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 2] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                row = eh.addNewCellToRow(index, row, dp.apellidoPaterno, headerColumns[i + 3] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                if (dp.apellidoMaterno != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.apellidoMaterno, headerColumns[i + 4] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 4] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                row = eh.addNewCellToRow(index, row, dp.nombre, headerColumns[i + 5] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                if (dp.fechaModificacion != null)
                {
                    DateTime fechaModif = (DateTime)dp.fechaModificacion;
                    row = eh.addNewCellToRow(index, row, fechaModif.ToShortDateString(), headerColumns[i + 6] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 6] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.SDI != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.SDI.descripcion, headerColumns[i + 7] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 7] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                row = eh.addNewCellToRow(index, row, "1", headerColumns[i + 8] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, "0", headerColumns[i + 9] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, "0", headerColumns[i + 10] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 11] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

            }

            return sheetData;
        }

        [HttpPost]
        public ActionResult crearAltasIDSE(String solicitudId, String tipoId, String trabajador, String salario, String jornada)
        {
            ViewBag.tipoId = tipoId;
            ViewBag.solcitudId=solicitudId;
            int solicitud = int.Parse(solicitudId.Trim());

            Solicitud sol = db.Solicituds.Find(solicitud);

            List<Empleado> empleadosList = new List<Empleado>();

            empleadosList = (from s in db.SolicitudEmpleadoes
                             //                                 where s.estatus.Equals("A")
                             where s.solicitudId.Equals(solicitud)
                             orderby s.id
                             select s.Empleado).ToList();

            DateTime date = DateTime.Now;
            String path = @"C:\\SUA\\Exceles\\";
            String fileName = @"IDSE-" + date.ToString("ddMMyyyyHHmm") + ".txt";
            String fullName = path + fileName;

            if (empleadosList.Count() > 0)
            {
                try
                {
                    System.IO.StreamWriter sw = new System.IO.StreamWriter(fullName);
                    foreach (Empleado dp in empleadosList)
                    {

                        String linea = sol.Patrone.registro;
                        if (dp.nss != null)
                        {
                            linea = linea + dp.nss;
                        }
                        linea = linea +  dp.apellidoPaterno.PadLeft(27,' ') ;

                        if (dp.apellidoMaterno != null)
                        {
                                       linea =linea + dp.apellidoMaterno.PadLeft(27,' ');
                        }

                        linea = linea + dp.nombre.PadLeft(27, ' ');
                        linea = linea + dp.SDI.descripcion.PadRight(6, '0') + "000000";
                        linea = linea + trabajador + salario + jornada;

                        if (dp.fechaAltaImss != null)
                        {
                            DateTime fechaAltaImss = (DateTime)sol.fechaInicioContrato;
                            linea = linea + fechaAltaImss.ToString("ddMMyyyy");
                        }

                        if (dp.UMF != null)
                        {
                            linea = linea + dp.UMF;

                        }else
                        {
                            linea = linea + "000";
                        }
                         linea = linea + "  " + "08"  + sol.Patrone.delegacion.Trim() + "400" + sol.Cliente.claveCliente.PadLeft(10,' ') +
                                       " " + dp.curp + "9" +
                                       "\r";
                        sw.WriteLine(linea);
                    }
                    sw.Close();
                }
                catch (Exception ex)
                {
                    Console.Write(ex);
                }
//                Console.ReadKey();
            }
            return RedirectToAction("Index", new { tipoId });
        }

        // GET: Aseguradoes/Delete/5
        public ActionResult Adicionales(int solicitudId, String tipoId)
        {
            ViewBag.solicitudId = solicitudId;
            ViewBag.tipoId = tipoId;
            return View();
            //            return RedirectToAction("Adicionales", new { solicitudId, tipoId });
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
