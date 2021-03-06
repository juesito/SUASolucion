﻿using System;
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
using System.Web.Helpers;
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

            Usuario usuario = Session["UsuarioData"] as Usuario;
            SecurityUserModel.llenarPermisos(usuario.roleId);
            ToolsHelper th = new ToolsHelper();

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
                    x.proyectoId.Equals(proyectoInt) && x.anno.Equals(ejercicioId.Trim()) && x.plazaId.Equals(plazaInt)).
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
            ToolsHelper th = new ToolsHelper();
            if (!String.IsNullOrEmpty(ejercicioId) && !String.IsNullOrEmpty(clienteId) && !String.IsNullOrEmpty(proyectoId)
                && !String.IsNullOrEmpty(plazaId))
            {

                //Agregamos los valores a nuestra solicitud.
                DateTime now = DateTime.Now;
                solicitudPrenomina.clienteId = int.Parse(clienteId);
                solicitudPrenomina.proyectoId = int.Parse(proyectoId);
                solicitudPrenomina.plazaId = int.Parse(plazaId);
                solicitudPrenomina.anno = ejercicioId.Trim();
                solicitudPrenomina.fechaSolicitud = now;
                solicitudPrenomina.fechaInicial = now;
                solicitudPrenomina.fechaFinal = now;
                solicitudPrenomina.fechaPago = now;

                int clienteInt = int.Parse(clienteId.Trim());
                Cliente cliente = db.Clientes.Find(clienteInt);
                int lvcc = cliente.ListaValidacionClientes.Count();
                if (lvcc != 0)
                {
                    ListaValidacionCliente lvc = cliente.ListaValidacionClientes.FirstOrDefault();
                    solicitudPrenomina.autoriza = lvc.autorizador;
                    solicitudPrenomina.valida = lvc.validador;
                }
                else
                {
                    solicitudPrenomina.autoriza = " ";
                    solicitudPrenomina.valida = " ";
                }

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
                //                ListaValidacionCliente lvc = cliente.ListaValidacionClientes.First();

                //Parametro de folios de solicitud de prenomina
                Parametro folioAlta = ph.getParameterByKey("FOLSPALTA");
                //Concepto del estatus de la solicitud.
                Concepto concepto = th.obtenerConceptoPorGrupo("ESTASOL", "apertura");

                //Asignamos los valores de nuestra solicitud.
                solicitudPrenomina.fechaSolicitud = DateTime.Now;
                solicitudPrenomina.noTrabajadores = 0;
                //                solicitudPrenomina.autoriza = lvc.autorizador;
                //                solicitudPrenomina.valida = lvc.validador;
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

                return RedirectToAction("Index", new
                {
                    clienteId = solicitudPrenomina.clienteId,
                    proyectoId = solicitudPrenomina.proyectoId,
                    plazaId = solicitudPrenomina.plazaId,
                    ejercicioId = solicitudPrenomina.anno
                });
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

            List<DetallePrenomina> empleados = db.DetallePrenominas.Where(x => x.solicitudId.Equals(id)).ToList();

            foreach (DetallePrenomina empleado in empleados)
            {
                db.DetallePrenominas.Remove(empleado);
            }

            db.SolicitudPrenominas.Remove(solicitudPrenomina);

            db.SaveChanges();

            return RedirectToAction("Index", new
            {
                clienteId = solicitudPrenomina.clienteId,
                proyectoId = solicitudPrenomina.proyectoId,
                plazaId = solicitudPrenomina.plazaId,
                ejercicioId = solicitudPrenomina.anno
            });
        }

        [HttpGet]
        public void generarLayout(string solicitudId)
        {
            int solicitudInt = int.Parse(solicitudId);
            SolicitudPrenomina sp = db.SolicitudPrenominas.Find(solicitudInt);

            if (sp.Concepto1.descripcion.Trim().Equals("SyS Dias Laborados"))
            {
                crearExcelSySdiasLaborados(solicitudInt.ToString());
            }
            else if (sp.Concepto1.descripcion.Trim().Equals("SyS Dias Por Ingreso"))
            {
                crearExcelSySporIngresos(solicitudInt.ToString());
            }
            else if (sp.Concepto1.descripcion.Trim().Equals("IAS"))
            {
                crearExcelIAS(solicitudInt.ToString());
            }
            else if (sp.Concepto1.descripcion.Trim().Equals("IAS Dias Laborados"))
            {
                crearExcelIASdiasLaborados(solicitudInt.ToString());
            }
            else if (sp.Concepto1.descripcion.Trim().Equals("Finiquito"))
            {
                crearExcelFiniquito(solicitudInt.ToString());
            }
            else if (sp.Concepto1.descripcion.Trim().Equals("Aguinaldo"))
            {
                crearExcelAguinaldo(solicitudInt.ToString());
            }
            else if (sp.Concepto1.descripcion.Trim().Equals("Pago Normal PAC"))
            {
                crearExcelPagoNormalPac(solicitudInt.ToString());
            }
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

        //Lay out SyS Dias Laborados
        [HttpGet]
        public void crearExcelSySdiasLaborados(string solicitudId)
        {
            ToolsHelper th = new ToolsHelper();
            FileStream fileStream = null;
            MemoryStream mem = new MemoryStream();
            try
            {

                Usuario usuario = Session["UsuarioData"] as Usuario;
                List<DetallePrenomina> detallePrenominaList = new List<DetallePrenomina>();

                int solicitudInt = int.Parse(solicitudId);

                var detallePrenomina = db.DetallePrenominas.Where(s => s.solicitudId.Equals(solicitudInt)).ToList();

                DateTime date = DateTime.Now;
                String path = @"C:\\SUA\\Exceles\\";
                String fileName = @"SySDiasLaborados-" + date.ToString("ddMMyyyyHHmm") + ".xlsx";
                String fullName = path + fileName;
                detallePrenominaList = detallePrenomina.ToList();

                if (detallePrenominaList.Count() > 0)
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

                    SheetData sd = crearContenidoHojaSySdiasLaborados(detallePrenominaList, eh);
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

        public SheetData crearContenidoHojaSySdiasLaborados(List<DetallePrenomina> detallePrenominas, ExcelHelper eh)
        {

            SheetData sheetData = new SheetData();
            int index = 1;

            //Creamos el Header
            Row row = new Row();

            index = index + 1;
            row = eh.addNewCellToRow(index, row, "SyS DÍAS LABORADOS", headerColumns[0] + index, 0U, CellValues.String);
            sheetData.AppendChild(row);

            index = index + 2;

            row = eh.addNewCellToRow(index, row, "N° TRABAJADORES", headerColumns[0] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "APELLIDO PATERNO", headerColumns[1] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "APELLIDO MATERNO", headerColumns[2] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "NOMBRES", headerColumns[3] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "DIAS LABORADOS", headerColumns[4] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "TOTAL SYS", headerColumns[5] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "GRATIFICACIONES", headerColumns[6] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "PRIMA VACACIONAL", headerColumns[7] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "INFONAVIT", headerColumns[8] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "FONACOT", headerColumns[9] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "DESCUENTO POR PENSIÓN", headerColumns[10] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "DESCUENTOS REEMBOLSO", headerColumns[11] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "OTROS DESCUENTOS", headerColumns[12] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "NETO A PAGAR", headerColumns[13] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "CUENTA", headerColumns[14] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "BANCO", headerColumns[15] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "CATEGORÍA", headerColumns[16] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);


            //Creamos las celdas que contienen los datos
            foreach (DetallePrenomina dp in detallePrenominas)
            {
                int i = 0;
                index++;
                row = eh.addNewCellToRow(index, row, dp.SolicitudPrenomina.noTrabajadores.ToString(), headerColumns[i] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Empleado.apellidoPaterno, headerColumns[i + 1] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Empleado.apellidoMaterno, headerColumns[i + 2] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Empleado.nombre, headerColumns[i + 3] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.diasLaborados.ToString(), headerColumns[i + 4] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.totalSyS.ToString(), headerColumns[i + 5] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.gratificacion.ToString(), headerColumns[i + 6] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.primaVacacional.ToString(), headerColumns[i + 7] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Empleado.creditoInfonavit, headerColumns[i + 8] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.descuentoFonacot.ToString(), headerColumns[i + 9] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.descuentoPension.ToString(), headerColumns[i + 10] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.reembolso.ToString(), headerColumns[i + 11] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.otrosDescuentos.ToString(), headerColumns[i + 12] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.netoPagar.ToString(), headerColumns[i + 13] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Empleado.cuentaBancaria, headerColumns[i + 14] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Empleado.Banco.descripcion, headerColumns[i + 15] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Empleado.categoria, headerColumns[i + 16] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

            }

            return sheetData;
        }

        //Lay out SyS Por Ingresos
        [HttpGet]
        public void crearExcelSySporIngresos(string solicitudId)
        {
            ToolsHelper th = new ToolsHelper();
            FileStream fileStream = null;
            MemoryStream mem = new MemoryStream();
            try
            {

                Usuario usuario = Session["UsuarioData"] as Usuario;
                List<DetallePrenomina> detallePrenominaList = new List<DetallePrenomina>();

                int solicitudInt = int.Parse(solicitudId);

                var detallePrenomina = db.DetallePrenominas.Where(s => s.solicitudId.Equals(solicitudInt)).ToList();

                DateTime date = DateTime.Now;
                String path = @"C:\\SUA\\Exceles\\";
                String fileName = @"SySporIngresos-" + date.ToString("ddMMyyyyHHmm") + ".xlsx";
                String fullName = path + fileName;
                detallePrenominaList = detallePrenomina.ToList();

                if (detallePrenominaList.Count() > 0)
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

                    SheetData sd = crearContenidoHojaSySporIngresos(detallePrenominaList, eh);
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

        public SheetData crearContenidoHojaSySporIngresos(List<DetallePrenomina> detallePrenominas, ExcelHelper eh)
        {

            SheetData sheetData = new SheetData();
            int index = 1;

            //Creamos el Header
            Row row = new Row();

            index = index + 1;
            row = eh.addNewCellToRow(index, row, "SyS POR INGRESOS", headerColumns[0] + index, 0U, CellValues.String);
            sheetData.AppendChild(row);

            index = index + 2;
            row = eh.addNewCellToRow(index, row, "N° TRABAJADORES", headerColumns[0] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "APELLIDO PATERNO", headerColumns[1] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "APELLIDO MATERNO", headerColumns[2] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "NOMBRES", headerColumns[3] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "DIAS LABORADOS", headerColumns[4] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "TOTAL SYS", headerColumns[5] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "GRATIFICACIONES", headerColumns[6] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "PRIMA VACACIONAL", headerColumns[7] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "INFONAVIT", headerColumns[8] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "FONACOT", headerColumns[9] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "DESCUENTO POR PENSIÓN", headerColumns[10] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "DESCUENTOS REEMBOLSO", headerColumns[11] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "OTROS DESCUENTOS", headerColumns[12] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "NETO A PAGAR", headerColumns[13] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "CUENTA", headerColumns[14] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "BANCO", headerColumns[15] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "CATEGORÍA", headerColumns[16] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);


            //Creamos las celdas que contienen los datos
            foreach (DetallePrenomina dp in detallePrenominas)
            {
                int i = 0;
                index++;
                row = eh.addNewCellToRow(index, row, dp.SolicitudPrenomina.noTrabajadores.ToString(), headerColumns[i] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Empleado.apellidoPaterno, headerColumns[i + 1] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Empleado.apellidoMaterno, headerColumns[i + 2] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Empleado.nombre, headerColumns[i + 3] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.diasLaborados.ToString(), headerColumns[i + 4] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.totalSyS.ToString(), headerColumns[i + 5] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.gratificacion.ToString(), headerColumns[i + 6] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.primaVacacional.ToString(), headerColumns[i + 7] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Empleado.creditoInfonavit, headerColumns[i + 8] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.descuentoFonacot.ToString(), headerColumns[i + 9] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.descuentoPension.ToString(), headerColumns[i + 10] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.reembolso.ToString(), headerColumns[i + 11] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.otrosDescuentos.ToString(), headerColumns[i + 12] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.netoPagar.ToString(), headerColumns[i + 13] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Empleado.cuentaBancaria, headerColumns[i + 14] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Empleado.Banco.descripcion, headerColumns[i + 15] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Empleado.categoria, headerColumns[i + 16] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);
            }

            return sheetData;
        }

        //Lay out SyS IAS
        [HttpGet]
        public void crearExcelIAS(string solicitudId)
        {
            ToolsHelper th = new ToolsHelper();
            FileStream fileStream = null;
            MemoryStream mem = new MemoryStream();
            try
            {

                Usuario usuario = Session["UsuarioData"] as Usuario;
                List<DetallePrenomina> detallePrenominaList = new List<DetallePrenomina>();

                int solicitudInt = int.Parse(solicitudId);

                var detallePrenomina = db.DetallePrenominas.Where(s => s.solicitudId.Equals(solicitudInt)).ToList();

                DateTime date = DateTime.Now;
                String path = @"C:\\SUA\\Exceles\\";
                String fileName = @"SySIAS-" + date.ToString("ddMMyyyyHHmm") + ".xlsx";
                String fullName = path + fileName;
                detallePrenominaList = detallePrenomina.ToList();

                if (detallePrenominaList.Count() > 0)
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

                    SheetData sd = crearContenidoHojaIAS(detallePrenominaList, eh);
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

        public SheetData crearContenidoHojaIAS(List<DetallePrenomina> detallePrenominas, ExcelHelper eh)
        {

            SheetData sheetData = new SheetData();
            int index = 1;

            //Creamos el Header
            Row row = new Row();

            index = index + 1;
            row = eh.addNewCellToRow(index, row, "IAS", headerColumns[0] + index, 0U, CellValues.String);
            sheetData.AppendChild(row);

            index = index + 2;
            row = eh.addNewCellToRow(index, row, "N° TRABAJADORES", headerColumns[0] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "APELLIDO PATERNO", headerColumns[1] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "APELLIDO MATERNO", headerColumns[2] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "NOMBRES", headerColumns[3] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "DIAS LABORADOS", headerColumns[4] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "OTROS DESCUENTOS", headerColumns[5] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "NETO A PAGAR", headerColumns[6] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "CUENTA", headerColumns[7] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "BANCO", headerColumns[8] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "CATEGORÍA", headerColumns[9] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);


            //Creamos las celdas que contienen los datos
            foreach (DetallePrenomina dp in detallePrenominas)
            {
                int i = 0;
                index++;
                row = eh.addNewCellToRow(index, row, dp.SolicitudPrenomina.noTrabajadores.ToString(), headerColumns[i] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Empleado.apellidoPaterno, headerColumns[i + 1] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Empleado.apellidoMaterno, headerColumns[i + 2] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Empleado.nombre, headerColumns[i + 3] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.totalIAS.ToString(), headerColumns[i + 4] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.otrosDescuentos.ToString(), headerColumns[i + 5] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.netoPagar.ToString(), headerColumns[i + 6] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Empleado.cuentaBancaria, headerColumns[i + 7] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Empleado.Banco.descripcion, headerColumns[i + 8] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Empleado.categoria, headerColumns[i + 9] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);
            }

            return sheetData;
        }

        //Lay out SyS IAS DIAS LABORADOS
        [HttpGet]
        public void crearExcelIASdiasLaborados(string solicitudId)
        {
            ToolsHelper th = new ToolsHelper();
            FileStream fileStream = null;
            MemoryStream mem = new MemoryStream();
            try
            {

                Usuario usuario = Session["UsuarioData"] as Usuario;
                List<DetallePrenomina> detallePrenominaList = new List<DetallePrenomina>();

                int solicitudInt = int.Parse(solicitudId);

                var detallePrenomina = db.DetallePrenominas.Where(s => s.solicitudId.Equals(solicitudInt)).ToList();

                DateTime date = DateTime.Now;
                String path = @"C:\\SUA\\Exceles\\";
                String fileName = @"SySIASdiasLaborados-" + date.ToString("ddMMyyyyHHmm") + ".xlsx";
                String fullName = path + fileName;
                detallePrenominaList = detallePrenomina.ToList();

                if (detallePrenominaList.Count() > 0)
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

                    SheetData sd = crearContenidoHojaIASdiasLaborados(detallePrenominaList, eh);
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

        public SheetData crearContenidoHojaIASdiasLaborados(List<DetallePrenomina> detallePrenominas, ExcelHelper eh)
        {

            SheetData sheetData = new SheetData();
            int index = 1;

            //Creamos el Header
            Row row = new Row();

            index = index + 1;
            row = eh.addNewCellToRow(index, row, "IAS POR DIAS LABORADOS", headerColumns[0] + index, 0U, CellValues.String);
            sheetData.AppendChild(row);

            index = index + 2;
            row = eh.addNewCellToRow(index, row, "N° TRABAJADORES", headerColumns[0] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "APELLIDO PATERNO", headerColumns[1] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "APELLIDO MATERNO", headerColumns[2] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "NOMBRES", headerColumns[3] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "DT", headerColumns[4] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "SDR", headerColumns[5] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "TOTAL IAS", headerColumns[6] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "OTROS DESCUENTOS", headerColumns[7] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "NETO A PAGAR", headerColumns[8] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "CUENTA", headerColumns[9] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "BANCO", headerColumns[10] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "CATEGORÍA", headerColumns[11] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);


            //Creamos las celdas que contienen los datos
            foreach (DetallePrenomina dp in detallePrenominas)
            {
                int i = 0;
                index++;
                row = eh.addNewCellToRow(index, row, dp.SolicitudPrenomina.noTrabajadores.ToString(), headerColumns[i] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Empleado.apellidoPaterno, headerColumns[i + 1] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Empleado.apellidoMaterno, headerColumns[i + 2] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Empleado.nombre, headerColumns[i + 3] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.diasLaborados.ToString(), headerColumns[i + 4] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Empleado.salarioReal.ToString(), headerColumns[i + 5] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.totalIAS.ToString(), headerColumns[i + 6] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.otrosDescuentos.ToString(), headerColumns[i + 7] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.netoPagar.ToString(), headerColumns[i + 8] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Empleado.cuentaBancaria, headerColumns[i + 9] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Empleado.Banco.descripcion, headerColumns[i + 10] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Empleado.categoria, headerColumns[i + 11] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);
            }

            return sheetData;
        }

        //Lay out Finiquito
        [HttpGet]
        public void crearExcelFiniquito(string solicitudId)
        {
            ToolsHelper th = new ToolsHelper();
            FileStream fileStream = null;
            MemoryStream mem = new MemoryStream();
            try
            {

                Usuario usuario = Session["UsuarioData"] as Usuario;
                List<DetallePrenomina> detallePrenominaList = new List<DetallePrenomina>();

                int solicitudInt = int.Parse(solicitudId);

                var detallePrenomina = db.DetallePrenominas.Where(s => s.solicitudId.Equals(solicitudInt)).ToList();

                DateTime date = DateTime.Now;
                String path = @"C:\\SUA\\Exceles\\";
                String fileName = @"Finiquito-" + date.ToString("ddMMyyyyHHmm") + ".xlsx";
                String fullName = path + fileName;
                detallePrenominaList = detallePrenomina.ToList();

                if (detallePrenominaList.Count() > 0)
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

                    SheetData sd = crearContenidoHojaFiniquito(detallePrenominaList, eh);
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

        public SheetData crearContenidoHojaFiniquito(List<DetallePrenomina> detallePrenominas, ExcelHelper eh)
        {

            SheetData sheetData = new SheetData();
            int index = 1;

            //Creamos el Header
            Row row = new Row();

            index = index + 1;
            row = eh.addNewCellToRow(index, row, "FINIQUITO", headerColumns[0] + index, 0U, CellValues.String);
            sheetData.AppendChild(row);

            index = index + 2;
            row = eh.addNewCellToRow(index, row, "N° TRABAJADORES", headerColumns[0] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "APELLIDO PATERNO", headerColumns[1] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "APELLIDO MATERNO", headerColumns[2] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "NOMBRES", headerColumns[3] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "DT", headerColumns[4] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "SR", headerColumns[5] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "SDI", headerColumns[6] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "SyS", headerColumns[7] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "VACACIONES", headerColumns[8] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "PRIMA VACACIONAL", headerColumns[9] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "AGUINALDO", headerColumns[10] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "GRATIFICACIONES", headerColumns[11] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "INFONAVIT", headerColumns[12] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "FONACOT", headerColumns[13] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "OTROS DESCUENTOS", headerColumns[14] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "NETO A PAGAR", headerColumns[15] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "CUENTA", headerColumns[16] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "BANCO", headerColumns[17] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "CATEGORÍA", headerColumns[18] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);


            //Creamos las celdas que contienen los datos
            foreach (DetallePrenomina dp in detallePrenominas)
            {
                int i = 0;
                index++;
                row = eh.addNewCellToRow(index, row, dp.SolicitudPrenomina.noTrabajadores.ToString(), headerColumns[i] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Empleado.apellidoPaterno, headerColumns[i + 1] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Empleado.apellidoMaterno, headerColumns[i + 2] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Empleado.nombre, headerColumns[i + 3] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.diasLaborados.ToString(), headerColumns[i + 4] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Empleado.salarioReal.ToString(), headerColumns[i + 5] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Empleado.SDI.descripcion, headerColumns[i + 6] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.totalSyS.ToString(), headerColumns[i + 7] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.vacaciones.ToString(), headerColumns[i + 8] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.primaVacacional.ToString(), headerColumns[i + 9] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.aguinaldo.ToString(), headerColumns[i + 10] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.gratificacion.ToString(), headerColumns[i + 11] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.descuentoInfonavit.ToString(), headerColumns[i + 12] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.descuentoFonacot.ToString(), headerColumns[i + 13] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.otrosDescuentos.ToString(), headerColumns[i + 14] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.netoPagar.ToString(), headerColumns[i + 15] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                if (!String.IsNullOrEmpty(dp.Empleado.cuentaBancaria))
                {
                    row = eh.addNewCellToRow(index, row, dp.Empleado.cuentaBancaria.ToString(), headerColumns[i + 16] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, "No Cuenta", headerColumns[i + 16] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                row = eh.addNewCellToRow(index, row, dp.Empleado.Banco.descripcion.ToString(), headerColumns[i + 17] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);


                row = eh.addNewCellToRow(index, row, dp.Empleado.categoria, headerColumns[i + 18] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

            }

            return sheetData;
        }


        //Lay out Aguinaldo
        [HttpGet]
        public void crearExcelAguinaldo(string solicitudId)
        {
            ToolsHelper th = new ToolsHelper();
            FileStream fileStream = null;
            MemoryStream mem = new MemoryStream();
            try
            {

                Usuario usuario = Session["UsuarioData"] as Usuario;
                List<DetallePrenomina> detallePrenominaList = new List<DetallePrenomina>();

                int solicitudInt = int.Parse(solicitudId);

                var detallePrenomina = db.DetallePrenominas.Where(s => s.solicitudId.Equals(solicitudInt)).ToList();

                DateTime date = DateTime.Now;
                String path = @"C:\\SUA\\Exceles\\";
                String fileName = @"Aguinaldo-" + date.ToString("ddMMyyyyHHmm") + ".xlsx";
                String fullName = path + fileName;
                detallePrenominaList = detallePrenomina.ToList();

                if (detallePrenominaList.Count() > 0)
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

                    SheetData sd = crearContenidoHojaAguinaldo(detallePrenominaList, eh);
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

        public SheetData crearContenidoHojaAguinaldo(List<DetallePrenomina> detallePrenominas, ExcelHelper eh)
        {

            SheetData sheetData = new SheetData();
            int index = 1;

            //Creamos el Header
            Row row = new Row();

            index = index + 1;
            row = eh.addNewCellToRow(index, row, "AGUINALDO", headerColumns[0] + index, 0U, CellValues.String);
            sheetData.AppendChild(row);

            index = index + 2;
            row = eh.addNewCellToRow(index, row, "N° TRABAJADORES", headerColumns[0] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "APELLIDO PATERNO", headerColumns[1] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "APELLIDO MATERNO", headerColumns[2] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "NOMBRES", headerColumns[3] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "AGUINALDO", headerColumns[4] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "OTROS DESCUENTOS", headerColumns[5] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "NETO A PAGAR", headerColumns[6] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "CUENTA", headerColumns[7] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "BANCO", headerColumns[8] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "CATEGORÍA", headerColumns[9] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);


            //Creamos las celdas que contienen los datos
            foreach (DetallePrenomina dp in detallePrenominas)
            {
                int i = 0;
                index++;
                row = eh.addNewCellToRow(index, row, dp.SolicitudPrenomina.noTrabajadores.ToString(), headerColumns[i] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Empleado.apellidoPaterno, headerColumns[i + 1] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Empleado.apellidoMaterno, headerColumns[i + 2] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Empleado.nombre, headerColumns[i + 3] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.aguinaldo.ToString(), headerColumns[i + 4] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.otrosDescuentos.ToString(), headerColumns[i + 5] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.netoPagar.ToString(), headerColumns[i + 6] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                if (!String.IsNullOrEmpty(dp.Empleado.cuentaBancaria))
                {
                    row = eh.addNewCellToRow(index, row, dp.Empleado.cuentaBancaria.ToString(), headerColumns[i + 7] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, "No Cuenta", headerColumns[i + 7] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                row = eh.addNewCellToRow(index, row, dp.Empleado.Banco.descripcion.ToString(), headerColumns[i + 8] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);


                row = eh.addNewCellToRow(index, row, dp.Empleado.categoria, headerColumns[i + 9] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

            }

            return sheetData;
        }

        //Lay out Pago Normal PAC
        [HttpGet]
        public void crearExcelPagoNormalPac(string solicitudId)
        {
            ToolsHelper th = new ToolsHelper();
            FileStream fileStream = null;
            MemoryStream mem = new MemoryStream();
            try
            {

                Usuario usuario = Session["UsuarioData"] as Usuario;
                List<DetallePrenomina> detallePrenominaList = new List<DetallePrenomina>();

                int solicitudInt = int.Parse(solicitudId);

                var detallePrenomina = db.DetallePrenominas.Where(s => s.solicitudId.Equals(solicitudInt)).ToList();

                DateTime date = DateTime.Now;
                String path = @"C:\\SUA\\Exceles\\";
                String fileName = @"PagoNormalPAC-" + date.ToString("ddMMyyyyHHmm") + ".xlsx";
                String fullName = path + fileName;
                detallePrenominaList = detallePrenomina.ToList();

                if (detallePrenominaList.Count() > 0)
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

                    SheetData sd = crearContenidoHojaPagoNormalPac(detallePrenominaList, eh);
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

        public SheetData crearContenidoHojaPagoNormalPac(List<DetallePrenomina> detallePrenominas, ExcelHelper eh)
        {

            SheetData sheetData = new SheetData();
            int index = 1;

            //Creamos el Header
            Row row = new Row();

            index = index + 1;
            row = eh.addNewCellToRow(index, row, "PAGO NORMAL PAC", headerColumns[0] + index, 0U, CellValues.String);
            sheetData.AppendChild(row);

            index = index + 2;
            row = eh.addNewCellToRow(index, row, "N° TRABAJADORES", headerColumns[0] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "APELLIDO PATERNO", headerColumns[1] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "APELLIDO MATERNO", headerColumns[2] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "NOMBRES", headerColumns[3] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "DT", headerColumns[4] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "SR", headerColumns[5] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "SDI", headerColumns[6] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "TOTAL SyS", headerColumns[7] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "PREMIO PUNTUALIDAD", headerColumns[8] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "PREMIO ASISTENCIA", headerColumns[9] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "AGUINALDO", headerColumns[10] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "UNIFORME", headerColumns[11] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "COMPENSACIÓN", headerColumns[12] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "CAJA DE AHORRO", headerColumns[13] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "HORAS EXTRAS", headerColumns[14] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "VIATICOS", headerColumns[15] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "COMIDA", headerColumns[16] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "GRATIFICACIONES", headerColumns[17] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "PRIMA VACACIONAL", headerColumns[18] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "INFONAVIT", headerColumns[19] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "FONACOT", headerColumns[20] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "DESCUENTO POR PENSIÓN", headerColumns[21] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "DESCUENTOS REEMBOLSO", headerColumns[22] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "OTROS DESCUENTOS", headerColumns[23] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "NETO A PAGAR", headerColumns[24] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "CUENTA", headerColumns[25] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "BANCO", headerColumns[26] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "CATEGORÍA", headerColumns[27] + index, 4U, CellValues.String);
            sheetData.AppendChild(row);

            //Creamos las celdas que contienen los datos
            foreach (DetallePrenomina dp in detallePrenominas)
            {
                int i = 0;
                index++;
                row = eh.addNewCellToRow(index, row, dp.SolicitudPrenomina.noTrabajadores.ToString(), headerColumns[i] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Empleado.apellidoPaterno, headerColumns[i + 1] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Empleado.apellidoMaterno, headerColumns[i + 2] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Empleado.nombre, headerColumns[i + 3] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.diasLaborados.ToString(), headerColumns[i + 4] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Empleado.salarioReal.ToString(), headerColumns[i + 5] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Empleado.SDI.descripcion, headerColumns[i + 6] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.totalSyS.ToString(), headerColumns[i + 7] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.premioPuntualidad.ToString(), headerColumns[i + 8] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.premioAsistencia.ToString(), headerColumns[i + 9] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.aguinaldo.ToString(), headerColumns[i + 10] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.uniforme.ToString(), headerColumns[i + 11] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.compensacion.ToString(), headerColumns[i + 12] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.cajaAhorro.ToString(), headerColumns[i + 13] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.horasExtra.ToString(), headerColumns[i + 14] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.viaticos.ToString(), headerColumns[i + 15] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.comida.ToString(), headerColumns[i + 16] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.gratificacion.ToString(), headerColumns[i + 17] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.primaVacacional.ToString(), headerColumns[i + 18] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.descuentoInfonavit.ToString(), headerColumns[i + 19] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.descuentoFonacot.ToString(), headerColumns[i + 20] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.descuentoPension.ToString(), headerColumns[i + 21] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.reembolso.ToString(), headerColumns[i + 22] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.otrosDescuentos.ToString(), headerColumns[i + 23] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.netoPagar.ToString(), headerColumns[i + 24] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

                if (!String.IsNullOrEmpty(dp.Empleado.cuentaBancaria))
                {
                    row = eh.addNewCellToRow(index, row, dp.Empleado.cuentaBancaria.ToString(), headerColumns[i + 25] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, "No Cuenta", headerColumns[i + 25] + index, 3U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                row = eh.addNewCellToRow(index, row, dp.Empleado.Banco.descripcion.ToString(), headerColumns[i + 26] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);


                row = eh.addNewCellToRow(index, row, dp.Empleado.categoria, headerColumns[i + 27] + index, 3U, CellValues.String);
                sheetData.AppendChild(row);

            }

            return sheetData;
        }


        public ActionResult CargarEmpleadosPorExcel(int id)
        {
            ViewBag.solicitud = db.Solicituds.Find(id);
            ViewBag.controllerDestiny = "Solicitudes";
            return View();
        }

        public ActionResult CargarPrenominaSYSDiasExcel(String solicitudId)
        {

            if (!String.IsNullOrEmpty(solicitudId))
            {
                ToolsHelper th = new ToolsHelper();
                int solicitudAct = int.Parse(solicitudId);
                SolicitudPrenomina solicitud = db.SolicitudPrenominas.Find(solicitudAct);
                Usuario usuario = Session["UsuarioData"] as Usuario;
                DateTime date = DateTime.Now;

                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];

                    if (file != null && file.ContentLength > 0)
                    {
                        ExcelHelper ex = new ExcelHelper();

                        String fileName = @"C:\SUA\Layouts\" + date.ToString("ddMMyyyyHHmmss") + "-" + file.FileName.Trim();
                        file.SaveAs(fileName.Trim());
                        LinqToExcelProvider provider = new LinqToExcelProvider(fileName.Trim());

                        provider.readExcel("Layout");

                        var query = (from row in provider.GetWorkSheet("Layout")
                                     let item = new PrenominaExcelLayout
                                     {
                                         //NOMBRE	APATERNO	AMATERNO	NSS	DT	GRATIFICACION	PVACACIONAL	
                                         //AGUINALDO	DINFONAVIT	DFONACOT	DPENSION	ODESC


                                         nombre = Convert.ToString(row.Field<Object>("NOMBRE")),
                                         apellidoMaterno = Convert.ToString(row.Field<Object>("AMATERNO")),
                                         apellidoPaterno = Convert.ToString(row.Field<Object>("APATERNO")),
                                         dt = Convert.ToInt32(row.Field<Object>("DT")),
                                         nss = Convert.ToString(row.Field<Object>("NSS")),
                                         ingresos = Convert.ToDecimal(row.Field<Object>("INGRESOS")),
                                         primaVacacional = Convert.ToDecimal(row.Field<Object>("PVACACIONAL")),
                                         gratificacion = Convert.ToDecimal(row.Field<Object>("GRATIFICACION")),
                                         aguinaldo = Convert.ToDecimal(row.Field<Object>("AGUINALDO")),
                                         descuentoInfonavit = Convert.ToDecimal(row.Field<Object>("DINFONAVIT")),
                                         descuentoFonacot = Convert.ToDecimal(row.Field<Object>("DFONACOT")),
                                         descuentoPension = Convert.ToDecimal(row.Field<Object>("DPENSION")),
                                         otrosDescuentos = Convert.ToDecimal(row.Field<Object>("ODESC")),
                                         isr = Convert.ToDecimal(row.Field<Object>("ISR")),
                                         reembolso = Convert.ToDecimal(row.Field<Object>("REEMBOLSO")),

                                     }
                                     select item).ToList();


                        Boolean founded = false;
                        int counter = 1;
                        LogHelper log = new LogHelper();

                        foreach (PrenominaExcelLayout empleadoL in query)
                        {
                            Empleado empleado = new Empleado();
                            founded = false;

                            // Validamos los campos basicos por tipo de pago
                            if (solicitud.Concepto1.descripcion.ToLower().Trim().Equals("ias"))
                            {

                                Boolean error = false;

                                if (empleadoL.ingresos == 0) {
                                    log.saveLog("Renglon ->" + counter, "El campo ingresos no puede ser cero. Tipo pago IAS",
                                        "Carga Prenomina Masiva", usuario.Id, "ER", solicitudId);
                                }

                                if (empleadoL.isr == 0)
                                {
                                    log.saveLog("Renglon ->" + counter, "El campo isr no puede ser cero. Tipo pago IAS",
                                        "Carga Prenomina Masiva", usuario.Id, "ER", solicitudId);
                                }

                                if (error)
                                {
                                    counter++;

                                    break;
                                }
                            }
                            else if (solicitud.Concepto1.descripcion.ToLower().Trim().Equals("ias dias laborados"))
                            {
                                Boolean error = false;

                                if (empleadoL.dt == 0)
                                {
                                    log.saveLog("Renglon ->" + counter, "El campo dias trabajados no puede ser cero. Tipo pago IAS dias laborados",
                                        "Carga Prenomina Masiva", usuario.Id, "ER", solicitudId);
                                    error = true;
                                }

                                if (error)
                                {
                                    counter++;

                                    break;
                                }
                            }
                            else if (solicitud.Concepto1.descripcion.ToLower().Trim().Equals("sys dias laborados"))
                            {
                                Boolean error = false;

                                if (empleadoL.dt == 0)
                                {
                                    log.saveLog("Renglon ->" + counter, "El campo dias trabajados no puede ser cero. Tipo pago SYS dias laborados",
                                        "Carga Prenomina Masiva", usuario.Id, "ER", solicitudId);
                                    error = true;
                                }

                                if (empleado.SDI.descripcion.Trim().Equals("0"))
                                {
                                    log.saveLog("Renglon ->" + counter, "El valor SDI del empleado no puede ser cero. Tipo pago SYS dias laborados",
                                        "Carga Prenomina Masiva", usuario.Id, "ER", solicitudId);
                                    error = true;
                                }

                                if (error)
                                {
                                    counter++;

                                    break;
                                }
                            }
                            else if (solicitud.Concepto1.descripcion.ToLower().Trim().Equals("sys dias por ingreso"))
                            {

                                Boolean error = false;

                                if (empleadoL.ingresos == 0)
                                {
                                    log.saveLog("Renglon ->" + counter, "El campo ingresos no puede ser cero. Tipo pago IAS",
                                        "Carga Prenomina Masiva", usuario.Id, "ER", solicitudId);
                                }

                                if (empleado.salarioReal == 0)
                                {
                                    log.saveLog("Renglon ->" + counter, "El campo Salario Real del empleado no puede ser cero. Tipo pago SYS dias por ingreso",
                                        "Carga Prenomina Masiva", usuario.Id, "ER", solicitudId);
                                }


                                if (error)
                                {
                                    counter++;

                                    break;
                                }
                            }

                            //Se valida que el nss no sea null
                            if (!String.IsNullOrEmpty(empleadoL.nss))
                            {
                                empleado.nss = empleadoL.nss.Trim();
                                empleado = th.obtenerEmpleadoPorNSS(empleadoL.nss.Trim());

                                founded = th.verificarEmpleadoPorNSSyCliente(empleadoL.nss.Trim(), solicitud.clienteId);
                            }
                            else
                            {
                                log.saveLog("Renglon ->" + counter, "NSS esta nulo",
                                    "Carga Prenomina Masiva", usuario.Id, "WA", solicitudId);

                                counter++;


                                if (String.IsNullOrEmpty(empleadoL.nombre) || String.IsNullOrEmpty(empleadoL.apellidoPaterno)
                                || String.IsNullOrEmpty(empleadoL.apellidoMaterno))
                                {

                                    log.saveLog("Renglon ->" + counter, "Nombre - Apellido Paterno - Apellido Materno. Campos obligatorios nulos",
                                        "Carga Prenomina Masiva", usuario.Id, "ER", solicitudId);

                                    counter++;

                                    break;
                                }
                                else
                                {

                                    empleado = th.verificarEmpleadoPorClienteProyecto(empleadoL.nombre, empleadoL.apellidoMaterno, empleadoL.apellidoPaterno, solicitud.clienteId, solicitud.proyectoId);

                                    founded = (empleado != null);
                                }

                            }


                            if (founded)
                            {

                                DetallePrenomina detallePrenomina = new DetallePrenomina();

                                detallePrenomina.solicitudId = solicitud.id;
                                detallePrenomina.empleadoId = empleado.id;
                                detallePrenomina.diasLaborados = empleadoL.dt;
                                detallePrenomina.ingresos = empleadoL.ingresos;
                                detallePrenomina.gratificacion = empleadoL.gratificacion;
                                detallePrenomina.primaVacacional = empleadoL.primaVacacional;
                                detallePrenomina.aguinaldo = empleadoL.aguinaldo;
                                detallePrenomina.descuentoInfonavit = empleadoL.descuentoInfonavit;
                                detallePrenomina.descuentoFonacot = empleadoL.descuentoFonacot;
                                detallePrenomina.descuentoPension = empleadoL.descuentoPension;
                                detallePrenomina.otrosDescuentos = empleadoL.otrosDescuentos;
                                detallePrenomina.isr = empleadoL.isr;
                                detallePrenomina.reembolso = empleadoL.reembolso;

                                detallePrenomina.fechaCreacion = DateTime.Now;
                                detallePrenomina.usuarioId = usuario.Id;


                                if (solicitud.Concepto1.descripcion.ToLower().Trim().Equals("ias"))
                                {
                                    detallePrenomina.totalIAS = detallePrenomina.ingresos;
                                    detallePrenomina.netoPagar = detallePrenomina.totalIAS - detallePrenomina.otrosDescuentos - detallePrenomina.isr;
                                }
                                if (solicitud.Concepto1.descripcion.ToLower().Trim().Equals("ias dias laborados"))
                                {
                                    detallePrenomina.totalIAS = detallePrenomina.diasLaborados * empleado.salarioReal;
                                    detallePrenomina.netoPagar = detallePrenomina.totalIAS - detallePrenomina.reembolso - detallePrenomina.otrosDescuentos;
                                }
                                else if (solicitud.Concepto1.descripcion.ToLower().Trim().Equals("sys dias laborados"))
                                {

                                    detallePrenomina.totalSyS = detallePrenomina.diasLaborados * int.Parse(empleado.SDI.descripcion);

                                    detallePrenomina.totalSyS = detallePrenomina.totalSyS + detallePrenomina.gratificacion + detallePrenomina.primaVacacional + detallePrenomina.aguinaldo;
                                    detallePrenomina.netoPagar = detallePrenomina.totalSyS - detallePrenomina.descuentoInfonavit - detallePrenomina.descuentoPension - detallePrenomina.descuentoFonacot -
                                        detallePrenomina.otrosDescuentos;

                                }
                                else if (solicitud.Concepto1.descripcion.ToLower().Trim().Equals("sys dias por ingreso"))
                                {

                                    detallePrenomina.totalSyS = detallePrenomina.ingresos * empleado.salarioReal;
                                    detallePrenomina.totalSyS = detallePrenomina.totalSyS + detallePrenomina.gratificacion + detallePrenomina.primaVacacional + detallePrenomina.aguinaldo;

                                    detallePrenomina.netoPagar = detallePrenomina.totalSyS - detallePrenomina.descuentoInfonavit - detallePrenomina.descuentoPension - detallePrenomina.descuentoFonacot -
                                        detallePrenomina.otrosDescuentos;
                                }


                                try
                                {

                                    db.DetallePrenominas.Add(detallePrenomina);
                                    db.SaveChanges();

                                    //Obtenemos la solicitud par modificar el noTrabjadores
                                    solicitud.noTrabajadores = solicitud.noTrabajadores + 1;

                                    //Preparamos las entidades para guardar
                                    db.Entry(solicitud).State = EntityState.Modified;
                                    db.SaveChanges();

                                }
                                catch (DbEntityValidationException exm)
                                {
                                    StringBuilder sb = new StringBuilder();

                                    foreach (var failure in exm.EntityValidationErrors)
                                    {
                                        sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
                                        foreach (var error in failure.ValidationErrors)
                                        {
                                            sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                                            sb.AppendLine();

                                            log.saveLog("Renglon ->" + counter, "Reigistro error sistema",
                                            "Carga Empleados Masiva", usuario.Id, "SE", solicitudId);
                                        }
                                    }
                                }

                            }
                            else
                            {

                                log.saveLog("Renglon ->" + counter, "Registro ya existente " + empleadoL.nombre.Trim(),
                                    "Carga Empleados Masiva", usuario.Id, "WA", solicitudId);
                            }//Se encontro ya el nss y cliente?
                            counter++;
                        }

                    }

                }
                return RedirectToAction("Index", "SolicitudPrenominas", new { clienteId = solicitud.clienteId, proyectoId = solicitud.proyectoId });
            }

            return RedirectToAction("CargarEmpleadosPorExcel", "SolicitudPrenominas", new { id = solicitudId });
        }

        public void DownLoadLayout()
        {
            FileStream fileStream = null;
            MemoryStream mem = new MemoryStream();

            fileStream = new FileStream("C:\\SUA\\Layouts\\Layout-Alta-Prenomina.xlsx", FileMode.Open);
            fileStream.Position = 0;
            mem = new MemoryStream();
            fileStream.CopyTo(mem);
            fileStream.Close();

            mem.Position = 0;
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=Layout-Alta-Prenomina.xlsx");
            ToolsHelper th = new ToolsHelper();
            Response.ContentType = th.getMimeType("C:\\SUA\\Layouts\\Layout-Alta-Prenomina.xlsx");
            Response.BinaryWrite(mem.ToArray());

            Response.End();
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
