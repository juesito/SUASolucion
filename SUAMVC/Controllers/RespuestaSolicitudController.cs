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
    public class RespuestaSolicitudController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: RespuestaSolicitud
        public ActionResult Index()
        {
            var respuestaSolicituds = db.RespuestaSolicituds.Include(r => r.Concepto).Include(r => r.Departamento).Include(r => r.Solicitud).Include(r => r.Usuario);
            return View(respuestaSolicituds.ToList());
        }

        // GET: RespuestaSolicitud/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RespuestaSolicitud respuestaSolicitud = db.RespuestaSolicituds.Find(id);
            if (respuestaSolicitud == null)
            {
                return HttpNotFound();
            }
            return View(respuestaSolicitud);
        }

        // GET: RespuestaSolicitud/Create
        public ActionResult Create(string departamentoId, string folioSolicitudId)
        {
            int folioSolicitudTempId = int.Parse(folioSolicitudId);
            Departamento dep = new Departamento();
            if (departamentoId.Equals("N"))
            {
                dep = db.Departamentos.Where(d => d.descripcion.Contains("Nomina")).FirstOrDefault();
            }
            else if(departamentoId.Equals("I"))
            {
                dep = db.Departamentos.Where(d => d.descripcion.Contains("IMSS")).FirstOrDefault();
            }
            else if (departamentoId.Equals("J")) 
            {
                dep = db.Departamentos.Where(d => d.descripcion.Contains("Juridico")).FirstOrDefault();
            }
            else if (departamentoId.Equals("T"))
            {
                dep = db.Departamentos.Where(d => d.descripcion.Contains("Tarjeta de Credito")).FirstOrDefault();
            }

            Solicitud solicitud = db.Solicituds.Find(folioSolicitudTempId);

            //Creamos objeto del tipo que la vista espera.
            RespuestaSolicitud respuestaSolicitud = new RespuestaSolicitud();

            respuestaSolicitud.solicitudId = solicitud.id;
            respuestaSolicitud.Solicitud = solicitud;
            respuestaSolicitud.departamentoId = dep.id;
            respuestaSolicitud.Departamento = dep;



            ViewBag.estatusId = new SelectList(db.Conceptos, "id", "grupo");
            ViewBag.departamentoId = new SelectList(db.Departamentos, "id", "descripcion");
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario");
            return View(respuestaSolicitud);
        }

        // POST: RespuestaSolicitud/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,solicitudId,departamentoId,estatusId,observaciones,usuarioId,fechaCreacion")] RespuestaSolicitud respuestaSolicitud)
        {
            if (ModelState.IsValid)
            {
                Usuario usuario = Session["UsuarioData"] as Usuario;



                respuestaSolicitud.fechaCreacion = DateTime.Now;
                respuestaSolicitud.usuarioId = usuario.Id;
                db.RespuestaSolicituds.Add(respuestaSolicitud);
                db.SaveChanges();
                return RedirectToAction("Index", "PanelSolicitud");
            }

            ViewBag.estatusId = new SelectList(db.Conceptos, "id", "grupo", respuestaSolicitud.estatusId);
            ViewBag.departamentoId = new SelectList(db.Departamentos, "id", "descripcion", respuestaSolicitud.departamentoId);
            ViewBag.solicitudId = new SelectList(db.Solicituds, "id", "folioSolicitud", respuestaSolicitud.solicitudId);
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", respuestaSolicitud.usuarioId);
            return View(respuestaSolicitud);
        }

        // GET: RespuestaSolicitud/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RespuestaSolicitud respuestaSolicitud = db.RespuestaSolicituds.Find(id);
            if (respuestaSolicitud == null)
            {
                return HttpNotFound();
            }
            ViewBag.estatusId = new SelectList(db.Conceptos, "id", "grupo", respuestaSolicitud.estatusId);
            ViewBag.departamentoId = new SelectList(db.Departamentos, "id", "descripcion", respuestaSolicitud.departamentoId);
            ViewBag.solicitudId = new SelectList(db.Solicituds, "id", "folioSolicitud", respuestaSolicitud.solicitudId);
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", respuestaSolicitud.usuarioId);
            return View(respuestaSolicitud);
        }

        // POST: RespuestaSolicitud/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,solicitudId,departamentoId,estatusId,observaciones,usuarioId,fechaCreacion")] RespuestaSolicitud respuestaSolicitud)
        {
            if (ModelState.IsValid)
            {
                Usuario usuario = Session["UsuarioData"] as Usuario;

                respuestaSolicitud.fechaCreacion = DateTime.Now;
                respuestaSolicitud.usuarioId = usuario.Id;
                db.Entry(respuestaSolicitud).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.estatusId = new SelectList(db.Conceptos, "id", "grupo", respuestaSolicitud.estatusId);
            ViewBag.departamentoId = new SelectList(db.Departamentos, "id", "descripcion", respuestaSolicitud.departamentoId);
            ViewBag.solicitudId = new SelectList(db.Solicituds, "id", "folioSolicitud", respuestaSolicitud.solicitudId);
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", respuestaSolicitud.usuarioId);
            return View(respuestaSolicitud);
        }

        // GET: RespuestaSolicitud/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RespuestaSolicitud respuestaSolicitud = db.RespuestaSolicituds.Find(id);
            if (respuestaSolicitud == null)
            {
                return HttpNotFound();
            }
            return View(respuestaSolicitud);
        }

        // POST: RespuestaSolicitud/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RespuestaSolicitud respuestaSolicitud = db.RespuestaSolicituds.Find(id);
            db.RespuestaSolicituds.Remove(respuestaSolicitud);
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
