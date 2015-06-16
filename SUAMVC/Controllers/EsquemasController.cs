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
    public class EsquemasController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: Esquemas
        public ActionResult Index()
        {
            var esquemas = db.Esquemas.Include(e => e.Usuario);
            return View(esquemas.ToList());
        }

        // GET: Esquemas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Esquema esquema = db.Esquemas.Find(id);
            if (esquema == null)
            {
                return HttpNotFound();
            }
            return View(esquema);
        }

        // GET: Esquemas/Create
        public ActionResult Create()
        {
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario");
            return View();
        }

        // POST: Esquemas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,descripcion,fechaCreacion,usuarioId")] Esquema esquema)
        {
            if (ModelState.IsValid)
            {
                db.Esquemas.Add(esquema);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", esquema.usuarioId);
            return View(esquema);
        }

        // GET: Esquemas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Esquema esquema = db.Esquemas.Find(id);
            if (esquema == null)
            {
                return HttpNotFound();
            }
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", esquema.usuarioId);
            return View(esquema);
        }

        // POST: Esquemas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,descripcion,fechaCreacion,usuarioId")] Esquema esquema)
        {
            if (ModelState.IsValid)
            {
                db.Entry(esquema).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", esquema.usuarioId);
            return View(esquema);
        }

        // GET: Esquemas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Esquema esquema = db.Esquemas.Find(id);
            if (esquema == null)
            {
                return HttpNotFound();
            }
            return View(esquema);
        }

        // POST: Esquemas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Esquema esquema = db.Esquemas.Find(id);
            db.Esquemas.Remove(esquema);
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
