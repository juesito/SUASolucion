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
    public class SDIsController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: SDIs
        public ActionResult Index()
        {
            var sDIs = db.SDIs.Include(s => s.Usuario);
            return View(sDIs.ToList());
        }

        // GET: SDIs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SDI sDI = db.SDIs.Find(id);
            if (sDI == null)
            {
                return HttpNotFound();
            }
            return View(sDI);
        }

        // GET: SDIs/Create
        public ActionResult Create()
        {
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario");
            return View();
        }

        // POST: SDIs/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,descripcion,fechaCreacion,usuarioId,clienteId")] SDI sDI)
        {
            if (ModelState.IsValid)
            {
                Usuario usuario = Session["UsuarioData"] as Usuario;

                sDI.fechaCreacion = DateTime.Now;
                sDI.usuarioId = usuario.Id;
                db.SDIs.Add(sDI);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", sDI.usuarioId);
            return View(sDI);
        }

        // GET: SDIs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SDI sDI = db.SDIs.Find(id);
            if (sDI == null)
            {
                return HttpNotFound();
            }
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", sDI.usuarioId);
            return View(sDI);
        }

        // POST: SDIs/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,descripcion,fechaCreacion,usuarioId,clienteId")] SDI sDI)
        {
            if (ModelState.IsValid)
            {
                Usuario usuario = Session["UsuarioData"] as Usuario;

                sDI.fechaCreacion = DateTime.Now;
                sDI.usuarioId = usuario.Id;
                db.Entry(sDI).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", sDI.usuarioId);
            return View(sDI);
        }

        // GET: SDIs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SDI sDI = db.SDIs.Find(id);
            if (sDI == null)
            {
                return HttpNotFound();
            }
            return View(sDI);
        }

        // POST: SDIs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SDI sDI = db.SDIs.Find(id);
            db.SDIs.Remove(sDI);
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
