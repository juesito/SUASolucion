using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SUADATOS;
using System.Data.Entity.Validation;
using System.Text;
using SUAMVC.Helpers;
using SUAMVC.Models;
using System.Web.Helpers;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;

namespace SUAMVC.Controllers
{
    public class SolicitudesBajaController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: SolicitudesBaja
        public ActionResult Index(string clienteId, String folioId, String proyectoId)
        {
            if ((!String.IsNullOrEmpty(clienteId) && !String.IsNullOrEmpty(proyectoId)) || !String.IsNullOrEmpty(folioId))
            {
                ToolsHelper th = new ToolsHelper();
                Concepto tipoSolicitud = th.obtenerConceptoPorGrupo("SOLCON", "baja");
                Usuario usuario = Session["UsuarioData"] as Usuario;

                var solicituds = (from s in db.Solicituds
                                  join top in db.TopicosUsuarios on s.clienteId equals top.topicoId
                                  where top.tipo.Trim().Equals("C") && top.usuarioId.Equals(usuario.Id)
                                    && !s.Concepto.descripcion.Trim().Contains("Baja") && !s.Concepto.descripcion.Trim().Contains("Cancelado")
                                  orderby s.fechaSolicitud descending
                                  select s).ToList();

                if (!String.IsNullOrEmpty(clienteId) && !String.IsNullOrEmpty(proyectoId))
                {

                    ViewBag.clienteId = clienteId;
                    ViewBag.proyectoId = proyectoId;
                    
                    Cliente cliente = db.Clientes.Find(int.Parse(clienteId));
                    Proyecto proyecto = db.Proyectos.Find(int.Parse(proyectoId));

                    if (!cliente.descripcion.ToLower().Contains("seleccion") && 
                        !proyecto.descripcion.ToLower().Contains("seleccion"))
                    {
                        solicituds = solicituds.Where(s => s.clienteId.Equals(int.Parse(clienteId))
                            && s.proyectoId.Equals(int.Parse(proyectoId))).ToList();
                    }

                }// Se va a filtrar por cliente  y proyecto?

                if (!String.IsNullOrEmpty(folioId))
                {
                    solicituds = solicituds.Where(s => s.folioSolicitud.Contains(folioId)).ToList();
                }//Se va a filtrar por folio?

                //Filtrar por el tipo de solicitud=baja
                solicituds = solicituds.Where(s => s.tipoSolicitud.Equals(tipoSolicitud.id)).ToList();
                return View(solicituds.ToList());
            }
            else
            {
                var solicituds = new List<Solicitud>();
                return View(solicituds);
            }

        }
            
        
        // GET: SolicitudesBaja/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Solicitud solicitud = db.Solicituds.Find(id);
            if (solicitud == null)
            {
                return HttpNotFound();
            }
            return View(solicitud);
        }

        // GET: SolicitudesBaja/Create
        public ActionResult Create(int clienteId, int proyectoId)
        {
            ViewBag.estatusSolicitud = new SelectList(db.Conceptos, "id", "grupo");

            Solicitud solicitud = new Solicitud();
            Cliente cliente = db.Clientes.Find(clienteId);
            solicitud.clienteId = clienteId;
            solicitud.proyectoId = proyectoId;
            solicitud.fechaSolicitud = DateTime.Now;
            ListaValidacionCliente lvc = cliente.ListaValidacionClientes.First();
            solicitud.autoriza = lvc.autorizador;
            solicitud.valida = lvc.validador;


            return View(solicitud);
        }

        // POST: SolicitudesBaja/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,folioSolicitud,clienteId,plazaId,fechaSolicitud,fechaBaja,esquemaId,sdiId,contratoId,fechaInicial,fechaFinal,tipoPersonalId,solicita,valida,autoriza,noTrabajadores,observaciones,estatusSolicitud,estatusNomina,estatusAfiliado,estatusJuridico,estatusTarjeta,usuarioId,proyectoId,fechaEnvio")] Solicitud solicitud)
        {
            if (ModelState.IsValid)
            {
                Usuario usuario = Session["usuarioData"] as Usuario;
                Cliente cliente = db.Clientes.Find(solicitud.clienteId);
                ListaValidacionCliente lvc = cliente.ListaValidacionClientes.First();
                ToolsHelper th = new ToolsHelper();
                ParametrosHelper ph = new ParametrosHelper();

                Parametro folioBaja = ph.getParameterByKey("FOLSBAJA");

                Concepto concepto = th.obtenerConceptoPorGrupo("ESTASOL", "apertura");
                Concepto tipoSolicitud = th.obtenerConceptoPorGrupo("SOLCON", "baja");


                solicitud.fechaSolicitud = DateTime.Now;
                solicitud.usuarioId = usuario.Id;
                solicitud.autoriza = lvc.autorizador;
                solicitud.valida = lvc.validador;
                solicitud.solicita = usuario.nombreUsuario;
                solicitud.estatusSolicitud = concepto.id;
                solicitud.estatusNomina = concepto.id;
                solicitud.estatusJuridico = concepto.id;
                solicitud.estatusAfiliado = concepto.id;
                solicitud.estatusTarjeta = concepto.id;
                solicitud.Cliente = cliente;
                solicitud.clienteId = cliente.Id;
                solicitud.folioSolicitud = "";
                solicitud.noTrabajadores = 0;
                solicitud.tipoSolicitud = tipoSolicitud.id;
                db.Solicituds.Add(solicitud);


                try
                {
                    db.SaveChanges();
                    solicitud.folioSolicitud = folioBaja.valorString.Trim().PadLeft(5, '0') + "B" + solicitud.Cliente.Plaza.cveCorta.Trim();
                    int folBaja = int.Parse(folioBaja.valorString.Trim());
                    folBaja = folBaja + 1;
                    folioBaja.valorString = folBaja.ToString();

                    db.Entry(folioBaja).State = EntityState.Modified;
                    db.Entry(solicitud).State = EntityState.Modified;
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
                return RedirectToAction("Index", new { clienteId = solicitud.clienteId, proyectoId = solicitud.proyectoId });
            }

            ViewBag.clienteId = new SelectList(db.Clientes, "Id", "claveCliente", solicitud.clienteId);
            ViewBag.plazaId = new SelectList(db.Plazas, "id", "descripcion", solicitud.plazaId);
            ViewBag.proyectoId = new SelectList(db.Proyectos, "id", "descripcion", solicitud.proyectoId);
            return View(solicitud);
        }

        // GET: SolicitudesBaja/Edit/5
        public ActionResult Edit(int? id, string clienteId, string proyectoId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Solicitud solicitud = db.Solicituds.Find(id);
            if (solicitud == null)
            {
                return HttpNotFound();
            }
            ViewBag.clienteId = new SelectList(db.Clientes, "Id", "claveCliente", solicitud.clienteId);
            ViewBag.plazaId = new SelectList(db.Plazas, "id", "descripcion", solicitud.plazaId);
            ViewBag.proyectoId = new SelectList(db.Proyectos, "id", "descripcion", solicitud.proyectoId);
            return View(solicitud);
        }

        // POST: SolicitudesBaja/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,folioSolicitud,clienteId,plazaId,fechaSolicitud,esquemaId,sdiId,contratoId,fechaInicial,fechaFinal,tipoPersonalId,solicita,valida,autoriza,noTrabajadores,observaciones,estatusSolicitud,estatusNomina,estatusAfiliado,estatusJuridico,estatusTarjeta,usuarioId,proyectoId,fechaEnvio")] Solicitud solicitud, string clienteId, string proyectoId)
        {
                if (ModelState.IsValid)
            {
                solicitud.fechaSolicitud = DateTime.Now;

                db.Entry(solicitud).State = EntityState.Modified;

                try
                {
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
                
                return RedirectToAction("Index", new { clienteId = solicitud.clienteId, proyectoId = solicitud.proyectoId });
            }
            ViewBag.clienteId = new SelectList(db.Clientes, "Id", "claveCliente", solicitud.clienteId);
            ViewBag.plazaId = new SelectList(db.Plazas, "id", "descripcion", solicitud.plazaId);
            ViewBag.proyectoId = new SelectList(db.Proyectos, "id", "descripcion", solicitud.proyectoId);

            ViewBag.clienteId = solicitud.clienteId;
            ViewBag.proyectoId = solicitud.proyectoId;
            return View(solicitud);
        }

        // GET: SolicitudesBaja/Delete/5
        public ActionResult Delete(int? id, string clienteId, string proyectoId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Solicitud solicitud = db.Solicituds.Find(id);

            if (solicitud == null)
            {
                return HttpNotFound();
            }

            ViewBag.clienteId = solicitud.clienteId;
            ViewBag.proyectoId = solicitud.proyectoId;

            return View(solicitud);
        }

        // POST: SolicitudesBaja/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, string clienteId, string proyectoId)
        {
            Solicitud solicitud = db.Solicituds.Find(id);

            if (solicitud.noTrabajadores > 0) {
                List<SolicitudEmpleado> solicitudEmpleados = db.SolicitudEmpleadoes.Where(x => x.solicitudId.Equals(id) && 
                    x.Concepto.descripcion.Trim().Equals("Baja")
                    && x.estatus.Trim().Equals("A")).ToList();

                foreach(SolicitudEmpleado solEmp in solicitudEmpleados){
                    solEmp.estatus = "C";
                    db.Entry(solEmp).State = EntityState.Modified;
                }
            }

            Concepto concepto = db.Conceptos.Where(s => s.grupo.Equals("ESTASOL") && s.descripcion.Equals("Cancelado")).First();
            solicitud.estatusSolicitud = concepto.id;

            db.Entry(solicitud).State = EntityState.Modified;
            db.SaveChanges();

            ViewBag.clienteId = solicitud.clienteId;
            ViewBag.proyectoId = solicitud.proyectoId;
            return RedirectToAction("Index", new { clienteId = solicitud.clienteId, proyectoId = solicitud.proyectoId });
        }

        public ActionResult EnviarSolicitud(string id)
        {

            Solicitud solicitud = new Solicitud();
            if (!String.IsNullOrEmpty(id))
            {
                int idTmp = int.Parse(id);
                solicitud = db.Solicituds.Find(idTmp);
                Concepto concepto = db.Conceptos.Where(s => s.grupo.Equals("ESTASOL") && s.descripcion.Equals("Enviado")).First();
                solicitud.estatusSolicitud = concepto.id;

                Email email = new Email();
                email.enviarPorClienteTipo("B", solicitud.id, true);

                db.Entry(solicitud).State = EntityState.Modified;
                db.SaveChanges();

                TempData["message"] = "Solicitud Enviada Satisfactoriamente.";
            }
            
            return RedirectToAction("Index");
        }

        //Lay out SolicitudBaja
        [HttpGet]
        public void crearExcelSolicitudBaja(string clienteId, string proyectoId)
        {
            ToolsHelper th = new ToolsHelper();
            FileStream fileStream = null;
            MemoryStream mem = new MemoryStream();
            try
            {

                Concepto tipoSolicitud = th.obtenerConceptoPorGrupo("SOLCON", "baja");
                Usuario usuario = Session["UsuarioData"] as Usuario;
                List<Solicitud> solicitudList = new List<Solicitud>();

                int cliente = int.Parse(clienteId);
                int proyecto = int.Parse(proyectoId);

                var solicituds = (from s in db.Solicituds
                                  where s.clienteId.Equals(cliente) && s.proyectoId.Equals(proyecto)
                                  orderby s.fechaSolicitud
                                  select s).ToList();

                solicituds = solicituds.Where(s => s.tipoSolicitud.Equals(tipoSolicitud.id)).ToList();


                DateTime date = DateTime.Now;
                String path = @"C:\\SUA\\Exceles\\";
                String fileName = @"SolicitudBaja-" + date.ToString("ddMMyyyyHHmm") + ".xlsx";
                String fullName = path + fileName;
                solicitudList = solicituds.ToList();

                if (solicitudList.Count() > 0)
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

                    SheetData sd = crearContenidoHojaSolicitudBaja(solicitudList,eh);
                    ws.Append(sd);
                    wsp.Worksheet = ws;
                    wsp.Worksheet.Save();

                    DocumentFormat.OpenXml.Spreadsheet.Sheets sheets = new Sheets();
                    Sheet sheet = new Sheet();
                    sheet.Name = "rptPanelBajaSolicitud";
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
        public SheetData crearContenidoHojaSolicitudBaja(List<Solicitud> solicitudBaja, ExcelHelper eh)
        {

            SheetData sheetData = new SheetData();
            int index = 1;

            //Creamos el Header
            Row row = new Row();

            index = index + 1;
            row = eh.addNewCellToRow(index, row, "SOLICITUDES BAJA", headerColumns[0] + index, 0U, CellValues.String);
            sheetData.AppendChild(row);

            index = index + 2;
            row = eh.addNewCellToRow(index, row, "FOLIO DE SOLICITUD", headerColumns[0] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "PROYECTO", headerColumns[1] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "PLAZA", headerColumns[2] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "FECHA SOLICITUD", headerColumns[3] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "FECHA BAJA", headerColumns[4] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "SOLICITÓ", headerColumns[5] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "OBSERVACIONES", headerColumns[6] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "N° TRABAJADORES", headerColumns[7] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "ESTATUS SOLICITUD", headerColumns[8] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "ESTATUS NÓMINA", headerColumns[9] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "ESTATUS JURÍDICO", headerColumns[10] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "ESTATUS AFILIACIÓN", headerColumns[11] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "ESTATUS TARJETA", headerColumns[12] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);


            //Creamos las celdas que contienen los datos
            foreach (Solicitud dp in solicitudBaja)
            {
                int i = 0;
                index = index + 1;
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

                row = eh.addNewCellToRow(index, row, dp.fechaSolicitud.ToString(), headerColumns[i + 3] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.fechaBaja.ToString(), headerColumns[i + 4] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.solicita, headerColumns[i + 5] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.observaciones, headerColumns[i + 6] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.noTrabajadores.ToString(), headerColumns[i + 7] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.estatusSolicitud.ToString(), headerColumns[i + 8] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Concepto1.descripcion, headerColumns[i + 9] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Concepto2.descripcion, headerColumns[i + 10] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Concepto3.descripcion, headerColumns[i + 11] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Concepto5.descripcion, headerColumns[i + 12] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

            }

            return sheetData;
        }

        public ActionResult SolicitudEmpleado(string solicitudId)
        {

            List<Empleado> listEmpleados = new List<Empleado>();

            @ViewBag.sourceController = "SolicitudesBaja";

            if (!String.IsNullOrEmpty(solicitudId))
            {
                int solicitudIdTemp = int.Parse(solicitudId);
                Solicitud solicitud = db.Solicituds.Find(solicitudIdTemp);
                int proyectoId = solicitud.proyectoId;
                int clienteTempId = solicitud.clienteId;

                TempData["solicitudId"] = solicitudIdTemp;

                //Filtramos solo empleados de solicitudes de alta
                List<Empleado> empleadosList = (from s in db.SolicitudEmpleadoes
                                                join e in db.Empleados on s.empleadoId equals e.id
                                                where s.Solicitud.clienteId.Equals(clienteTempId)
                                                && s.Solicitud.id.Equals(solicitud.id) && 
                                                (s.estatus.Equals("A") || s.estatus.Equals("C")) 
                                                && e.estatus.Equals("A") && s.Solicitud.proyectoId.Equals(proyectoId)
                                                orderby s.id
                                                select s.Empleado).ToList();

                foreach (Empleado emp in empleadosList)
                {
                    listEmpleados.Add(emp);

                }
            }
            

            return View(listEmpleados);
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
