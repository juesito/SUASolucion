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
using System.Web.Helpers;

namespace SUAMVC.Controllers
{
    public class SolicitudesBajaController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: SolicitudesBaja
        public ActionResult Index(string clienteId, String folioId, String proyectoId)
        {
            if ((!String.IsNullOrEmpty(clienteId) && !String.IsNullOrEmpty(proyectoId)) || !String.IsNullOrEmpty(folioId))
            {
                ToolsHelper th = new ToolsHelper();
                Concepto tipoSolicitud = th.obtenerConceptoPorGrupo("SOLCON", "baja");
                Usuario usuario = Session["UsuarioData"] as Usuario;

                var solicituds = (from s in db.Solicituds
                                  join top in db.TopicosUsuarios on s.clienteId equals top.topicoId
                                  where top.tipo.Trim().Equals("C") && top.usuarioId.Equals(usuario.Id)
                                  orderby s.fechaSolicitud
                                  select s).ToList();

                if (!String.IsNullOrEmpty(clienteId) && !String.IsNullOrEmpty(proyectoId))
                {

                    ViewBag.clienteId = clienteId;
                    ViewBag.proyectoId = proyectoId;
                    
                    Cliente cliente = db.Clientes.Find(int.Parse(clienteId));
                    Proyecto proyecto = db.Proyectos.Find(int.Parse(proyectoId));

                    if (!cliente.descripcion.ToLower().Contains("seleccion") && 
                        !proyecto.descripcion.ToLower().Contains("seleccion"))
                    {
                        solicituds = solicituds.Where(s => s.clienteId.Equals(int.Parse(clienteId))
                            && s.proyectoId.Equals(int.Parse(proyectoId))).ToList();
                    }

                }// Se va a filtrar por cliente  y proyecto?

                if (!String.IsNullOrEmpty(folioId))
                {
                    solicituds = solicituds.Where(s => s.folioSolicitud.Contains(folioId)).ToList();
                }//Se va a filtrar por folio?

                //Filtrar por el tipo de solicitud=baja
                solicituds = solicituds.Where(s => s.tipoSolicitud.Equals(tipoSolicitud.id)).ToList();
                return View(solicituds.ToList());
            }
            else
            {
                var solicituds = new List<Solicitud>();
                return View(solicituds);
            }

        }
            
        
        // GET: SolicitudesBaja/Details/5
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

        // GET: SolicitudesBaja/Create
        public ActionResult Create()
        {
            ViewBag.clienteId = new SelectList(db.Clientes, "Id", "claveCliente");
            ViewBag.estatusSolicitud = new SelectList(db.Conceptos, "id", "grupo");
            ViewBag.plazaId = new SelectList(db.Plazas, "id", "descripcion");
            ViewBag.proyectoId = new SelectList(db.Proyectos, "id", "descripcion");
            return View();
        }

        // POST: SolicitudesBaja/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,folioSolicitud,clienteId,plazaId,fechaSolicitud,fechaBaja,esquemaId,sdiId,contratoId,fechaInicial,fechaFinal,tipoPersonalId,solicita,valida,autoriza,noTrabajadores,observaciones,estatusSolicitud,estatusNomina,estatusAfiliado,estatusJuridico,estatusTarjeta,usuarioId,proyectoId,fechaEnvio")] Solicitud solicitud)
        {
            if (ModelState.IsValid)
            {
                Usuario usuario = Session["usuarioData"] as Usuario;
                Cliente cliente = db.Clientes.Find(solicitud.clienteId);
                ListaValidacionCliente lvc = cliente.ListaValidacionClientes.First();
                ToolsHelper th = new ToolsHelper();
                ParametrosHelper ph = new ParametrosHelper();

                Parametro folioBaja = ph.getParameterByKey("FOLSBAJA");

                Concepto concepto = th.obtenerConceptoPorGrupo("ESTASOL", "apertura");
                Concepto tipoSolicitud = th.obtenerConceptoPorGrupo("SOLCON", "baja");


                solicitud.fechaSolicitud = DateTime.Now;
                solicitud.usuarioId = usuario.Id;
                solicitud.autoriza = lvc.autorizador;
                solicitud.valida = lvc.validador;
                solicitud.solicita = usuario.nombreUsuario;
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
                    solicitud.folioSolicitud = folioBaja.valorString.Trim().PadLeft(5, '0') + "B" + solicitud.Cliente.Plaza.cveCorta.Trim();
                    int folBaja = int.Parse(folioBaja.valorString.Trim());
                    folBaja = folBaja + 1;
                    folioBaja.valorString = folBaja.ToString();

                    db.Entry(folioBaja).State = EntityState.Modified;
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
            ViewBag.plazaId = new SelectList(db.Plazas, "id", "descripcion", solicitud.plazaId);
            ViewBag.proyectoId = new SelectList(db.Proyectos, "id", "descripcion", solicitud.proyectoId);
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
            ViewBag.plazaId = new SelectList(db.Plazas, "id", "descripcion", solicitud.plazaId);
            ViewBag.proyectoId = new SelectList(db.Proyectos, "id", "descripcion", solicitud.proyectoId);
            return View(solicitud);
        }

        // POST: SolicitudesBaja/Edit/5
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
            ViewBag.plazaId = new SelectList(db.Plazas, "id", "descripcion", solicitud.plazaId);
            ViewBag.proyectoId = new SelectList(db.Proyectos, "id", "descripcion", solicitud.proyectoId);
            return View(solicitud);
        }

        // GET: SolicitudesBaja/Delete/5
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

        // POST: SolicitudesBaja/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Solicitud solicitud = db.Solicituds.Find(id);
            db.Solicituds.Remove(solicitud);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult EnviarSolicitud(string id)
        {

            Solicitud solicitud = new Solicitud();
            if (!String.IsNullOrEmpty(id))
            {
                int idTmp = int.Parse(id);
                solicitud = db.Solicituds.Find(idTmp);
                Concepto concepto = db.Conceptos.Where(s => s.grupo.Equals("ESTASOL") && s.descripcion.Equals("Enviado")).First();
                solicitud.estatusSolicitud = concepto.id;

                Email email = new Email();
                email.enviarPorClienteTipo("B", solicitud.id, true);

                db.Entry(solicitud).State = EntityState.Modified;
                db.SaveChanges();

                TempData["message"] = "Solicitud Enviada Satisfactoriamente.";
            }

            return RedirectToAction("Index", new { clienteId = solicitud.clienteId, folioId = solicitud.folioSolicitud });
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
