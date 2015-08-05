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
    public class FamiliaresEmpleadosController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: FamiliaresEmpleados
        public ActionResult Index(int empleadoid)
        {
            
            ViewBag.empleado = db.Empleados.Find(empleadoid);
            var familiaresEmpleadoes = db.FamiliaresEmpleadoes.Include(f => f.Concepto).Include(f => f.Empleado).Include(f => f.Usuario);
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
            Empleado empleado = db.Empleados.Find(empleadoId);
            FamiliaresEmpleado familiaresEmpleado = new FamiliaresEmpleado();
            familiaresEmpleado.empleadoId = empleadoId;
            familiaresEmpleado.Empleado = empleado;

            return View(familiaresEmpleado);
        }

        // POST: FamiliaresEmpleados/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,empleadoId,parentescoId,nombre,apellidoMaterno,apellidoPaterno,nombreCompleto,telefonoCelular,telefonoCasa,email,fechaCreacion,usuarioId")] FamiliaresEmpleado familiaresEmpleado)
        {
            if (ModelState.IsValid)
            {
                db.FamiliaresEmpleadoes.Add(familiaresEmpleado);
                db.SaveChanges();
                return RedirectToAction("Index");
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
