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
using SUAMVC.Helpers;
using SUAMVC.Models;

namespace SUAMVC.Controllers
{
    public class SolicitudesModificacionController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: SolicitudesModificacion
        public ActionResult Index(string clientesId, String folioId)
        {
            Concepto tipoSolicitud = db.Conceptos.Where(s => s.grupo.Equals("SOLCON") &&
                s.descripcion.ToLower().Trim().Contains("modificacion")).FirstOrDefault();
            ;

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

            //Filtramos solo solicitudes de Alta
            solicituds = solicituds.Where(s => s.tipoSolicitud.Equals(tipoSolicitud.id));


            return View(solicituds.ToList());
        }

        // GET: SolicitudesModificacion/Details/5
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

        // GET: SolicitudesModificacion/Create
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

        // POST: SolicitudesModificacion/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,folioSolicitud,clienteId,plazaId,fechaSolicitud,fechaModificacion,esquemaId,sdiId,contratoId,fechaInicial,fechaFinal,tipoPersonalId,solicita,valida,autoriza,noTrabajadores,observaciones,estatusSolicitud,estatusNomina,estatusAfiliado,estatusJuridico,estatusTarjeta,usuarioId,proyectoId,fechaEnvio")] Solicitud solicitud)
        {
            if (ModelState.IsValid)
            {
                Usuario usuario = Session["usuarioData"] as Usuario;
                Cliente cliente = db.Clientes.Find(solicitud.clienteId);
                ListaValidacionCliente lvc = cliente.ListaValidacionClientes.First();
                ToolsHelper th = new ToolsHelper();
                ParametrosHelper ph = new ParametrosHelper();

                Parametro folioModificacion = ph.getParameterByKey("FOLSMODIF");
                
                //Concepto concepto = db.Conceptos.Where(s => s.grupo.Equals("SOLCON") && s.descripcion.ToLower().Contains("modificacion")).First();
                Concepto concepto = th.obtenerConceptoPorGrupo("ESTASOL", "apertura");
                Concepto tipoSolicitud = th.obtenerConceptoPorGrupo("SOLCON", "modificacion");

                solicitud.fechaSolicitud = DateTime.Now;
                solicitud.usuarioId = usuario.Id;
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
                solicitud.tipoSolicitud = tipoSolicitud.id;
                db.Solicituds.Add(solicitud);
                

                try
                {
                    db.SaveChanges();
                    solicitud.folioSolicitud = folioModificacion.valorString.Trim().PadLeft(5, '0') + "MS" + solicitud.Cliente.Plaza.cveCorta.Trim();
                    int folModificacion = int.Parse(folioModificacion.valorString.Trim());
                    folModificacion = folModificacion + 1;
                    folioModificacion.valorString = folModificacion.ToString();


                    db.Entry(folioModificacion).State = EntityState.Modified;
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

        // GET: SolicitudesBaja/Edit/5
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

        // POST: SolicitudesModificacion/Edit/5
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

        // GET: SolicitudesModificacion/Delete/5
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

        // POST: SolicitudesModificacion/Delete/5
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
