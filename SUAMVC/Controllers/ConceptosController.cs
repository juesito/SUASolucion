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
    public class ConceptosController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: Conceptos
        public ActionResult Index()
        {
            var conceptos = db.Conceptos.Include(c => c.Usuario);
            return View(conceptos.ToList());
        }

        // GET: Conceptos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Concepto concepto = db.Conceptos.Find(id);
            if (concepto == null)
            {
                return HttpNotFound();
            }
            return View(concepto);
        }

        // GET: Conceptos/Create
        public ActionResult Create()
        {
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario");
            return View();
        }

        // POST: Conceptos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,grupo,descripcion,fechaCreacion,usuarioId")] Concepto concepto)
        {
            if (ModelState.IsValid)
            {
                Usuario usuario = Session["UsuarioData"] as Usuario;

                concepto.fechaCreacion = DateTime.Now;
                concepto.usuarioId = usuario.Id;
                db.Conceptos.Add(concepto);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", concepto.usuarioId);
            return View(concepto);
        }

        // GET: Conceptos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Concepto concepto = db.Conceptos.Find(id);
            if (concepto == null)
            {
                return HttpNotFound();
            }
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", concepto.usuarioId);
            return View(concepto);
        }

        // POST: Conceptos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,grupo,descripcion,fechaCreacion,usuarioId")] Concepto concepto)
        {
            if (ModelState.IsValid)
            {
                Usuario usuario = Session["UsuarioData"] as Usuario;

                concepto.fechaCreacion = DateTime.Now;
                concepto.usuarioId = usuario.Id;
                db.Entry(concepto).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", concepto.usuarioId);
            return View(concepto);
        }

        // GET: Conceptos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Concepto concepto = db.Conceptos.Find(id);
            if (concepto == null)
            {
                return HttpNotFound();
            }
            return View(concepto);
        }

        // POST: Conceptos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Concepto concepto = db.Conceptos.Find(id);
            db.Conceptos.Remove(concepto);
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
