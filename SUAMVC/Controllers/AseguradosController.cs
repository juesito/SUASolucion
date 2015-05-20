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

namespace SUAMVC.Controllers
{
    public class AseguradosController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: Aseguradoes
        public ActionResult Index(String plazasId, String patronesId, String clientesId, String gruposId, string currentPlaza, string currentPatron, string currentCliente, string currentGrupo, string opcion, string valor, int page = 1, String sortOrder = null, String lastSortOrder = null)
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
            List<int> tai = clientesAsignados.ToList();

            var patronesAsignados = (from x in db.TopicosUsuarios
                                     where x.usuarioId.Equals(user.Id)
                                     && x.tipo.Equals("B")
                                     select x.topicoId);

            //ViewBag.patronesId = new SelectList(db.Patrones, "id", "nombre")

            ViewBag.plazasId = new SelectList((from s in db.Plazas.ToList()
                                               join top in db.TopicosUsuarios on s.id equals top.topicoId 
                                               where top.tipo.Trim().Equals("P")   && top.usuarioId.Equals(user.Id)
                                               orderby s.descripcion
                                               select new
                                               {
                                                   id = s.id,
                                                   FUllName = s.descripcion
                                               }).Distinct(), "id", "FullName");

            ViewBag.patronesId = new SelectList((from s in db.Patrones.ToList()
//                                                 join ase in db.Asegurados on s.Id equals ase.PatroneId
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

            ViewBag.opcion = new SelectList(new List<Object> {
                              "Reg. Patronal",
                              "Num. Afiliación",
                              "CURP",
                              "RFC",
                              "Nombre",
                              "Fecha Alta",
                              "Fecha Baja",
                              "Salario IMMS",
                              "Ubicación",
                              "ID Grupo",
                              "Ocupación",
                              "ID Plaza",
                              "Extranjero?" });

            var asegurados = from s in db.Asegurados
                            join cli in db.Clientes on s.ClienteId equals cli.Id
                             where plazasAsignadas.Contains(s.Cliente.Plaza_id) &&
                                   clientesAsignados.Contains(s.Cliente.Id) &&
                                   patronesAsignados.Contains(s.PatroneId)
                            select s;

            if (!String.IsNullOrEmpty(plazasId))
            {
                @ViewBag.pzaId = plazasId;
            }
            if (!String.IsNullOrEmpty(patronesId))
            {
                @ViewBag.patId = patronesId;
            }
            if (!String.IsNullOrEmpty(clientesId))
            {
                @ViewBag.cteId = clientesId;
            }
            if (!String.IsNullOrEmpty(gruposId))
            {
                @ViewBag.gpoId = gruposId;
            }

            if (!String.IsNullOrEmpty(plazasId))
            {
                if (!String.IsNullOrEmpty(patronesId))
                {
                    if (!String.IsNullOrEmpty(clientesId))
                    {
                        if (!String.IsNullOrEmpty(gruposId))
                        {
                            @ViewBag.pzaId = plazasId;
                            @ViewBag.patId = patronesId;
                            @ViewBag.cteId = clientesId;
                            @ViewBag.gpoId = gruposId;
                            int idPlaza = int.Parse(plazasId.Trim());
                            int idPatron = int.Parse(patronesId.Trim());
                            int idCliente = int.Parse(clientesId.Trim());
                            int idGrupo = int.Parse(gruposId.Trim());
                            asegurados = asegurados.Where(s => s.Cliente.Plaza_id.Equals(idPlaza) && s.PatroneId.Equals(idPatron) && s.Cliente.Id.Equals(idCliente) && s.Cliente.Grupo_id.Equals(idGrupo));
                        }
                        else
                        {
                            @ViewBag.pzaId = plazasId;
                            @ViewBag.patId = patronesId;
                            @ViewBag.cteId = clientesId;
                            int idPlaza = int.Parse(plazasId.Trim());
                            int idPatron = int.Parse(patronesId.Trim());
                            int idCliente = int.Parse(clientesId.Trim());
                            asegurados = asegurados.Where(s => s.Cliente.Plaza_id.Equals(idPlaza) && s.PatroneId.Equals(idPatron) && s.Cliente.Id.Equals(idCliente));
                        }

                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(gruposId))
                        {
                            @ViewBag.pzaId = plazasId;
                            @ViewBag.patId = patronesId;
                            @ViewBag.gpoId = gruposId;
                            int idPlaza = int.Parse(plazasId.Trim());
                            int idPatron = int.Parse(patronesId.Trim());
                            int idGrupo = int.Parse(gruposId.Trim());
                            asegurados = asegurados.Where(s => s.Cliente.Plaza_id.Equals(idPlaza) && s.PatroneId.Equals(idPatron) && s.Cliente.Grupo_id.Equals(idGrupo));
                        }
                        else
                        {
                            @ViewBag.pzaId = plazasId;
                            @ViewBag.patId = patronesId;
                            int idPlaza = int.Parse(plazasId.Trim());
                            int idPatron = int.Parse(patronesId.Trim());
                            asegurados = asegurados.Where(s => s.Cliente.Plaza_id.Equals(idPlaza) && s.PatroneId.Equals(idPatron));
                        }

                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(clientesId))
                    {
                        if (!String.IsNullOrEmpty(gruposId))
                        {
                            @ViewBag.pzaId = plazasId;
                            @ViewBag.cteId = clientesId;
                            @ViewBag.gpoId = gruposId;
                            int idPlaza = int.Parse(plazasId.Trim());
                            int idCliente = int.Parse(clientesId.Trim());
                            int idGrupo = int.Parse(gruposId.Trim());
                            asegurados = asegurados.Where(s => s.Cliente.Plaza_id.Equals(idPlaza) && s.Cliente.Id.Equals(idCliente) && s.Cliente.Grupo_id.Equals(idGrupo));
                        }
                        else
                        {
                            @ViewBag.pzaId = plazasId;
                            @ViewBag.cteId = clientesId;
                            int idPlaza = int.Parse(plazasId.Trim());
                            int idCliente = int.Parse(clientesId.Trim());
                            asegurados = asegurados.Where(s => s.Cliente.Plaza_id.Equals(idPlaza) && s.Cliente.Id.Equals(idCliente));
                        }

                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(gruposId))
                        {
                            @ViewBag.pzaId = plazasId;
                            @ViewBag.gpoId = gruposId;
                            int idPlaza = int.Parse(plazasId.Trim());
                            int idGrupo = int.Parse(gruposId.Trim());
                            asegurados = asegurados.Where(s => s.Cliente.Plaza_id.Equals(idPlaza) && s.Cliente.Grupo_id.Equals(idGrupo));
                        }
                        else
                        {
                            @ViewBag.pzaId = plazasId;
                            int idPlaza = int.Parse(plazasId.Trim());
                            asegurados = asegurados.Where(s => s.Cliente.Plaza_id.Equals(idPlaza));
                        }

                    }
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(patronesId))
                {
                    if (!String.IsNullOrEmpty(clientesId))
                    {
                        if (!String.IsNullOrEmpty(gruposId))
                        {
                            @ViewBag.patId = patronesId;
                            @ViewBag.cteId = clientesId;
                            @ViewBag.gpoId = gruposId;
                            int idPatron = int.Parse(patronesId.Trim());
                            int idCliente = int.Parse(clientesId.Trim());
                            int idGrupo = int.Parse(gruposId.Trim());
                            asegurados = asegurados.Where(s => s.PatroneId.Equals(idPatron) && s.Cliente.Id.Equals(idCliente) && s.Cliente.Grupo_id.Equals(idGrupo));
                        }
                        else
                        {
                            @ViewBag.patId = patronesId;
                            @ViewBag.cteId = clientesId;
                            int idPatron = int.Parse(patronesId.Trim());
                            int idCliente = int.Parse(clientesId.Trim());
                            asegurados = asegurados.Where(s => s.PatroneId.Equals(idPatron) && s.Cliente.Id.Equals(idCliente));
                        }

                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(gruposId))
                        {
                            @ViewBag.patId = patronesId;
                            @ViewBag.gpoId = gruposId;
                            int idPatron = int.Parse(patronesId.Trim());
                            int idGrupo = int.Parse(gruposId.Trim());
                            asegurados = asegurados.Where(s => s.PatroneId.Equals(idPatron) && s.Cliente.Grupo_id.Equals(idGrupo));
                        }
                        else
                        {
                            @ViewBag.patId = patronesId;
                            int idPatron = int.Parse(patronesId.Trim());
                            asegurados = asegurados.Where(s => s.PatroneId.Equals(idPatron));
                        }

                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(clientesId))
                    {
                        if (!String.IsNullOrEmpty(gruposId))
                        {
                            @ViewBag.cteId = clientesId;
                            @ViewBag.gpoId = gruposId;
                            int idCliente = int.Parse(clientesId.Trim());
                            int idGrupo = int.Parse(gruposId.Trim());
                            asegurados = asegurados.Where(s => s.Cliente.Id.Equals(idCliente) && s.Cliente.Grupo_id.Equals(idGrupo));
                        }
                        else
                        {
                            @ViewBag.cteId = clientesId;
                            int idCliente = int.Parse(clientesId.Trim());
                            asegurados = asegurados.Where(s => s.Cliente.Id.Equals(idCliente));
                        }

                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(gruposId))
                        {
                            @ViewBag.gpoId = gruposId;
                            int idGrupo = int.Parse(gruposId.Trim());
                            asegurados = asegurados.Where(s => s.Cliente.Grupo_id.Equals(idGrupo));
                        }
                    }
                }
            }

            if (!String.IsNullOrEmpty(opcion))
            {
                @ViewBag.opBuscador = opcion;
                @ViewBag.valBuscador = valor;
                TempData["buscador"] = "0";

                switch (opcion)
                {
                    case "Reg. Patronal":
                        asegurados = asegurados.Where(s => s.Patrone.registro.Contains(valor));
                        break;
                    case "Num. Afiliación":
                        asegurados = asegurados.Where(s => s.numeroAfiliacion.Contains(valor));
                        break;
                    case "CURP":
                        asegurados = asegurados.Where(s => s.CURP.Contains(valor));
                        break;
                    case "RFC":
                        asegurados = asegurados.Where(s => s.RFC.Contains(valor));
                        break;
                    case "Nombre":
                        asegurados = asegurados.Where(s => s.nombre.Contains(valor));
                        break;
                    case "Fecha Alta":
                        asegurados = asegurados.Where(s => s.fechaAlta.ToString().Contains(valor));
                        break;
                    case "Fecha Baja":
                        asegurados = asegurados.Where(s => s.fechaBaja.ToString().Contains(valor));
                        break;
                    case "Salario IMMS":
                        asegurados = asegurados.Where(s => s.salarioImss.ToString().Contains(valor.Trim()));
                        break;
                    case "Ubicación":
                        asegurados = asegurados.Where(s => s.Cliente.claveCliente.Contains(valor));
                        break;
                    case "ID Grupo":
                        asegurados = asegurados.Where(s => s.Cliente.Grupos.nombre.Contains(valor));
                        break;
                    case "Ocupación":
                        asegurados = asegurados.Where(s => s.ocupacion.Contains(valor));
                        break;
                    case "ID Plaza":
                        asegurados = asegurados.Where(s => s.Cliente.Plaza.cve.Contains(valor));
                        break;
                    case "Extranjero?":
                        asegurados = asegurados.Where(s => s.extranjero.Contains(valor));
                        break;
                }
            }
            if (page < 1) page = 1;

            ViewBag.activos = asegurados.Where(s => !s.fechaBaja.HasValue).Count();

            ViewBag.registros = asegurados.Count();

            if (page == 1)
            {
                asegurados = asegurados.OrderBy(s => s.nombre);
            }
            else
                asegurados = asegurados.OrderBy(s => s.nombreTemporal);      //.Skip((page-1) * 12);

            return View(asegurados.ToList());
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
                Asegurado asegurado = db.Asegurados.Find(id);
                var movtosTemp = db.Movimientos.Where(x => x.aseguradoId == id
                                 && x.tipo.Equals(option)).OrderBy(x => x.fechaTransaccion).ToList();

                Movimiento movto = new Movimiento();
                if (movtosTemp != null && movtosTemp.Count > 0)
                {
                    foreach (var movtosItem in movtosTemp)
                    {
                        movto = movtosItem;
                        break;
                    }//Definimos los valores para la plaza

                    var fileName = "C:\\SUA\\Asegurados\\" + asegurado.numeroAfiliacion + "\\" + option + "\\" + movto.nombreArchivo.Trim();

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
        }        // GET: Aseguradoes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Aseguradoes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "registroaseguradoal,numeroAfiliacion,CURP,RFC,nombre,salarioImss,salarioInfo,fechaAlta,fechaBaja,tipoTrabajo,semanaJornada,paginaInfo,tipoDescuento,valorDescuento,claveUbicacion,nombreTemporal,fechaDescuento,finDescuento,articulo33,salarioArticulo33,trapeniv,estado,claveMunicipio")] Asegurado asegurado)
        {
            if (ModelState.IsValid)
            {
                db.Asegurados.Add(asegurado);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(asegurado);
        }

        // GET: Aseguradoes/Edit/5
        public ActionResult Edit(int id)
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

        // POST: Aseguradoes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "registroaseguradoal,numeroAfiliacion,CURP,RFC,nombre,salarioImss,salarioInfo,fechaAlta,fechaBaja,tipoTrabajo,semanaJornada,paginaInfo,tipoDescuento,valorDescuento,claveUbicacion,nombreTemporal,fechaDescuento,finDescuento,articulo33,salarioArticulo33,trapeniv,estado,claveMunicipio")] Asegurado asegurado)
        {
            if (ModelState.IsValid)
            {
                db.Entry(asegurado).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(asegurado);
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
        public void GetExcel(String plazasId, String patronesId, String clientesId, String gruposId, string opcion, string valor)
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
            List<int> tai = clientesAsignados.ToList();

            List<Asegurado> allCust = new List<Asegurado>();

            var asegurados = from s in db.Asegurados
                              join cli in db.Clientes on s.ClienteId equals cli.Id
                             where plazasAsignadas.Contains(s.Cliente.Plaza_id) &&
                                    clientesAsignados.Contains(s.Cliente.Id)
                              select s;

            if (!String.IsNullOrEmpty(plazasId))
            {
                @ViewBag.pzaId = plazasId;
            }
            if (!String.IsNullOrEmpty(patronesId))
            {
                @ViewBag.patId = patronesId;
            }
            if (!String.IsNullOrEmpty(clientesId))
            {
                @ViewBag.cteId = clientesId;
            }
            if (!String.IsNullOrEmpty(gruposId))
            {
                @ViewBag.gpoId = gruposId;
            }

            if (!String.IsNullOrEmpty(plazasId))
            {
                if (!String.IsNullOrEmpty(patronesId))
                {
                    if (!String.IsNullOrEmpty(clientesId))
                    {
                        if (!String.IsNullOrEmpty(gruposId))
                        {
                            @ViewBag.pzaId = plazasId;
                            @ViewBag.patId = patronesId;
                            @ViewBag.cteId = clientesId;
                            @ViewBag.gpoId = gruposId;
                            int idPlaza = int.Parse(plazasId.Trim());
                            int idPatron = int.Parse(patronesId.Trim());
                            int idCliente = int.Parse(clientesId.Trim());
                            int idGrupo = int.Parse(gruposId.Trim());
                            asegurados = asegurados.Where(s => s.Cliente.Plaza_id.Equals(idPlaza) && s.PatroneId.Equals(idPatron) && s.Cliente.Id.Equals(idCliente) && s.Cliente.Grupo_id.Equals(idGrupo));
                        }
                        else
                        {
                            @ViewBag.pzaId = plazasId;
                            @ViewBag.patId = patronesId;
                            @ViewBag.cteId = clientesId;
                            int idPlaza = int.Parse(plazasId.Trim());
                            int idPatron = int.Parse(patronesId.Trim());
                            int idCliente = int.Parse(clientesId.Trim());
                            asegurados = asegurados.Where(s => s.Cliente.Plaza_id.Equals(idPlaza) && s.PatroneId.Equals(idPatron) && s.Cliente.Id.Equals(idCliente));
                        }

                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(gruposId))
                        {
                            @ViewBag.pzaId = plazasId;
                            @ViewBag.patId = patronesId;
                            @ViewBag.gpoId = gruposId;
                            int idPlaza = int.Parse(plazasId.Trim());
                            int idPatron = int.Parse(patronesId.Trim());
                            int idGrupo = int.Parse(gruposId.Trim());
                            asegurados = asegurados.Where(s => s.Cliente.Plaza_id.Equals(idPlaza) && s.PatroneId.Equals(idPatron) && s.Cliente.Grupo_id.Equals(idGrupo));
                        }
                        else
                        {
                            @ViewBag.pzaId = plazasId;
                            @ViewBag.patId = patronesId;
                            int idPlaza = int.Parse(plazasId.Trim());
                            int idPatron = int.Parse(patronesId.Trim());
                            asegurados = asegurados.Where(s => s.Cliente.Plaza_id.Equals(idPlaza) && s.PatroneId.Equals(idPatron));
                        }

                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(clientesId))
                    {
                        if (!String.IsNullOrEmpty(gruposId))
                        {
                            @ViewBag.pzaId = plazasId;
                            @ViewBag.cteId = clientesId;
                            @ViewBag.gpoId = gruposId;
                            int idPlaza = int.Parse(plazasId.Trim());
                            int idCliente = int.Parse(clientesId.Trim());
                            int idGrupo = int.Parse(gruposId.Trim());
                            asegurados = asegurados.Where(s => s.Cliente.Plaza_id.Equals(idPlaza) && s.Cliente.Id.Equals(idCliente) && s.Cliente.Grupo_id.Equals(idGrupo));
                        }
                        else
                        {
                            @ViewBag.pzaId = plazasId;
                            @ViewBag.cteId = clientesId;
                            int idPlaza = int.Parse(plazasId.Trim());
                            int idCliente = int.Parse(clientesId.Trim());
                            asegurados = asegurados.Where(s => s.Cliente.Plaza_id.Equals(idPlaza) && s.Cliente.Id.Equals(idCliente));
                        }

                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(gruposId))
                        {
                            @ViewBag.pzaId = plazasId;
                            @ViewBag.gpoId = gruposId;
                            int idPlaza = int.Parse(plazasId.Trim());
                            int idGrupo = int.Parse(gruposId.Trim());
                            asegurados = asegurados.Where(s => s.Cliente.Plaza_id.Equals(idPlaza) && s.Cliente.Grupo_id.Equals(idGrupo));
                        }
                        else
                        {
                            @ViewBag.pzaId = plazasId;
                            int idPlaza = int.Parse(plazasId.Trim());
                            asegurados = asegurados.Where(s => s.Cliente.Plaza_id.Equals(idPlaza));
                        }

                    }
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(patronesId))
                {
                    if (!String.IsNullOrEmpty(clientesId))
                    {
                        if (!String.IsNullOrEmpty(gruposId))
                        {
                            @ViewBag.patId = patronesId;
                            @ViewBag.cteId = clientesId;
                            @ViewBag.gpoId = gruposId;
                            int idPatron = int.Parse(patronesId.Trim());
                            int idCliente = int.Parse(clientesId.Trim());
                            int idGrupo = int.Parse(gruposId.Trim());
                            asegurados = asegurados.Where(s => s.PatroneId.Equals(idPatron) && s.Cliente.Id.Equals(idCliente) && s.Cliente.Grupo_id.Equals(idGrupo));
                        }
                        else
                        {
                            @ViewBag.patId = patronesId;
                            @ViewBag.cteId = clientesId;
                            int idPatron = int.Parse(patronesId.Trim());
                            int idCliente = int.Parse(clientesId.Trim());
                            asegurados = asegurados.Where(s => s.PatroneId.Equals(idPatron) && s.Cliente.Id.Equals(idCliente));
                        }

                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(gruposId))
                        {
                            @ViewBag.patId = patronesId;
                            @ViewBag.gpoId = gruposId;
                            int idPatron = int.Parse(patronesId.Trim());
                            int idGrupo = int.Parse(gruposId.Trim());
                            asegurados = asegurados.Where(s => s.PatroneId.Equals(idPatron) && s.Cliente.Grupo_id.Equals(idGrupo));
                        }
                        else
                        {
                            @ViewBag.patId = patronesId;
                            int idPatron = int.Parse(patronesId.Trim());
                            asegurados = asegurados.Where(s => s.PatroneId.Equals(idPatron));
                        }

                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(clientesId))
                    {
                        if (!String.IsNullOrEmpty(gruposId))
                        {
                            @ViewBag.cteId = clientesId;
                            @ViewBag.gpoId = gruposId;
                            int idCliente = int.Parse(clientesId.Trim());
                            int idGrupo = int.Parse(gruposId.Trim());
                            asegurados = asegurados.Where(s => s.Cliente.Id.Equals(idCliente) && s.Cliente.Grupo_id.Equals(idGrupo));
                        }
                        else
                        {
                            @ViewBag.cteId = clientesId;
                            int idCliente = int.Parse(clientesId.Trim());
                            asegurados = asegurados.Where(s => s.Cliente.Id.Equals(idCliente));
                        }

                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(gruposId))
                        {
                            @ViewBag.gpoId = gruposId;
                            int idGrupo = int.Parse(gruposId.Trim());
                            asegurados = asegurados.Where(s => s.Cliente.Grupo_id.Equals(idGrupo));
                        }
                    }
                }
            }

            if (!String.IsNullOrEmpty(opcion))
            {
                @ViewBag.opBuscador = opcion;
                @ViewBag.valBuscador = valor;
                TempData["buscador"] = "0";
                switch (opcion)
                {
                    case "Reg. Patronal":
                        asegurados = asegurados.Where(s => s.Patrone.registro.Contains(valor));
                        break;
                    case "Num. Afiliación":
                        asegurados = asegurados.Where(s => s.numeroAfiliacion.Contains(valor));
                        break;
                    case "CURP":
                        asegurados = asegurados.Where(s => s.CURP.Contains(valor));
                        break;
                    case "RFC":
                        asegurados = asegurados.Where(s => s.RFC.Contains(valor));
                        break;
                    case "Nombre":
                        asegurados = asegurados.Where(s => s.nombre.Contains(valor));
                        break;
                    case "Fecha Alta":
                        asegurados = asegurados.Where(s => s.fechaAlta.ToString().Contains(valor));
                        break;
                    case "Fecha Baja":
                        asegurados = asegurados.Where(s => s.fechaBaja.ToString().Contains(valor));
                        break;
                    case "Salario IMMS":
                        asegurados = asegurados.Where(s => s.salarioImss.ToString().Contains(valor));
                        break;
                    case "Ubicación":
                        asegurados = asegurados.Where(s => s.Cliente.claveCliente.Contains(valor));
                        break;
                    case "ID Grupo":
                        asegurados = asegurados.Where(s => s.Cliente.Grupos.nombre.Contains(valor));
                        break;
                    case "Ocupación":
                        asegurados = asegurados.Where(s => s.ocupacion.Contains(valor));
                        break;
                    case "ID Plaza":
                        asegurados = asegurados.Where(s => s.Cliente.Plaza.cve.Contains(valor));
                        break;
                    case "Extranjero?":
                        asegurados = asegurados.Where(s => s.extranjero.Contains(valor));
                        break;
                }
            }

            allCust = asegurados.ToList();

            WebGrid grid = new WebGrid(source: allCust, canPage: false, canSort: false);

            string gridData = grid.GetHtml(
                columns: grid.Columns(
                        grid.Column("Patrone.registro", "Registro "),
                        grid.Column("numeroAfiliacion", "Numero Afiliacion"),
                        grid.Column("curp", "CURP"),
                        grid.Column("rfc", "RFC"),
                        grid.Column("nombreTemporal", "Nombre"),
                        grid.Column("fechaAlta", "Fecha Alta"),
                        grid.Column("fechaBaja", "Fecha Baja"),
                        grid.Column("Cliente.claveCliente", "Cliente"),
                        grid.Column("Cliente.Grupos.nombreCorto", "Grupo"),
                        grid.Column("Patrone.Plaza.cve", "Plaza"),
                        grid.Column("extranjero", "Extranjero"),
                        grid.Column("alta", "Alta"),
                        grid.Column("baja", "Baja"),
                        grid.Column("modificacion", "Modificación"),
                        grid.Column("permanente", "Permanente")
                        )
                    ).ToString();

            Response.ClearContent();
            DateTime date = DateTime.Now;
            String fileName = "Asegurados-" + date.ToString("ddMMyyyyHHmm") + ".xls";
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

        public ActionResult ActivaVariable()
        {
            TempData["buscador"] = "1";
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
