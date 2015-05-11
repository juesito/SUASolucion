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
        public ActionResult Index2(String plazasId, String patronesId, String clientesId, String gruposId, string currentPlaza, string currentPatron, string currentCliente, string currentGrupo, int page = 1, String sortOrder=null, String lastSortOrder=null)
        {

            //ViewBag.patronesId = new SelectList(db.Patrones, "id", "nombre");
            ViewBag.plazasId = new SelectList((from s in db.Plazas.ToList()
                                               orderby s.descripcion
                                               select new
                                               {
                                                   id = s.id,
                                                   FUllName = s.descripcion
                                               }), "id", "FullName");

            ViewBag.patronesId = new SelectList((from s in db.Patrones.ToList()
                                                 orderby s.registro
                                                 select new
                                                 {
                                                     id = s.Id,
                                                     FullName = s.registro + " - " + s.nombre
                                                 }), "id", "FullName", null);
            ViewBag.clientesId = new SelectList((from s in db.Clientes.ToList()
                                                 orderby s.descripcion
                                                 select new
                                                 {
                                                     id = s.Id,
                                                     FUllName = s.claveCliente + " - " + s.descripcion
                                                 }), "id", "FullName");
            ViewBag.gruposId = new SelectList((from s in db.Grupos.ToList()
                                               orderby s.claveGrupo
                                               select new
                                               {
                                                   id = s.Id,
                                                   FUllName = s.claveGrupo + " - " + s.nombreCorto
                                               }), "id", "FullName");
            
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
                             select s;
            if (!String.IsNullOrEmpty(plazasId))
            {
                if (!String.IsNullOrEmpty(patronesId))
                {
                    if (!String.IsNullOrEmpty(clientesId))
                    {
                        if (!String.IsNullOrEmpty(gruposId))
                        {
                            int idPlaza = int.Parse(plazasId.Trim());
                            int idPatron = int.Parse(patronesId.Trim());
                            int idCliente = int.Parse(clientesId.Trim());
                            int idGrupo = int.Parse(gruposId.Trim());
                            asegurados = asegurados.Where(s => s.Patrone.Plaza_id.Equals(idPlaza) && s.PatroneId.Equals(idPatron) && s.Cliente.Id.Equals(idCliente) && s.Cliente.Grupo_id.Equals(idGrupo));
                        }
                        else
                        {
                            int idPlaza = int.Parse(plazasId.Trim());
                            int idPatron = int.Parse(patronesId.Trim());
                            int idCliente = int.Parse(clientesId.Trim());
                            asegurados = asegurados.Where(s => s.Patrone.Plaza_id.Equals(idPlaza) && s.PatroneId.Equals(idPatron) && s.Cliente.Id.Equals(idCliente));
                        }

                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(gruposId))
                        {
                            int idPlaza = int.Parse(plazasId.Trim());
                            int idPatron = int.Parse(patronesId.Trim());
                            int idGrupo = int.Parse(gruposId.Trim());
                            asegurados = asegurados.Where(s => s.Patrone.Plaza_id.Equals(idPlaza) && s.PatroneId.Equals(idPatron) && s.Cliente.Grupo_id.Equals(idGrupo));
                        }
                        else
                        {
                            int idPlaza = int.Parse(plazasId.Trim());
                            int idPatron = int.Parse(patronesId.Trim());
                            asegurados = asegurados.Where(s => s.Patrone.Plaza_id.Equals(idPlaza) && s.PatroneId.Equals(idPatron));
                        }

                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(clientesId))
                    {
                        if (!String.IsNullOrEmpty(gruposId))
                        {
                            int idPlaza = int.Parse(plazasId.Trim());
                            int idCliente = int.Parse(clientesId.Trim());
                            int idGrupo = int.Parse(gruposId.Trim());
                            asegurados = asegurados.Where(s => s.Patrone.Plaza_id.Equals(idPlaza) && s.Cliente.Id.Equals(idCliente) && s.Cliente.Grupo_id.Equals(idGrupo));
                        }
                        else
                        {
                            int idPlaza = int.Parse(plazasId.Trim());
                            int idCliente = int.Parse(clientesId.Trim());
                            asegurados = asegurados.Where(s => s.Patrone.Plaza_id.Equals(idPlaza) && s.Cliente.Id.Equals(idCliente));
                        }

                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(gruposId))
                        {
                            int idPlaza = int.Parse(plazasId.Trim());
                            int idGrupo = int.Parse(gruposId.Trim());
                            asegurados = asegurados.Where(s => s.Patrone.Plaza_id.Equals(idPlaza) && s.Cliente.Grupo_id.Equals(idGrupo));
                        }
                        else
                        {
                            int idPlaza = int.Parse(plazasId.Trim());
                            asegurados = asegurados.Where(s => s.Patrone.Plaza_id.Equals(idPlaza));
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
                            int idPatron = int.Parse(patronesId.Trim());
                            int idCliente = int.Parse(clientesId.Trim());
                            int idGrupo = int.Parse(gruposId.Trim());
                            asegurados = asegurados.Where(s => s.PatroneId.Equals(idPatron) && s.Cliente.Id.Equals(idCliente) && s.Cliente.Grupo_id.Equals(idGrupo));
                        }
                        else
                        {
                            int idPatron = int.Parse(patronesId.Trim());
                            int idCliente = int.Parse(clientesId.Trim());
                            asegurados = asegurados.Where(s => s.PatroneId.Equals(idPatron) && s.Cliente.Id.Equals(idCliente));
                        }

                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(gruposId))
                        {
                            int idPatron = int.Parse(patronesId.Trim());
                            int idGrupo = int.Parse(gruposId.Trim());
                            asegurados = asegurados.Where(s => s.PatroneId.Equals(idPatron) && s.Cliente.Grupo_id.Equals(idGrupo));
                        }
                        else
                        {
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
                            int idCliente = int.Parse(clientesId.Trim());
                            int idGrupo = int.Parse(gruposId.Trim());
                            asegurados = asegurados.Where(s => s.Cliente.Id.Equals(idCliente) && s.Cliente.Grupo_id.Equals(idGrupo));
                        }
                        else
                        {
                            int idCliente = int.Parse(clientesId.Trim());
                            asegurados = asegurados.Where(s => s.Cliente.Id.Equals(idCliente));
                        }

                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(gruposId))
                        {
                            int idGrupo = int.Parse(gruposId.Trim());
                            asegurados = asegurados.Where(s => s.Cliente.Grupo_id.Equals(idGrupo));
                        }
                    }
                }
            }

            if (page < 1 ) page = 1;

            ViewBag.activos = asegurados.Where(s => !s.fechaBaja .HasValue).Count();

            ViewBag.registros = asegurados.Count();

            if (page == 1)
            {
                asegurados = asegurados.OrderBy(s => s.nombre);
            }
            else
                asegurados = asegurados.OrderBy(s => s.nombreTemporal).Skip(page  * 12 );

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
                var movtosTemp = from b in db.Movimientos 
                                 where b.aseguradoId.Equals(id)
                                   && b.tipo.Equals(option)
                                   orderby b.fechaTransaccion
                                     select b;

                Movimiento movto = new Movimiento();
                if (movtosTemp != null)
                {
                    foreach (var movtosItem in movtosTemp)
                    {
                        movto = movtosItem;
                        break;
                    }//Definimos los valores para la plaza
                }

                var fileName = "C:\\SUA\\Asegurados\\" + asegurado.numeroAfiliacion + "\\" + option + "\\" + movto.nombreArchivo.Trim();

                if (System.IO.File.Exists(fileName))
                {
                    FileStream fs = new FileStream(fileName, FileMode.Open);

                    return File(fs, "application/pdf");
                }
                else {
                    return RedirectToAction("Index");
                }
            }
            else {
                return RedirectToAction("Index");
            }
        }

        // GET: Aseguradoes/Create
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
        public void GetExcel(String patronesId, String clientesId)
        {
            /*
            ViewBag.CurrentPatron = patronesId;
            ViewBag.CurrentCliente = clientesId;
            */
            List<Asegurado> allCust = new List<Asegurado>();

            var asegurados = from s in db.Asegurados
                             select s;
            if (!String.IsNullOrEmpty(patronesId))
            {
                int id = int.Parse(patronesId.Trim());
                if (!String.IsNullOrEmpty(clientesId))
                {
                    asegurados = asegurados.Where(s => s.PatroneId.Equals(id));
                }
                else
                {
                    asegurados = asegurados.Where(s => s.PatroneId.Equals(id));
                }
            }

            allCust = asegurados.ToList();

            WebGrid grid = new WebGrid(source: allCust, canPage: false, canSort: false);

            string gridData = grid.GetHtml(
                columns: grid.Columns(
                        grid.Column("Patrone.registro", "Registro"),
                        grid.Column("numeroAfiliacion", "Numero Afiliacion"),
                        grid.Column("curp", "CURP"),
                        grid.Column("rfc", "RFC"),
                        grid.Column("nombreTemporal", "Nombre"),
                        grid.Column("fechaAlta", "Fecha Alta"),
                        grid.Column("fechaBaja", "Fecha Baja"),
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult Index(String plazasId, String patronesId, String clientesId, String gruposId, string currentPlaza, string currentPatron, string currentCliente, string currentGrupo, string opcion, string valor, int page = 1, String sortOrder=null, String lastSortOrder=null)
        {

            //ViewBag.patronesId = new SelectList(db.Patrones, "id", "nombre");
            ViewBag.plazasId = new SelectList((from s in db.Plazas.ToList()
                                               orderby s.descripcion
                                               select new
                                               {
                                                   id = s.id,
                                                   FUllName = s.descripcion
                                               }), "id", "FullName");

            ViewBag.patronesId = new SelectList((from s in db.Patrones.ToList()
                                                 orderby s.registro
                                                 select new
                                                 {
                                                     id = s.Id,
                                                     FullName = s.registro + " - " + s.nombre
                                                 }), "id", "FullName", null);
            ViewBag.clientesId = new SelectList((from s in db.Clientes.ToList()
                                                 orderby s.descripcion
                                                 select new
                                                 {
                                                     id = s.Id,
                                                     FUllName = s.claveCliente + " - " + s.descripcion
                                                 }), "id", "FullName");
            ViewBag.gruposId = new SelectList((from s in db.Grupos.ToList()
                                               orderby s.claveGrupo
                                               select new
                                               {
                                                   id = s.Id,
                                                   FUllName = s.claveGrupo + " - " + s.nombreCorto
                                               }), "id", "FullName");
            
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
                             select s;
            if (!String.IsNullOrEmpty(plazasId))
            {
                if (!String.IsNullOrEmpty(patronesId))
                {
                    if (!String.IsNullOrEmpty(clientesId))
                    {
                        if (!String.IsNullOrEmpty(gruposId))
                        {
                            int idPlaza = int.Parse(plazasId.Trim());
                            int idPatron = int.Parse(patronesId.Trim());
                            int idCliente = int.Parse(clientesId.Trim());
                            int idGrupo = int.Parse(gruposId.Trim());
                            asegurados = asegurados.Where(s => s.Patrone.Plaza_id.Equals(idPlaza) && s.PatroneId.Equals(idPatron) && s.Cliente.Id.Equals(idCliente) && s.Cliente.Grupo_id.Equals(idGrupo));
                        }
                        else
                        {
                            int idPlaza = int.Parse(plazasId.Trim());
                            int idPatron = int.Parse(patronesId.Trim());
                            int idCliente = int.Parse(clientesId.Trim());
                            asegurados = asegurados.Where(s => s.Patrone.Plaza_id.Equals(idPlaza) && s.PatroneId.Equals(idPatron) && s.Cliente.Id.Equals(idCliente));
                        }

                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(gruposId))
                        {
                            int idPlaza = int.Parse(plazasId.Trim());
                            int idPatron = int.Parse(patronesId.Trim());
                            int idGrupo = int.Parse(gruposId.Trim());
                            asegurados = asegurados.Where(s => s.Patrone.Plaza_id.Equals(idPlaza) && s.PatroneId.Equals(idPatron) && s.Cliente.Grupo_id.Equals(idGrupo));
                        }
                        else
                        {
                            int idPlaza = int.Parse(plazasId.Trim());
                            int idPatron = int.Parse(patronesId.Trim());
                            asegurados = asegurados.Where(s => s.Patrone.Plaza_id.Equals(idPlaza) && s.PatroneId.Equals(idPatron));
                        }

                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(clientesId))
                    {
                        if (!String.IsNullOrEmpty(gruposId))
                        {
                            int idPlaza = int.Parse(plazasId.Trim());
                            int idCliente = int.Parse(clientesId.Trim());
                            int idGrupo = int.Parse(gruposId.Trim());
                            asegurados = asegurados.Where(s => s.Patrone.Plaza_id.Equals(idPlaza) && s.Cliente.Id.Equals(idCliente) && s.Cliente.Grupo_id.Equals(idGrupo));
                        }
                        else
                        {
                            int idPlaza = int.Parse(plazasId.Trim());
                            int idCliente = int.Parse(clientesId.Trim());
                            asegurados = asegurados.Where(s => s.Patrone.Plaza_id.Equals(idPlaza) && s.Cliente.Id.Equals(idCliente));
                        }

                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(gruposId))
                        {
                            int idPlaza = int.Parse(plazasId.Trim());
                            int idGrupo = int.Parse(gruposId.Trim());
                            asegurados = asegurados.Where(s => s.Patrone.Plaza_id.Equals(idPlaza) && s.Cliente.Grupo_id.Equals(idGrupo));
                        }
                        else
                        {
                            int idPlaza = int.Parse(plazasId.Trim());
                            asegurados = asegurados.Where(s => s.Patrone.Plaza_id.Equals(idPlaza));
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
                            int idPatron = int.Parse(patronesId.Trim());
                            int idCliente = int.Parse(clientesId.Trim());
                            int idGrupo = int.Parse(gruposId.Trim());
                            asegurados = asegurados.Where(s => s.PatroneId.Equals(idPatron) && s.Cliente.Id.Equals(idCliente) && s.Cliente.Grupo_id.Equals(idGrupo));
                        }
                        else
                        {
                            int idPatron = int.Parse(patronesId.Trim());
                            int idCliente = int.Parse(clientesId.Trim());
                            asegurados = asegurados.Where(s => s.PatroneId.Equals(idPatron) && s.Cliente.Id.Equals(idCliente));
                        }

                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(gruposId))
                        {
                            int idPatron = int.Parse(patronesId.Trim());
                            int idGrupo = int.Parse(gruposId.Trim());
                            asegurados = asegurados.Where(s => s.PatroneId.Equals(idPatron) && s.Cliente.Grupo_id.Equals(idGrupo));
                        }
                        else
                        {
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
                            int idCliente = int.Parse(clientesId.Trim());
                            int idGrupo = int.Parse(gruposId.Trim());
                            asegurados = asegurados.Where(s => s.Cliente.Id.Equals(idCliente) && s.Cliente.Grupo_id.Equals(idGrupo));
                        }
                        else
                        {
                            int idCliente = int.Parse(clientesId.Trim());
                            asegurados = asegurados.Where(s => s.Cliente.Id.Equals(idCliente));
                        }

                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(gruposId))
                        {
                            int idGrupo = int.Parse(gruposId.Trim());
                            asegurados = asegurados.Where(s => s.Cliente.Grupo_id.Equals(idGrupo));
                        }
                    }
                }
            }

            if (!String.IsNullOrEmpty(opcion)){
                int opcionNum = int.Parse(opcion.Trim());
                switch (opcionNum)
                {
                    case 1:
                        asegurados = asegurados.Where(s => s.Patrone.registro.Contains(valor) );
                        break;
                    case 2:
                        asegurados = asegurados.Where(s => s.numeroAfiliacion.Contains(valor) );
                        break;
                    case 3:
                        asegurados = asegurados.Where(s => s.CURP.Contains(valor));
                        break;
                    case 4:
                        asegurados = asegurados.Where(s => s.RFC.Contains(valor));
                        break;
                    case 5:
                        asegurados = asegurados.Where(s => s.nombre.Contains(valor));
                        break;
                    case 6:
                        asegurados = asegurados.Where(s => s.fechaAlta.ToString().Contains(valor));
                        break;
                    case 7:
                        asegurados = asegurados.Where(s => s.fechaBaja.ToString().Contains(valor));
                        break;
                    case 8:
                        asegurados = asegurados.Where(s => s.salarioImss.ToString().Contains(valor));
                        break;
                    case 9:
                        asegurados = asegurados.Where(s => s.Cliente.claveCliente.Contains(valor));
                        break;
                    case 10:
                        asegurados = asegurados.Where(s => s.Cliente.Grupos.nombre.Contains(valor));
                        break;
                    case 11:
                        asegurados = asegurados.Where(s => s.ocupacion.Contains(valor));
                        break;
                    case 12:
                        asegurados = asegurados.Where(s => s.Cliente.Plaza.cve.Contains(valor));
                        break;
                    case 13:
                        asegurados = asegurados.Where(s => s.extranjero.Contains(valor));
                        break;
                }
            }
            if (page < 1 ) page = 1;

            ViewBag.activos = asegurados.Where(s => !s.fechaBaja .HasValue).Count();

            ViewBag.registros = asegurados.Count();

            if (page == 1)
            {
                asegurados = asegurados.OrderBy(s => s.nombre);
            }
            else
                asegurados = asegurados.OrderBy(s => s.nombreTemporal).Skip(page  * 12 );

            return View(asegurados.ToList());
        }

    }
}
