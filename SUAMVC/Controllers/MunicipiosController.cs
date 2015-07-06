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
    public class MunicipiosController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: Municipios
        public ActionResult Index(String estadoId)
        {
            var municipios = db.Municipios.Include(m => m.Estado).Include(m => m.Pais).Include(m => m.Usuario);
            if (!String.IsNullOrEmpty(estadoId)) {
                int estadoIntId = int.Parse(estadoId);
                municipios = municipios.Where(m => m.estadoId.Equals(estadoIntId));
            }

            return View(municipios.ToList());
        }

        // GET: Municipios/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Municipio municipio = db.Municipios.Find(id);
            if (municipio == null)
            {
                return HttpNotFound();
            }
            return View(municipio);
        }

        // GET: Municipios/Create
        public ActionResult Create()
        {
            ViewBag.estadoId = new SelectList(db.Estados, "id", "descripcion");
            ViewBag.paisId = new SelectList(db.Paises, "id", "descripcion");
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario");
            return View();
        }

        // POST: Municipios/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,paisId,estadoId,descripcion,fechaCreacion,usuarioId")] Municipio municipio)
        {
            if (ModelState.IsValid)
            {
                Usuario usuario = Session["UsuarioData"] as Usuario;

                municipio.fechaCreacion = DateTime.Now;
                municipio.usuarioId = usuario.Id;
                db.Municipios.Add(municipio);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.estadoId = new SelectList(db.Estados, "id", "descripcion", municipio.estadoId);
            ViewBag.paisId = new SelectList(db.Paises, "id", "descripcion", municipio.paisId);
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", municipio.usuarioId);
            return View(municipio);
        }

        // GET: Municipios/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Municipio municipio = db.Municipios.Find(id);
            if (municipio == null)
            {
                return HttpNotFound();
            }
            ViewBag.estadoId = new SelectList(db.Estados, "id", "descripcion", municipio.estadoId);
            ViewBag.paisId = new SelectList(db.Paises, "id", "descripcion", municipio.paisId);
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", municipio.usuarioId);
            return View(municipio);
        }

        // POST: Municipios/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,paisId,estadoId,descripcion,fechaCreacion,usuarioId")] Municipio municipio)
        {
            if (ModelState.IsValid)
            {
                Usuario usuario = Session["UsuarioData"] as Usuario;

                municipio.fechaCreacion = DateTime.Now;
                municipio.usuarioId = usuario.Id;
                db.Entry(municipio).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.estadoId = new SelectList(db.Estados, "id", "descripcion", municipio.estadoId);
            ViewBag.paisId = new SelectList(db.Paises, "id", "descripcion", municipio.paisId);
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", municipio.usuarioId);
            return View(municipio);
        }

        // GET: Municipios/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Municipio municipio = db.Municipios.Find(id);
            if (municipio == null)
            {
                return HttpNotFound();
            }
            return View(municipio);
        }

        // POST: Municipios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Municipio municipio = db.Municipios.Find(id);
            db.Municipios.Remove(municipio);
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
