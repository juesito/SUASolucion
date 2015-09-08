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
using System.Web.Helpers;
using SUAMVC.Models;
using SUAMVC.Helpers;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using System.Data.OleDb;


namespace SUAMVC.Controllers
{
    public class SumarizadoClientesController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: SumarizadoClientes
        public ActionResult Index(String plazasId, String patronesId, String periodoId,
            String ejercicioId, String clientesId, String usuarioId)
        {
            SumarizadoClienteModel sumarizadoClienteModel = new SumarizadoClienteModel();
            Usuario user = Session["UsuarioData"] as Usuario;
            ViewBag.filtered = true;


            var plazasAsignadas = (from x in db.TopicosUsuarios
                                   where x.usuarioId.Equals(user.Id)
                                   && x.tipo.Equals("P")
                                   select x.topicoId);

            var patronesAsignados = (from x in db.TopicosUsuarios
                                     where x.usuarioId.Equals(user.Id)
                                     && x.tipo.Equals("B")
                                     select x.topicoId);

            var clientesAsignados = (from x in db.TopicosUsuarios
                                     where x.usuarioId.Equals(user.Id)
                                     && x.tipo.Equals("C")
                                     select x.topicoId);

            //Query principal
            int result = db.Database.ExecuteSqlCommand("sp_SumarizadoClientesTodos @usuarioId", new SqlParameter("@usuarioId", user.Id));
            var sumarizadoClientes = from s in db.SumarizadoClientes
                                     join cli in db.Clientes on s.clienteId equals cli.Id
                                     where plazasAsignadas.Contains(s.Cliente.Plaza_id) &&
                                           clientesAsignados.Contains(s.Cliente.Id) &&
                                           patronesAsignados.Contains(s.patronId) &&
                                           s.usuarioId.Equals(user.Id)
                                     select s;

            //            var sumarizadoClientes = db.SumarizadoClientes.Include(s => s.Cliente).Include(s => s.Patrone).Include(s => s.Usuario);


            if (!String.IsNullOrEmpty(clientesId))
            {
                @ViewBag.cteId = clientesId;
                int clienteId = int.Parse(clientesId.Trim());
                sumarizadoClientes = sumarizadoClientes.Where(s => s.clienteId.Equals(clienteId));
            }
            if (!String.IsNullOrEmpty(plazasId))
            {
                @ViewBag.pzaId = plazasId;
                int plazaTempId = int.Parse(plazasId.Trim());
                sumarizadoClientes = sumarizadoClientes.Where(s => s.Patrone.Plaza_id.Equals(plazaTempId));
            }
            if (!String.IsNullOrEmpty(patronesId))
            {
                @ViewBag.patId = patronesId;
                int patronesTempId = int.Parse(patronesId);
                sumarizadoClientes = sumarizadoClientes.Where(s => s.Patrone.Id.Equals(patronesTempId));
            }
            if (!String.IsNullOrEmpty(periodoId))
            {
                @ViewBag.periodoId = periodoId;
                sumarizadoClientes = sumarizadoClientes.Where(s => s.mes.Trim().Equals(periodoId.Trim()));
            }
            if (!String.IsNullOrEmpty(ejercicioId))
            {
                @ViewBag.ejercicioId = ejercicioId;
                sumarizadoClientes = sumarizadoClientes.Where(s => s.anno.Trim().Equals(ejercicioId));
            }

            sumarizadoClientes = sumarizadoClientes.OrderBy(p => p.Patrone.registro);

            sumarizadoClienteModel.sumarizadoCliente = sumarizadoClientes.ToList();
            SumarizadoAcumulado sa = new SumarizadoAcumulado();

            foreach (SumarizadoCliente sc in sumarizadoClientes)
            {
                sa.sumImss = sa.sumImss + System.Convert.ToDouble(sc.imss);
                sa.sumRcv = sa.sumRcv + System.Convert.ToDouble(sc.rcv);
                sa.sumInfonavit = sa.sumInfonavit + System.Convert.ToDouble(sc.infonavit);
                sa.sumTotal = sa.sumTotal + System.Convert.ToDouble(sc.total);
                sa.sumNt = sa.sumNt + System.Convert.ToDouble(sc.nt);
            }

            sumarizadoClienteModel.sumarizadoAcumulado = sa;

            return View(sumarizadoClienteModel);
        }

        // GET: SumarizadoClientes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SumarizadoCliente sumarizadoCliente = db.SumarizadoClientes.Find(id);
            if (sumarizadoCliente == null)
            {
                return HttpNotFound();
            }
            return View(sumarizadoCliente);
        }

        // GET: SumarizadoClientes/Create
        public ActionResult Create()
        {
            ViewBag.clienteId = new SelectList(db.Clientes, "Id", "claveCliente");
            ViewBag.patronId = new SelectList(db.Patrones, "Id", "registro");
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario");
            return View();
        }

        // POST: SumarizadoClientes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,patronId,anno,mes,imss,rcv,infonavit,total,nt,usuarioId,fechaCreacion,clienteId")] SumarizadoCliente sumarizadoCliente)
        {
            if (ModelState.IsValid)
            {
                db.SumarizadoClientes.Add(sumarizadoCliente);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.clienteId = new SelectList(db.Clientes, "Id", "claveCliente", sumarizadoCliente.clienteId);
            ViewBag.patronId = new SelectList(db.Patrones, "Id", "registro", sumarizadoCliente.patronId);
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", sumarizadoCliente.usuarioId);
            return View(sumarizadoCliente);
        }

        // GET: SumarizadoClientes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SumarizadoCliente sumarizadoCliente = db.SumarizadoClientes.Find(id);
            if (sumarizadoCliente == null)
            {
                return HttpNotFound();
            }
            ViewBag.clienteId = new SelectList(db.Clientes, "Id", "claveCliente", sumarizadoCliente.clienteId);
            ViewBag.patronId = new SelectList(db.Patrones, "Id", "registro", sumarizadoCliente.patronId);
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", sumarizadoCliente.usuarioId);
            return View(sumarizadoCliente);
        }

        // POST: SumarizadoClientes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,patronId,anno,mes,imss,rcv,infonavit,total,nt,usuarioId,fechaCreacion,clienteId")] SumarizadoCliente sumarizadoCliente)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sumarizadoCliente).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.clienteId = new SelectList(db.Clientes, "Id", "claveCliente", sumarizadoCliente.clienteId);
            ViewBag.patronId = new SelectList(db.Patrones, "Id", "registro", sumarizadoCliente.patronId);
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", sumarizadoCliente.usuarioId);
            return View(sumarizadoCliente);
        }

        // GET: SumarizadoClientes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SumarizadoCliente sumarizadoCliente = db.SumarizadoClientes.Find(id);
            if (sumarizadoCliente == null)
            {
                return HttpNotFound();
            }
            return View(sumarizadoCliente);
        }

        // POST: SumarizadoClientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SumarizadoCliente sumarizadoCliente = db.SumarizadoClientes.Find(id);
            db.SumarizadoClientes.Remove(sumarizadoCliente);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        [HttpGet]
        public void makeExcel(String anio, String mes, String idPatron, String idCliente)
        {
            FileStream fileStream = null;
            MemoryStream mem = new MemoryStream();
            try
            {

                int anioTemp = int.Parse(anio.Trim());
                int mesTemp = int.Parse(mes.Trim());
                int idPatronTemp = int.Parse(idPatron.Trim());
                //           int idClienteTemp = int.Parse(idCliente.Trim());

                Pago pago = db.Pagos.Where(p => p.patronId.Equals(idPatronTemp) && p.anno.Equals(anio) && p.mes.Equals(mes)).FirstOrDefault();

                List<DetallePago> detallePago = db.DetallePagoes.Where(r => r.pagoId.Equals(pago.id) && r.Asegurado.Cliente.claveCliente.Trim().Equals(idCliente.Trim())).ToList();

                DateTime date = DateTime.Now;
                String path = @"C:\\SUA\\Exceles\\";
                String fileName = @"DetalleCliente-" + date.ToString("ddMMyyyyHHmm") + ".xlsx";
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

                SheetData sd = crearContenidoHoja(detallePago, eh);//CreateContentRow(); 
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


        string[] headerColumns = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD", "AE", "AF", "AG" };
        public SheetData crearContenidoHoja(List<DetallePago> detallePago, ExcelHelper eh)
        {
            SheetData sheetData = new SheetData();
            int index = 1;

            //Creamos el Header
            Row row = new Row();
            row = eh.addNewCellToRow(index, row, "Patrón", headerColumns[0] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Periodo", headerColumns[1] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Ejercicio", headerColumns[2] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "NSS", headerColumns[3] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Nombre", headerColumns[4] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Ubicación", headerColumns[5] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Días", headerColumns[6] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "S.D.I.", headerColumns[7] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Inc.", headerColumns[8] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Aus.", headerColumns[9] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "C.F.", headerColumns[10] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Ex.P", headerColumns[11] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "EX. O", headerColumns[12] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "PDP", headerColumns[13] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "GMP. Patrón", headerColumns[14] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "GMP. Obrero", headerColumns[15] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "R. T.", headerColumns[16] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "I.V.P", headerColumns[17] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "I.V.O", headerColumns[18] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "G.P.S.", headerColumns[19] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Patronal", headerColumns[20] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Obrera", headerColumns[21] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "IMSS", headerColumns[22] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Dias cotizados Bim", headerColumns[23] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Retiro", headerColumns[24] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Patronal Bim", headerColumns[25] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Obrera Bim", headerColumns[26] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "R.C.V.", headerColumns[27] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Aportación SC", headerColumns[28] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Aportación CC", headerColumns[29] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Amortización", headerColumns[30] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Infonavit", headerColumns[31] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Total", headerColumns[32] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            index++;
            //Create the cells that contain the data.
            foreach (DetallePago dp in detallePago)
            {
                int i = 0;

                row = eh.addNewCellToRow(index, row, dp.Pago.Patrone.registro, headerColumns[i] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Pago.mes, headerColumns[i + 1] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Pago.anno, headerColumns[i + 2] + index, 2U, CellValues.Number);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Asegurado.numeroAfiliacion, headerColumns[i + 3] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Asegurado.nombreTemporal, headerColumns[i + 4] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Asegurado.Cliente.claveCliente, headerColumns[i + 5] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.diasCotizados.ToString(), headerColumns[i + 6] + index, 2U, CellValues.Number);
                sheetData.AppendChild(row);

                String var1 = String.Format("{0:###,###,##0.00}", dp.sdi);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 7] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.diasIncapacidad.ToString(), headerColumns[i + 8] + index, 2U, CellValues.Number);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.diasAusentismo.ToString(), headerColumns[i + 9] + index, 2U, CellValues.Number);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.cuotaFija);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 10] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.expa);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 11] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.exO);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 12] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.pdp);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 13] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.gmpp);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 14] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.gmpo);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 15] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.rt);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 16] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.ivp);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 17] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.ivo);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 18] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.gps);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 19] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.patronal);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 20] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.obrera);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 21] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.imss);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 22] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.diasCotizBim.ToString(), headerColumns[i + 23] + index, 2U, CellValues.Number);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.retiro);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 24] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.patronalBimestral);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 25] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.obreraBimestral);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 26] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.rcv);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 27] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.aportacionsc);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 28] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.aportacioncc);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 29] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.amortizacion);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 30] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.infonavit);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 31] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.total);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 32] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                index++;
            }

            return sheetData;
        }


        public ActionResult ResumenPagosINF(String plazasId, String patronesId, String periodoId,
                                            String ejercicioId, String clientesId, String gruposId)
        {
            SumarizadoClienteModel sumarizadoClienteModel = new SumarizadoClienteModel();
            Usuario user = Session["UsuarioData"] as Usuario;
            ViewBag.filtered = true;


            var plazasAsignadas = (from x in db.TopicosUsuarios
                                   where x.usuarioId.Equals(user.Id)
                                   && x.tipo.Equals("P")
                                   select x.topicoId);

            var patronesAsignados = (from x in db.TopicosUsuarios
                                     where x.usuarioId.Equals(user.Id)
                                     && x.tipo.Equals("B")
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
            int result = db.Database.ExecuteSqlCommand("sp_ResumenPagosINF @usuarioId", new SqlParameter("@usuarioId", user.Id));
            var sumarizadoClientes = from s in db.SumarizadoClientes
                                     join cli in db.Clientes on s.clienteId equals cli.Id
                                     where plazasAsignadas.Contains(s.Cliente.Plaza_id) &&
                                           clientesAsignados.Contains(s.Cliente.Id) &&
                                           patronesAsignados.Contains(s.patronId) &&
                                           gruposAsignados.Contains(s.Cliente.Grupo_id) &&
                                           s.usuarioId.Equals(user.Id) 
                                     select s;

            if (!String.IsNullOrEmpty(clientesId))
            {
                @ViewBag.cteId = clientesId;
                int clienteId = int.Parse(clientesId.Trim());
                sumarizadoClientes = sumarizadoClientes.Where(s => s.clienteId.Equals(clienteId));
            }
            if (!String.IsNullOrEmpty(gruposId))
            {
                @ViewBag.gpoId = gruposId;
                int grupoId = int.Parse(gruposId.Trim());
                sumarizadoClientes = sumarizadoClientes.Where(s => s.Cliente.Grupo_id.Equals(grupoId));
            }
            if (!String.IsNullOrEmpty(plazasId))
            {
                @ViewBag.pzaId = plazasId;
                int plazaTempId = int.Parse(plazasId.Trim());
                sumarizadoClientes = sumarizadoClientes.Where(s => s.Patrone.Plaza_id.Equals(plazaTempId));
            }
            if (!String.IsNullOrEmpty(patronesId))
            {
                @ViewBag.patId = patronesId;
                int patronesTempId = int.Parse(patronesId);
                sumarizadoClientes = sumarizadoClientes.Where(s => s.Patrone.Id.Equals(patronesTempId));
            }
            if (!String.IsNullOrEmpty(periodoId))
            {
                @ViewBag.periodoId = periodoId;
                sumarizadoClientes = sumarizadoClientes.Where(s => s.mes.Trim().Equals(periodoId.Trim()));
            }
            if (!String.IsNullOrEmpty(ejercicioId))
            {
                @ViewBag.ejercicioId = ejercicioId;
                sumarizadoClientes = sumarizadoClientes.Where(s => s.anno.Trim().Equals(ejercicioId));
            }

            sumarizadoClientes = sumarizadoClientes.OrderBy(p => p.Patrone.registro);

            sumarizadoClienteModel.sumarizadoCliente = sumarizadoClientes.ToList();
            SumarizadoAcumulado sa = new SumarizadoAcumulado();

            foreach (SumarizadoCliente sc in sumarizadoClientes)
            {
                sa.sumImss = sa.sumImss + System.Convert.ToDouble(sc.imss);
                //sa.sumRcv = sa.sumRcv + System.Convert.ToDouble(sc.rcv);
                //sa.sumInfonavit = sa.sumInfonavit + System.Convert.ToDouble(sc.infonavit);
                //sa.sumTotal = sa.sumTotal + System.Convert.ToDouble(sc.total);
                sa.sumNt = sa.sumNt + System.Convert.ToDouble(sc.nt);
            }

            sumarizadoClienteModel.sumarizadoAcumulado = sa;

            return View(sumarizadoClienteModel);
        }


        [HttpGet]
        public void excelResumenPagosDeatlleINF(String anio, String mes, String idPatron, String idCliente)
        {
            FileStream fileStream = null;
            MemoryStream mem = new MemoryStream();
            try
            {

                int idPatronTemp = int.Parse(idPatron.Trim());

                Pago pago = db.Pagos.Where(p => p.patronId.Equals(idPatronTemp) && p.anno.Equals(anio) && p.mes.Equals(mes)).FirstOrDefault();

                List<DetallePago> detallePago = db.DetallePagoes.Where(r => r.pagoId.Equals(pago.id) && r.Asegurado.Cliente.claveCliente.Equals(idCliente) && !r.Asegurado.paginaInfo.Equals("") && r.amortizacion != 0).ToList();

                DateTime date = DateTime.Now;
                String path = @"C:\\SUA\\Exceles\\";
                String fileName = @"DetalleResumenPagos-" + date.ToString("ddMMyyyyHHmm") + ".xlsx";
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

                SheetData sd = crearContenidoHoja2(detallePago, eh);//CreateContentRow(); 
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


        string[] headerColumns2 = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD", "AE", "AF", "AG" };
        public SheetData crearContenidoHoja2(List<DetallePago> detallePago, ExcelHelper eh)
        {
            SheetData sheetData = new SheetData();
            int index = 1;

            //Creamos el Header
            Row row = new Row();
            row = eh.addNewCellToRow(index, row, "Patrón", headerColumns2[0] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Periodo", headerColumns2[1] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Ejercicio", headerColumns2[2] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "NSS", headerColumns2[3] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "N. Crédito", headerColumns2[4] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Nombre", headerColumns2[5] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Ubicación", headerColumns2[6] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Días", headerColumns2[7] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "S.D.I.", headerColumns2[8] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Amortización", headerColumns2[9] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            index++;
            //Create the cells that contain the data.
            foreach (DetallePago dp in detallePago)
            {
                int i = 0;

                row = eh.addNewCellToRow(index, row, dp.Pago.Patrone.registro, headerColumns[i] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Pago.mes, headerColumns2[i + 1] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Pago.anno, headerColumns2[i + 2] + index, 2U, CellValues.Number);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Asegurado.numeroAfiliacion, headerColumns2[i + 3] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Asegurado.paginaInfo, headerColumns2[i + 4] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Asegurado.nombreTemporal, headerColumns2[i + 5] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Asegurado.Cliente.claveCliente, headerColumns2[i + 6] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.diasCotizados.ToString(), headerColumns2[i + 7] + index, 2U, CellValues.Number);
                sheetData.AppendChild(row);

                String var1 = String.Format("{0:###,###,##0.00}", dp.sdi);
                row = eh.addNewCellToRow(index, row, var1, headerColumns2[i + 8] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.amortizacion);
                row = eh.addNewCellToRow(index, row, var1, headerColumns2[i + 9] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                index++;
            }

            return sheetData;
        }

        [HttpGet]
        public void GetExcel(String plazasId, String patronesId, String periodoId,
            String ejercicioId, String clientesId, String usuarioId)
        {
            int userId = int.Parse(usuarioId.Trim());

            var plazasAsignadas = (from x in db.TopicosUsuarios
                                   where x.usuarioId.Equals(userId)
                                   && x.tipo.Equals("P")
                                   select x.topicoId);

            var patronesAsignados = (from x in db.TopicosUsuarios
                                     where x.usuarioId.Equals(userId)
                                     && x.tipo.Equals("B")
                                     select x.topicoId);

            var clientesAsignados = (from x in db.TopicosUsuarios
                                     where x.usuarioId.Equals(userId)
                                     && x.tipo.Equals("C")
                                     select x.topicoId);

            //Query principal
            int result = db.Database.ExecuteSqlCommand("sp_SumarizadoClientesTodos @usuarioId", new SqlParameter("@usuarioId", userId));
            var sumarizadoClientes = from s in db.SumarizadoClientes
                                     join cli in db.Clientes on s.clienteId equals cli.Id
                                     where plazasAsignadas.Contains(s.Cliente.Plaza_id) &&
                                           clientesAsignados.Contains(s.Cliente.Id) &&
                                           patronesAsignados.Contains(s.patronId) &&
                                           s.usuarioId.Equals(userId)
                                     select s;

            //            var sumarizadoClientes = db.SumarizadoClientes.Include(s => s.Cliente).Include(s => s.Patrone).Include(s => s.Usuario);


            if (!String.IsNullOrEmpty(clientesId))
            {
                @ViewBag.cteId = clientesId;
                int clienteId = int.Parse(clientesId.Trim());
                sumarizadoClientes = sumarizadoClientes.Where(s => s.clienteId.Equals(clienteId));
            }
            if (!String.IsNullOrEmpty(plazasId))
            {
                @ViewBag.pzaId = plazasId;
                int plazaTempId = int.Parse(plazasId.Trim());
                sumarizadoClientes = sumarizadoClientes.Where(s => s.Patrone.Plaza_id.Equals(plazaTempId));
            }
            if (!String.IsNullOrEmpty(patronesId))
            {
                @ViewBag.patId = patronesId;
                int patronesTempId = int.Parse(patronesId);
                sumarizadoClientes = sumarizadoClientes.Where(s => s.Patrone.Id.Equals(patronesTempId));
            }
            if (!String.IsNullOrEmpty(periodoId))
            {
                @ViewBag.periodoId = periodoId;
                sumarizadoClientes = sumarizadoClientes.Where(s => s.mes.Trim().Equals(periodoId.Trim()));
            }
            if (!String.IsNullOrEmpty(ejercicioId))
            {
                @ViewBag.ejercicioId = ejercicioId;
                sumarizadoClientes = sumarizadoClientes.Where(s => s.anno.Trim().Equals(ejercicioId));
            }

            sumarizadoClientes = sumarizadoClientes.OrderBy(p => p.Patrone.registro);

            List<SumarizadoCliente> allCust = new List<SumarizadoCliente>();

            allCust = sumarizadoClientes.ToList();

            FileStream fileStream = null;
            MemoryStream mem = new MemoryStream();
            try
            {

                DateTime date = DateTime.Now;
                String path = @"C:\\SUA\\Exceles\\";
                String fileName = @"ClientesPagos-" + date.ToString("ddMMyyyyHHmm") + ".xlsx";
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

                SheetData sd = crearContenidoHoja3(allCust, eh);//CreateContentRow(); 
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
        public SheetData crearContenidoHoja3(List<SumarizadoCliente> allCust, ExcelHelper eh)
        {
            SheetData sheetData = new SheetData();
            int index = 1;

            //Creamos el Header
            Row row = new Row();
            row = eh.addNewCellToRow(index, row, "Patrón", headerColumns3[0] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Año", headerColumns3[1] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Mes", headerColumns3[2] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "ID. Cliente", headerColumns3[3] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Nombre", headerColumns3[4] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "ID. Grupo", headerColumns3[5] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "IMSS", headerColumns3[6] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "RCV", headerColumns3[7] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Infonavit.", headerColumns3[8] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Total.", headerColumns3[9] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "NT", headerColumns3[10] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Plaza", headerColumns3[11] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            index++;
            //Create the cells that contain the data.
            foreach (SumarizadoCliente dp in allCust)
            {
                int i = 0;

                row = eh.addNewCellToRow(index, row, dp.Patrone.registro, headerColumns3[i] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.anno, headerColumns3[i + 1] + index, 2U, CellValues.Number);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.mes, headerColumns3[i + 2] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Cliente.claveCliente, headerColumns3[i + 3] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Cliente.descripcion, headerColumns3[i + 4] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Cliente.Grupos.claveGrupo, headerColumns3[i + 5] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                String var1 = String.Format("{0:###,###,##0.00}", dp.imss);
                row = eh.addNewCellToRow(index, row, var1, headerColumns3[i + 6] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.rcv);
                row = eh.addNewCellToRow(index, row, var1, headerColumns3[i + 7] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.infonavit);
                row = eh.addNewCellToRow(index, row, var1, headerColumns3[i + 8] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.total);
                row = eh.addNewCellToRow(index, row, var1, headerColumns3[i + 9] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.nt);
                row = eh.addNewCellToRow(index, row, var1, headerColumns3[i + 10] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Patrone.Plaza.cveCorta, headerColumns3[i + 11] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                index++;
            }

            return sheetData;
        }

        [HttpGet]
        public void ExcelResumenPagosINF(String plazasId, String patronesId, String periodoId,
                                      String ejercicioId, String clientesId, String gruposId)
        {
            Usuario user = Session["UsuarioData"] as Usuario;

            var plazasAsignadas = (from x in db.TopicosUsuarios
                                   where x.usuarioId.Equals(user.Id)
                                   && x.tipo.Equals("P")
                                   select x.topicoId);

            var patronesAsignados = (from x in db.TopicosUsuarios
                                     where x.usuarioId.Equals(user.Id)
                                     && x.tipo.Equals("B")
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
            int result = db.Database.ExecuteSqlCommand("sp_ResumenPagosINF @usuarioId", new SqlParameter("@usuarioId", user.Id));
            var sumarizadoClientes = from s in db.SumarizadoClientes
                                     join cli in db.Clientes on s.clienteId equals cli.Id
                                     where plazasAsignadas.Contains(s.Cliente.Plaza_id) &&
                                           clientesAsignados.Contains(s.Cliente.Id) &&
                                           patronesAsignados.Contains(s.patronId) &&
                                           gruposAsignados.Contains(s.Cliente.Grupo_id) &&
                                           s.usuarioId.Equals(user.Id) 
                                     select s;

            if (!String.IsNullOrEmpty(clientesId))
            {
                int clienteId = int.Parse(clientesId.Trim());
                sumarizadoClientes = sumarizadoClientes.Where(s => s.clienteId.Equals(clienteId));
            }
            if (!String.IsNullOrEmpty(gruposId))
            {
                int grupoId = int.Parse(gruposId.Trim());
                sumarizadoClientes = sumarizadoClientes.Where(s => s.Cliente.Grupo_id.Equals(grupoId));
            }
            if (!String.IsNullOrEmpty(plazasId))
            {
                int plazaTempId = int.Parse(plazasId.Trim());
                sumarizadoClientes = sumarizadoClientes.Where(s => s.Patrone.Plaza_id.Equals(plazaTempId));
            }
            if (!String.IsNullOrEmpty(patronesId))
            {
                int patronesTempId = int.Parse(patronesId);
                sumarizadoClientes = sumarizadoClientes.Where(s => s.Patrone.Id.Equals(patronesTempId));
            }
            if (!String.IsNullOrEmpty(periodoId))
            {
                sumarizadoClientes = sumarizadoClientes.Where(s => s.mes.Trim().Equals(periodoId.Trim()));
            }
            if (!String.IsNullOrEmpty(ejercicioId))
            {
                sumarizadoClientes = sumarizadoClientes.Where(s => s.anno.Trim().Equals(ejercicioId));
            }

            sumarizadoClientes = sumarizadoClientes.OrderBy(p => p.Patrone.registro);

            List<SumarizadoCliente> allCust = new List<SumarizadoCliente>();

            allCust = sumarizadoClientes.ToList();

            FileStream fileStream = null;
            MemoryStream mem = new MemoryStream();
            try
            {

                DateTime date = DateTime.Now;
                String path = @"C:\\SUA\\Exceles\\";
                String fileName = @"ResumenPagos-" + date.ToString("ddMMyyyyHHmm") + ".xlsx";
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


        string[] headerColumns4 = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD", "AE", "AF", "AG" };
        public SheetData crearContenidoHoja4(List<SumarizadoCliente> allCust, ExcelHelper eh)
        {
            SheetData sheetData = new SheetData();
            int index = 1;

            //Creamos el Header
            Row row = new Row();
            row = eh.addNewCellToRow(index, row, "Patrón", headerColumns3[0] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Año", headerColumns3[1] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Mes", headerColumns3[2] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "ID. Cliente", headerColumns3[3] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Nombre", headerColumns3[4] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "ID. Grupo", headerColumns3[5] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Amortización", headerColumns3[6] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "NT", headerColumns3[7] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Plaza", headerColumns3[8] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            index++;
            //Create the cells that contain the data.
            foreach (SumarizadoCliente dp in allCust)
            {
                int i = 0;

                row = eh.addNewCellToRow(index, row, dp.Patrone.registro, headerColumns3[i] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.anno, headerColumns3[i + 1] + index, 2U, CellValues.Number);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.mes, headerColumns3[i + 2] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Cliente.claveCliente, headerColumns3[i + 3] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Cliente.descripcion, headerColumns3[i + 4] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Cliente.Grupos.claveGrupo, headerColumns3[i + 5] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                String var1 = String.Format("{0:###,###,##0.00}", dp.imss);
                row = eh.addNewCellToRow(index, row, var1, headerColumns3[i + 6] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.nt);
                row = eh.addNewCellToRow(index, row, var1, headerColumns3[i + 7] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Patrone.Plaza.cveCorta, headerColumns3[i + 8] + index, 2U, CellValues.String);
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
