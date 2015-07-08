using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SUADATOS;

namespace SUAMVC.Controllers
{
    public class ArchivosEmpleadosController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: ArchivosEmpleados
        public ActionResult Index()
        {
            var archivosEmpleados = db.ArchivosEmpleados.Include(a => a.Concepto).Include(a => a.Empleado).Include(a => a.Usuario);
            return View(archivosEmpleados.ToList());
        }

        // GET: ArchivosEmpleados/Details/5
        public ActionResult Details(int? id)
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

        // GET: ArchivosEmpleados/Create
        public ActionResult Create()
        {
            ViewBag.tipoArchivo = new SelectList(db.Conceptos, "id", "grupo");
            ViewBag.empleadoId = new SelectList(db.Empleados, "id", "folioEmpleado");
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario");
            return View();
        }

        // POST: ArchivosEmpleados/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,empleadoId,archivo,tipoArchivo,usuarioId,fechaCreacoin")] ArchivosEmpleado archivosEmpleado)
        {
            if (ModelState.IsValid)
            {
                db.ArchivosEmpleados.Add(archivosEmpleado);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.tipoArchivo = new SelectList(db.Conceptos, "id", "grupo", archivosEmpleado.tipoArchivo);
            ViewBag.empleadoId = new SelectList(db.Empleados, "id", "folioEmpleado", archivosEmpleado.empleadoId);
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", archivosEmpleado.usuarioId);
            return View(archivosEmpleado);
        }

        // GET: ArchivosEmpleados/Edit/5
        public ActionResult Edit(int? id)
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
            ViewBag.tipoArchivo = new SelectList(db.Conceptos, "id", "grupo", archivosEmpleado.tipoArchivo);
            ViewBag.empleadoId = new SelectList(db.Empleados, "id", "folioEmpleado", archivosEmpleado.empleadoId);
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", archivosEmpleado.usuarioId);
            return View(archivosEmpleado);
        }

        // POST: ArchivosEmpleados/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,empleadoId,archivo,tipoArchivo,usuarioId,fechaCreacoin")] ArchivosEmpleado archivosEmpleado)
        {
            if (ModelState.IsValid)
            {
                db.Entry(archivosEmpleado).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.tipoArchivo = new SelectList(db.Conceptos, "id", "grupo", archivosEmpleado.tipoArchivo);
            ViewBag.empleadoId = new SelectList(db.Empleados, "id", "folioEmpleado", archivosEmpleado.empleadoId);
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", archivosEmpleado.usuarioId);
            return View(archivosEmpleado);
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
            ArchivosEmpleado archivosEmpleado = db.ArchivosEmpleados.Find(id);
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
