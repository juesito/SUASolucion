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
    public class DetallePrenominasController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: DetallePrenominas
        public ActionResult Index(String solicitudId)
        {
            var detallePrenominas = db.DetallePrenominas.Include(d => d.CuentaEmpleado).Include(d => d.Empleado).Include(d => d.SolicitudPrenomina).Include(d => d.Usuario);

            if(!String.IsNullOrEmpty(solicitudId)){
                int solicitudInt = int.Parse(solicitudId);
                detallePrenominas = detallePrenominas.Where(s => s.solicitudId.Equals(solicitudInt));
                ViewBag.solicitudId = solicitudInt;
            }
            return View(detallePrenominas.ToList());
        }

        // GET: DetallePrenominas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DetallePrenomina detallePrenomina = db.DetallePrenominas.Find(id);
            if (detallePrenomina == null)
            {
                return HttpNotFound();
            }
            return View(detallePrenomina);
        }

        // GET: DetallePrenominas/Create
        public ActionResult Create()
        {
            ViewBag.cuentaId = new SelectList(db.CuentaEmpleadoes, "id", "cuenta");
            ViewBag.empleadoId = new SelectList(db.Empleados, "id", "folioEmpleado");
            ViewBag.solicitudId = new SelectList(db.SolicitudPrenominas, "id", "folioSolicitud");
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario");
            return View();
        }

        public ActionResult AddEmployees(String solicitudId)
        {
            
            List<Empleado> empleados = new List<Empleado>();
            SolicitudEmpleado solicitudEmpleado = new SolicitudEmpleado();

            if (!String.IsNullOrEmpty(solicitudId))
            {
                int solicitudInt = int.Parse(solicitudId);
                SolicitudPrenomina solicitud = db.SolicitudPrenominas.Find(solicitudInt);
                int clienteId = solicitud.clienteId;
                int proyectoId = solicitud.proyectoId;

                empleados = (from s in db.SolicitudEmpleadoes
                                      where s.Solicitud.clienteId.Equals(clienteId)
                                      && s.Solicitud.proyectoId.Equals(proyectoId)
                                      orderby s.empleadoId
                                      select s.Empleado).ToList();  


                ViewBag.solicitudId = solicitudInt;
            }
            return View(empleados);
        }

        public ActionResult AsignarEmpleado(String solicitudId, String[] ids)
        {

            if (ids != null && ids.Length > 0 && !String.IsNullOrEmpty(solicitudId))
            {
                Usuario usuario = Session["UsuarioData"] as Usuario;
                DateTime date = DateTime.Now;

                int solicitudIdTemp = int.Parse(solicitudId);

                foreach (String empleadoId in ids)
                {

                    int empleadoIdTemp = int.Parse(empleadoId);
                    DetallePrenomina detallePrenomina = new DetallePrenomina();

                    detallePrenomina.empleadoId = empleadoIdTemp;
                    detallePrenomina.solicitudId = solicitudIdTemp;
                    detallePrenomina.diasLaborados = 0;
                    detallePrenomina.ingresos = 0;
                    detallePrenomina.gratificacion = 0;
                    detallePrenomina.usuarioId = usuario.Id;
                    detallePrenomina.fechaCreacion = date;

                    db.DetallePrenominas.Add(detallePrenomina);
                    db.SaveChanges();

                }
                return RedirectToAction("Index", "DetallePrenominas", new { solicitudId = solicitudId });
            }
            else {
                return RedirectToAction("AddEmployees", "DetallePrenominas", new { solicitudId = solicitudId });
            }
            
        }

        // POST: DetallePrenominas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,solicitudId,empleadoId,diasLaborados,ingresos,gratificacion,primaVacacioal,aguinaldo,descuentoInfonavit,descuentoFonacot,descuentoPension,otrosDescuentos,netoPagar,cuentaId,fechaCreacion,usuarioId")] DetallePrenomina detallePrenomina)
        {
            if (ModelState.IsValid)
            {
                db.DetallePrenominas.Add(detallePrenomina);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.cuentaId = new SelectList(db.CuentaEmpleadoes, "id", "cuenta", detallePrenomina.cuentaId);
            ViewBag.empleadoId = new SelectList(db.Empleados, "id", "folioEmpleado", detallePrenomina.empleadoId);
            ViewBag.solicitudId = new SelectList(db.SolicitudPrenominas, "id", "folioSolicitud", detallePrenomina.solicitudId);
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", detallePrenomina.usuarioId);
            return View(detallePrenomina);
        }

        // GET: DetallePrenominas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DetallePrenomina detallePrenomina = db.DetallePrenominas.Find(id);
            if (detallePrenomina == null)
            {
                return HttpNotFound();
            }
            ViewBag.cuentaId = new SelectList(db.CuentaEmpleadoes, "id", "cuenta", detallePrenomina.cuentaId);
            ViewBag.empleadoId = new SelectList(db.Empleados, "id", "folioEmpleado", detallePrenomina.empleadoId);
            ViewBag.solicitudId = new SelectList(db.SolicitudPrenominas, "id", "folioSolicitud", detallePrenomina.solicitudId);
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", detallePrenomina.usuarioId);
            return View(detallePrenomina);
        }

        // POST: DetallePrenominas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,solicitudId,empleadoId,diasLaborados,ingresos,gratificacion,primaVacacioal,aguinaldo,descuentoInfonavit,descuentoFonacot,descuentoPension,otrosDescuentos,netoPagar,cuentaId,fechaCreacion,usuarioId")] DetallePrenomina detallePrenomina)
        {
            if (ModelState.IsValid)
            {
                db.Entry(detallePrenomina).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.cuentaId = new SelectList(db.CuentaEmpleadoes, "id", "cuenta", detallePrenomina.cuentaId);
            ViewBag.empleadoId = new SelectList(db.Empleados, "id", "folioEmpleado", detallePrenomina.empleadoId);
            ViewBag.solicitudId = new SelectList(db.SolicitudPrenominas, "id", "folioSolicitud", detallePrenomina.solicitudId);
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", detallePrenomina.usuarioId);
            return View(detallePrenomina);
        }

        // GET: DetallePrenominas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DetallePrenomina detallePrenomina = db.DetallePrenominas.Find(id);
            if (detallePrenomina == null)
            {
                return HttpNotFound();
            }
            return View(detallePrenomina);
        }

        // POST: DetallePrenominas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DetallePrenomina detallePrenomina = db.DetallePrenominas.Find(id);
            db.DetallePrenominas.Remove(detallePrenomina);
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
