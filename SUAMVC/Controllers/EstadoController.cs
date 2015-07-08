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
    public class EstadoController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: Estado
        public ActionResult Index()
        {
            var estados = db.Estados.Include(e => e.Pais).Include(e => e.Usuario);
            return View(estados.ToList());
        }

        // GET: Estado/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Estado estado = db.Estados.Find(id);
            if (estado == null)
            {
                return HttpNotFound();
            }
            return View(estado);
        }

        // GET: Estado/Create
        public ActionResult Create()
        {
            ViewBag.paisId = new SelectList(db.Paises, "id", "descripcion");
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario");
            return View();
        }

        // POST: Estado/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,paisId,descripcion,fechaCreacion,usuarioId")] Estado estado)
        {
            if (ModelState.IsValid)
            {
                Usuario usuario = Session["UsuarioData"] as Usuario;

                estado.fechaCreacion = DateTime.Now;
                estado.usuarioId = usuario.Id;
                db.Estados.Add(estado);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.paisId = new SelectList(db.Paises, "id", "descripcion", estado.paisId);
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", estado.usuarioId);
            return View(estado);
        }

        // GET: Estado/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Estado estado = db.Estados.Find(id);
            if (estado == null)
            {
                return HttpNotFound();
            }
            ViewBag.paisId = new SelectList(db.Paises, "id", "descripcion", estado.paisId);
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", estado.usuarioId);
            return View(estado);
        }

        // POST: Estado/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,paisId,descripcion,fechaCreacion,usuarioId")] Estado estado)
        {
            if (ModelState.IsValid)
            {
                Usuario usuario = Session["UsuarioData"] as Usuario;

                estado.fechaCreacion = DateTime.Now;
                estado.usuarioId = usuario.Id;
                db.Entry(estado).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.paisId = new SelectList(db.Paises, "id", "descripcion", estado.paisId);
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", estado.usuarioId);
            return View(estado);
        }

        // GET: Estado/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Estado estado = db.Estados.Find(id);
            if (estado == null)
            {
                return HttpNotFound();
            }
            return View(estado);
        }

        // POST: Estado/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Estado estado = db.Estados.Find(id);
            db.Estados.Remove(estado);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult ObtenerEstadoPorId(int paisId)
        {
            List<Estado> estados = new List<Estado>();
            estados = db.Estados.Where(m => m.paisId == paisId).ToList();
            SelectList ciudad = new SelectList(estados, "Id", "descripcion", 0);
            return Json(ciudad);
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
