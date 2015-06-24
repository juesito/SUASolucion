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
    public class GirosController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: Giros
        public ActionResult Index()
        {
            var giros = db.Giros.Include(g => g.Usuario);
            return View(giros.ToList());
        }

        // GET: Giros/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Giro giro = db.Giros.Find(id);
            if (giro == null)
            {
                return HttpNotFound();
            }
            return View(giro);
        }

        // GET: Giros/Create
        public ActionResult Create()
        {
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario");
            return View();
        }

        // POST: Giros/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,descripcion,fechaCreacion,usuarioId")] Giro giro)
        {
            if (ModelState.IsValid)
            {
                db.Giros.Add(giro);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", giro.usuarioId);
            return View(giro);
        }

        // GET: Giros/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Giro giro = db.Giros.Find(id);
            if (giro == null)
            {
                return HttpNotFound();
            }
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", giro.usuarioId);
            return View(giro);
        }

        // POST: Giros/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,descripcion,fechaCreacion,usuarioId")] Giro giro)
        {
            if (ModelState.IsValid)
            {
                db.Entry(giro).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", giro.usuarioId);
            return View(giro);
        }

        // GET: Giros/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Giro giro = db.Giros.Find(id);
            if (giro == null)
            {
                return HttpNotFound();
            }
            return View(giro);
        }

        // POST: Giros/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Giro giro = db.Giros.Find(id);
            db.Giros.Remove(giro);
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
