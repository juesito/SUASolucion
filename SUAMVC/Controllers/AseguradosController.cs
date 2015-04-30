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
        public ActionResult Index(String patronesId, String clientesId, String gruposId, string currentPatron, string currentCliente, string currentGrupo, int? page, String sortOrder, String lastSortOrder)
        {

            //ViewBag.patronesId = new SelectList(db.Patrones, "id", "nombre");
            ViewBag.patronesId = new SelectList((from s in db.Patrones.ToList() 
                                                 select new{
                                                    id = s.Id,
                                                    FullName = s.registro + " - " + s.nombre}), "id", "FullName", null);

            ViewBag.clientesId = new SelectList((from s in db.Clientes.ToList()
                                                 select new
                                                 {
                                                     id = s.Id,
                                                     FUllName = s.claveCliente + " - " + s.descripcion
                                                 }), "id", "FullName");
     
            ViewBag.gruposId = new SelectList((from s in db.Grupos.ToList()
                                                 select new
                                                 {
                                                     id = s.Id,
                                                     FUllName = s.claveGrupo + " - " + s.nombreCorto
                                                 }), "id", "FullName");

            var asegurados = from s in db.Asegurados
                              select s;
            if (!String.IsNullOrEmpty(patronesId) && String.IsNullOrEmpty(clientesId))
            {
                int id = int.Parse(patronesId.Trim());
                asegurados = asegurados.Where(s => s.PatroneId.Equals(id));
            }

            if (!String.IsNullOrEmpty(clientesId) && String.IsNullOrEmpty(patronesId))
            {
                int id = int.Parse(clientesId.Trim());
                asegurados = asegurados.Where(s => s.Cliente.Id.Equals(id));
            }

            if (!String.IsNullOrEmpty(gruposId) )
            {
                int id = int.Parse(gruposId.Trim());
                asegurados = asegurados.Where(s => s.Cliente.Grupo_id.Equals(id));
            }

            if (!String.IsNullOrEmpty(clientesId) && !String.IsNullOrEmpty(patronesId))
            {
                int idPatron = int.Parse(patronesId.Trim());
                int id = int.Parse(clientesId.Trim());
                asegurados = asegurados.Where(s => s.PatroneId.Equals(idPatron) && s.Cliente.Id.Equals(id));
            }

            asegurados = asegurados.OrderBy(s => s.nombreTemporal);

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
        public ActionResult Delete(int id)
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

        // POST: Aseguradoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Asegurado asegurado = db.Asegurados.Find(id);
            db.Asegurados.Remove(asegurado);
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
                        grid.Column("curp","CURP"),
                        grid.Column("rfc", "RFC"),
                        grid.Column("nombreTemporal","Nombre"),
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
