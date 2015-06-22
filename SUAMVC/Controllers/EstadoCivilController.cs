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
    public class EstadoCivilController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: EstadoCivil
        public ActionResult Index()
        {
            var estadoCivils = db.EstadoCivils.Include(e => e.Usuario);
            return View(estadoCivils.ToList());
        }

        // GET: EstadoCivil/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EstadoCivil estadoCivil = db.EstadoCivils.Find(id);
            if (estadoCivil == null)
            {
                return HttpNotFound();
            }
            return View(estadoCivil);
        }

        // GET: EstadoCivil/Create
        public ActionResult Create()
        {
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario");
            return View();
        }

        // POST: EstadoCivil/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,descripcion,fechaCreacion,usuarioId")] EstadoCivil estadoCivil)
        {
            if (ModelState.IsValid)
            {
                estadoCivil.fechaCreacion = DateTime.Now;
                estadoCivil.usuarioId = 1;
                db.EstadoCivils.Add(estadoCivil);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", estadoCivil.usuarioId);
            return View(estadoCivil);
        }

        // GET: EstadoCivil/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EstadoCivil estadoCivil = db.EstadoCivils.Find(id);
            if (estadoCivil == null)
            {
                return HttpNotFound();
            }
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", estadoCivil.usuarioId);
            return View(estadoCivil);
        }

        // POST: EstadoCivil/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,descripcion,fechaCreacion,usuarioId")] EstadoCivil estadoCivil)
        {
            if (ModelState.IsValid)
            {
                db.Entry(estadoCivil).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", estadoCivil.usuarioId);
            return View(estadoCivil);
        }

        // GET: EstadoCivil/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EstadoCivil estadoCivil = db.EstadoCivils.Find(id);
            if (estadoCivil == null)
            {
                return HttpNotFound();
            }
            return View(estadoCivil);
        }

        // POST: EstadoCivil/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EstadoCivil estadoCivil = db.EstadoCivils.Find(id);
            db.EstadoCivils.Remove(estadoCivil);
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
