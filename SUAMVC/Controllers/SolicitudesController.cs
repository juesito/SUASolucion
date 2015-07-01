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
using System.Text;

namespace SUAMVC.Controllers
{
    public class SolicitudesController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: Solicitudes
        public ActionResult Index(string clientesId, String folioId)
        {
            var solicituds = db.Solicituds.Include(s => s.Cliente).Include(s => s.Concepto).Include(s => s.Concepto1).Include(s => s.Concepto2).Include(s => s.Concepto3).Include(s => s.Concepto4).Include(s => s.EsquemasPago).Include(s => s.Plaza).Include(s => s.Proyecto).Include(s => s.SDI).Include(s => s.TipoContrato).Include(s => s.TipoPersonal).Include(s => s.Usuario);

            if (!String.IsNullOrEmpty(clientesId))
            {
                int clienteId = int.Parse(clientesId);
                Cliente cliente = db.Clientes.Find(clienteId);
                if (!cliente.descripcion.ToLower().Contains("seleccion"))
                {
                    
                    solicituds = solicituds.Where(s => s.clienteId.Equals(clienteId));
                }
            }
            if (!String.IsNullOrEmpty(folioId))
            {
                solicituds = solicituds.Where(s => s.folioSolicitud.Contains(folioId));
            }
            
            return View(solicituds.ToList());
        }

        // GET: Solicitudes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Solicitud solicitud = db.Solicituds.Find(id);
            if (solicitud == null)
            {
                return HttpNotFound();
            }
            return View(solicitud);
        }

        // GET: Solicitudes/Create
        public ActionResult Create()
        {
            ViewBag.clienteId = new SelectList(db.Clientes, "Id", "claveCliente");
            ViewBag.estatusSolicitud = new SelectList(db.Conceptos, "id", "grupo");
            ViewBag.estatusNomina = new SelectList(db.Conceptos, "id", "grupo");
            ViewBag.estatusJuridico = new SelectList(db.Conceptos, "id", "grupo");
            ViewBag.estatusAfiliado = new SelectList(db.Conceptos, "id", "grupo");
            ViewBag.estatusTarjeta = new SelectList(db.Conceptos, "id", "grupo");
            ViewBag.esquemaId = new SelectList(db.EsquemasPagoes, "id", "descripcion");
            ViewBag.plazaId = new SelectList(db.Plazas, "id", "descripcion");
            ViewBag.proyectoId = new SelectList(db.Proyectos, "id", "descripcion");
            ViewBag.sdiId = new SelectList(db.SDIs, "id", "descripcion");
            ViewBag.contratoId = new SelectList(db.TipoContratoes, "id", "descripcion");
            ViewBag.tipoPersonalId = new SelectList(db.TipoPersonals, "id", "descripcion");
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario");
            return View();
        }

        // POST: Solicitudes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,folioSolicitud,clienteId,plazaId,fechaSolicitud,esquemaId,sdiId,contratoId,fechaInicial,fechaFinal,tipoPersonalId,solicita,valida,autoriza,noTrabajadores,observaciones,estatusSolicitud,estatusNomina,estatusAfiliado,estatusJuridico,estatusTarjeta,usuarioId,proyectoId,fechaEnvio")] Solicitud solicitud)
        {
            if (ModelState.IsValid)
            {
                Usuario usuario = Session["usuarioData"] as Usuario;
                Cliente cliente = db.Clientes.Find(solicitud.clienteId);
                ListaValidacionCliente lvc = cliente.ListaValidacionClientes.First();
                Concepto concepto = db.Conceptos.Where(s => s.grupo.Equals("ESTASOL") && s.descripcion.ToLower().Contains("apertura")).First();
                solicitud.usuarioId = usuario.Id;
                solicitud.fechaSolicitud = DateTime.Now;
                solicitud.autoriza = lvc.autorizador;
                solicitud.valida = lvc.validador;
                solicitud.estatusSolicitud = concepto.id;
                solicitud.estatusNomina = concepto.id;
                solicitud.estatusJuridico = concepto.id;
                solicitud.estatusAfiliado = concepto.id;
                solicitud.estatusTarjeta = concepto.id;
                solicitud.Cliente = cliente;
                solicitud.clienteId = cliente.Id;
                solicitud.folioSolicitud = "";
                solicitud.noTrabajadores = 0;
                db.Solicituds.Add(solicitud);
                db.SaveChanges();

                try
                {
                    solicitud.folioSolicitud = solicitud.id.ToString().PadLeft(5, '0') + "A" + solicitud.Cliente.Plaza.cveCorta.Trim();
                    db.Entry(solicitud).State = EntityState.Modified;
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
                return RedirectToAction("Index");
            }

            ViewBag.clienteId = new SelectList(db.Clientes, "Id", "claveCliente", solicitud.clienteId);
            ViewBag.estatusSolicitud = new SelectList(db.Conceptos, "id", "grupo", solicitud.estatusSolicitud);
            ViewBag.estatusNomina = new SelectList(db.Conceptos, "id", "grupo", solicitud.estatusNomina);
            ViewBag.estatusJuridico = new SelectList(db.Conceptos, "id", "grupo", solicitud.estatusJuridico);
            ViewBag.estatusAfiliado = new SelectList(db.Conceptos, "id", "grupo", solicitud.estatusAfiliado);
            ViewBag.estatusTarjeta = new SelectList(db.Conceptos, "id", "grupo", solicitud.estatusTarjeta);
            ViewBag.esquemaId = new SelectList(db.EsquemasPagoes, "id", "descripcion", solicitud.esquemaId);
            ViewBag.plazaId = new SelectList(db.Plazas, "id", "descripcion", solicitud.plazaId);
            ViewBag.proyectoId = new SelectList(db.Proyectos, "id", "descripcion", solicitud.proyectoId);
            ViewBag.sdiId = new SelectList(db.SDIs, "id", "descripcion", solicitud.sdiId);
            ViewBag.contratoId = new SelectList(db.TipoContratoes, "id", "descripcion", solicitud.contratoId);
            ViewBag.tipoPersonalId = new SelectList(db.TipoPersonals, "id", "descripcion", solicitud.tipoPersonalId);
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", solicitud.usuarioId);
            return View(solicitud);
        }

        // GET: Solicitudes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Solicitud solicitud = db.Solicituds.Find(id);
            if (solicitud == null)
            {
                return HttpNotFound();
            }
            ViewBag.clienteId = new SelectList(db.Clientes, "Id", "claveCliente", solicitud.clienteId);
            ViewBag.estatusSolicitud = new SelectList(db.Conceptos, "id", "grupo", solicitud.estatusSolicitud);
            ViewBag.estatusNomina = new SelectList(db.Conceptos, "id", "grupo", solicitud.estatusNomina);
            ViewBag.estatusJuridico = new SelectList(db.Conceptos, "id", "grupo", solicitud.estatusJuridico);
            ViewBag.estatusAfiliado = new SelectList(db.Conceptos, "id", "grupo", solicitud.estatusAfiliado);
            ViewBag.estatusTarjeta = new SelectList(db.Conceptos, "id", "grupo", solicitud.estatusTarjeta);
            ViewBag.esquemaId = new SelectList(db.EsquemasPagoes, "id", "descripcion", solicitud.esquemaId);
            ViewBag.plazaId = new SelectList(db.Plazas, "id", "descripcion", solicitud.plazaId);
            ViewBag.proyectoId = new SelectList(db.Proyectos, "id", "descripcion", solicitud.proyectoId);
            ViewBag.sdiId = new SelectList(db.SDIs, "id", "descripcion", solicitud.sdiId);
            ViewBag.contratoId = new SelectList(db.TipoContratoes, "id", "descripcion", solicitud.contratoId);
            ViewBag.tipoPersonalId = new SelectList(db.TipoPersonals, "id", "descripcion", solicitud.tipoPersonalId);
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", solicitud.usuarioId);
            return View(solicitud);
        }

        // POST: Solicitudes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,folioSolicitud,clienteId,plazaId,fechaSolicitud,esquemaId,sdiId,contratoId,fechaInicial,fechaFinal,tipoPersonalId,solicita,valida,autoriza,noTrabajadores,observaciones,estatusSolicitud,estatusNomina,estatusAfiliado,estatusJuridico,estatusTarjeta,usuarioId,proyectoId,fechaEnvio")] Solicitud solicitud)
        {
            if (ModelState.IsValid)
            {
                db.Entry(solicitud).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.clienteId = new SelectList(db.Clientes, "Id", "claveCliente", solicitud.clienteId);
            ViewBag.estatusSolicitud = new SelectList(db.Conceptos, "id", "grupo", solicitud.estatusSolicitud);
            ViewBag.estatusNomina = new SelectList(db.Conceptos, "id", "grupo", solicitud.estatusNomina);
            ViewBag.estatusJuridico = new SelectList(db.Conceptos, "id", "grupo", solicitud.estatusJuridico);
            ViewBag.estatusAfiliado = new SelectList(db.Conceptos, "id", "grupo", solicitud.estatusAfiliado);
            ViewBag.estatusTarjeta = new SelectList(db.Conceptos, "id", "grupo", solicitud.estatusTarjeta);
            ViewBag.esquemaId = new SelectList(db.EsquemasPagoes, "id", "descripcion", solicitud.esquemaId);
            ViewBag.plazaId = new SelectList(db.Plazas, "id", "descripcion", solicitud.plazaId);
            ViewBag.proyectoId = new SelectList(db.Proyectos, "id", "descripcion", solicitud.proyectoId);
            ViewBag.sdiId = new SelectList(db.SDIs, "id", "descripcion", solicitud.sdiId);
            ViewBag.contratoId = new SelectList(db.TipoContratoes, "id", "descripcion", solicitud.contratoId);
            ViewBag.tipoPersonalId = new SelectList(db.TipoPersonals, "id", "descripcion", solicitud.tipoPersonalId);
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", solicitud.usuarioId);
            return View(solicitud);
        }

        // GET: Solicitudes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Solicitud solicitud = db.Solicituds.Find(id);
            if (solicitud == null)
            {
                return HttpNotFound();
            }
            return View(solicitud);
        }

        // POST: Solicitudes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Solicitud solicitud = db.Solicituds.Find(id);
            db.Solicituds.Remove(solicitud);
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
