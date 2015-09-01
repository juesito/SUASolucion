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
    public class SolicitudPrenominasController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: SolicitudPrenominas
        public ActionResult Index(String clienteId, String proyectoId, String ejercicioId)
        {

            ViewBag.clienteId = clienteId;
            ViewBag.proyectoId = proyectoId;
            ViewBag.ejercicioId = ejercicioId;

            var solicitudPrenominas = db.SolicitudPrenominas.Include(s => s.Cliente).Include(s => s.Concepto).Include(s => s.Concepto1).Include(s => s.Concepto2).Include(s => s.Concepto3).Include(s => s.Plaza).Include(s => s.Usuario);
            return View(solicitudPrenominas.ToList());
        }

        // GET: SolicitudPrenominas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SolicitudPrenomina solicitudPrenomina = db.SolicitudPrenominas.Find(id);
            if (solicitudPrenomina == null)
            {
                return HttpNotFound();
            }
            return View(solicitudPrenomina);
        }

        // GET: SolicitudPrenominas/Create
        public ActionResult Create(String clienteId, String proyectoId, String ejercicioId)
        {
            SolicitudPrenomina solicitudPrenomina = new SolicitudPrenomina();
            if (!String.IsNullOrEmpty(ejercicioId) && !String.IsNullOrEmpty(clienteId) && !String.IsNullOrEmpty(proyectoId))
            {
                DateTime now = DateTime.Now;
                solicitudPrenomina.clienteId = int.Parse(clienteId);
                solicitudPrenomina.proyectoId = int.Parse(proyectoId);
                solicitudPrenomina.anno = ejercicioId;
                solicitudPrenomina.fechaSolicitud = now;
                solicitudPrenomina.fechaInicial = now;
                solicitudPrenomina.fechaFinal = now;
                solicitudPrenomina.fechaPago = now;

                return View(solicitudPrenomina);
            }

            return RedirectToAction("Index", "SolicitudPrenomina");
        }

        // POST: SolicitudPrenominas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,folioSolicitud,clienteId,plazaId,fechaSolicitud,fechaInicial,fechaFinal,fechaPago,folioCliente,tipoPagoId,periodoId,tipoContratoId,monedaId,tipoCambio,solicita,valida,autoriza,noTrabajadores,observaciones,estatusSolicitud,proyectoId,anno,fechaEnvio,usuarioId")] SolicitudPrenomina solicitudPrenomina)
        {
            if (ModelState.IsValid)
            {
                db.SolicitudPrenominas.Add(solicitudPrenomina);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.clienteId = new SelectList(db.Clientes, "Id", "claveCliente", solicitudPrenomina.clienteId);
            ViewBag.tipoPagoId = new SelectList(db.Conceptos, "id", "grupo", solicitudPrenomina.tipoPagoId);
            ViewBag.periodoId = new SelectList(db.Conceptos, "id", "grupo", solicitudPrenomina.periodoId);
            ViewBag.tipoContratoId = new SelectList(db.Conceptos, "id", "grupo", solicitudPrenomina.tipoContratoId);
            ViewBag.monedaId = new SelectList(db.Conceptos, "id", "grupo", solicitudPrenomina.monedaId);
            ViewBag.plazaId = new SelectList(db.Plazas, "id", "descripcion", solicitudPrenomina.plazaId);
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", solicitudPrenomina.usuarioId);
            return View(solicitudPrenomina);
        }

        // GET: SolicitudPrenominas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SolicitudPrenomina solicitudPrenomina = db.SolicitudPrenominas.Find(id);
            if (solicitudPrenomina == null)
            {
                return HttpNotFound();
            }
            ViewBag.clienteId = new SelectList(db.Clientes, "Id", "claveCliente", solicitudPrenomina.clienteId);
            ViewBag.tipoPagoId = new SelectList(db.Conceptos, "id", "grupo", solicitudPrenomina.tipoPagoId);
            ViewBag.periodoId = new SelectList(db.Conceptos, "id", "grupo", solicitudPrenomina.periodoId);
            ViewBag.tipoContratoId = new SelectList(db.Conceptos, "id", "grupo", solicitudPrenomina.tipoContratoId);
            ViewBag.monedaId = new SelectList(db.Conceptos, "id", "grupo", solicitudPrenomina.monedaId);
            ViewBag.plazaId = new SelectList(db.Plazas, "id", "descripcion", solicitudPrenomina.plazaId);
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", solicitudPrenomina.usuarioId);
            return View(solicitudPrenomina);
        }

        // POST: SolicitudPrenominas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,folioSolicitud,clienteId,plazaId,fechaSolicitud,fechaInicial,fechaFinal,fechaPago,folioCliente,tipoPagoId,periodoId,tipoContratoId,monedaId,tipoCambio,solicita,valida,autoriza,noTrabajadores,observaciones,estatusSolicitud,proyectoId,anno,fechaEnvio,usuarioId")] SolicitudPrenomina solicitudPrenomina)
        {
            if (ModelState.IsValid)
            {
                db.Entry(solicitudPrenomina).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.clienteId = new SelectList(db.Clientes, "Id", "claveCliente", solicitudPrenomina.clienteId);
            ViewBag.tipoPagoId = new SelectList(db.Conceptos, "id", "grupo", solicitudPrenomina.tipoPagoId);
            ViewBag.periodoId = new SelectList(db.Conceptos, "id", "grupo", solicitudPrenomina.periodoId);
            ViewBag.tipoContratoId = new SelectList(db.Conceptos, "id", "grupo", solicitudPrenomina.tipoContratoId);
            ViewBag.monedaId = new SelectList(db.Conceptos, "id", "grupo", solicitudPrenomina.monedaId);
            ViewBag.plazaId = new SelectList(db.Plazas, "id", "descripcion", solicitudPrenomina.plazaId);
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", solicitudPrenomina.usuarioId);
            return View(solicitudPrenomina);
        }

        // GET: SolicitudPrenominas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SolicitudPrenomina solicitudPrenomina = db.SolicitudPrenominas.Find(id);
            if (solicitudPrenomina == null)
            {
                return HttpNotFound();
            }
            return View(solicitudPrenomina);
        }

        // POST: SolicitudPrenominas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SolicitudPrenomina solicitudPrenomina = db.SolicitudPrenominas.Find(id);
            db.SolicitudPrenominas.Remove(solicitudPrenomina);
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
