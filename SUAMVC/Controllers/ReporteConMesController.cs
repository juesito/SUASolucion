using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SUADATOS;
using System.Data.SqlClient;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using System.Data.OleDb;
using SUAMVC.Helpers;

namespace SUAMVC.Controllers
{
    public class ReporteConMesController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: ReporteConMes
        public ActionResult Index(String plazasId, String ejercicioId,
                                   String clientesId, String usuarioId)
        {
            Usuario user = Session["UsuarioData"] as Usuario;

            var plazasAsignadas = (from x in db.TopicosUsuarios
                                   where x.usuarioId.Equals(user.Id)
                                   && x.tipo.Equals("P")
                                   select x.topicoId);

            var clientesAsignados = (from x in db.TopicosUsuarios
                                     where x.usuarioId.Equals(user.Id)
                                     && x.tipo.Equals("C")
                                     select x.topicoId);

            //Query principal
            int result = db.Database.ExecuteSqlCommand("sp_AmortizacionBimestralINF @usuarioId", new SqlParameter("@usuarioId", user.Id));
            var reporteAmortizaciónBim = from s in db.ReporteConMeses
                                         join cli in db.Clientes on s.clienteId equals cli.Id
                                         where plazasAsignadas.Contains(s.Cliente.Plaza_id) &&
                                               clientesAsignados.Contains(s.Cliente.Id) &&
                                               s.usuarioId.Equals(user.Id)
                                         select s;

            if (!String.IsNullOrEmpty(clientesId))
            {
                int clienteId = int.Parse(clientesId.Trim());
                reporteAmortizaciónBim = reporteAmortizaciónBim.Where(s => s.clienteId.Equals(clienteId));
            }
            if (!String.IsNullOrEmpty(plazasId))
            {
                int plazaTempId = int.Parse(plazasId.Trim());
                reporteAmortizaciónBim = reporteAmortizaciónBim.Where(s => s.Patrone.Plaza_id.Equals(plazaTempId));
            }
            if (!String.IsNullOrEmpty(ejercicioId))
            {
                reporteAmortizaciónBim = reporteAmortizaciónBim.Where(s => s.anno.Trim().Equals(ejercicioId));
            }

            reporteAmortizaciónBim = reporteAmortizaciónBim.OrderBy(p => p.Patrone.registro);

            return View(reporteAmortizaciónBim.ToList());
        }

        // GET: ReporteConMes/Details/5
        public ActionResult Details(String clienteId, String anio)
        {

            Usuario user = Session["UsuarioData"] as Usuario;

            int result = db.Database.ExecuteSqlCommand("sp_AmortizacionBimestralINFDet @usuarioId, @clienteId, @anio", new SqlParameter("@usuarioId", user.Id), new SqlParameter("@clienteId", clienteId), new SqlParameter("@anio", anio));

            var reporteAmortizaciónBimDet = from s in db.ReporteConMeses
                                            select s;
            ViewBag.cteId = clienteId;
            ViewBag.ejercicioId = anio;
            return View(reporteAmortizaciónBimDet);
        }

        // GET: ReporteConMes/Create
        public ActionResult Create()
        {
            ViewBag.clienteId = new SelectList(db.Clientes, "Id", "claveCliente");
            ViewBag.patronId = new SelectList(db.Patrones, "Id", "registro");
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario");
            return View();
        }

        // POST: ReporteConMes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,patronId,anno,mes,enero,febrero,marzo,abril,mayo,junio,julio,agosto,septiembre,octubre,noviembre,diciembre,total,nt,usuarioId,fechaCreacion,clienteId")] ReporteConMes reporteConMes)
        {
            if (ModelState.IsValid)
            {
                db.ReporteConMeses.Add(reporteConMes);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.clienteId = new SelectList(db.Clientes, "Id", "claveCliente", reporteConMes.clienteId);
            ViewBag.patronId = new SelectList(db.Patrones, "Id", "registro", reporteConMes.patronId);
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", reporteConMes.usuarioId);
            return View(reporteConMes);
        }

        // GET: ReporteConMes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReporteConMes reporteConMes = db.ReporteConMeses.Find(id);
            if (reporteConMes == null)
            {
                return HttpNotFound();
            }
            ViewBag.clienteId = new SelectList(db.Clientes, "Id", "claveCliente", reporteConMes.clienteId);
            ViewBag.patronId = new SelectList(db.Patrones, "Id", "registro", reporteConMes.patronId);
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", reporteConMes.usuarioId);
            return View(reporteConMes);
        }

        // POST: ReporteConMes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,patronId,anno,mes,enero,febrero,marzo,abril,mayo,junio,julio,agosto,septiembre,octubre,noviembre,diciembre,total,nt,usuarioId,fechaCreacion,clienteId")] ReporteConMes reporteConMes)
        {
            if (ModelState.IsValid)
            {
                db.Entry(reporteConMes).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.clienteId = new SelectList(db.Clientes, "Id", "claveCliente", reporteConMes.clienteId);
            ViewBag.patronId = new SelectList(db.Patrones, "Id", "registro", reporteConMes.patronId);
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", reporteConMes.usuarioId);
            return View(reporteConMes);
        }

        // GET: ReporteConMes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReporteConMes reporteConMes = db.ReporteConMeses.Find(id);
            if (reporteConMes == null)
            {
                return HttpNotFound();
            }
            return View(reporteConMes);
        }

        // POST: ReporteConMes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ReporteConMes reporteConMes = db.ReporteConMeses.Find(id);
            db.ReporteConMeses.Remove(reporteConMes);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public void ExcelAnualINF(String plazasId, String ejercicioId, String clientesId, String gruposId)
        {
            Usuario user = Session["UsuarioData"] as Usuario;

            var plazasAsignadas = (from x in db.TopicosUsuarios
                                   where x.usuarioId.Equals(user.Id)
                                   && x.tipo.Equals("P")
                                   select x.topicoId);

            var clientesAsignados = (from x in db.TopicosUsuarios
                                     where x.usuarioId.Equals(user.Id)
                                     && x.tipo.Equals("C")
                                     select x.topicoId);

            var gruposAsignados = (from s in db.Grupos
                                   join cli in db.Clientes on s.Id equals cli.Grupo_id
                                   join top in db.TopicosUsuarios on cli.Id equals top.topicoId
                                   where top.tipo.Trim().Equals("C") && top.usuarioId.Equals(user.Id)
                                   orderby s.claveGrupo
                                   select s.Id);

            //Query principal
            int result = db.Database.ExecuteSqlCommand("sp_AmortizacionBimestralINF @usuarioId", new SqlParameter("@usuarioId", user.Id));
            var reporteAmortizaciónBim = from s in db.ReporteConMeses
                                         join cli in db.Clientes on s.clienteId equals cli.Id
                                         where plazasAsignadas.Contains(s.Cliente.Plaza_id) &&
                                               clientesAsignados.Contains(s.Cliente.Id) &&
                                               s.usuarioId.Equals(user.Id)
                                         select s;

            if (!String.IsNullOrEmpty(clientesId))
            {
                int clienteId = int.Parse(clientesId.Trim());
                reporteAmortizaciónBim = reporteAmortizaciónBim.Where(s => s.clienteId.Equals(clienteId));
            }
            if (!String.IsNullOrEmpty(plazasId))
            {
                int plazaTempId = int.Parse(plazasId.Trim());
                reporteAmortizaciónBim = reporteAmortizaciónBim.Where(s => s.Patrone.Plaza_id.Equals(plazaTempId));
            }
            if (!String.IsNullOrEmpty(ejercicioId))
            {
                reporteAmortizaciónBim = reporteAmortizaciónBim.Where(s => s.anno.Trim().Equals(ejercicioId));
            }

            reporteAmortizaciónBim = reporteAmortizaciónBim.OrderBy(p => p.Cliente.claveSua);

            List<ReporteConMes> allCust = new List<ReporteConMes>();

            allCust = reporteAmortizaciónBim.ToList();

            FileStream fileStream = null;
            MemoryStream mem = new MemoryStream();
            try
            {

                DateTime date = DateTime.Now;
                String path = @"C:\\SUA\\Exceles\\";
                String fileName = @"AmortizacionAnual-" + date.ToString("ddMMyyyyHHmm") + ".xlsx";
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

                SheetData sd = crearContenidoHoja4(allCust, eh);//CreateContentRow(); 
                ws.Append(sd);
                wsp.Worksheet = ws;
                wsp.Worksheet.Save();

                Sheets sheets = new Sheets();
                Sheet sheet = new Sheet();
                sheet.Name = "Sheet1";
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


        string[] headerColumns3 = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD", "AE", "AF", "AG" };
        public SheetData crearContenidoHoja4(List<ReporteConMes> allCust, ExcelHelper eh)
        {
            SheetData sheetData = new SheetData();
            int index = 1;

            //Creamos el Header
            Row row = new Row();

            row = eh.addNewCellToRow(index, row, "Cliente", headerColumns3[0] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Nombre", headerColumns3[1] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "ID. Grupo", headerColumns3[2] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "ID. Plaza", headerColumns3[3] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Año", headerColumns3[4] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Febrero", headerColumns3[5] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Abril", headerColumns3[6] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Junio", headerColumns3[7] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Agosto", headerColumns3[8] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Octubre", headerColumns3[9] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Diciembre", headerColumns3[10] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Total", headerColumns3[11] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            index++;
            //Create the cells that contain the data.
            foreach (ReporteConMes dp in allCust)
            {
                int i = 0;

                row = eh.addNewCellToRow(index, row, dp.Cliente.claveCliente, headerColumns3[i] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Cliente.descripcion, headerColumns3[i + 1] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Cliente.Grupos.claveGrupo, headerColumns3[i + 2] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Cliente.Plaza.cveCorta, headerColumns3[i + 3] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.anno, headerColumns3[i + 4] + index, 2U, CellValues.Number);
                sheetData.AppendChild(row);

                String var1 = "";
                if (dp.febrero != null)
                {
                    var1 = String.Format("{0:###,###,##0.00}", dp.febrero);
                    row = eh.addNewCellToRow(index, row, var1, headerColumns3[i + 5] + index, 2U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.abril != null)
                {
                    var1 = String.Format("{0:###,###,##0.00}", dp.abril);
                    row = eh.addNewCellToRow(index, row, var1, headerColumns3[i + 6] + index, 2U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.junio != null)
                {
                    var1 = String.Format("{0:###,###,##0.00}", dp.junio);
                    row = eh.addNewCellToRow(index, row, var1, headerColumns3[i + 7] + index, 2U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.agosto != null)
                {
                    var1 = String.Format("{0:###,###,##0.00}", dp.agosto);
                    row = eh.addNewCellToRow(index, row, var1, headerColumns3[i + 8] + index, 2U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.octubre != null)
                {
                    var1 = String.Format("{0:###,###,##0.00}", dp.octubre);
                    row = eh.addNewCellToRow(index, row, var1, headerColumns3[i + 9] + index, 2U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.diciembre != null)
                {
                    var1 = String.Format("{0:###,###,##0.00}", dp.diciembre);
                    row = eh.addNewCellToRow(index, row, var1, headerColumns3[i + 10] + index, 2U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.total != null)
                {
                    var1 = String.Format("{0:###,###,##0.00}", dp.total);
                    row = eh.addNewCellToRow(index, row, var1, headerColumns3[i + 11] + index, 2U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                index++;
            }

            return sheetData;
        }

        [HttpGet]
        public void ExcelAnualINFDet(String plazasId, String ejercicioId, String clientesId, String gruposId)
        {
            Usuario user = Session["UsuarioData"] as Usuario;

            var plazasAsignadas = (from x in db.TopicosUsuarios
                                   where x.usuarioId.Equals(user.Id)
                                   && x.tipo.Equals("P")
                                   select x.topicoId);

            var clientesAsignados = (from x in db.TopicosUsuarios
                                     where x.usuarioId.Equals(user.Id)
                                     && x.tipo.Equals("C")
                                     select x.topicoId);

            var gruposAsignados = (from s in db.Grupos
                                   join cli in db.Clientes on s.Id equals cli.Grupo_id
                                   join top in db.TopicosUsuarios on cli.Id equals top.topicoId
                                   where top.tipo.Trim().Equals("C") && top.usuarioId.Equals(user.Id)
                                   orderby s.claveGrupo
                                   select s.Id);

            //Query principal

            int result = db.Database.ExecuteSqlCommand("sp_AmortizacionBimestralINFDetExcel @usuarioId", new SqlParameter("@usuarioId", user.Id));
            var reporteAmortizaciónBimDet = from s in db.ReporteConMeses
                                            join cli in db.Clientes on s.clienteId equals cli.Id
                                            where plazasAsignadas.Contains(s.Cliente.Plaza_id) &&
                                                  clientesAsignados.Contains(s.Cliente.Id) &&
                                                  s.usuarioId.Equals(user.Id)
                                            select s;

            if (!String.IsNullOrEmpty(clientesId))
            {
                int clienteId = int.Parse(clientesId.Trim());
                reporteAmortizaciónBimDet = reporteAmortizaciónBimDet.Where(s => s.Cliente.Id.Equals(clienteId));
            }
            if (!String.IsNullOrEmpty(plazasId))
            {
                int plazaTempId = int.Parse(plazasId.Trim());
                reporteAmortizaciónBimDet = reporteAmortizaciónBimDet.Where(s => s.Patrone.Plaza_id.Equals(plazaTempId));
            }
            if (!String.IsNullOrEmpty(ejercicioId))
            {
                reporteAmortizaciónBimDet = reporteAmortizaciónBimDet.Where(s => s.anno.Trim().Equals(ejercicioId));
            }

            reporteAmortizaciónBimDet = reporteAmortizaciónBimDet.OrderBy(p => p.Cliente.claveSua);

            List<ReporteConMes> allCust = new List<ReporteConMes>();

            allCust = reporteAmortizaciónBimDet.ToList();

            FileStream fileStream = null;
            MemoryStream mem = new MemoryStream();
            try
            {

                DateTime date = DateTime.Now;
                String path = @"C:\\SUA\\Exceles\\";
                String fileName = @"AmortizacionAnualDetalle-" + date.ToString("ddMMyyyyHHmm") + ".xlsx";
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

                SheetData sd = crearContenidoHoja5(allCust, eh);//CreateContentRow(); 
                ws.Append(sd);
                wsp.Worksheet = ws;
                wsp.Worksheet.Save();

                Sheets sheets = new Sheets();
                Sheet sheet = new Sheet();
                sheet.Name = "Sheet1";
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


        string[] headerColumns5 = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD", "AE", "AF", "AG" };
        public SheetData crearContenidoHoja5(List<ReporteConMes> allCust, ExcelHelper eh)
        {
            SheetData sheetData = new SheetData();
            int index = 1;

            //Creamos el Header
            Row row = new Row();

            row = eh.addNewCellToRow(index, row, "Cliente", headerColumns5[0] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Nombre", headerColumns5[1] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "ID. Grupo", headerColumns5[2] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "ID. Plaza", headerColumns5[3] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Año", headerColumns5[4] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Reg. Patronal", headerColumns5[5] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Número Afiliación", headerColumns5[6] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Nombre Asegurado", headerColumns5[7] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Febrero", headerColumns5[8] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Abril", headerColumns5[9] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Junio", headerColumns5[10] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Agosto", headerColumns5[11] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Octubre", headerColumns5[12] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Diciembre", headerColumns5[13] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Total", headerColumns5[14] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            Decimal feb = Decimal.Parse("0.0");
            Decimal abr = Decimal.Parse("0.0");
            Decimal jun = Decimal.Parse("0.0");
            Decimal ago = Decimal.Parse("0.0");
            Decimal oct = Decimal.Parse("0.0");
            Decimal dic = Decimal.Parse("0.0");
            Decimal tot = Decimal.Parse("0.0");
            index++;
            int grupo = 0;
            String var1 = "";
            int i = 0;
            //Create the cells that contain the data.
            foreach (ReporteConMes dp in allCust)
            {

                if (grupo == 0)
                {
                    grupo = dp.Cliente.Grupo_id;
                }

                if (!dp.Cliente.Grupo_id.Equals(grupo))
                {
                    row = eh.addNewCellToRow(index, row, "Total Grupo:", headerColumns3[i + 1] + index, 2U, CellValues.String);
                    sheetData.AppendChild(row);

                    var1 = String.Format("{0:###,###,##0.00}", feb);
                    row = eh.addNewCellToRow(index, row, var1, headerColumns5[i + 8] + index, 2U, CellValues.String);
                    sheetData.AppendChild(row);

                    var1 = String.Format("{0:###,###,##0.00}", abr);
                    row = eh.addNewCellToRow(index, row, var1, headerColumns5[i + 9] + index, 2U, CellValues.String);
                    sheetData.AppendChild(row);

                    var1 = String.Format("{0:###,###,##0.00}", jun);
                    row = eh.addNewCellToRow(index, row, var1, headerColumns5[i + 10] + index, 2U, CellValues.String);
                    sheetData.AppendChild(row);

                    var1 = String.Format("{0:###,###,##0.00}", ago);
                    row = eh.addNewCellToRow(index, row, var1, headerColumns5[i + 11] + index, 2U, CellValues.String);
                    sheetData.AppendChild(row);

                    var1 = String.Format("{0:###,###,##0.00}", oct);
                    row = eh.addNewCellToRow(index, row, var1, headerColumns5[i + 12] + index, 2U, CellValues.String);
                    sheetData.AppendChild(row);

                    var1 = String.Format("{0:###,###,##0.00}", dic);
                    row = eh.addNewCellToRow(index, row, var1, headerColumns5[i + 13] + index, 2U, CellValues.String);
                    sheetData.AppendChild(row);

                    var1 = String.Format("{0:###,###,##0.00}", feb + abr + jun + ago + oct + dic);
                    row = eh.addNewCellToRow(index, row, var1, headerColumns5[i + 14] + index, 2U, CellValues.String);
                    sheetData.AppendChild(row);

                    index++;
                    feb = Decimal.Parse("0.0");
                    abr = Decimal.Parse("0.0");
                    jun = Decimal.Parse("0.0");
                    ago = Decimal.Parse("0.0");
                    oct = Decimal.Parse("0.0");
                    dic = Decimal.Parse("0.0");
                    tot = Decimal.Parse("0.0");
                    grupo = dp.Cliente.Grupo_id;
                }

                row = eh.addNewCellToRow(index, row, dp.Cliente.claveCliente, headerColumns5[i] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Cliente.descripcion, headerColumns5[i + 1] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Cliente.Grupos.claveGrupo, headerColumns5[i + 2] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Patrone.Plaza.cveCorta, headerColumns5[i + 3] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.anno, headerColumns5[i + 4] + index, 2U, CellValues.Number);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Patrone.registro, headerColumns5[i + 5] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Asegurado.numeroAfiliacion, headerColumns5[i + 6] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Asegurado.nombreTemporal, headerColumns5[i + 7] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                if (dp.febrero != null)
                {
                    feb = feb + Decimal.Parse(dp.febrero.ToString());
                }
                if (dp.abril != null)
                {
                    abr = abr + Decimal.Parse(dp.abril.ToString());
                }
                if (dp.junio != null)
                {
                    jun = jun + Decimal.Parse(dp.junio.ToString());
                }
                if (dp.agosto != null)
                {
                    ago = ago + Decimal.Parse(dp.agosto.ToString());
                }
                if (dp.octubre != null)
                {
                    oct = oct + Decimal.Parse(dp.octubre.ToString());
                }
                if (dp.diciembre != null)
                {
                    dic = dic + Decimal.Parse(dp.diciembre.ToString());
                }

                var1 = String.Format("{0:###,###,##0.00}", dp.febrero);
                row = eh.addNewCellToRow(index, row, var1, headerColumns5[i + 8] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.abril);
                row = eh.addNewCellToRow(index, row, var1, headerColumns5[i + 9] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.junio);
                row = eh.addNewCellToRow(index, row, var1, headerColumns5[i + 10] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.agosto);
                row = eh.addNewCellToRow(index, row, var1, headerColumns5[i + 11] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.octubre);
                row = eh.addNewCellToRow(index, row, var1, headerColumns5[i + 12] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.diciembre);
                row = eh.addNewCellToRow(index, row, var1, headerColumns5[i + 13] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.total);
                row = eh.addNewCellToRow(index, row, var1, headerColumns5[i + 14] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                index++;

            }

            row = eh.addNewCellToRow(index, row, "Total Grupo:", headerColumns3[i + 1] + index, 2U, CellValues.String);
            sheetData.AppendChild(row);

            var1 = String.Format("{0:###,###,##0.00}", feb);
            row = eh.addNewCellToRow(index, row, var1, headerColumns5[i + 8] + index, 2U, CellValues.String);
            sheetData.AppendChild(row);

            var1 = String.Format("{0:###,###,##0.00}", abr);
            row = eh.addNewCellToRow(index, row, var1, headerColumns5[i + 9] + index, 2U, CellValues.String);
            sheetData.AppendChild(row);

            var1 = String.Format("{0:###,###,##0.00}", jun);
            row = eh.addNewCellToRow(index, row, var1, headerColumns5[i + 10] + index, 2U, CellValues.String);
            sheetData.AppendChild(row);

            var1 = String.Format("{0:###,###,##0.00}", ago);
            row = eh.addNewCellToRow(index, row, var1, headerColumns5[i + 11] + index, 2U, CellValues.String);
            sheetData.AppendChild(row);

            var1 = String.Format("{0:###,###,##0.00}", oct);
            row = eh.addNewCellToRow(index, row, var1, headerColumns5[i + 12] + index, 2U, CellValues.String);
            sheetData.AppendChild(row);

            var1 = String.Format("{0:###,###,##0.00}", dic);
            row = eh.addNewCellToRow(index, row, var1, headerColumns5[i + 13] + index, 2U, CellValues.String);
            sheetData.AppendChild(row);

            var1 = String.Format("{0:###,###,##0.00}", feb + abr + jun + ago + oct + dic);
            row = eh.addNewCellToRow(index, row, var1, headerColumns5[i + 14] + index, 2U, CellValues.String);
            sheetData.AppendChild(row);

            return sheetData;
        }


        [HttpGet]
        public void ExcelAnualINFDetCliente(String clienteId, String anio)
        {
            Usuario user = Session["UsuarioData"] as Usuario;

            //Query principal

            int result = db.Database.ExecuteSqlCommand("sp_AmortizacionBimestralINFDet @usuarioId, @clienteId, @anio", new SqlParameter("@usuarioId", user.Id), new SqlParameter("@clienteId", clienteId), new SqlParameter("@anio", anio));

            var reporteAmortizaciónBimDet = from s in db.ReporteConMeses
                                            select s;

            reporteAmortizaciónBimDet = reporteAmortizaciónBimDet.OrderBy(p => p.Cliente.claveSua);

            List<ReporteConMes> allCust = new List<ReporteConMes>();

            allCust = reporteAmortizaciónBimDet.ToList();

            FileStream fileStream = null;
            MemoryStream mem = new MemoryStream();
            try
            {

                DateTime date = DateTime.Now;
                String path = @"C:\\SUA\\Exceles\\";
                String fileName = @"AmortizacionAnualDetalleCliente-" + date.ToString("ddMMyyyyHHmm") + ".xlsx";
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

                SheetData sd = crearContenidoHoja5(allCust, eh);//CreateContentRow(); 
                ws.Append(sd);
                wsp.Worksheet = ws;
                wsp.Worksheet.Save();

                Sheets sheets = new Sheets();
                Sheet sheet = new Sheet();
                sheet.Name = "Sheet1";
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


        public ActionResult AnualCostoSocial(String plazasId, String ejercicioId,
                           String clientesId, String usuarioId)
        {
            Usuario user = Session["UsuarioData"] as Usuario;

            var plazasAsignadas = (from x in db.TopicosUsuarios
                                   where x.usuarioId.Equals(user.Id)
                                   && x.tipo.Equals("P")
                                   select x.topicoId);

            var clientesAsignados = (from x in db.TopicosUsuarios
                                     where x.usuarioId.Equals(user.Id)
                                     && x.tipo.Equals("C")
                                     select x.topicoId);

            //Query principal
            int result = db.Database.ExecuteSqlCommand("sp_CostoSocialAnual @usuarioId", new SqlParameter("@usuarioId", user.Id));
            var reporteAmortizaciónBim = from s in db.ReporteConMeses
                                         join cli in db.Clientes on s.clienteId equals cli.Id
                                         where plazasAsignadas.Contains(s.Cliente.Plaza_id) &&
                                               clientesAsignados.Contains(s.Cliente.Id) &&
                                               s.usuarioId.Equals(user.Id)
                                         select s;

            if (!String.IsNullOrEmpty(clientesId))
            {
                int clienteId = int.Parse(clientesId.Trim());
                reporteAmortizaciónBim = reporteAmortizaciónBim.Where(s => s.clienteId.Equals(clienteId));
            }
            if (!String.IsNullOrEmpty(plazasId))
            {
                int plazaTempId = int.Parse(plazasId.Trim());
                reporteAmortizaciónBim = reporteAmortizaciónBim.Where(s => s.Patrone.Plaza_id.Equals(plazaTempId));
            }
            if (!String.IsNullOrEmpty(ejercicioId))
            {
                reporteAmortizaciónBim = reporteAmortizaciónBim.Where(s => s.anno.Trim().Equals(ejercicioId));
            }

            reporteAmortizaciónBim = reporteAmortizaciónBim.OrderBy(p => p.Patrone.registro);

            return View(reporteAmortizaciónBim.ToList());
        }


        [HttpGet]
        public void ExcelAnualCostoSocial(String plazasId, String ejercicioId, String clientesId, String gruposId)
        {
            Usuario user = Session["UsuarioData"] as Usuario;

            var plazasAsignadas = (from x in db.TopicosUsuarios
                                   where x.usuarioId.Equals(user.Id)
                                   && x.tipo.Equals("P")
                                   select x.topicoId);

            var clientesAsignados = (from x in db.TopicosUsuarios
                                     where x.usuarioId.Equals(user.Id)
                                     && x.tipo.Equals("C")
                                     select x.topicoId);

            var gruposAsignados = (from s in db.Grupos
                                   join cli in db.Clientes on s.Id equals cli.Grupo_id
                                   join top in db.TopicosUsuarios on cli.Id equals top.topicoId
                                   where top.tipo.Trim().Equals("C") && top.usuarioId.Equals(user.Id)
                                   orderby s.claveGrupo
                                   select s.Id);

            //Query principal
            int result = db.Database.ExecuteSqlCommand("sp_CostoSocialAnual @usuarioId", new SqlParameter("@usuarioId", user.Id));
            var reporteAmortizaciónBim = from s in db.ReporteConMeses
                                         join cli in db.Clientes on s.clienteId equals cli.Id
                                         where plazasAsignadas.Contains(s.Cliente.Plaza_id) &&
                                               clientesAsignados.Contains(s.Cliente.Id) &&
                                               s.usuarioId.Equals(user.Id)
                                         select s;

            if (!String.IsNullOrEmpty(clientesId))
            {
                int clienteId = int.Parse(clientesId.Trim());
                reporteAmortizaciónBim = reporteAmortizaciónBim.Where(s => s.clienteId.Equals(clienteId));
            }
            if (!String.IsNullOrEmpty(plazasId))
            {
                int plazaTempId = int.Parse(plazasId.Trim());
                reporteAmortizaciónBim = reporteAmortizaciónBim.Where(s => s.Patrone.Plaza_id.Equals(plazaTempId));
            }
            if (!String.IsNullOrEmpty(ejercicioId))
            {
                reporteAmortizaciónBim = reporteAmortizaciónBim.Where(s => s.anno.Trim().Equals(ejercicioId));
            }

            reporteAmortizaciónBim = reporteAmortizaciónBim.OrderBy(p => p.Cliente.claveSua);

            List<ReporteConMes> allCust = new List<ReporteConMes>();

            allCust = reporteAmortizaciónBim.ToList();

            FileStream fileStream = null;
            MemoryStream mem = new MemoryStream();
            try
            {

                DateTime date = DateTime.Now;
                String path = @"C:\\SUA\\Exceles\\";
                String fileName = @"AnualCostoSocial-" + date.ToString("ddMMyyyyHHmm") + ".xlsx";
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

                SheetData sd = crearContenidoHoja6(allCust, eh);//CreateContentRow(); 
                ws.Append(sd);
                wsp.Worksheet = ws;
                wsp.Worksheet.Save();

                Sheets sheets = new Sheets();
                Sheet sheet = new Sheet();
                sheet.Name = "Sheet1";
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


        string[] headerColumns4 = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD", "AE", "AF", "AG" };
        public SheetData crearContenidoHoja6(List<ReporteConMes> allCust, ExcelHelper eh)
        {
            SheetData sheetData = new SheetData();
            int index = 1;

            //Creamos el Header
            Row row = new Row();

            row = eh.addNewCellToRow(index, row, "Cliente", headerColumns4[0] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Nombre", headerColumns4[1] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "ID. Grupo", headerColumns4[2] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "ID. Plaza", headerColumns4[3] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Año", headerColumns4[4] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Enero", headerColumns4[5] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Febrero", headerColumns4[6] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Marzo", headerColumns4[7] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Abril", headerColumns4[8] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Mayo", headerColumns4[9] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Junio", headerColumns4[10] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Julio", headerColumns4[11] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Agosto", headerColumns4[12] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Septiembre", headerColumns4[13] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Octubre", headerColumns4[14] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Noviembre", headerColumns4[15] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Diciembre", headerColumns4[16] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Total", headerColumns4[17] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            index++;
            //Create the cells that contain the data.
            foreach (ReporteConMes dp in allCust)
            {
                int i = 0;

                row = eh.addNewCellToRow(index, row, dp.Cliente.claveCliente, headerColumns4[i] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Cliente.descripcion, headerColumns4[i + 1] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Cliente.Grupos.claveGrupo, headerColumns4[i + 2] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Cliente.Plaza.cveCorta, headerColumns4[i + 3] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.anno, headerColumns4[i + 4] + index, 2U, CellValues.Number);
                sheetData.AppendChild(row);

                String var1 = String.Format("{0:###,###,##0.00}", dp.enero);
                row = eh.addNewCellToRow(index, row, var1, headerColumns4[i + 5] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.febrero);
                row = eh.addNewCellToRow(index, row, var1, headerColumns4[i + 6] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.marzo);
                row = eh.addNewCellToRow(index, row, var1, headerColumns4[i + 7] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.abril);
                row = eh.addNewCellToRow(index, row, var1, headerColumns4[i + 8] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.mayo);
                row = eh.addNewCellToRow(index, row, var1, headerColumns4[i + 9] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.junio);
                row = eh.addNewCellToRow(index, row, var1, headerColumns4[i + 10] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.julio);
                row = eh.addNewCellToRow(index, row, var1, headerColumns4[i + 11] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.agosto);
                row = eh.addNewCellToRow(index, row, var1, headerColumns4[i + 12] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.septiembre);
                row = eh.addNewCellToRow(index, row, var1, headerColumns4[i + 13] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.octubre);
                row = eh.addNewCellToRow(index, row, var1, headerColumns4[i + 14] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.noviembre);
                row = eh.addNewCellToRow(index, row, var1, headerColumns4[i + 15] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.diciembre);
                row = eh.addNewCellToRow(index, row, var1, headerColumns4[i + 16] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.total);
                row = eh.addNewCellToRow(index, row, var1, headerColumns4[i + 17] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                index++;
            }

            return sheetData;
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
