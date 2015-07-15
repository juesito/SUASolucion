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
    public class CuentaEmpleadosController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: CuentaEmpleados
        public ActionResult Index(int empleadoId)
        {
            Empleado empleado = db.Empleados.Find(empleadoId);
            var cuentaEmpleadoes = db.CuentaEmpleadoes.Where(c => c.empleadoId.Equals(empleadoId));
            ViewBag.empleado = empleado;
            return View(cuentaEmpleadoes.ToList());
        }

        // GET: CuentaEmpleados/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CuentaEmpleado cuentaEmpleado = db.CuentaEmpleadoes.Find(id);
            if (cuentaEmpleado == null)
            {
                return HttpNotFound();
            }
            return View(cuentaEmpleado);
        }

        // GET: CuentaEmpleados/Create
        public ActionResult Create(int empleadoId)
        {
            CuentaEmpleado cuentaEmpleado = new CuentaEmpleado();

            Empleado empleado = db.Empleados.Find(empleadoId);
            cuentaEmpleado.Empleado = empleado;
            cuentaEmpleado.empleadoId = empleadoId;

            return View(cuentaEmpleado);
        }

        // POST: CuentaEmpleados/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,empleadoId,bancoId,cuenta,cuentaClabe,fechaCreacion,usuarioId")] CuentaEmpleado cuentaEmpleado)
        {
            if (ModelState.IsValid)
            {

                Usuario usuario = Session["UsuarioData"] as Usuario;
                cuentaEmpleado.usuarioId = usuario.Id;
                cuentaEmpleado.fechaCreacion = DateTime.Now;

                db.CuentaEmpleadoes.Add(cuentaEmpleado);
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
                return RedirectToAction("Index", new { empleadoId = cuentaEmpleado.empleadoId });
            }

            return View(cuentaEmpleado);
        }

        // GET: CuentaEmpleados/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CuentaEmpleado cuentaEmpleado = db.CuentaEmpleadoes.Find(id);
            if (cuentaEmpleado == null)
            {
                return HttpNotFound();
            }
            ViewBag.bancoId = new SelectList(db.Bancos, "id", "descripcion", cuentaEmpleado.bancoId);
            ViewBag.empleadoId = new SelectList(db.Empleados, "id", "folioEmpleado", cuentaEmpleado.empleadoId);
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", cuentaEmpleado.usuarioId);
            return View(cuentaEmpleado);
        }

        // POST: CuentaEmpleados/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,empleadoId,bancoId,cuenta,cuentaClabe,fechaCreacion,usuarioId")] CuentaEmpleado cuentaEmpleado)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cuentaEmpleado).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { empleadoId = cuentaEmpleado.empleadoId });
            }
            return View(cuentaEmpleado);
        }

        // GET: CuentaEmpleados/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CuentaEmpleado cuentaEmpleado = db.CuentaEmpleadoes.Find(id);
            if (cuentaEmpleado == null)
            {
                return HttpNotFound();
            }
            return View(cuentaEmpleado);
        }

        // POST: CuentaEmpleados/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CuentaEmpleado cuentaEmpleado = db.CuentaEmpleadoes.Find(id);
            db.CuentaEmpleadoes.Remove(cuentaEmpleado);
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
