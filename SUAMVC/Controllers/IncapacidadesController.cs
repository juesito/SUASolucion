using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SUADATOS;
using System.Data.OleDb;
using System.Data.Entity.Validation;
using System.Text;
using PagedList;
using System.IO;
using System.Web.Helpers;
using SUAMVC.Code52.i18n;

namespace SUAMVC.Controllers
{
    public class IncapacidadesController : Controller
    {
        private suaEntities db = new suaEntities();

        private void setVariables(String plazasId, String patronesId, String clientesId,
           String gruposId, String opcion, String valor, String statusId)
        {
            if (!String.IsNullOrEmpty(plazasId))
            {
                ViewBag.pzaId = plazasId;
            }
            if (!String.IsNullOrEmpty(patronesId))
            {
                ViewBag.patId = patronesId;
            }
            if (!String.IsNullOrEmpty(clientesId))
            {
                ViewBag.cteId = clientesId;
            }
            if (!String.IsNullOrEmpty(gruposId))
            {
                ViewBag.gpoId = gruposId;
            }
            if (!String.IsNullOrEmpty(opcion))
            {
                ViewBag.opBuscador = opcion;
            }
            if (!String.IsNullOrEmpty(valor))
            {
                ViewBag.valBuscador = valor;
            }
            if (statusId != null)
            {
                ViewBag.statusId = statusId;
            }

        }

        // GET: Aseguradoes
        public ActionResult Index(String plazasId, String patronesId, String clientesId,
            String gruposId, String currentPlaza, String currentPatron, String currentCliente,
            String currentGrupo, String opcion, String valor, String statusId, int page = 1, String sortOrder = null,
            String lastSortOrder = null)
        {

            Usuario user = Session["UsuarioData"] as Usuario;

            setVariables(plazasId, patronesId, clientesId, gruposId, opcion, valor, statusId);

            var plazasAsignadas = (from x in db.TopicosUsuarios
                                   where x.usuarioId.Equals(user.Id)
                                   && x.tipo.Equals("P")
                                   select x.topicoId);

            var patronesAsignados = (from x in db.TopicosUsuarios
                                     where x.usuarioId.Equals(user.Id)
                                     && x.tipo.Equals("B")
                                     select x.topicoId);

            var clientesAsignados = (from x in db.TopicosUsuarios
                                     where x.usuarioId.Equals(user.Id)
                                     && x.tipo.Equals("C")
                                     select x.topicoId);

            var gruposAsignados = (from s in db.Grupos
                                   join cli in db.Clientes on s.Id equals cli.Grupo_id
                                   join top in db.TopicosUsuarios on cli.Id equals top.topicoId
                                   where top.tipo.Trim().Equals("C") && top.usuarioId.Equals(user.Id)
                                   orderby s.claveGrupo
                                   select s.Id);

            ViewBag.plazasId = new SelectList((from s in db.Plazas.ToList()
                                               join top in db.TopicosUsuarios on s.id equals top.topicoId
                                               where top.tipo.Trim().Equals("P") && top.usuarioId.Equals(user.Id)
                                               orderby s.descripcion
                                               select new
                                               {
                                                   id = s.id,
                                                   FUllName = s.descripcion
                                               }).Distinct(), "id", "FullName");

            ViewBag.patronesId = new SelectList((from s in db.Patrones.ToList()
                                                 join top in db.TopicosUsuarios on s.Id equals top.topicoId
                                                 where top.tipo.Trim().Equals("B") && top.usuarioId.Equals(user.Id)
                                                 orderby s.registro
                                                 select new
                                                 {
                                                     id = s.Id,
                                                     FullName = s.registro + " - " + s.nombre
                                                 }).Distinct(), "id", "FullName", null);

            ViewBag.clientesId = new SelectList((from s in db.Clientes.ToList()
                                                 join top in db.TopicosUsuarios on s.Id equals top.topicoId
                                                 where top.tipo.Trim().Equals("C") && top.usuarioId.Equals(user.Id)
                                                 orderby s.descripcion
                                                 select new
                                                 {
                                                     id = s.Id,
                                                     FUllName = s.claveCliente + " - " + s.descripcion
                                                 }).Distinct(), "id", "FullName");

            ViewBag.gruposId = new SelectList((from s in db.Grupos.ToList()
                                               join cli in db.Clientes on s.Id equals cli.Grupo_id
                                               join top in db.TopicosUsuarios on cli.Id equals top.topicoId
                                               where top.tipo.Trim().Equals("C") && top.usuarioId.Equals(user.Id)
                                               orderby s.claveGrupo
                                               select new
                                               {
                                                   id = s.Id,
                                                   FUllName = s.claveGrupo + " - " + s.nombreCorto
                                               }).Distinct(), "id", "FullName");

            //Query principal
            var incapacidades = from s in db.Incapacidades
                             join cli in db.Clientes on s.Asegurado.ClienteId equals cli.Id
                             where plazasAsignadas.Contains(s.Asegurado.Cliente.Plaza_id) &&
                                   clientesAsignados.Contains(s.Asegurado.Cliente.Id) &&
                                   patronesAsignados.Contains(s.Asegurado.PatroneId) &&
                                   gruposAsignados.Contains(s.Asegurado.Cliente.Grupo_id)
                             select s;

            //Comenzamos los filtros
            if (!String.IsNullOrEmpty(plazasId))
            {
                @ViewBag.pzaId = plazasId;
                int idPlaza = int.Parse(plazasId.Trim());
                incapacidades = incapacidades.Where(s => s.Asegurado.Cliente.Plaza_id.Equals(idPlaza));
            }
            if (!String.IsNullOrEmpty(patronesId))
            {
                @ViewBag.patId = patronesId;
                int idPatron = int.Parse(patronesId.Trim());
                incapacidades = incapacidades.Where(s => s.Asegurado.PatroneId.Equals(idPatron));
            }

            if (!String.IsNullOrEmpty(clientesId))
            {
                @ViewBag.cteId = clientesId;
                int idCliente = int.Parse(clientesId.Trim());
                incapacidades = incapacidades.Where(s => s.Asegurado.Cliente.Id.Equals(idCliente));
            }

            if (!String.IsNullOrEmpty(gruposId))
            {
                @ViewBag.gpoId = gruposId;
                int idGrupo = int.Parse(gruposId.Trim());
                incapacidades = incapacidades.Where(s => s.Asegurado.Cliente.Grupo_id.Equals(idGrupo));
            }

            if (!String.IsNullOrEmpty(opcion))
            {

                switch (opcion)
                {
                    case "1":
                        incapacidades = incapacidades.Where(s => s.Asegurado.Patrone.registro.Contains(valor));
                        break;
                    case "2":
                        incapacidades = incapacidades.Where(s => s.Asegurado.numeroAfiliacion.Contains(valor));
                        break;
                    case "3":
                        incapacidades = incapacidades.Where(s => s.Asegurado.CURP.Contains(valor));
                        break;
                    case "4":
                        incapacidades = incapacidades.Where(s => s.Asegurado.RFC.Contains(valor));
                        break;
                    case "5":
                        incapacidades = incapacidades.Where(s => s.Asegurado.nombre.Contains(valor));
                        break;
                    case "6":
                        incapacidades = incapacidades.Where(s => s.Asegurado.fechaAlta.ToString().Contains(valor));
                        break;
                    case "11":
                        incapacidades = incapacidades.Where(s => s.Asegurado.ocupacion.Contains(valor));
                        break;
                    case "12":
                        incapacidades = incapacidades.Where(s => s.Asegurado.Cliente.Plaza.cvecorta.Contains(valor));
                        break;
                    case "13":
                        incapacidades = incapacidades.Where(s => s.Asegurado.extranjero.Contains(valor));
                        break;
                }
            }

            if (statusId != null)
            {
                @ViewBag.statusId = statusId;

                if (statusId.Trim().Equals("A"))
                {
                    ViewBag.statusId = statusId;
                    incapacidades = incapacidades.Where(s => !s.Asegurado.fechaBaja.HasValue);
                }
                else if (statusId.Trim().Equals("B"))
                {
                    ViewBag.statusId = statusId;
                    incapacidades = incapacidades.Where(s => s.Asegurado.fechaBaja.HasValue);
                }
            }

            ViewBag.activos = incapacidades.Where(s => !s.Asegurado.fechaBaja.HasValue).Count();
            ViewBag.registros = incapacidades.Count();

            incapacidades = incapacidades.OrderBy(s => s.Asegurado.nombreTemporal);

            return View(incapacidades.ToList());
        }

        // GET: Aseguradoes/Details/5
        public ActionResult Details(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Asegurado asegurado = db.Asegurados.Find(id);
            if (asegurado == null)
            {
                return HttpNotFound();
            }
            return View(asegurado);
        }

        public ActionResult ViewAttachment(int id, String option, String carga)
        {
            if (carga != null)
            {
                Incapacidade incapacidades = db.Incapacidades.Find(id);
                var movtosTemp = db.Movimientos.Where(x => x.aseguradoId == id
                                 && x.tipo.Equals(option)).OrderByDescending(x => x.fechaTransaccion).ToList();

                Movimiento movto = new Movimiento();
                if (movtosTemp != null && movtosTemp.Count > 0)
                {
                    foreach (var movtosItem in movtosTemp)
                    {
                        movto = movtosItem;
                        break;
                    }//Definimos los valores para la plaza

                    var fileName = "C:\\SUA\\Incapacidades\\" + incapacidades.Asegurado.numeroAfiliacion + "\\" + option + "\\" + movto.nombreArchivo.Trim();

                    if (System.IO.File.Exists(fileName))
                    {
                        FileStream fs = new FileStream(fileName, FileMode.Open);

                        return File(fs, "application/pdf");
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

         // GET: Aseguradoes/Delete/5
        public ActionResult DeleteMov(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MovimientosAsegurado asegurado = db.MovimientosAseguradoes.Find(id);
            if (asegurado == null)
            {
                return HttpNotFound();
            }
            return View(asegurado);
        }

        // POST: Aseguradoes/Delete/5
        [HttpPost, ActionName("DeleteMov")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MovimientosAsegurado asegurado = db.MovimientosAseguradoes.Find(id);
            db.MovimientosAseguradoes.Remove(asegurado);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public void GetExcel(String plazasId, String patronesId, String clientesId,
            String gruposId, String opcion, String valor, String statusId)
        {

            Usuario user = Session["UsuarioData"] as Usuario;
            var plazasAsignadas = (from x in db.TopicosUsuarios
                                   where x.usuarioId.Equals(user.Id)
                                   && x.tipo.Equals("P")
                                   select x.topicoId);

            var clientesAsignados = (from x in db.TopicosUsuarios
                                     where x.usuarioId.Equals(user.Id)
                                     && x.tipo.Equals("C")
                                     select x.topicoId);

            var patronesAsignados = (from x in db.TopicosUsuarios
                                     where x.usuarioId.Equals(user.Id)
                                     && x.tipo.Equals("B")
                                     select x.topicoId);

            List<Incapacidade> allCust = new List<Incapacidade>();

            var incapacidades = from s in db.Incapacidades
                             join cli in db.Clientes on s.Asegurado.ClienteId equals cli.Id
                             where plazasAsignadas.Contains(s.Asegurado.Cliente.Plaza_id) &&
                                   clientesAsignados.Contains(s.Asegurado.Cliente.Id) &&
                                   patronesAsignados.Contains(s.Asegurado.PatroneId)
                             select s;

            if (!String.IsNullOrEmpty(plazasId))
            {
                @ViewBag.pzaId = plazasId;
                int idPlaza = int.Parse(plazasId.Trim());
                incapacidades = incapacidades.Where(s => s.Asegurado.Cliente.Plaza_id.Equals(idPlaza));
            }
            if (!String.IsNullOrEmpty(patronesId))
            {
                @ViewBag.patId = patronesId;
                int idPatron = int.Parse(patronesId.Trim());
                incapacidades = incapacidades.Where(s => s.Asegurado.PatroneId.Equals(idPatron));
            }

            if (!String.IsNullOrEmpty(clientesId))
            {
                @ViewBag.cteId = clientesId;
                int idCliente = int.Parse(clientesId.Trim());
                incapacidades = incapacidades.Where(s => s.Asegurado.Cliente.Id.Equals(idCliente));
            }

            if (!String.IsNullOrEmpty(gruposId))
            {
                @ViewBag.gpoId = gruposId;
                int idGrupo = int.Parse(gruposId.Trim());
                incapacidades = incapacidades.Where(s => s.Asegurado.Cliente.Grupo_id.Equals(idGrupo));
            }

            if (!String.IsNullOrEmpty(opcion))
            {
                @ViewBag.opBuscador = opcion;
                @ViewBag.valBuscador = valor;
                TempData["buscador"] = "0";

                switch (opcion)
                {
                    case "1":
                        incapacidades = incapacidades.Where(s => s.Asegurado.Patrone.registro.Contains(valor));
                        break;
                    case "2":
                        incapacidades = incapacidades.Where(s => s.Asegurado.numeroAfiliacion.Contains(valor));
                        break;
                    case "3":
                        incapacidades = incapacidades.Where(s => s.Asegurado.CURP.Contains(valor));
                        break;
                    case "4":
                        incapacidades = incapacidades.Where(s => s.Asegurado.RFC.Contains(valor));
                        break;
                    case "5":
                        incapacidades = incapacidades.Where(s => s.Asegurado.nombre.Contains(valor));
                        break;
                    case "6":
                        incapacidades = incapacidades.Where(s => s.Asegurado.fechaAlta.ToString().Contains(valor));
                        break;
                    case "11":
                        incapacidades = incapacidades.Where(s => s.Asegurado.ocupacion.Contains(valor));
                        break;
                    case "12":
                        incapacidades = incapacidades.Where(s => s.Asegurado.Cliente.Plaza.cvecorta.Contains(valor));
                        break;
                    case "13":
                        incapacidades = incapacidades.Where(s => s.Asegurado.extranjero.Contains(valor));
                        break;
                }
            }

            if (statusId != null)
            {
                @ViewBag.statusId = statusId;

                if (statusId.Trim().Equals("A"))
                {
                    ViewBag.statusId = statusId;
                    incapacidades = incapacidades.Where(s => !s.Asegurado.fechaBaja.HasValue);
                }
                else if (statusId.Trim().Equals("B"))
                {
                    ViewBag.statusId = statusId;
                    incapacidades = incapacidades.Where(s => s.Asegurado.fechaBaja.HasValue);
                }
            }

            allCust = incapacidades.ToList();

            WebGrid grid = new WebGrid(source: allCust, canPage: false, canSort: false);

            List<WebGridColumn> gridColumns = new List<WebGridColumn>();
            gridColumns.Add(grid.Column("Asegurado.Patrone.registro", "Registro Patronal"));
            gridColumns.Add(grid.Column("Asegurado.numeroAfiliacion", "Num.Afiliacion"));
            gridColumns.Add(grid.Column("Asegurado.curp", "CURP"));
            gridColumns.Add(grid.Column("Asegurado.rfc", "RFC"));
            gridColumns.Add(grid.Column("Asegurado.nombreTemporal", "Nombre"));
            gridColumns.Add(grid.Column("Asegurado.ocupacion", "Ocupación"));
            gridColumns.Add(grid.Column("tieRie", "Riesgo de Trabajo"));
            gridColumns.Add(grid.Column("fechaAcc", "Fecha Inicio", format: (item) => String.Format("{0:yyyy-MM-dd}", item.fechaAcc)));
            gridColumns.Add(grid.Column("diaSub", "Días Subsidiados"));

            gridColumns.Add(grid.Column("Asegurado.fechaAlta", "Fecha Alta", format: (item) => String.Format("{0:yyyy-MM-dd}", item.Asegurado.fechaAlta)));
            gridColumns.Add(grid.Column("Asegurado.extranjero", "Extranjero"));
            gridColumns.Add(grid.Column("Asegurado.Cliente.Plaza.cveCorta", "ID.Plaza"));
            gridColumns.Add(grid.Column("Asegurado.fechaCreacion", "Fecha Creacion", format: (item) => String.Format("{0:yyyy-MM-dd}", item.Asegurado.fechaAlta)));
            gridColumns.Add(grid.Column("Asegurado.fechaModificacion", "Fecha Modificación", format: (item) => item.Asegurado.fechaModificacion != null ? String.Format("{0:yyyy-MM-dd}", item.Asegurado.fechaModificacion) : String.Empty));

            string gridData = grid.GetHtml(
                columns: grid.Columns(gridColumns.ToArray())
                    ).ToString();

            Response.ClearContent();
            DateTime date = DateTime.Now;
            String fileName = "Incapacidades-" + date.ToString("ddMMyyyyHHmm") + ".xls";
            Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
            Response.ContentType = "application/excel";
            Response.Write(gridData);
            Response.End();
        }

        // GET: Aseguradoes/Delete/5
        public ActionResult DeleteMovs(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MovimientosAsegurado asegurado = db.MovimientosAseguradoes.Find(id);
            db.MovimientosAseguradoes.Remove(asegurado);
            db.SaveChanges();
            return View(asegurado);
        }

        public ActionResult ActivaVariable(String buscador, String plazasId, String patronesId, String clientesId,
            String gruposId, String opcion, String valor, String statusId)
        {
            if (buscador != null)
            {
                if (!buscador.Equals("1"))
                {
                    TempData["buscador"] = "1";
                }
                else
                {
                    TempData["buscador"] = "0";
                }
            }
            else
            {
                TempData["buscador"] = "1";
            }
            return RedirectToAction("Index", new { plazasId, patronesId, clientesId, gruposId, opcion, valor, statusId });
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

