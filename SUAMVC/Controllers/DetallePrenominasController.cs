using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SUADATOS;
using Newtonsoft.Json;
using SUAMVC.Models;
using System.Data.Entity.Validation;
using System.Text;

namespace SUAMVC.Controllers
{
    public class DetallePrenominasController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: DetallePrenominas
        public ActionResult Index(String solicitudId)
        {
            var detallePrenominas = db.DetallePrenominas.Include(d => d.CuentaEmpleado).Include(d => d.Empleado).Include(d => d.SolicitudPrenomina).Include(d => d.Usuario);

            if (!String.IsNullOrEmpty(solicitudId))
            {
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

                //Tenemos que tomar solo los empleados que no esten en otra prenomina
                //y si ya estan incluidos en alguna prenomina del mismo año no incluirla
                //o si?

                //Tomo los empleados que ya estan en la solicitud incluidos para excluirlos.
                List<int> empleadosIds = solicitud.DetallePrenominas.
                    Where(s=> s.solicitudId.Equals(solicitudInt)).
                    Select(s => s.empleadoId).ToList();

                empleados = (from s in db.SolicitudEmpleadoes
                             where s.Solicitud.clienteId.Equals(clienteId)
                             && s.Solicitud.proyectoId.Equals(proyectoId)
                             && !empleadosIds.Contains(s.empleadoId)
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
                SolicitudPrenomina solicitud = db.SolicitudPrenominas.Find(solicitudIdTemp);
                
                foreach (String empleadoId in ids)
                {

                    int empleadoIdTemp = int.Parse(empleadoId);
                    DetallePrenomina detallePrenomina = new DetallePrenomina();

                    detallePrenomina.empleadoId = empleadoIdTemp;
                    detallePrenomina.solicitudId = solicitudIdTemp;
                    detallePrenomina.diasLaborados = 0;
                    detallePrenomina.ingresos = 0;
                    detallePrenomina.gratificacion = 0;
                    detallePrenomina.descuentoFonacot = 0;
                    detallePrenomina.descuentoInfonavit = 0;
                    detallePrenomina.descuentoPension = 0;
                    detallePrenomina.otrosDescuentos = 0;
                    detallePrenomina.netoPagar = 0;
                    detallePrenomina.premioAsistencia = 0;
                    detallePrenomina.usuarioId = usuario.Id;
                    detallePrenomina.fechaCreacion = date;
                    detallePrenomina.diasLaborados = int.Parse(solicitud.Concepto.valorConcepto);

                    db.DetallePrenominas.Add(detallePrenomina);
                    db.SaveChanges();

                }
                solicitud.noTrabajadores = db.DetallePrenominas.Where(s => s.solicitudId.Equals(solicitudIdTemp)).Count();
                db.Entry(solicitud).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index", "DetallePrenominas", new { solicitudId = solicitudId });
            }
            else
            {
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
            SolicitudPrenomina solicitud = detallePrenomina.SolicitudPrenomina;

            
            db.DetallePrenominas.Remove(detallePrenomina);
            db.SaveChanges();
            int solicitudIdTemp = solicitud.id;
            solicitud.noTrabajadores = db.DetallePrenominas.Where(s => s.solicitudId.Equals(solicitudIdTemp)).Count();
            db.Entry(solicitud).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index", new { solicitudId = solicitudIdTemp});
        }

        [HttpPost]
        public ActionResult updateEmployee(DetallePrenomina detallePrenomina)
        {
            Usuario usuario = Session["UsuarioData"] as Usuario;
            DetallePrenomina dt = db.DetallePrenominas.Find(detallePrenomina.id);
            dt.totalSyS = 0;
            
            dt.ingresos = dt.Empleado.salarioReal;
            dt.diasLaborados = detallePrenomina.diasLaborados;
            dt.gratificacion = (Decimal)detallePrenomina.gratificacion;
            dt.primaVacacional = (Decimal)detallePrenomina.primaVacacional;
            dt.descuentoInfonavit = (Decimal)detallePrenomina.descuentoInfonavit;
            dt.descuentoFonacot = (Decimal)detallePrenomina.descuentoFonacot;
            dt.descuentoPension = (Decimal)detallePrenomina.descuentoPension;
            dt.otrosDescuentos = (Decimal)detallePrenomina.otrosDescuentos;
            dt.netoPagar = dt.ingresos + dt.gratificacion + dt.primaVacacional -
                dt.descuentoInfonavit - dt.descuentoPension - dt.descuentoFonacot -
                dt.otrosDescuentos;
            dt.fechaCreacion = DateTime.Now;
            dt.usuarioId = usuario.Id;

            //Guardamos los cambios
            try
            {
                db.Entry(dt).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                StringBuilder sb = new StringBuilder();

                foreach (var failure in ex.EntityValidationErrors)
                {
                    sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
                    foreach (var error in failure.ValidationErrors)
                    {
                        sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                        sb.AppendLine();
                    }
                }
            }

            DetallePrenomina dtp = new DetallePrenomina();
            dtp.id = dt.id;
            dtp.diasLaborados = dt.diasLaborados;
            dtp.gratificacion = dt.gratificacion;
            dtp.primaVacacional = dt.primaVacacional;
            dtp.descuentoInfonavit = dt.descuentoInfonavit;
            dtp.descuentoFonacot = dt.descuentoFonacot;
            dtp.otrosDescuentos = dt.otrosDescuentos;
            dtp.descuentoPension = dt.descuentoPension;
            dtp.netoPagar = dt.netoPagar;

            return Json(new { employee = dtp }, JsonRequestBehavior.AllowGet);

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
