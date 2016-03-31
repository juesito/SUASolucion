using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SUADATOS;
using System.Data.OleDb;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Text;
using PagedList;
using System.IO;
using System.Web.Helpers;
using SUAMVC.Code52.i18n;
using SUAMVC.Helpers;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using SUAMVC.Models;

namespace SUAMVC.Controllers
{
    public class PatronesController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: Patrones
        public ActionResult Index(String plazasId, String statusId)
        {
            Usuario user = Session["UsuarioData"] as Usuario;

            var patronesAsignados = (from x in db.TopicosUsuarios
                                     where x.usuarioId.Equals(user.Id)
                                     && x.tipo.Equals("B")
                                     select x.topicoId);

            var patrones = from s in db.Patrones
                           where patronesAsignados.Contains(s.Id)
                           select s;

            if (!String.IsNullOrEmpty(plazasId))
            {
                int plazaIdTemp = int.Parse(plazasId);
                patrones = patrones.Where(p => p.Plaza.id.Equals(plazaIdTemp));
            }

            if (statusId != null)
            {
                @ViewBag.statusId = statusId;

                if (statusId.Trim().Equals("A"))
                {
                    ViewBag.activos = patrones.Where(s => s.Concepto.descripcion.Equals("Activo")).Count();
                    patrones = patrones.Where(s => s.Concepto.descripcion.Equals("Activo"));
                }
                else if (statusId.Trim().Equals("B"))
                {
                    ViewBag.activos = patrones.Where(s => s.Concepto.descripcion.Equals("Inactivo")).Count();
                    patrones = patrones.Where(s => s.Concepto.descripcion.Equals("Inactivo"));
                }
            }

            ViewBag.activos = patrones.Where(s => s.Concepto.descripcion.Equals("Activo")).Count();
            ViewBag.registros = patrones.Count();

            patrones.OrderBy(p => p.nombre);

            return View(patrones.ToList());
        }

        // GET: Patrones/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Patrone patrone = db.Patrones.Find(id);
            if (patrone == null)
            {
                return HttpNotFound();
            }
            return View(patrone);
        }

        // GET: Patrones/Create
        public ActionResult Create()
        {
            ViewBag.Plaza_id = new SelectList(db.Plazas, "id", "descripcion");
            return View();
        }

        // POST: Patrones/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,registro,rfc,nombre,actividad,domicilio,municipio,codigoPostal,entidad,telefono,remision,zona,delegacion,carEnt,numeroDelegacion,carDel,numSub,tipoConvenio,convenio,inicioAfiliacion,patRep,clase,fraccion,STyPS,Plaza_id,direccionArchivo,porcentajeNomina,unidadMedica,estatus")] Patrone patrone)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    patrone.nombre = patrone.nombre.ToUpper();
                    db.Patrones.Add(patrone);
                    db.SaveChanges();
                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        }
                    }
                }
                return RedirectToAction("Index");
            }

            ViewBag.Plaza_id = new SelectList(db.Plazas, "id", "descripcion", patrone.Plaza_id);
            return View(patrone);
        }

        // GET: Patrones/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Patrone patrone = db.Patrones.Find(id);
            if (patrone == null)
            {
                return HttpNotFound();
            }
            ViewBag.Plaza_id = new SelectList(db.Plazas, "id", "descripcion", patrone.Plaza_id);
            ViewBag.estatus = new SelectList(db.Conceptos, "id", "descripcion", patrone.estatus);
            return View(patrone);
        }

        // POST: Patrones/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,registro,rfc,nombre,actividad,domicilio,municipio,codigoPostal,entidad,telefono,remision,zona,delegacion,carEnt,numeroDelegacion,carDel,numSub,tipoConvenio,convenio,inicioAfiliacion,patRep,clase,fraccion,STyPS,Plaza_id,direccionArchivo,porcentajeNomina,unidadMedica,estatus")] Patrone patrone)
        {
            if (ModelState.IsValid)
            {
                string value = Request["SampleChkIntBool"];
                if (value.Substring(0, 4) == "true") 
                {
                    patrone.direccionArchivo = patrone.registro;
                }
                else
                {
                    patrone.direccionArchivo = null;
                }
                db.Entry(patrone).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Plaza_id = new SelectList(db.Plazas, "id", "descripcion", patrone.Plaza_id);
            ViewBag.estatus = new SelectList(db.Conceptos, "id", "descripcion", patrone.estatus);
            return View(patrone);
        }

        // GET: Patrones/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Patrone patrone = db.Patrones.Find(id);
            if (patrone == null)
            {
                return HttpNotFound();
            }
            return View(patrone);
        }

        // POST: Patrones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Patrone patrone = db.Patrones.Find(id);
            db.Patrones.Remove(patrone);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        
        
        [HttpGet]
        public void GetExcel(String plazasId, String statusId)
        {

            FileStream fileStream = null;
            MemoryStream mem = new MemoryStream();
            try
            {

                Usuario user = Session["UsuarioData"] as Usuario;
            var patronesAsignados = (from x in db.TopicosUsuarios
                                     where x.usuarioId.Equals(user.Id)
                                     && x.tipo.Equals("B")
                                     select x.topicoId);

            var patrones = from s in db.Patrones
                           where patronesAsignados.Contains(s.Id)
                           select s;

            if (!String.IsNullOrEmpty(plazasId))
            {
                int plazaIdTemp = int.Parse(plazasId);
                patrones = patrones.Where(p => p.Plaza.id.Equals(plazaIdTemp));
            }

            if (statusId != null)
            {
                @ViewBag.statusId = statusId;

                if (statusId.Trim().Equals("A"))
                {
                    ViewBag.activos = patrones.Where(s => s.Concepto.descripcion.Equals("Activo")).Count();
                    patrones = patrones.Where(s => s.Concepto.descripcion.Equals("Activo"));
                }
                else if (statusId.Trim().Equals("B"))
                {
                    ViewBag.activos = patrones.Where(s => s.Concepto.descripcion.Equals("Inactivo")).Count();
                    patrones = patrones.Where(s => s.Concepto.descripcion.Equals("Inactivo"));
                }
            }

            ViewBag.activos = patrones.Where(s => s.Concepto.descripcion.Equals("Activo")).Count();
            ViewBag.registros = patrones.Count();

            patrones.OrderBy(p => p.nombre);
               
            List<Patrone> allCust = new List<Patrone>();

            allCust = patrones.ToList();

                DateTime date = DateTime.Now;
                String path = @"C:\\SUA\\Exceles\\";
                String fileName = @"Patrones-" + date.ToString("ddMMyyyyHHmm") + ".xlsx";
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

                SheetData sd = crearContenidoHojaD(allCust, eh);//CreateContentRow(); 
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
        public SheetData crearContenidoHojaD(List<Patrone> patrones, ExcelHelper eh)
        {
            SheetData sheetData = new SheetData();
            int index = 1;

            //Creamos el Header
            Row row = new Row();
            row = eh.addNewCellToRow(index, row, "Registro Patronal", headerColumns[0] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "ID Plaza", headerColumns[1] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "RFC", headerColumns[2] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Nombre", headerColumns[3] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Teléfono", headerColumns[4] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Domicilio", headerColumns[5] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Zona", headerColumns[6] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Ini.Afiliación", headerColumns[7] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "STyPS", headerColumns[8] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);
            
            row = eh.addNewCellToRow(index, row, "Entidad", headerColumns[9] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Delegación", headerColumns[10] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "C. P.", headerColumns[11] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);
            
            row = eh.addNewCellToRow(index, row, "Carpeta", headerColumns[12] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "% sobre nómina", headerColumns[13] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "UMF", headerColumns[14] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Estaus A/B", headerColumns[15] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            index++;
            //Create the cells that contain the data.
            foreach (Patrone dp in patrones)
            {
                int i = 0;

                row = eh.addNewCellToRow(index, row, dp.registro, headerColumns[i] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Plaza.cveCorta, headerColumns[i + 1] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.rfc, headerColumns[i + 2] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.nombre, headerColumns[i + 3] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.telefono, headerColumns[i + 4] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.domicilio, headerColumns[i + 5] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.zona, headerColumns[i + 6] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.inicioAfiliacion, headerColumns[i + 7] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.STyPS, headerColumns[i + 8] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);
                
                row = eh.addNewCellToRow(index, row, dp.entidad, headerColumns[i + 9] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);
                
                row = eh.addNewCellToRow(index, row, dp.delegacion, headerColumns[i + 10] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);
                
                row = eh.addNewCellToRow(index, row, dp.codigoPostal, headerColumns[i + 11] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                if (dp.direccionArchivo != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.direccionArchivo, headerColumns[i + 12] + index, 2U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 12] + index, 2U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.porcentajeNomina != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.porcentajeNomina.ToString(), headerColumns[i + 13] + index, 2U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 13] + index, 2U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.unidadMedica != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.unidadMedica, headerColumns[i + 14] + index, 2U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 14] + index, 2U, CellValues.String);
                    sheetData.AppendChild(row);
                }

                if (dp.Concepto != null)
                {
                    row = eh.addNewCellToRow(index, row, dp.Concepto.descripcion, headerColumns[i + 15] + index, 2U, CellValues.String);
                    sheetData.AppendChild(row);
                }
                else
                {
                    row = eh.addNewCellToRow(index, row, " ", headerColumns[i + 15] + index, 2U, CellValues.String);
                    sheetData.AppendChild(row);
                }

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
