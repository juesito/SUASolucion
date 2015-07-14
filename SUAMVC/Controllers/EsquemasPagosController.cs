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
    public class EsquemasPagosController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: EsquemasPagos
        public ActionResult Index()
        {
            var esquemasPagoes = db.EsquemasPagoes.Include(e => e.Usuario);
            return View(esquemasPagoes.ToList());
        }

        // GET: EsquemasPagos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EsquemasPago esquemasPago = db.EsquemasPagoes.Find(id);
            if (esquemasPago == null)
            {
                return HttpNotFound();
            }
            return View(esquemasPago);
        }

        // GET: EsquemasPagos/Create
        public ActionResult Create()
        {
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario");
            return View();
        }

        // POST: EsquemasPagos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,descripcion,fechaCreacion,usuarioId")] EsquemasPago esquemasPago)
        {
            if (ModelState.IsValid)
            {
                Usuario usuario = Session["UsuarioData"] as Usuario;

                esquemasPago.fechaCreacion = DateTime.Now;
                esquemasPago.usuarioId = usuario.Id;
                db.EsquemasPagoes.Add(esquemasPago);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", esquemasPago.usuarioId);
            return View(esquemasPago);
        }

        // GET: EsquemasPagos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EsquemasPago esquemasPago = db.EsquemasPagoes.Find(id);
            if (esquemasPago == null)
            {
                return HttpNotFound();
            }
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", esquemasPago.usuarioId);
            return View(esquemasPago);
        }

        // POST: EsquemasPagos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,descripcion,fechaCreacion,usuarioId")] EsquemasPago esquemasPago)
        {
            if (ModelState.IsValid)
            {
                Usuario usuario = Session["UsuarioData"] as Usuario;

                esquemasPago.fechaCreacion = DateTime.Now;
                esquemasPago.usuarioId = usuario.Id;
                db.Entry(esquemasPago).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", esquemasPago.usuarioId);
            return View(esquemasPago);
        }

        // GET: EsquemasPagos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EsquemasPago esquemasPago = db.EsquemasPagoes.Find(id);
            if (esquemasPago == null)
            {
                return HttpNotFound();
            }
            return View(esquemasPago);
        }

        // POST: EsquemasPagos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EsquemasPago esquemasPago = db.EsquemasPagoes.Find(id);
            db.EsquemasPagoes.Remove(esquemasPago);
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
