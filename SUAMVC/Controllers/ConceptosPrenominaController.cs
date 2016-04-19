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
    public class ConceptosPrenominaController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: ConceptosPrenómina
        public ActionResult Index()
        {
            var conceptosPrenómina = db.ConceptosPrenómina.Include(c => c.Usuario).OrderByDescending(s => s.dedPer);
            return View(conceptosPrenómina.ToList());
        }

        // GET: ConceptosPrenómina/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConceptosPrenómina conceptosPrenómina = db.ConceptosPrenómina.Find(id);
            if (conceptosPrenómina == null)
            {
                return HttpNotFound();
            }
            return View(conceptosPrenómina);
        }

        // GET: ConceptosPrenómina/Create
        public ActionResult Create()
        {
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario");
            return View();
        }

        // POST: ConceptosPrenómina/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,descripcion,encabezado,dedPer")] ConceptosPrenómina conceptosPrenómina)
        {
            if (ModelState.IsValid)
            {
                Usuario usuario = Session["UsuarioData"] as Usuario;

                conceptosPrenómina.fechaCreacion = DateTime.Now;
                conceptosPrenómina.usuarioId = usuario.Id;
                db.ConceptosPrenómina.Add(conceptosPrenómina);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", conceptosPrenómina.usuarioId);
            return View(conceptosPrenómina);
        }

        // GET: ConceptosPrenómina/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConceptosPrenómina conceptosPrenómina = db.ConceptosPrenómina.Find(id);
            if (conceptosPrenómina == null)
            {
                return HttpNotFound();
            }
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", conceptosPrenómina.usuarioId);
            return View(conceptosPrenómina);
        }

        // POST: ConceptosPrenómina/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,descripcion,encabezado,dedPer,fechaCreacion,usuarioId")] ConceptosPrenómina conceptosPrenómina)
        {
            if (ModelState.IsValid)
            {
                Usuario usuario = Session["UsuarioData"] as Usuario;

                conceptosPrenómina.fechaCreacion = DateTime.Now;
                conceptosPrenómina.usuarioId = usuario.Id;
                db.Entry(conceptosPrenómina).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", conceptosPrenómina.usuarioId);
            return View(conceptosPrenómina);
        }

        // GET: ConceptosPrenómina/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConceptosPrenómina conceptosPrenómina = db.ConceptosPrenómina.Find(id);
            if (conceptosPrenómina == null)
            {
                return HttpNotFound();
            }
            return View(conceptosPrenómina);
        }

        // POST: ConceptosPrenómina/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ConceptosPrenómina conceptosPrenómina = db.ConceptosPrenómina.Find(id);
            db.ConceptosPrenómina.Remove(conceptosPrenómina);
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
