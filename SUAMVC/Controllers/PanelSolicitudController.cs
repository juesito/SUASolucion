using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SUADATOS;
using SUAMVC.Helpers;

namespace SUAMVC.Controllers
{
    public class PanelSolicitudController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: PanelSolicitud
        public ActionResult Index(string clientesId, String folioId)
        {

            ToolsHelper cp = new ToolsHelper();
            Concepto concepto = cp.obtenerConceptoPorGrupo("ESTASOL", "Apertura");
            Usuario usuario = Session["UsuarioData"] as Usuario;

            //Buscamos las solicitudes que puede ver ese usuario
            //de acuerdo a sus clientes permitidos
            var solicituds = (from s in db.Solicituds
                              join top in db.TopicosUsuarios on s.clienteId equals top.topicoId
                              where top.tipo.Trim().Equals("C") && top.usuarioId.Equals(usuario.Id)
                              orderby s.fechaSolicitud
                              select s).ToList();


            if (!String.IsNullOrEmpty(clientesId))
            {
                int clienteId = int.Parse(clientesId);
                Cliente cliente = db.Clientes.Find(clienteId);
                if (!cliente.descripcion.ToLower().Contains("seleccion"))
                {
                    solicituds = solicituds.Where(s => s.clienteId.Equals(clienteId)).ToList();
                }
            }// Se va a filtrar por cliente ?
            if (!String.IsNullOrEmpty(folioId))
            {
                solicituds = solicituds.Where(s => s.folioSolicitud.Contains(folioId)).ToList();
            }//Se va a filtrar por folio?
            solicituds = solicituds.Where(s => !s.estatusSolicitud.Equals(concepto.id)).ToList();
            
            return View(solicituds.ToList());
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
