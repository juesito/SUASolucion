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
    public class SexoController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: Sexoes
        public ActionResult Index()
        {
            var sexos = db.Sexos.Include(s => s.Usuario);
            return View(sexos.ToList());
        }

        // GET: Sexoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sexo sexo = db.Sexos.Find(id);
            if (sexo == null)
            {
                return HttpNotFound();
            }
            return View(sexo);
        }

        // GET: Sexoes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Sexoes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,descripcion")] Sexo sexo)
        {
            if (ModelState.IsValid)
            {
                sexo.fechaCreacion = DateTime.Now;
                sexo.usuarioId = 1;
                db.Sexos.Add(sexo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", sexo.usuarioId);
            return View(sexo);
        }

        // GET: Sexoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sexo sexo = db.Sexos.Find(id);
            if (sexo == null)
            {
                return HttpNotFound();
            }
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", sexo.usuarioId);
            return View(sexo);
        }

        // POST: Sexoes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,descripcion")] Sexo sexo)
        {
            if (ModelState.IsValid)
            {
                sexo.fechaCreacion = DateTime.Now;
                sexo.usuarioId = 1;
                db.Entry(sexo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", sexo.usuarioId);
            return View(sexo);
        }

        // GET: Sexoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sexo sexo = db.Sexos.Find(id);
            if (sexo == null)
            {
                return HttpNotFound();
            }
            return View(sexo);
        }

        // POST: Sexoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Sexo sexo = db.Sexos.Find(id);
            db.Sexos.Remove(sexo);
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
