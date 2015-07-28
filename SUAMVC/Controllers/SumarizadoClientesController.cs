using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SUADATOS;
using System.Data.SqlClient;
using System.Web.Helpers;
using SUAMVC.Models;


namespace SUAMVC.Controllers
{
    public class SumarizadoClientesController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: SumarizadoClientes
        public ActionResult Index(String plazasId, String patronesId, String periodoId,
            String ejercicioId, String clientesId, String usuarioId)
        {
            SumarizadoClienteModel sumarizadoClienteModel = new SumarizadoClienteModel();
            Usuario user = Session["UsuarioData"] as Usuario;
            ViewBag.filtered = true;


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

            //Query principal
            int result = db.Database.ExecuteSqlCommand("sp_SumarizadoClientesTodos @usuarioId", new SqlParameter("@usuarioId", user.Id));
            var sumarizadoClientes = from s in db.SumarizadoClientes
                             join cli in db.Clientes on s.clienteId equals cli.Id
                             where plazasAsignadas.Contains(s.Cliente.Plaza_id) &&
                                   clientesAsignados.Contains(s.Cliente.Id) &&
                                   patronesAsignados.Contains(s.patronId) &&
                                   s.usuarioId.Equals(user.Id)
                             select s;
            
 //            var sumarizadoClientes = db.SumarizadoClientes.Include(s => s.Cliente).Include(s => s.Patrone).Include(s => s.Usuario);


                if (!String.IsNullOrEmpty(clientesId))
                {
                    int clienteId = int.Parse(clientesId.Trim());
                    sumarizadoClientes = sumarizadoClientes.Where(s => s.clienteId.Equals(clienteId));
                }
                if (!String.IsNullOrEmpty(plazasId))
                {
                    int plazaTempId = int.Parse(plazasId.Trim());
                    sumarizadoClientes = sumarizadoClientes.Where(s => s.Patrone.Plaza_id.Equals(plazaTempId));
                }
                if (!String.IsNullOrEmpty(patronesId))
                {
                    int patronesTempId = int.Parse(patronesId);
                    sumarizadoClientes = sumarizadoClientes.Where(s => s.Patrone.Id.Equals(patronesTempId));
                }
                if (!String.IsNullOrEmpty(periodoId))
                {
                    sumarizadoClientes = sumarizadoClientes.Where(s => s.mes.Trim().Equals(periodoId.Trim()));
                }
                if (!String.IsNullOrEmpty(ejercicioId))
                {
                    sumarizadoClientes = sumarizadoClientes.Where(s => s.anno.Trim().Equals(ejercicioId));
                }

            sumarizadoClientes = sumarizadoClientes.OrderBy(p => p.Patrone.registro);
                
                sumarizadoClienteModel.sumarizadoCliente = sumarizadoClientes.ToList();
                SumarizadoAcumulado sa = new SumarizadoAcumulado();

                foreach (SumarizadoCliente sc in sumarizadoClientes) {
                    sa.sumImss = sa.sumImss + System.Convert.ToDouble(sc.imss);
                    sa.sumRcv = sa.sumRcv + System.Convert.ToDouble(sc.rcv);
                    sa.sumInfonavit = sa.sumInfonavit + System.Convert.ToDouble(sc.infonavit);
                    sa.sumTotal = sa.sumTotal + System.Convert.ToDouble(sc.total);
                    sa.sumNt = sa.sumNt + System.Convert.ToDouble(sc.nt);
                }

            sumarizadoClienteModel.sumarizadoAcumulado = sa;

            return View(sumarizadoClienteModel);
        }

        // GET: SumarizadoClientes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SumarizadoCliente sumarizadoCliente = db.SumarizadoClientes.Find(id);
            if (sumarizadoCliente == null)
            {
                return HttpNotFound();
            }
            return View(sumarizadoCliente);
        }

        // GET: SumarizadoClientes/Create
        public ActionResult Create()
        {
            ViewBag.clienteId = new SelectList(db.Clientes, "Id", "claveCliente");
            ViewBag.patronId = new SelectList(db.Patrones, "Id", "registro");
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario");
            return View();
        }

        // POST: SumarizadoClientes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,patronId,anno,mes,imss,rcv,infonavit,total,nt,usuarioId,fechaCreacion,clienteId")] SumarizadoCliente sumarizadoCliente)
        {
            if (ModelState.IsValid)
            {
                db.SumarizadoClientes.Add(sumarizadoCliente);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.clienteId = new SelectList(db.Clientes, "Id", "claveCliente", sumarizadoCliente.clienteId);
            ViewBag.patronId = new SelectList(db.Patrones, "Id", "registro", sumarizadoCliente.patronId);
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", sumarizadoCliente.usuarioId);
            return View(sumarizadoCliente);
        }

        // GET: SumarizadoClientes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SumarizadoCliente sumarizadoCliente = db.SumarizadoClientes.Find(id);
            if (sumarizadoCliente == null)
            {
                return HttpNotFound();
            }
            ViewBag.clienteId = new SelectList(db.Clientes, "Id", "claveCliente", sumarizadoCliente.clienteId);
            ViewBag.patronId = new SelectList(db.Patrones, "Id", "registro", sumarizadoCliente.patronId);
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", sumarizadoCliente.usuarioId);
            return View(sumarizadoCliente);
        }

        // POST: SumarizadoClientes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,patronId,anno,mes,imss,rcv,infonavit,total,nt,usuarioId,fechaCreacion,clienteId")] SumarizadoCliente sumarizadoCliente)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sumarizadoCliente).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.clienteId = new SelectList(db.Clientes, "Id", "claveCliente", sumarizadoCliente.clienteId);
            ViewBag.patronId = new SelectList(db.Patrones, "Id", "registro", sumarizadoCliente.patronId);
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", sumarizadoCliente.usuarioId);
            return View(sumarizadoCliente);
        }

        // GET: SumarizadoClientes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SumarizadoCliente sumarizadoCliente = db.SumarizadoClientes.Find(id);
            if (sumarizadoCliente == null)
            {
                return HttpNotFound();
            }
            return View(sumarizadoCliente);
        }

        // POST: SumarizadoClientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SumarizadoCliente sumarizadoCliente = db.SumarizadoClientes.Find(id);
            db.SumarizadoClientes.Remove(sumarizadoCliente);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        [HttpGet]
        public void GetExcel(String plazasId, String patronesId, String periodoId,
            String ejercicioId, String clientesId, String usuarioId)
        {
            var sumarizadoClientes = db.SumarizadoClientes.Include(s => s.Cliente).Include(s => s.Patrone).Include(s => s.Usuario);
            if (!String.IsNullOrEmpty(clientesId))
            {
                ViewBag.filtered = true;
                int clienteId = int.Parse(clientesId.Trim());
                int userId = int.Parse(usuarioId.Trim());

                int result = db.Database.ExecuteSqlCommand("sp_SumarizadoClientes @usuarioId, @clienteId", new SqlParameter("@usuarioId", userId), new SqlParameter("@clienteId", clienteId));
                sumarizadoClientes = db.SumarizadoClientes.Include(s => s.Cliente).Include(s => s.Patrone).Include(s => s.Usuario);
                if (!String.IsNullOrEmpty(plazasId))
                {
                    int plazaTempId = int.Parse(plazasId.Trim());
                    sumarizadoClientes = sumarizadoClientes.Where(s => s.Patrone.Plaza_id.Equals(plazaTempId));
                }
                if (!String.IsNullOrEmpty(patronesId))
                {
                    int patronesTempId = int.Parse(patronesId);
                    sumarizadoClientes = sumarizadoClientes.Where(s => s.Patrone.Id.Equals(patronesTempId));
                }
                if (!String.IsNullOrEmpty(periodoId))
                {
                    sumarizadoClientes = sumarizadoClientes.Where(s => s.mes.Trim().Equals(periodoId.Trim()));
                }
                if (!String.IsNullOrEmpty(ejercicioId))
                {
                    sumarizadoClientes = sumarizadoClientes.Where(s => s.anno.Trim().Equals(ejercicioId));
                }
            }

            sumarizadoClientes = sumarizadoClientes.OrderBy(p => p.Patrone.registro);
            List<SumarizadoCliente> allCust = new List<SumarizadoCliente>();

            allCust = sumarizadoClientes.ToList();

            WebGrid grid = new WebGrid(source: allCust, canPage: false, canSort: false);

            List<WebGridColumn> gridColumns = new List<WebGridColumn>();

            gridColumns.Add(grid.Column("Patrone.registro", "Reg. Patronal", null, null, true));
            gridColumns.Add(grid.Column("anno", "Año", null, null, true));
            gridColumns.Add(grid.Column("mes", "Mes", null, null, true));
            gridColumns.Add(grid.Column("Cliente.claveCliente", "ID. Cliente", null, null, true));
            gridColumns.Add(grid.Column("Cliente.descripcion", "Nombre", null, null, true));
            gridColumns.Add(grid.Column("Cliente.Grupos.claveGrupo", "ID. Grupo", null, null, true));
            gridColumns.Add(grid.Column("imss", "IMSS", null, null, true));
            gridColumns.Add(grid.Column("rcv", "RCV", null, null, true));
            gridColumns.Add(grid.Column("infonavit", "Infonavit", null, null, true));
            gridColumns.Add(grid.Column("total", "Total", null, null, true));
            gridColumns.Add(grid.Column("nt", "NT", null, null, true));
            gridColumns.Add(grid.Column("Patrone.Plaza.cveCorta", "Plaza", null, null, true));

            string gridData = grid.GetHtml(
                columns: grid.Columns(gridColumns.ToArray())
                    ).ToString();

            Response.ClearContent();
            DateTime date = DateTime.Now;
            String fileName = "ClientesPagos-" + date.ToString("ddMMyyyyHHmm") + ".xls";
            Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
            Response.ContentType = "application/excel";
            Response.Write(gridData);
            Response.End();
        }

        [HttpGet]
        public void ExcelDetalle(String anio, String mes, String idPatron, String idCliente)
        {

     //       int anioTemp = int.Parse(anio.Trim());
     //       int mesTemp = int.Parse(mes.Trim());
            int idPatronTemp = int.Parse(idPatron.Trim());
     //       int idClienteTemp = int.Parse(idCliente.Trim());

            Pago pago = db.Pagos.Where(p => p.patronId.Equals(idPatronTemp) && p.anno.Equals(anio) && p.mes.Equals(mes)).FirstOrDefault();

            List<DetallePago> detallePago = db.DetallePagoes.Where(r => r.pagoId.Equals(pago.id) && r.Asegurado.Cliente.claveCliente.Equals(idCliente)).ToList();

            List<DetallePago> allCust = new List<DetallePago>();

            allCust = detallePago;

            WebGrid grid = new WebGrid(source: allCust, canPage: false, canSort: false);

            List<WebGridColumn> gridColumns = new List<WebGridColumn>();

            gridColumns.Add(grid.Column("Pago.Patrone.registro", "Patrón", null, null, true));
            gridColumns.Add(grid.Column("Pago.mes", "Periodo", null, null, true));
            gridColumns.Add(grid.Column("Pago.anno", "Ejercicio", null, null, true));
            gridColumns.Add(grid.Column("Pago.fechaDeposito", "Fecha depósito", null, null, true));
            gridColumns.Add(grid.Column("Asegurado.numeroAfiliacion", "NSS", null, null, true));
            gridColumns.Add(grid.Column("Asegurado.nombreTemporal", "Nombre", null, null, true));
            gridColumns.Add(grid.Column("diasCotizados", "Dias", null, null, true));
            gridColumns.Add(grid.Column("sdi", "S.D.I.", null, null, true));
            gridColumns.Add(grid.Column("diasIncapacidad", "Inc.", null, null, true));
            gridColumns.Add(grid.Column("diasAusentismo", "Aus.", null, null, true));
            gridColumns.Add(grid.Column("cuotaFija", "C.F.", null, null, true));
            gridColumns.Add(grid.Column("expa", "Ex.P", null, null, true));
            gridColumns.Add(grid.Column("exo", "Ex. O.", null, null, true));
            gridColumns.Add(grid.Column("Asegurado.Cliente.claveCliente", "Ubicación", null, null, true));
            gridColumns.Add(grid.Column("PDP", "PDP", null, null, true));
            gridColumns.Add(grid.Column("GMPP", "GMP. Patron", null, null, true));
            gridColumns.Add(grid.Column("GMPO", "GMP. Obrero", null, null, true));
            gridColumns.Add(grid.Column("rt", "R.T.", null, null, true));
            gridColumns.Add(grid.Column("ivp", "I.V.P", null, null, true));
            gridColumns.Add(grid.Column("ivo", "I.V.O", null, null, true));
            gridColumns.Add(grid.Column("gps", "G.P.S.", null, null, true));
            gridColumns.Add(grid.Column("patronal", "Patronal", null, null, true));
            gridColumns.Add(grid.Column("obrera", "Obrera", null, null, true));
            gridColumns.Add(grid.Column("imss", "IMSS", null, null, true));
            gridColumns.Add(grid.Column("diasCotizBim", "Diascotizados Bim", null, null, true));
            gridColumns.Add(grid.Column("retiro", "Retiro", null, null, true));
            gridColumns.Add(grid.Column("patronalBimestral", "Patronal Bim", null, null, true));
            gridColumns.Add(grid.Column("obreraBimestral", "Obrera Bim", null, null, true));
            gridColumns.Add(grid.Column("rcv", "R.C.V.", null, null, true));
            gridColumns.Add(grid.Column("aportacionsc", "Aportacion SC", null, null, true));
            gridColumns.Add(grid.Column("aportacioncc", "Aportacion CC", null, null, true));
            gridColumns.Add(grid.Column("amortizacion", "Amortizacion", null, null, true));
            gridColumns.Add(grid.Column("infonavit", "Infonavit", null, null, true));
            gridColumns.Add(grid.Column("total", "Total", null, null, true));

            string gridData = grid.GetHtml(
                columns: grid.Columns(gridColumns.ToArray())
                    ).ToString();

            Response.ClearContent();
            DateTime date = DateTime.Now;
            String fileName = "DetallePagosCliente-" + date.ToString("ddMMyyyyHHmm") + ".xls";
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
