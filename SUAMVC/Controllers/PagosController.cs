using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SUADATOS;
using SUAMVC.Models;
using System.IO;

namespace SUAMVC.Controllers
{
    public class PagosController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: Pagos
        public ActionResult Index()
        {
            var pagos = db.Pagos.Include(p => p.ResumenPago);
            return View(pagos.ToList());
        }

        public ActionResult UploadPagos()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload(String patronesId, String periodoId, String ejercicioId)
        {
            if (!String.IsNullOrEmpty(patronesId) && !String.IsNullOrEmpty(periodoId) && !String.IsNullOrEmpty(ejercicioId))
            {
                String periodo = ejercicioId.Trim() + periodoId.Trim();
                int patronTemp = int.Parse(patronesId);
                Patrone patron = db.Patrones.Find(patronTemp);
                String path = this.UploadFile(patron.direccionArchivo);


                if (!path.Equals(""))
                {
                    Boolean existe = false;
                    SUAHelper suaHelper = new SUAHelper(path);
                    ResumenPago resumenPago = new ResumenPago();

                    //Preparamos el query del resúmen
                    String sSQL = "SELECT * FROM Registro_02" +
                        "  WHERE Registro_Patronal = '" + patron.registro + "'" +
                        "    AND Periodo_Pago = '" + periodo + "'" +
                        "ORDER BY Registro_Patronal";

                    DataTable dt = suaHelper.ejecutarSQL(sSQL);

                    foreach (DataRow rows in dt.Rows)
                    {
                        resumenPago.ip = rows["IP"].ToString().Trim();
                        resumenPago.patronId = patron.Id;
                        resumenPago.rfc = rows["RFC"].ToString().Trim();
                        resumenPago.periodoPago = periodo;
                        resumenPago.mes = periodoId;
                        resumenPago.anno = ejercicioId;
                        resumenPago.folioSUA = rows["Folio_SUA"].ToString().Trim();
                        resumenPago.razonSocial = rows["Razon_Social"].ToString().Trim();
                        resumenPago.calleColonia = rows["Calle_Colonia"].ToString().Trim();
                        resumenPago.poblacion = rows["Poblacion"].ToString().Trim();
                        resumenPago.entidadFederativa = rows["Entidad_Federativa"].ToString().Trim();
                        resumenPago.codigoPostal = rows["CP"].ToString().Trim();
                        resumenPago.primaRT = rows["Prima_RT"].ToString().Trim();
                        resumenPago.fechaPrimaRT = rows["Fecha_Prima_RT"].ToString().Trim();
                        resumenPago.actividadEconomica = rows["Actividad_Economica"].ToString().Trim();
                        resumenPago.delegacionIMSS = rows["Delegacion_IMSS"].ToString().Trim();
                        resumenPago.subDelegacionIMMS = rows["SubDelegacion_IMSS"].ToString().Trim();
                        resumenPago.zonaEconomica = rows["Zona_Economica"].ToString().Trim();
                        resumenPago.convenioReembolso = rows["Convenio_Reembolso"].ToString().Trim();
                        resumenPago.tipoCotizacion = rows["Tipo_Cotizacion"].ToString().Trim();
                        resumenPago.cotizantes = rows["Cotizantes"].ToString().Trim();
                        resumenPago.apoPat = rows["Apo_Pat"].ToString().Trim();
                        resumenPago.delSubDel = rows["Del_Subdel"].ToString().Trim();

                        existe = true;
                        db.ResumenPagoes.Add(resumenPago);
                        db.SaveChanges();
                    }

                    if (existe)
                    {

                        sSQL = "SELECT * FROM RESUMEN" +
                               "  WHERE Reg_Patr = '" + patron.registro + "'" +
                               "    AND Mes_Ano = '" + periodo + "'" +
                               "   ORDER BY Reg_Patr";

                        DataTable dt2 = suaHelper.ejecutarSQL(sSQL);

                        foreach (DataRow rows in dt2.Rows)
                        {
                            Pago pago = new Pago();

                            pago.imss = Decimal.Parse(rows["CTA_FIJ"].ToString()) + Decimal.Parse(rows["CTA_EXC"].ToString()) +
                                        Decimal.Parse(rows["PRE_DIN"].ToString()) + Decimal.Parse(rows["PRE_ESP"].ToString()) +
                                        Decimal.Parse(rows["RIE_TRA"].ToString()) + Decimal.Parse(rows["INV_VID"].ToString()) +
                                        Decimal.Parse(rows["GUA_DER"].ToString());

                            pago.rcv = Decimal.Parse(rows["RET_SAR"].ToString()) + Decimal.Parse(rows["CEN_VEJPat"].ToString()) +
                                       Decimal.Parse(rows["Cen_VEJObr"].ToString());

                            pago.infonavit = Decimal.Parse(rows["VIV_SIN"].ToString()) + Decimal.Parse(rows["VIV_CON"].ToString()) +
                                             Decimal.Parse(rows["AMO_INF"].ToString());

                            pago.total = pago.imss + pago.rcv + pago.infonavit;

                            pago.recargos = Decimal.Parse(rows["REC_IMS"].ToString()) + Decimal.Parse(rows["REC_SAR"].ToString()) +
                                            Decimal.Parse(rows["REC_VIV"].ToString());

                            pago.actualizaciones = Decimal.Parse(rows["ACT_IMS"].ToString()) + Decimal.Parse(rows["ACT_SAR"].ToString()) +
                                                   Decimal.Parse(rows["ACT_VIV"].ToString());

                            pago.granTotal = pago.recargos + pago.actualizaciones;

                            sSQL = "SELECT COUNT(*) FROM RELTRA" +
                               "  WHERE Reg_Pat = '" + patron.registro + "'" +
                               "    AND Periodo = '" + periodo + "'" +
                               "   ORDER BY Reg_Pat";

                            DataTable dt3 = suaHelper.ejecutarSQL(sSQL);

                            foreach (DataRow rows1 in dt3.Rows) {
                                pago.nt = int.Parse(rows1["1"].ToString());
                            }



                            //Guardamos el pago.
                            db.Pagos.Add(pago);
                            db.SaveChanges();
                        }

                    }
                }
            }
            return RedirectToAction("UploadPagos");
        }

        public String UploadFile(String subFolder)
        {
            String path = "";
            ParametrosHelper parameterHelper = new ParametrosHelper();
            Parametro rutaParameter = parameterHelper.getParameterByKey("SUARUTA");

            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];

                if (file != null && file.ContentLength > 0)
                {

                    if (!subFolder.Equals(""))
                    {
                        path = Path.Combine(rutaParameter.valorString.Trim(), subFolder);
                        if (!System.IO.File.Exists(path))
                        {
                            System.IO.Directory.CreateDirectory(path);
                        }
                    }
                    else
                    {
                        path = rutaParameter.valorString.Trim();
                    }

                    var fileName = Path.GetFileName(file.FileName);
                    //var path = Path.Combine(Server.MapPath("~/App_LocalResources/"), fileName);
                    var pathFinal = Path.Combine(path, fileName);
                    file.SaveAs(pathFinal);
                    ViewBag.dbUploaded = true;
                    TempData["error"] = false;
                    TempData["viewMessage"] = "Se ha realizado la actualización con exito!";
                }
            }

            return path;
        }

        // GET: Pagos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pago pago = db.Pagos.Find(id);
            if (pago == null)
            {
                return HttpNotFound();
            }
            return View(pago);
        }

        // GET: Pagos/Create
        public ActionResult Create()
        {
            ViewBag.trabajadorId = new SelectList(db.Asegurados, "id", "numeroAfiliacion");
            ViewBag.resumenPagoId = new SelectList(db.ResumenPagoes, "id", "ip");
            return View();
        }

        // POST: Pagos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,resumenPagoId,ip,NSS,RFC,CURP,creditoInfonavit,fid,trabajador,sdi,tipoTrabajador,jornadaSemanaReducida,diasCotizadosMes,diasIncapacidad,diasAusentismo,cuotaFija,cuotaExcedente,prestacionesDinero,gastosMedicosPensionado,riesgoTrabajo,invalidezVida,guarderias,actRecargosIMSS,diasCotizadosBimestre,diasIncapacidadBimestre,diasAusentismoBimestre,retiro,actRecargosRetiro,cesantiaVejezPatronal,cesantiaVejezObrera,actRecargosCyV,aportacionVoluntaria,aportacionComp,aportacionPatronal,amortizacion,actIMSS,recIMSS,actRetiro,recRetiro,actCesPat,recCesPat,actCesObr,recCesObr,cuotaExcObr,cuotaPdObr,cuotaGmpObr,cuotaIvObr,actPatIMSS,recPatIMSS,actObrIMSS,recObrIMSS,trabajadorId,anoPago,mesPago")] Pago pago)
        {
            if (ModelState.IsValid)
            {
                db.Pagos.Add(pago);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.resumenPagoId = new SelectList(db.ResumenPagoes, "id", "ip", pago.resumenPagoId);
            return View(pago);
        }

        // GET: Pagos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pago pago = db.Pagos.Find(id);
            if (pago == null)
            {
                return HttpNotFound();
            }
            ViewBag.resumenPagoId = new SelectList(db.ResumenPagoes, "id", "ip", pago.resumenPagoId);
            return View(pago);
        }

        // POST: Pagos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,resumenPagoId,ip,NSS,RFC,CURP,creditoInfonavit,fid,trabajador,sdi,tipoTrabajador,jornadaSemanaReducida,diasCotizadosMes,diasIncapacidad,diasAusentismo,cuotaFija,cuotaExcedente,prestacionesDinero,gastosMedicosPensionado,riesgoTrabajo,invalidezVida,guarderias,actRecargosIMSS,diasCotizadosBimestre,diasIncapacidadBimestre,diasAusentismoBimestre,retiro,actRecargosRetiro,cesantiaVejezPatronal,cesantiaVejezObrera,actRecargosCyV,aportacionVoluntaria,aportacionComp,aportacionPatronal,amortizacion,actIMSS,recIMSS,actRetiro,recRetiro,actCesPat,recCesPat,actCesObr,recCesObr,cuotaExcObr,cuotaPdObr,cuotaGmpObr,cuotaIvObr,actPatIMSS,recPatIMSS,actObrIMSS,recObrIMSS,trabajadorId,anoPago,mesPago")] Pago pago)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pago).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.resumenPagoId = new SelectList(db.ResumenPagoes, "id", "ip", pago.resumenPagoId);
            return View(pago);
        }

        // GET: Pagos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pago pago = db.Pagos.Find(id);
            if (pago == null)
            {
                return HttpNotFound();
            }
            return View(pago);
        }

        // POST: Pagos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Pago pago = db.Pagos.Find(id);
            db.Pagos.Remove(pago);
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
