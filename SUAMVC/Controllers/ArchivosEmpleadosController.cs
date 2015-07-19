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
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;

namespace SUAMVC.Controllers
{
    public class ArchivosEmpleadosController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: ArchivosEmpleados
        public ActionResult Index(String empleadoId)
        {
            var archivosEmpleados = db.ArchivosEmpleados.Include(a => a.Concepto).Include(a => a.Empleado).Include(a => a.Usuario);
            if (!String.IsNullOrEmpty(empleadoId))
            {
                int empId = int.Parse(empleadoId.Trim());
                Empleado empleado = db.Empleados.Find(empId);
                @ViewBag.folioEmpleado = empleado.folioEmpleado;
                archivosEmpleados = archivosEmpleados.Where(a => a.empleadoId.Equals(empId));
            }
            else {
                return RedirectToAction("Index", "Empleados");
            }
            archivosEmpleados = archivosEmpleados.OrderBy(a => a.fechaCreacion);

            return View(archivosEmpleados.ToList());
        }

                
        // GET: ArchivosEmpleados/Create
        public ActionResult Create(String empleadoId)
        {
            ArchivosEmpleado archivoEmpleado = new ArchivosEmpleado();
            if (!String.IsNullOrEmpty(empleadoId))
            {
                int emplId = int.Parse(empleadoId);
                Empleado empleado = db.Empleados.Find(emplId);
                archivoEmpleado.Empleado = empleado;
                archivoEmpleado.empleadoId = empleado.id;
            }

            return View(archivoEmpleado);
        }

        // POST: ArchivosEmpleados/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,empleadoId,archivo,tipoArchivo,usuarioId,fechaCreacion")] 
            ArchivosEmpleado archivosEmpleado, String usuarioId)
        {
            if (ModelState.IsValid)
            {

                if (Request.Files.Count > 0)
                {
                    //Obtenemos el empleado y el documento.
                    Empleado empleado = db.Empleados.Find(archivosEmpleado.empleadoId);
                    archivosEmpleado.Empleado = empleado;
                    HttpFileCollectionBase files = Request.Files;

                    archivosEmpleado.archivo = GuardarDocumentos(usuarioId, archivosEmpleado.empleadoId,
                        archivosEmpleado.tipoArchivo, archivosEmpleado.Empleado.folioEmpleado, files);

                    archivosEmpleado.fechaCreacion = DateTime.Now;
                    //Guardamos nuestras entidades

                    try
                    {
                        db.ArchivosEmpleados.Add(archivosEmpleado);
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
            }

            return View(archivosEmpleado);
        }

        public ActionResult VerDocumento(String fileNameString)
        {
            if (!String.IsNullOrEmpty(fileNameString))
            {

                var fileName = fileNameString.Trim();

                if (System.IO.File.Exists(fileName))
                {
                    FileStream fs = new FileStream(fileName, FileMode.Open);
                    ToolsHelper th = new ToolsHelper();

                    return File(fs, th.getMimeType(fileName.Trim()));
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public String GuardarDocumentos(String usuarioId, int empleadoId, int documentoId, String folioCliente,
            HttpFileCollectionBase files)
        {
            String fileName = "";

            //Obtenemos el concepto
            Concepto concepto = db.Conceptos.Find(documentoId);

            ToolsHelper th = new ToolsHelper();
            ParametrosHelper ph = new ParametrosHelper();

            //C:\SUA\Empleados\CONFIDENCIAL or DOC or CV or PSI or CONTRATOS\
            Parametro parametro = ph.getParameterByKey("DOCFOLDER");
            String destino = parametro.valorString.Trim() + folioCliente.Trim() + "\\" + concepto.descripcion.Trim() + "\\";

            //Guardamos el archivo
            fileName = th.cargarArchivo(files, destino);

            return fileName;
        }

        // GET: ArchivosEmpleados/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ArchivosEmpleado archivosEmpleado = db.ArchivosEmpleados.Find(id);
            if (archivosEmpleado == null)
            {
                return HttpNotFound();
            }
            return View(archivosEmpleado);
        }

        // POST: ArchivosEmpleados/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ToolsHelper th = new ToolsHelper();

            ArchivosEmpleado archivosEmpleado = db.ArchivosEmpleados.Find(id);
            th.BorrarArchivo(archivosEmpleado.archivo.Trim());
            db.ArchivosEmpleados.Remove(archivosEmpleado);
            db.SaveChanges();
            return RedirectToAction("Index");
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
