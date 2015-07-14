using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SUADATOS;
using System.Data.Entity.Validation;
using System.Diagnostics;

namespace SUAMVC.Controllers
{
    public class FamiliaresEmpleadosController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: FamiliaresEmpleados
        public ActionResult Index(int empleadoId)
        {
            Empleado empleado = db.Empleados.Find(empleadoId);
            var familiaresEmpleadoes = db.FamiliaresEmpleadoes.Where(fe => fe.empleadoId.Equals(empleadoId));
            ViewBag.empleado = empleado;
            
            return View(familiaresEmpleadoes.ToList());
        }

        // GET: FamiliaresEmpleados/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FamiliaresEmpleado familiaresEmpleado = db.FamiliaresEmpleadoes.Find(id);
            if (familiaresEmpleado == null)
            {
                return HttpNotFound();
            }
            return View(familiaresEmpleado);
        }

        // GET: FamiliaresEmpleados/Create
        public ActionResult Create(int empleadoId)
        {
            FamiliaresEmpleado familiaresEmpleado = new FamiliaresEmpleado();
            Empleado empleado = db.Empleados.Find(empleadoId);

            familiaresEmpleado.empleadoId = empleado.id;
            familiaresEmpleado.Empleado = empleado;
            

            return View(familiaresEmpleado);
        }

        // POST: FamiliaresEmpleados/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,empleadoId,parentescoId,nombre,apellidoMaterno,apellidoPaterno,nombreCompleto,telefonoCelular,telefonoCasa,email,fechaCreacion,usuarioId")] FamiliaresEmpleado familiaresEmpleado, String empleadoId)
        {
            if (ModelState.IsValid)
            {
                Usuario usuario = Session["UsuarioData"] as Usuario;

                familiaresEmpleado.nombreCompleto = familiaresEmpleado.nombre.Trim() + " " + familiaresEmpleado.apellidoPaterno.Trim() + " " + familiaresEmpleado.apellidoMaterno.Trim();
                familiaresEmpleado.usuarioId = usuario.Id;
                familiaresEmpleado.fechaCreacion = DateTime.Now;
                db.FamiliaresEmpleadoes.Add(familiaresEmpleado);
                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        }
                    }
                }
                return RedirectToAction("Index", new { empleadoId = familiaresEmpleado.empleadoId});
            }

            return View(familiaresEmpleado);
        }

        // GET: FamiliaresEmpleados/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FamiliaresEmpleado familiaresEmpleado = db.FamiliaresEmpleadoes.Find(id);
            if (familiaresEmpleado == null)
            {
                return HttpNotFound();
            }
            ViewBag.parentescoId = new SelectList(db.Conceptos, "id", "grupo", familiaresEmpleado.parentescoId);
            ViewBag.empleadoId = new SelectList(db.Empleados, "id", "folioEmpleado", familiaresEmpleado.empleadoId);
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", familiaresEmpleado.usuarioId);
            return View(familiaresEmpleado);
        }

        // POST: FamiliaresEmpleados/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,empleadoId,parentescoId,nombre,apellidoMaterno,apellidoPaterno,nombreCompleto,telefonoCelular,telefonoCasa,email,fechaCreacion,usuarioId")] FamiliaresEmpleado familiaresEmpleado)
        {
            if (ModelState.IsValid)
            {
                db.Entry(familiaresEmpleado).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.parentescoId = new SelectList(db.Conceptos, "id", "grupo", familiaresEmpleado.parentescoId);
            ViewBag.empleadoId = new SelectList(db.Empleados, "id", "folioEmpleado", familiaresEmpleado.empleadoId);
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", familiaresEmpleado.usuarioId);
            return View(familiaresEmpleado);
        }

        // GET: FamiliaresEmpleados/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FamiliaresEmpleado familiaresEmpleado = db.FamiliaresEmpleadoes.Find(id);
            if (familiaresEmpleado == null)
            {
                return HttpNotFound();
            }
            return View(familiaresEmpleado);
        }

        // POST: FamiliaresEmpleados/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FamiliaresEmpleado familiaresEmpleado = db.FamiliaresEmpleadoes.Find(id);
            db.FamiliaresEmpleadoes.Remove(familiaresEmpleado);
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
