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
using SUAMVC.Models;
using System.Text;
using System.Data.Entity.Validation;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;

namespace SUAMVC.Controllers
{
    public class SolicitudPrenominasController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: SolicitudPrenominas
        public ActionResult Index(String clienteId, String proyectoId, String ejercicioId, String plazaId)
        {

            if (!String.IsNullOrEmpty(clienteId) && !String.IsNullOrEmpty(proyectoId) && !String.IsNullOrEmpty(ejercicioId)
                 && !String.IsNullOrEmpty(plazaId))
            {

                ViewBag.proyectoId = proyectoId;
                ViewBag.ejercicioId = ejercicioId;
                ViewBag.clienteId = clienteId;
                ViewBag.plazaId = plazaId;

                var solicitudPrenominas = db.SolicitudPrenominas.Include(s => s.Cliente).
                    Include(s => s.Concepto).Include(s => s.Concepto1).
                    Include(s => s.Concepto2).
                    Include(s => s.Plaza).Include(s => s.Usuario);

                    int proyectoInt = int.Parse(proyectoId.Trim());
                    int clienteInt = int.Parse(clienteId.Trim());
                    int plazaInt = int.Parse(plazaId.Trim());
                    solicitudPrenominas = solicitudPrenominas.Where(x => x.clienteId.Equals(clienteInt) &&
                        x.proyectoId.Equals(proyectoInt) && x.anno.Equals(ejercicioId) && x.plazaId.Equals(plazaInt)).
                        OrderBy(x => x.fechaSolicitud);

                return View(solicitudPrenominas.ToList());
            }
            else
            {
                return View(new List<SolicitudPrenomina>());
            }
        }

        // GET: SolicitudPrenominas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SolicitudPrenomina solicitudPrenomina = db.SolicitudPrenominas.Find(id);
            if (solicitudPrenomina == null)
            {
                return HttpNotFound();
            }
            return View(solicitudPrenomina);
        }

        // GET: SolicitudPrenominas/Create
        public ActionResult Create(String clienteId, String proyectoId, String ejercicioId, String plazaId)
        {
            SolicitudPrenomina solicitudPrenomina = new SolicitudPrenomina();
            if (!String.IsNullOrEmpty(ejercicioId) && !String.IsNullOrEmpty(clienteId) && !String.IsNullOrEmpty(proyectoId)
                && !String.IsNullOrEmpty(plazaId))
            {
                //Agregamos los valores a nuestra solicitud.
                DateTime now = DateTime.Now;
                solicitudPrenomina.clienteId = int.Parse(clienteId);
                solicitudPrenomina.proyectoId = int.Parse(proyectoId);
                solicitudPrenomina.plazaId = int.Parse(plazaId);
                solicitudPrenomina.anno = ejercicioId;
                solicitudPrenomina.fechaSolicitud = now;
                solicitudPrenomina.fechaInicial = now;
                solicitudPrenomina.fechaFinal = now;
                solicitudPrenomina.fechaPago = now;

                return View(solicitudPrenomina);
            }

            return RedirectToAction("Index", "SolicitudPrenomina");
        }

        // POST: SolicitudPrenominas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,folioSolicitud,clienteId,plazaId,fechaSolicitud,fechaInicial,fechaFinal,fechaPago,folioCliente,tipoPagoId,periodoId,tipoContratoId,monedaId,tipoCambio,solicita,valida,autoriza,noTrabajadores,observaciones,estatusSolicitud,proyectoId,anno,fechaEnvio,usuarioId")] SolicitudPrenomina solicitudPrenomina)
        {

            if (ModelState.IsValid)
            {

                Usuario usuario = Session["usuarioData"] as Usuario;
                Cliente cliente = db.Clientes.Find(solicitudPrenomina.clienteId);

                ToolsHelper th = new ToolsHelper();
                ParametrosHelper ph = new ParametrosHelper();
                ListaValidacionCliente lvc = cliente.ListaValidacionClientes.First();

                //Parametro de folios de solicitud de prenomina
                Parametro folioAlta = ph.getParameterByKey("FOLSPALTA");
                //Concepto del estatus de la solicitud.
                Concepto concepto = th.obtenerConceptoPorGrupo("ESTASOL", "apertura");

                //Asignamos los valores de nuestra solicitud.
                solicitudPrenomina.fechaSolicitud = DateTime.Now;
                solicitudPrenomina.noTrabajadores = 0;
                solicitudPrenomina.autoriza = lvc.autorizador;
                solicitudPrenomina.valida = lvc.validador;
                solicitudPrenomina.usuarioId = usuario.Id;
                solicitudPrenomina.estatusSolicitud = concepto.id;
                solicitudPrenomina.solicita = usuario.nombreUsuario.Trim();
                solicitudPrenomina.folioSolicitud = folioAlta.valorString.Trim().PadLeft(5, '0') + "P" + cliente.Plaza.cveCorta.Trim();

                if (solicitudPrenomina.tipoCambio == null)
                {
                    solicitudPrenomina.tipoCambio = 1;
                } //Tipo de cambio es nullo se toma la paridad.

                try
                {
                    //Guardamos la solicitud de prenomina.
                    db.SolicitudPrenominas.Add(solicitudPrenomina);
                    db.SaveChanges();

                    //Actualizamos el folio
                    int folAlta = int.Parse(folioAlta.valorString.Trim());
                    folAlta = folAlta + 1;
                    folioAlta.valorString = folAlta.ToString();

                    db.Entry(folioAlta).State = EntityState.Modified;
                    db.SaveChanges();

                }
                catch (DbEntityValidationException ex)
                {
                    StringBuilder sb = new StringBuilder();

                    foreach (var failure in ex.EntityValidationErrors)
                    {
                        sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
                        foreach (var error in failure.ValidationErrors)
                        {
                            sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                            sb.AppendLine();
                        }
                    }
                }
                return RedirectToAction("Index", new
                {
                    clienteId = solicitudPrenomina.clienteId,
                    proyectoId = solicitudPrenomina.proyectoId,
                    ejercicioId = solicitudPrenomina.anno,
                    plazaId = solicitudPrenomina.plazaId
                });
            }

            ViewBag.clienteId = new SelectList(db.Clientes, "Id", "claveCliente", solicitudPrenomina.clienteId);
            ViewBag.periodoId = new SelectList(db.Conceptos, "id", "grupo", solicitudPrenomina.periodoId);
            return View(solicitudPrenomina);
        }

        // GET: SolicitudPrenominas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SolicitudPrenomina solicitudPrenomina = db.SolicitudPrenominas.Find(id);
            if (solicitudPrenomina == null)
            {
                return HttpNotFound();
            }
            ViewBag.clienteId = new SelectList(db.Clientes, "Id", "claveCliente", solicitudPrenomina.clienteId);
            ViewBag.tipoPagoId = new SelectList(db.Conceptos, "id", "grupo", solicitudPrenomina.tipoPagoId);
            ViewBag.periodoId = new SelectList(db.Conceptos, "id", "grupo", solicitudPrenomina.periodoId);
            ViewBag.tipoContratoId = new SelectList(db.Conceptos, "id", "grupo", solicitudPrenomina.tipoContratoId);
            ViewBag.monedaId = new SelectList(db.Conceptos, "id", "grupo", solicitudPrenomina.monedaId);
            ViewBag.plazaId = new SelectList(db.Plazas, "id", "descripcion", solicitudPrenomina.plazaId);
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", solicitudPrenomina.usuarioId);
            return View(solicitudPrenomina);
        }

        // POST: SolicitudPrenominas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,folioSolicitud,clienteId,plazaId,fechaSolicitud,fechaInicial,fechaFinal,fechaPago,folioCliente,tipoPagoId,periodoId,tipoContratoId,monedaId,tipoCambio,solicita,valida,autoriza,noTrabajadores,observaciones,estatusSolicitud,proyectoId,anno,fechaEnvio,usuarioId")] SolicitudPrenomina solicitudPrenomina)
        {
            if (ModelState.IsValid)
            {
                db.Entry(solicitudPrenomina).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.clienteId = new SelectList(db.Clientes, "Id", "claveCliente", solicitudPrenomina.clienteId);
            ViewBag.tipoPagoId = new SelectList(db.Conceptos, "id", "grupo", solicitudPrenomina.tipoPagoId);
            ViewBag.periodoId = new SelectList(db.Conceptos, "id", "grupo", solicitudPrenomina.periodoId);
            ViewBag.tipoContratoId = new SelectList(db.Conceptos, "id", "grupo", solicitudPrenomina.tipoContratoId);
            ViewBag.monedaId = new SelectList(db.Conceptos, "id", "grupo", solicitudPrenomina.monedaId);
            ViewBag.plazaId = new SelectList(db.Plazas, "id", "descripcion", solicitudPrenomina.plazaId);
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", solicitudPrenomina.usuarioId);
            return View(solicitudPrenomina);
        }

        // GET: SolicitudPrenominas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SolicitudPrenomina solicitudPrenomina = db.SolicitudPrenominas.Find(id);
            if (solicitudPrenomina == null)
            {
                return HttpNotFound();
            }
            return View(solicitudPrenomina);
        }

        // POST: SolicitudPrenominas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SolicitudPrenomina solicitudPrenomina = db.SolicitudPrenominas.Find(id);
            db.SolicitudPrenominas.Remove(solicitudPrenomina);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //Lay out Solicitud Prenomina
        [HttpGet]
        public void crearExcelSolicitudPrenomina(string clienteId, string proyectoId, string plazaId, string ejercicioId)
        {
            ToolsHelper th = new ToolsHelper();
            FileStream fileStream = null;
            MemoryStream mem = new MemoryStream();
            try
            {

                Usuario usuario = Session["UsuarioData"] as Usuario;
                List<SolicitudPrenomina> solicitudPrenominasList = new List<SolicitudPrenomina>();

                int proyectoInt = int.Parse(proyectoId.Trim());
                int clienteInt = int.Parse(clienteId.Trim());
                int plazaInt = int.Parse(plazaId.Trim());


                var solicitudPrenominas = db.SolicitudPrenominas.Where(x => x.clienteId.Equals(clienteInt) &&
                                            x.proyectoId.Equals(proyectoInt) && x.anno.Equals(ejercicioId) && 
                                            x.plazaId.Equals(plazaInt)).ToList();

                
                DateTime date = DateTime.Now;
                String path = @"C:\\SUA\\Exceles\\";
                String fileName = @"SolicitudPrenomina-" + date.ToString("ddMMyyyyHHmm") + ".xlsx";
                String fullName = path + fileName;
                solicitudPrenominasList = solicitudPrenominas.ToList(); 
                
                if (solicitudPrenominasList.Count() > 0)
                {

                    ExcelHelper eh = new ExcelHelper();
                    //Creamos el objeto del workbook
                    SpreadsheetDocument xl = SpreadsheetDocument.Create(fullName, SpreadsheetDocumentType.Workbook);

                    WorkbookPart wbp = xl.AddWorkbookPart();
                    WorksheetPart wsp = wbp.AddNewPart<WorksheetPart>();
                    DocumentFormat.OpenXml.Spreadsheet.Workbook wb = new Workbook();
                    FileVersion fv = new FileVersion();
                    fv.ApplicationName = "Microsoft Office Excel";

                    Worksheet ws = new Worksheet();
                    WorkbookStylesPart wbsp = wbp.AddNewPart<WorkbookStylesPart>();
                    // add styles to sheet
                    wbsp.Stylesheet = eh.CreateStylesheet();
                    wbsp.Stylesheet.Save();

                    SheetData sd = crearContenidoHojaSolicitudPrenomina(solicitudPrenominasList, eh);
                    ws.Append(sd);
                    wsp.Worksheet = ws;
                    wsp.Worksheet.Save();

                    DocumentFormat.OpenXml.Spreadsheet.Sheets sheets = new Sheets();
                    Sheet sheet = new Sheet();
                    sheet.Name = "rptPanelSolicitudPrenomina";
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
        public SheetData crearContenidoHojaSolicitudPrenomina(List<SolicitudPrenomina> SolicitudPrenominas, ExcelHelper eh)
        {

            SheetData sheetData = new SheetData();
            int index = 1;

            //Creamos el Header
            Row row = new Row();

            index = index + 1;
            row = eh.addNewCellToRow(index, row, "SOLICITUDES PRENOMINA", headerColumns[0] + index, 0U, CellValues.String);
            sheetData.AppendChild(row);

            index = index + 2;
            row = eh.addNewCellToRow(index, row, "FOLIO DE SOLICITUD", headerColumns[0] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "PROYECTO", headerColumns[1] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "PLAZA", headerColumns[2] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "FECHA INICIO", headerColumns[3] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "FECHA FINAL", headerColumns[4] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "SOLICITÓ", headerColumns[5] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "OBSERVACIONES", headerColumns[6] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "N° TRABAJADORES", headerColumns[7] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            //Creamos las celdas que contienen los datos
            foreach (SolicitudPrenomina dp in SolicitudPrenominas)
            {
                int i = 0;
                index++;
                row = eh.addNewCellToRow(index, row, dp.folioSolicitud, headerColumns[i] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                if (dp.Proyecto != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.Proyecto.descripcion, headerColumns[i + 1] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 1] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                row = eh.addNewCellToRow(index, row, dp.Plaza.descripcion.ToString(), headerColumns[i + 2] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.fechaInicial.ToString(), headerColumns[i + 3] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.fechaFinal.ToString(), headerColumns[i + 4] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.solicita, headerColumns[i + 5] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.observaciones, headerColumns[i + 6] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.noTrabajadores.ToString(), headerColumns[i + 7] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);


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
