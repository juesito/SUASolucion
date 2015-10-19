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
using System.IO;
using SUAMVC.Models;
using System.Data.Entity.Validation;
using System.Diagnostics;

namespace SUAMVC.Controllers
{
    public class ArchivoEmpleadoesController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: ArchivoEmpleadoes
        public ActionResult Index(String empleadoId, String tipo)
        {
            if (!String.IsNullOrEmpty(tipo))
            {
                ToolsHelper th = new ToolsHelper();
                Concepto tipoArchivo = th.obtenerConceptoPorGrupo("ARCHEMP", tipo);

                if (!String.IsNullOrEmpty(empleadoId) && tipoArchivo != null)
                {
                    int empId = int.Parse(empleadoId.Trim());
                    Empleado empleado = db.Empleados.Find(empId);

                    //Filtramos los archivos por id y tipo documento
                    var ArchivoEmpleados = db.ArchivoEmpleadoes.
                        Include(a => a.Concepto).Include(a => a.Empleado).Include(a => a.Usuario)
                        .Where(a => a.empleadoId.Equals(empId) && a.tipoArchivo.Equals(tipoArchivo.id));

                    //Asignamos las variables a mostrar en la vista
                    ViewBag.empleadoId = empleado.id;
                    ViewBag.folioEmpleado = empleado.folioEmpleado;
                    ViewBag.tipoArchivo = tipoArchivo.descripcion;
                    ViewBag.nombreEmpleado = empleado.nombreCompleto;

                    //ordenamos por fecha de creación
                    ArchivoEmpleados = ArchivoEmpleados.OrderBy(a => a.fechaCreacion);

                    return View(ArchivoEmpleados.ToList());
                }

                else
                {
                    return RedirectToAction("Index", "Empleados");
                }//empleadoId y tipoArchivo Diferente de Null?
            }
            else
            {
                if (!String.IsNullOrEmpty(empleadoId))
                {
                    int empId = int.Parse(empleadoId.Trim());
                    Empleado empleado = db.Empleados.Find(empId);

                    //Filtramos los archivos por id y tipo documento
                    var archivoEmpleados = db.ArchivoEmpleadoes.
                        Include(a => a.Concepto).Include(a => a.Empleado).Include(a => a.Usuario)
                        .Where(a => a.empleadoId.Equals(empId));

                    //Asignamos las variables a mostrar en la vista
                    ViewBag.empleadoId = empleado.id;
                    ViewBag.folioEmpleado = empleado.folioEmpleado;
                    ViewBag.tipoArchivo = "Todos: CV, Confidencial, Psicometrico, Documentos varios";
                    ViewBag.nombreEmpleado = empleado.nombreCompleto;

                    //ordenamos por fecha de creación
                    archivoEmpleados = archivoEmpleados.OrderBy(a => a.fechaCreacion);

                    return View(archivoEmpleados.ToList());
                }//tipoArchivo Diferente de Null?

            }
            return RedirectToAction("Index", "Empleados");

        }

        // GET: ArchivoEmpleadoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ArchivoEmpleado archivoEmpleado = db.ArchivoEmpleadoes.Find(id);
            if (archivoEmpleado == null)
            {
                return HttpNotFound();
            }
            return View(archivoEmpleado);
        }

        // GET: ArchivoEmpleadoes/Create
        public ActionResult Create(String empleadoId)
        {
            ArchivoEmpleado archivoEmpleado = new ArchivoEmpleado();
            if (!String.IsNullOrEmpty(empleadoId))
            {
                int emplId = int.Parse(empleadoId);
                Empleado empleado = db.Empleados.Find(emplId);
                archivoEmpleado.Empleado = empleado;
                archivoEmpleado.empleadoId = empleado.id;
            }

            return View(archivoEmpleado);
        }

        // POST: ArchivoEmpleadoes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,empleadoId,archivo,tipoArchivo,usuarioId,fechaCreacion")] ArchivoEmpleado archivoEmpleado, String usuarioId)
        {
            if (ModelState.IsValid)
            {
                if (Request.Files.Count > 0)
                {
                    //Obtenemos el empleado y el documento.
                    Empleado empleado = db.Empleados.Find(archivoEmpleado.empleadoId);
                    archivoEmpleado.Empleado = empleado;
                    HttpFileCollectionBase files = Request.Files;

                    archivoEmpleado.archivo = GuardarDocumentos(usuarioId, archivoEmpleado.empleadoId,
                        archivoEmpleado.tipoArchivo, archivoEmpleado.Empleado.folioEmpleado, files);

                    archivoEmpleado.fechaCreacion = DateTime.Now;
                    //Guardamos nuestras entidades

                    try
                    {
                        db.ArchivoEmpleadoes.Add(archivoEmpleado);
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

            ViewBag.tipoArchivo = new SelectList(db.Conceptos, "id", "grupo", archivoEmpleado.tipoArchivo);
            ViewBag.empleadoId = new SelectList(db.Empleados, "id", "folioEmpleado", archivoEmpleado.empleadoId);
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", archivoEmpleado.usuarioId);
            return View(archivoEmpleado);
        }

        // GET: ArchivoEmpleadoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ArchivoEmpleado archivoEmpleado = db.ArchivoEmpleadoes.Find(id);
            if (archivoEmpleado == null)
            {
                return HttpNotFound();
            }
            ViewBag.tipoArchivo = new SelectList(db.Conceptos, "id", "grupo", archivoEmpleado.tipoArchivo);
            ViewBag.empleadoId = new SelectList(db.Empleados, "id", "folioEmpleado", archivoEmpleado.empleadoId);
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", archivoEmpleado.usuarioId);
            return View(archivoEmpleado);
        }

        // POST: ArchivoEmpleadoes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,empleadoId,archivo,tipoArchivo,usuarioId,fechaCreacion")] ArchivoEmpleado archivoEmpleado)
        {
            if (ModelState.IsValid)
            {
                db.Entry(archivoEmpleado).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.tipoArchivo = new SelectList(db.Conceptos, "id", "grupo", archivoEmpleado.tipoArchivo);
            ViewBag.empleadoId = new SelectList(db.Empleados, "id", "folioEmpleado", archivoEmpleado.empleadoId);
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", archivoEmpleado.usuarioId);
            return View(archivoEmpleado);
        }


        public ActionResult VerDocumento(String fileNameString, String folioEmpleado, String tipo)
        {
            if (!String.IsNullOrEmpty(fileNameString))
            {

                ParametrosHelper ph = new ParametrosHelper();
                Parametro parameter = ph.getParameterByKey("DOCFOLDER");
                var fileName = parameter.valorString.Trim() + folioEmpleado.Trim() + "\\" + tipo + "\\" + fileNameString.Trim();

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

        // GET: ArchivoEmpleadoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ArchivoEmpleado archivoEmpleado = db.ArchivoEmpleadoes.Find(id);
            if (archivoEmpleado == null)
            {
                return HttpNotFound();
            }
            return View(archivoEmpleado);
        }

        // POST: ArchivoEmpleadoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ToolsHelper th = new ToolsHelper();

            ArchivoEmpleado archivosEmpleado = db.ArchivoEmpleadoes.Find(id);
            archivosEmpleado = db.ArchivoEmpleadoes.Find(id);
            th.BorrarArchivo(archivosEmpleado.archivo.Trim());
            db.ArchivoEmpleadoes.Remove(archivosEmpleado);
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
