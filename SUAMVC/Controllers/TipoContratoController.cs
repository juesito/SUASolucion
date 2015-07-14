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
    public class TipoContratoController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: TipoContratoes
        public ActionResult Index()
        {
            var tipoContratoes = db.TipoContratoes.Include(t => t.Usuario);
            return View(tipoContratoes.ToList());
        }

        // GET: TipoContratoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoContrato tipoContrato = db.TipoContratoes.Find(id);
            if (tipoContrato == null)
            {
                return HttpNotFound();
            }
            return View(tipoContrato);
        }

        // GET: TipoContratoes/Create
        public ActionResult Create()
        {
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario");
            return View();
        }

        // POST: TipoContratoes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,descripcion,fechaCreacion,usuarioId")] TipoContrato tipoContrato)
        {
            if (ModelState.IsValid)
            {
                Usuario usuario = Session["UsuarioData"] as Usuario;

                tipoContrato.fechaCreacion = DateTime.Now;
                tipoContrato.usuarioId = usuario.Id;
                db.TipoContratoes.Add(tipoContrato);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", tipoContrato.usuarioId);
            return View(tipoContrato);
        }

        // GET: TipoContratoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoContrato tipoContrato = db.TipoContratoes.Find(id);
            if (tipoContrato == null)
            {
                return HttpNotFound();
            }
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", tipoContrato.usuarioId);
            return View(tipoContrato);
        }

        // POST: TipoContratoes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,descripcion,fechaCreacion,usuarioId")] TipoContrato tipoContrato)
        {
            if (ModelState.IsValid)
            {
                Usuario usuario = Session["UsuarioData"] as Usuario;

                tipoContrato.fechaCreacion = DateTime.Now;
                tipoContrato.usuarioId = usuario.Id;
                db.Entry(tipoContrato).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", tipoContrato.usuarioId);
            return View(tipoContrato);
        }

        // GET: TipoContratoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoContrato tipoContrato = db.TipoContratoes.Find(id);
            if (tipoContrato == null)
            {
                return HttpNotFound();
            }
            return View(tipoContrato);
        }

        // POST: TipoContratoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TipoContrato tipoContrato = db.TipoContratoes.Find(id);
            db.TipoContratoes.Remove(tipoContrato);
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
