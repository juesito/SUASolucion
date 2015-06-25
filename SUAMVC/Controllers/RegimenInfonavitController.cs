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
    public class RegimenInfonavitController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: RegimenInfonavit
        public ActionResult Index()
        {
            var regimenInfonavits = db.RegimenInfonavits.Include(r => r.Usuario);
            return View(regimenInfonavits.ToList());
        }

        // GET: RegimenInfonavit/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RegimenInfonavit regimenInfonavit = db.RegimenInfonavits.Find(id);
            if (regimenInfonavit == null)
            {
                return HttpNotFound();
            }
            return View(regimenInfonavit);
        }

        // GET: RegimenInfonavit/Create
        public ActionResult Create()
        {
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario");
            return View();
        }

        // POST: RegimenInfonavit/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,descripcion,fechaCreacion,usuarioId")] RegimenInfonavit regimenInfonavit)
        {
            if (ModelState.IsValid)
            {
                Usuario usuario = Session["UsuarioData"] as Usuario;

                regimenInfonavit.fechaCreacion = DateTime.Now;
                regimenInfonavit.usuarioId = usuario.Id;
                db.RegimenInfonavits.Add(regimenInfonavit);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", regimenInfonavit.usuarioId);
            return View(regimenInfonavit);
        }

        // GET: RegimenInfonavit/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RegimenInfonavit regimenInfonavit = db.RegimenInfonavits.Find(id);
            if (regimenInfonavit == null)
            {
                return HttpNotFound();
            }
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", regimenInfonavit.usuarioId);
            return View(regimenInfonavit);
        }

        // POST: RegimenInfonavit/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,descripcion,fechaCreacion,usuarioId")] RegimenInfonavit regimenInfonavit)
        {
            if (ModelState.IsValid)
            {
                Usuario usuario = Session["UsuarioData"] as Usuario;

                regimenInfonavit.fechaCreacion = DateTime.Now;
                regimenInfonavit.usuarioId = usuario.Id;
                db.Entry(regimenInfonavit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", regimenInfonavit.usuarioId);
            return View(regimenInfonavit);
        }

        // GET: RegimenInfonavit/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RegimenInfonavit regimenInfonavit = db.RegimenInfonavits.Find(id);
            if (regimenInfonavit == null)
            {
                return HttpNotFound();
            }
            return View(regimenInfonavit);
        }

        // POST: RegimenInfonavit/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RegimenInfonavit regimenInfonavit = db.RegimenInfonavits.Find(id);
            db.RegimenInfonavits.Remove(regimenInfonavit);
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
