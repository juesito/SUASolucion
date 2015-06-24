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
    public class TipoPersonalController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: TipoPersonal
        public ActionResult Index()
        {
            var tipoPersonals = db.TipoPersonals.Include(t => t.Usuario);
            return View(tipoPersonals.ToList());
        }

        // GET: TipoPersonal/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoPersonal tipoPersonal = db.TipoPersonals.Find(id);
            if (tipoPersonal == null)
            {
                return HttpNotFound();
            }
            return View(tipoPersonal);
        }

        // GET: TipoPersonal/Create
        public ActionResult Create()
        {
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario");
            return View();
        }

        // POST: TipoPersonal/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,descripcion,fechaCreacion,usuarioId")] TipoPersonal tipoPersonal)
        {
            if (ModelState.IsValid)
            {
                tipoPersonal.fechaCreacion = DateTime.Now;
                tipoPersonal.usuarioId = 1;
                db.TipoPersonals.Add(tipoPersonal);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", tipoPersonal.usuarioId);
            return View(tipoPersonal);
        }

        // GET: TipoPersonal/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoPersonal tipoPersonal = db.TipoPersonals.Find(id);
            if (tipoPersonal == null)
            {
                return HttpNotFound();
            }
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", tipoPersonal.usuarioId);
            return View(tipoPersonal);
        }

        // POST: TipoPersonal/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,descripcion,fechaCreacion,usuarioId")] TipoPersonal tipoPersonal)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tipoPersonal).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", tipoPersonal.usuarioId);
            return View(tipoPersonal);
        }

        // GET: TipoPersonal/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoPersonal tipoPersonal = db.TipoPersonals.Find(id);
            if (tipoPersonal == null)
            {
                return HttpNotFound();
            }
            return View(tipoPersonal);
        }

        // POST: TipoPersonal/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TipoPersonal tipoPersonal = db.TipoPersonals.Find(id);
            db.TipoPersonals.Remove(tipoPersonal);
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
