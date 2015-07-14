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
    public class FuncionesController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: Funciones
        public ActionResult Index(String moduloId)
        {
            var funcions = db.Funcions.Include(f => f.Modulo);
            if (!String.IsNullOrEmpty(moduloId)) {
                int moduloIntId = int.Parse(moduloId);
                funcions = funcions.Where(f => f.moduloId.Equals(moduloIntId));
            }
            
            return View(funcions.ToList());
        }

        // GET: Funciones/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Funcion funcion = db.Funcions.Find(id);
            if (funcion == null)
            {
                return HttpNotFound();
            }
            return View(funcion);
        }

        // GET: Funciones/Create
        public ActionResult Create()
        {
            ViewBag.moduloId = new SelectList(db.Modulos, "id", "descripcionCorta");
            return View();
        }

        // POST: Funciones/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,moduloId,descripcionCorta,descripcionLarga,accion,controlador,estatus,usuarioId,fechaCreacion,tipo")] Funcion funcion)
        {
            if (ModelState.IsValid)
            {
                //Usuario loggeado
                Usuario usuario = Session["UsuarioData"] as Usuario;

                funcion.fechaCreacion = DateTime.Now;
                funcion.usuarioId = usuario.Id;
                funcion.estatus = "A";
                db.Funcions.Add(funcion);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.moduloId = new SelectList(db.Modulos, "id", "descripcionCorta", funcion.moduloId);
            return View(funcion);
        }

        // GET: Funciones/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Funcion funcion = db.Funcions.Find(id);
            if (funcion == null)
            {
                return HttpNotFound();
            }
            ViewBag.moduloId = new SelectList(db.Modulos, "id", "descripcionCorta", funcion.moduloId);
            return View(funcion);
        }

        // POST: Funciones/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,moduloId,descripcionCorta,descripcionLarga,accion,controlador,estatus,tipo")] Funcion funcion)
        {
            if (ModelState.IsValid)
            {
                Usuario usuario = Session["UsuarioData"] as Usuario;

                funcion.fechaCreacion = DateTime.Now;
                funcion.usuarioId = usuario.Id;
                funcion.estatus = "A";
                db.Entry(funcion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.moduloId = new SelectList(db.Modulos, "id", "descripcionCorta", funcion.moduloId);
            return View(funcion);
        }

        // GET: Funciones/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Funcion funcion = db.Funcions.Find(id);
            if (funcion == null)
            {
                return HttpNotFound();
            }
            return View(funcion);
        }

        // POST: Funciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Funcion funcion = db.Funcions.Find(id);
            db.Funcions.Remove(funcion);
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
