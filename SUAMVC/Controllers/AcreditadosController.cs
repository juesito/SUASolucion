using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SUADATOS;
using System.Text;
using System.Data.Entity.Validation;
using System.Data.OleDb;
using PagedList;
using System.IO;
using System.Web.Helpers;

namespace SUAMVC.Controllers
{
    public class AcreditadosController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: Acreditados
        public ActionResult Index(String patronesId, String clientesId)
        {
            ViewBag.patronesId = new SelectList((from s in db.Patrones.ToList()
                                                 select new
                                                 {
                                                     id = s.Id,
                                                     FullName = s.registro + " - " + s.nombre
                                                 }), "id", "FullName", null);

            ViewBag.clientesId = new SelectList((from s in db.Clientes.ToList()
                                                 select new { 
                                                     id = s.Id,
                                                     FUllName = s.claveCliente + " - " + s.descripcion
                                                 }), "id", "FullName");

            ViewBag.CurrentPatron = patronesId;
            ViewBag.CurrentCliente = clientesId;

            var acreditados = from s in db.Acreditados
                           select s;
            if (!String.IsNullOrEmpty(patronesId) && String.IsNullOrEmpty(clientesId))
            {
                int id = int.Parse(patronesId.Trim());
                acreditados = acreditados.Where(s => s.PatroneId.Equals(id));
            }

            if (!String.IsNullOrEmpty(clientesId) && String.IsNullOrEmpty(patronesId))
            {
                int id = int.Parse(clientesId.Trim());
                acreditados = acreditados.Where(s => s.Cliente.Id.Equals(id));
            }

            if (!String.IsNullOrEmpty(clientesId) && !String.IsNullOrEmpty(patronesId))
            {
                int idPatron = int.Parse(patronesId.Trim());
                int id = int.Parse(clientesId.Trim());
                acreditados = acreditados.Where(s => s.PatroneId.Equals(idPatron) && s.Cliente.Id.Equals(id));
            }
                        
            acreditados = acreditados.OrderBy(s => s.nombreCompleto);
            
            return View(acreditados.ToList());
        }

        public ActionResult UploadFile(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Acreditado acreditado = db.Acreditados.Find(id);
            if (acreditado == null)
            {
                return HttpNotFound();
            }

            return View(acreditado);
        }

        [HttpPost]
        public ActionResult UploadPDFFile([Bind(Include = "id")] Acreditado acreditado, String answer)
        {
            if (acreditado.id > 0)
            {
                acreditado = db.Acreditados.Find(acreditado.id);
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];

                    if (file != null && file.ContentLength > 0)
                    {
                        String path = "C:\\SUA\\Acreditados\\" + acreditado.numeroAfiliacion.Trim() + "\\" + answer + "\\"; //Path.Combine("C:\\SUA\\", uploadModel.subFolder);
                        if (!System.IO.File.Exists(path))
                        {
                            System.IO.Directory.CreateDirectory(path);
                        }

                        //var fileName = Path.GetFileName(file.FileName);
                        var fileName = answer + "-" + acreditado.numeroAfiliacion.Trim() + ".pdf";
                        var pathFinal = Path.Combine(path, fileName);
                        file.SaveAs(pathFinal);
                        //Move();

                        ViewBag.dbUploaded = true;

                        //Validamos la acción realizada
                        if (answer.Equals("Alta"))
                        {
                            acreditado.alta = "S";
                        }
                        else if (answer.Equals("Baja"))
                        {
                            acreditado.baja = "S";
                        }
                        else if (answer.Equals("Modificacion"))
                        {
                            acreditado.modificacion = "S";
                        }
                        else
                        {
                            acreditado.permanente = "S";
                        }

                        db.Acreditados.Add(acreditado);
                        db.SaveChanges();
                    }
                }
            }

            return RedirectToAction("Index");
        }

        public ActionResult ViewAttachment(int id, String option, String carga)
        {
            if (carga != null)
            {
                Acreditado acreditado = db.Acreditados.Find(id);

                var fileName = "C:\\SUA\\Acreditados\\" + acreditado.numeroAfiliacion.Trim() + "\\" + option + "\\" + option + "-" + acreditado.numeroAfiliacion.Trim() + ".pdf";
                FileStream fs = new FileStream(fileName, FileMode.Open);

                return File(fs, "application/pdf");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        // GET: Acreditados/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Acreditado acreditado = db.Acreditados.Find(id);
            if (acreditado == null)
            {
                return HttpNotFound();
            }
            return View(acreditado);
        }

        // GET: Acreditados/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Acreditados/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "registroPatronal,apellidoPaterno,apellidoMaterno,nombre,nombreCompleto,CURP,RFC,ubicacion,ocupacion,idGrupo,numeroAfiliacion,numeroCredito,fechaAlta,fechaBaja,fechaInicioDescuento,fechaFinDescuento,smdv,sdi,sd,vsm,porcentaje,cuotaFija,descuentoBimestral,descuentoMensual,descuentoSemanal,descuentoCatorcenal,descuentoQuincenal,descuentoVeintiochonal,descuentoDiario,idPlaza,acuseRetencion")] Acreditado acreditado)
        {
            if (ModelState.IsValid)
            {
                db.Acreditados.Add(acreditado);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(acreditado);
        }

        // GET: Acreditados/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Acreditado acreditado = db.Acreditados.Find(id);
            if (acreditado == null)
            {
                return HttpNotFound();
            }
            return View(acreditado);
        }

        // POST: Acreditados/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "registroPatronal,apellidoPaterno,apellidoMaterno,nombre,nombreCompleto,CURP,RFC,ubicacion,ocupacion,idGrupo,numeroAfiliacion,numeroCredito,fechaAlta,fechaBaja,fechaInicioDescuento,fechaFinDescuento,smdv,sdi,sd,vsm,porcentaje,cuotaFija,descuentoBimestral,descuentoMensual,descuentoSemanal,descuentoCatorcenal,descuentoQuincenal,descuentoVeintiochonal,descuentoDiario,idPlaza,acuseRetencion")] Acreditado acreditado)
        {
            if (ModelState.IsValid)
            {
                db.Entry(acreditado).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(acreditado);
        }

        // GET: Acreditados/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Acreditado acreditado = db.Acreditados.Find(id);
            if (acreditado == null)
            {
                return HttpNotFound();
            }
            return View(acreditado);
        }

        // POST: Acreditados/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Acreditado acreditado = db.Acreditados.Find(id);
            db.Acreditados.Remove(acreditado);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult UploadData()
        {
            ViewBag.patronesId = new SelectList(db.Patrones, "registro", "nombre");
            ViewBag.clientesId = new SelectList(db.Clientes, "id", "descripcion");
            return View();
        }

        [HttpGet]
        public void GetExcel(String patronesId, String clientesId)
        {
            /*
            ViewBag.CurrentPatron = patronesId;
            ViewBag.CurrentCliente = clientesId;
            */
            List<Acreditado> allCust = new List<Acreditado>();

            var acreditados = from s in db.Acreditados
                             select s;
            if (!String.IsNullOrEmpty(patronesId))
            {
                int id = int.Parse(patronesId.Trim());
                if (!String.IsNullOrEmpty(clientesId))
                {
                    acreditados = acreditados.Where(s => s.PatroneId.Equals(id));
                }
                else
                {
                    acreditados = acreditados.Where(s => s.PatroneId.Equals(id));
                }
            }

            allCust = acreditados.ToList();

            WebGrid grid = new WebGrid(source: allCust, canPage: false, canSort: false);

            string gridData = grid.GetHtml(
                columns: grid.Columns(
                        grid.Column("Patrone.registro", "Registro"),
            grid.Column("apellidoPaterno", "Apellido Paterno"),
            grid.Column("apellidoMaterno", "Apellido Materno"),
            grid.Column("nombre", "Nombre"),
            grid.Column("nombreCompleto", "Nombre Completo"),
            grid.Column("curp","CURP"),
            grid.Column("rfc", "RFC"),
            /*grid.Column("Cliente.descripcion", "Cliente/Ubicación"),*/
            grid.Column("ocupacion", "Ocupación"),
            grid.Column("idGrupo", "Grupo"),
            grid.Column("numeroAfiliacion", "Numero Afiliación"),
            grid.Column("numeroCredito", "Numero Credito"),
            grid.Column("fechaAlta", "Fecha Alta"),
            grid.Column("fechaBaja", "Fecha Baja"),
            grid.Column("fechaInicioDescuento", "Inicio descuento"),
            grid.Column("fechaFinDescuento", "Fin descuento"),
            grid.Column("smdv", "SMDV"),
            grid.Column("sdi", "SDI"),
            grid.Column("sd", "SD"),
            grid.Column("vsm", "VSM"),
            grid.Column("porcentaje", "Porcentaje"),
            grid.Column("cuotaFija", "Cuota Fija"),
            grid.Column("descuentoBimestral", "Descuento Bimestral"),
            grid.Column("descuentoMensual", "Descuento Mensual"),
            grid.Column("descuentoSemanal", "Descuento Semanal"),
            grid.Column("descuentoCatorcenal", "Descuento Catorcenal"),
            grid.Column("descuentoQuincenal", "Descuento Quincenal"),
            grid.Column("descuentoVeintiochonal", "Descuento Veintiochonal"),
            grid.Column("descuentoDiario", "Descuento Diario"),
            grid.Column("Plaza.descripcion", "Plaza")
                        )
                    ).ToString();

            Response.ClearContent();
            DateTime date = DateTime.Now;
            String fileName = "Acreditados-" + date.ToString("ddMMyyyyHHmm") + ".xls";
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
