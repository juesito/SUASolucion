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
    public class SolicitudesController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: Solicitudes
        public ActionResult Index(String clienteId, String proyectoId, String folioId)
        {

            if ((!String.IsNullOrEmpty(clienteId) && !String.IsNullOrEmpty(proyectoId)) || !String.IsNullOrEmpty(folioId))
            {

                ToolsHelper th = new ToolsHelper();
                Concepto tipoSolicitud = th.obtenerConceptoPorGrupo("SOLCON", "alta");
                Usuario usuario = Session["UsuarioData"] as Usuario;

                //Buscamos las solicitudes que puede ver ese usuario
                //de acuerdo a sus clientes permitidos
                var solicituds = (from s in db.Solicituds
                                  join top in db.TopicosUsuarios on s.clienteId equals top.topicoId
                                  where top.tipo.Trim().Equals("C") && top.usuarioId.Equals(usuario.Id)
                                  orderby s.fechaSolicitud
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

                //Filtrar por el tipo de solicitud=alta
                solicituds = solicituds.Where(s => s.tipoSolicitud.Equals(tipoSolicitud.id)).ToList();
                return View(solicituds.ToList());
            }
            else
            {
                var solicituds = new List<Solicitud>();
                return View(solicituds);
            }

        }

        // GET: Solicitudes/Details/5
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

        // GET: Solicitudes/Create
        public ActionResult Create(int clienteId, int proyectoId)
        {
            Solicitud solicitud = new Solicitud();
            Cliente cliente = db.Clientes.Find(clienteId);
            solicitud.clienteId = clienteId;
            solicitud.proyectoId = proyectoId;
            solicitud.fechaSolicitud = DateTime.Now;
            ListaValidacionCliente lvc = cliente.ListaValidacionClientes.First();
            solicitud.autoriza = lvc.autorizador;
            solicitud.valida = lvc.validador;


            ViewBag.estatusSolicitud = new SelectList(db.Conceptos, "id", "grupo");
            ViewBag.estatusNomina = new SelectList(db.Conceptos, "id", "grupo");
            ViewBag.estatusJuridico = new SelectList(db.Conceptos, "id", "grupo");
            ViewBag.estatusAfiliado = new SelectList(db.Conceptos, "id", "grupo");
            ViewBag.estatusTarjeta = new SelectList(db.Conceptos, "id", "grupo");
            ViewBag.esquemaId = new SelectList(db.EsquemasPagoes, "id", "descripcion");
            ViewBag.sdiId = new SelectList(db.SDIs, "id", "descripcion");
            ViewBag.contratoId = new SelectList(db.TipoContratoes, "id", "descripcion");
            ViewBag.tipoPersonalId = new SelectList(db.TipoPersonals, "id", "descripcion");
            return View(solicitud);
        }

        // POST: Solicitudes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,folioSolicitud,clienteId,plazaId,fechaSolicitud,esquemaId,sdiId,contratoId,fechaInicial,fechaFinal,tipoPersonalId,solicita,valida,autoriza,noTrabajadores,observaciones,estatusSolicitud,estatusNomina,estatusAfiliado,estatusJuridico,estatusTarjeta,usuarioId,proyectoId,fechaEnvio,fechaInicioContrato")] Solicitud solicitud)
        {
            if (ModelState.IsValid)
            {
                Usuario usuario = Session["usuarioData"] as Usuario;
                Cliente cliente = db.Clientes.Find(solicitud.clienteId);
                ListaValidacionCliente lvc = cliente.ListaValidacionClientes.First();
                ToolsHelper th = new ToolsHelper();
                ParametrosHelper ph = new ParametrosHelper();

                Parametro folioAlta = ph.getParameterByKey("FOLSALTA");

                Concepto concepto = th.obtenerConceptoPorGrupo("ESTASOL", "apertura");
                Concepto tipoSolicitud = th.obtenerConceptoPorGrupo("SOLCON", "alta");

                solicitud.usuarioId = usuario.Id;
                solicitud.fechaSolicitud = DateTime.Now;

                solicitud.solicita = usuario.nombreUsuario;
                solicitud.fechaSolicitud = DateTime.Now;
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

                try
                {
                    db.Solicituds.Add(solicitud);
                    db.SaveChanges();
                    solicitud.folioSolicitud = folioAlta.valorString.Trim().PadLeft(5, '0') + "A" + solicitud.Cliente.Plaza.cveCorta.Trim();
                    int folAlta = int.Parse(folioAlta.valorString.Trim());
                    folAlta = folAlta + 1;
                    folioAlta.valorString = folAlta.ToString();


                    db.Entry(folioAlta).State = EntityState.Modified;
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
                return RedirectToAction("Index");
            }

            ViewBag.clienteId = new SelectList(db.Clientes, "Id", "claveCliente", solicitud.clienteId);
            ViewBag.estatusSolicitud = new SelectList(db.Conceptos, "id", "grupo", solicitud.estatusSolicitud);
            ViewBag.estatusNomina = new SelectList(db.Conceptos, "id", "grupo", solicitud.estatusNomina);
            ViewBag.estatusJuridico = new SelectList(db.Conceptos, "id", "grupo", solicitud.estatusJuridico);
            ViewBag.estatusAfiliado = new SelectList(db.Conceptos, "id", "grupo", solicitud.estatusAfiliado);
            ViewBag.estatusTarjeta = new SelectList(db.Conceptos, "id", "grupo", solicitud.estatusTarjeta);
            ViewBag.esquemaId = new SelectList(db.EsquemasPagoes, "id", "descripcion", solicitud.esquemaId);
            ViewBag.plazaId = new SelectList(db.Plazas, "id", "descripcion", solicitud.plazaId);
            ViewBag.proyectoId = new SelectList(db.Proyectos, "id", "descripcion", solicitud.proyectoId);
            ViewBag.sdiId = new SelectList(db.SDIs, "id", "descripcion", solicitud.sdiId);
            ViewBag.contratoId = new SelectList(db.TipoContratoes, "id", "descripcion", solicitud.contratoId);
            ViewBag.tipoPersonalId = new SelectList(db.TipoPersonals, "id", "descripcion", solicitud.tipoPersonalId);
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", solicitud.usuarioId);
            return View(solicitud);
        }

        // GET: Solicitudes/Edit/5
        public ActionResult Edit(int? id)
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
            ViewBag.esquemaId = solicitud.esquemaId;
            ViewBag.plazaId = solicitud.plazaId;
            ViewBag.sdiId = solicitud.sdiId;
            ViewBag.contratoId = solicitud.contratoId;
            ViewBag.tipoPersonalId = solicitud.tipoPersonalId;
            return View(solicitud);
        }

        // POST: Solicitudes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,folioSolicitud,clienteId,plazaId,fechaSolicitud,esquemaId,sdiId,contratoId,fechaInicial,fechaFinal,tipoPersonalId,solicita,valida,autoriza,noTrabajadores,observaciones,estatusSolicitud,estatusNomina,estatusAfiliado,estatusJuridico,estatusTarjeta,usuarioId,proyectoId,fechaEnvio")] Solicitud solicitud)
        {
            if (ModelState.IsValid)
            {
                db.Entry(solicitud).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.clienteId = new SelectList(db.Clientes, "Id", "claveCliente", solicitud.clienteId);
            ViewBag.estatusSolicitud = new SelectList(db.Conceptos, "id", "grupo", solicitud.estatusSolicitud);
            ViewBag.estatusNomina = new SelectList(db.Conceptos, "id", "grupo", solicitud.estatusNomina);
            ViewBag.estatusJuridico = new SelectList(db.Conceptos, "id", "grupo", solicitud.estatusJuridico);
            ViewBag.estatusAfiliado = new SelectList(db.Conceptos, "id", "grupo", solicitud.estatusAfiliado);
            ViewBag.estatusTarjeta = new SelectList(db.Conceptos, "id", "grupo", solicitud.estatusTarjeta);
            ViewBag.esquemaId = new SelectList(db.EsquemasPagoes, "id", "descripcion", solicitud.esquemaId);
            ViewBag.plazaId = new SelectList(db.Plazas, "id", "descripcion", solicitud.plazaId);
            ViewBag.proyectoId = new SelectList(db.Proyectos, "id", "descripcion", solicitud.proyectoId);
            ViewBag.sdiId = new SelectList(db.SDIs, "id", "descripcion", solicitud.sdiId);
            ViewBag.contratoId = new SelectList(db.TipoContratoes, "id", "descripcion", solicitud.contratoId);
            ViewBag.tipoPersonalId = new SelectList(db.TipoPersonals, "id", "descripcion", solicitud.tipoPersonalId);
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", solicitud.usuarioId);
            return View(solicitud);
        }

        // GET: Solicitudes/Delete/5
        public ActionResult Delete(int? id)
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

        // POST: Solicitudes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Solicitud solicitud = db.Solicituds.Find(id);
            db.Solicituds.Remove(solicitud);
            db.SaveChanges();
            return RedirectToAction("Index");
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
                email.enviarPorClienteTipo("A", solicitud.id, true);

                db.Entry(solicitud).State = EntityState.Modified;
                db.SaveChanges();

                TempData["message"] = "Solicitud Enviada Satisfactoriamente.";
            }

            return RedirectToAction("Index", new { clienteId = solicitud.clienteId, folioId = solicitud.folioSolicitud });
        }

        //Lay out Solicitud
        [HttpGet]
        public void crearExcelSolicitud(string clienteId, string proyectoId)
        {
            ToolsHelper th = new ToolsHelper();
            FileStream fileStream = null;
            MemoryStream mem = new MemoryStream();
            try
            {

                Concepto tipoSolicitud = th.obtenerConceptoPorGrupo("SOLCON", "alta");
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
                String fileName = @"Solicitud-" + date.ToString("ddMMyyyyHHmm") + ".xlsx";
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

                    SheetData sd = crearContenidoHojaSolicitud(solicitudList, eh);
                    ws.Append(sd);
                    wsp.Worksheet = ws;
                    wsp.Worksheet.Save();

                    DocumentFormat.OpenXml.Spreadsheet.Sheets sheets = new Sheets();
                    Sheet sheet = new Sheet();
                    sheet.Name = "rptPanelSolicitud";
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
        public SheetData crearContenidoHojaSolicitud(List<Solicitud> solicituds, ExcelHelper eh)
        {

            SheetData sheetData = new SheetData();
            int index = 1;

            //Creamos el Header
            Row row = new Row();

            index = index + 1;
            row = eh.addNewCellToRow(index, row, "SOLICITUDES", headerColumns[0] + index, 0U, CellValues.String);
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
            foreach (Solicitud dp in solicituds)
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

                row = eh.addNewCellToRow(index, row, dp.Concepto4.descripcion, headerColumns[i + 12] + index, 3U, CellValues.String);
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

        [HttpGet]
        public void GetExcel(string solicitudId)
        {

            //traigo de la base de datos Solicitudes los registros

            var solicitudes = from s in db.Solicituds
                              select s;

            //Valida que la variable no sea nula
            if (!String.IsNullOrEmpty(solicitudId))
            {
                int solicitudIdTemp = int.Parse(solicitudId);
                solicitudes = solicitudes.Where(s => s.id.Equals(solicitudIdTemp));
            }

            List<Solicitud> allCust = new List<Solicitud>();

            allCust = solicitudes.ToList();

            WebGrid grid = new WebGrid(source: allCust, canPage: false, canSort: false);

            List<WebGridColumn> gridColumns = new List<WebGridColumn>();
            gridColumns.Add(grid.Column("folioSolicitud", "Folio de Solicitud"));
            gridColumns.Add(grid.Column("Proyecto.descripcion", "Proyecto"));
            gridColumns.Add(grid.Column("plazaId", "Residencia"));
            gridColumns.Add(grid.Column("fechaSolicitud", "Fecha Solicitud"));
            gridColumns.Add(grid.Column("esquemaId", "Esquema"));
            gridColumns.Add(grid.Column("sdiId", "SDI"));
            gridColumns.Add(grid.Column("contratoId", "Tipo de Contrato"));
            gridColumns.Add(grid.Column("tipoPersonalId", "Tipo de Personal"));
            gridColumns.Add(grid.Column("fechaInicial", "Fecha Inicial"));
            gridColumns.Add(grid.Column("fechaFinal", "Fecha Final"));
            gridColumns.Add(grid.Column("solicita", "Solicita"));
            gridColumns.Add(grid.Column("observaciones", "Observaciones"));
            gridColumns.Add(grid.Column("noTrabajadores", "N° Trabajadores"));
            gridColumns.Add(grid.Column("estatusSolicitud", "Estatus Solicitud"));
            gridColumns.Add(grid.Column("estatusNomina", "Estatus Nom."));
            gridColumns.Add(grid.Column("estatusAfiliado", "Estatus Juri."));
            gridColumns.Add(grid.Column("estatusJuridico", "Estatus Afil."));
            gridColumns.Add(grid.Column("estatusTarjeta", "Estatus Tarj."));

            string gridData = grid.GetHtml(
                columns: grid.Columns(gridColumns.ToArray())
                    ).ToString();

            Response.ClearContent();
            DateTime date = DateTime.Now;
            String fileName = "Solicitudes" + date.ToString("ddMMyyyyHHmm") + ".xls";
            Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
            Response.ContentType = "application/excel";
            Response.Write(gridData);
            Response.End();
        }

    }
}
