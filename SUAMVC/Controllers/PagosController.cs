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
using System.Data.Entity.Validation;
using System.Text;

namespace SUAMVC.Controllers
{
    public class PagosController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: Pagos
        public ActionResult Index(String plazasId, String patronesId, String periodoId, String ejercicioId)
        {
            var pagos = db.Pagos.ToList();

            if (!String.IsNullOrEmpty(plazasId)) {
                int plazaTempId = int.Parse(plazasId.Trim());
                pagos = pagos.Where(s => s.Patrone.Plaza_id.Equals(plazaTempId)).ToList();
            }
            if (!String.IsNullOrEmpty(patronesId))
            {
                int patronesTempId = int.Parse(patronesId);
                pagos = pagos.Where(s => s.Patrone.Id.Equals(patronesTempId)).ToList();
            }
            if (!String.IsNullOrEmpty(periodoId))
            {
                pagos = pagos.Where(s => s.mes.Trim().Equals(periodoId.Trim())).ToList();
            }
            if (!String.IsNullOrEmpty(ejercicioId))
            {
                pagos = pagos.Where(s => s.anno.Trim().Equals(ejercicioId)).ToList();
            }

            pagos = pagos.OrderBy(p => p.Patrone.registro).ToList();

            return View(pagos.ToList());
        }

        public ActionResult UploadPagos()
        {
            return View();
        }

        /**
         * Se cargan los pagos por periodo y patron via carga masiva desde el SUA.mdb
         */
        [HttpPost]
        public ActionResult Upload(String patronesId, String periodoId, String ejercicioId)
        {
            if (!String.IsNullOrEmpty(patronesId) && !String.IsNullOrEmpty(periodoId) && !String.IsNullOrEmpty(ejercicioId))
            {
                String periodo = ejercicioId.Trim() + periodoId.Trim();
                int mes = int.Parse(periodoId.Trim());

                int patronTemp = int.Parse(patronesId);
                Patrone patron = db.Patrones.Find(patronTemp);
                String path = this.UploadFile(patron.direccionArchivo);

                bool esBimestral = ((mes % 2) == 0);

                if (!path.Equals(""))
                {
                    Boolean existe = false;
                    SUAHelper suaHelper = new SUAHelper(path);

                    String sSQL = "SELECT * FROM RESUMEN" +
                               "  WHERE Reg_Patr = '" + patron.registro + "'" +
                               "    AND Mes_Ano = '" + periodo + "'" +
                               "   ORDER BY Reg_Patr";

                    DataTable dt2 = suaHelper.ejecutarSQL(sSQL);

                    foreach (DataRow rows in dt2.Rows)
                    {
                        Pago pago = new Pago();

                        pago.mes = periodoId;
                        pago.anno = ejercicioId;

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
                           "    AND Periodo = '" + periodo + "'";

                        DataTable dt3 = suaHelper.ejecutarSQL(sSQL);

                        foreach (DataRow rows1 in dt3.Rows)
                        {
                            pago.nt = int.Parse(rows1[0].ToString());
                        }

                        if (pago.nt == 0) {
                            break;
                        }

                        pago.patronId = patron.Id;
                        pago.Patrone = patron;
                        pago.fechaCreacion = DateTime.Now;
                        pago.usuarioId = 1;


                        //Guardamos el pago.
                        db.Pagos.Add(pago);
                        db.SaveChanges();
                        existe = true;

                        if (existe)
                        {

                            //Preparamos el query del resúmen
                            sSQL = "SELECT * FROM RELTRA " +
                                   "  WHERE Reg_Pat = '" + patron.registro + "'" +
                                   "    AND Periodo = '" + periodo + "'" +
                                   "   ORDER BY Reg_Pat";

                            DataTable dt4 = suaHelper.ejecutarSQL(sSQL);

                            foreach (DataRow row2 in dt4.Rows)
                            {

                                DetallePago detallePago = new DetallePago();
                                String nss = row2["Num_Afi"].ToString().Trim();

                                Asegurado asegurado = db.Asegurados.Where(a => a.numeroAfiliacion.Equals(nss.Trim())).FirstOrDefault();

                                detallePago.pagoId = pago.id;
                                detallePago.Pago = pago;
                                detallePago.aseguradoId = asegurado.id;
                                detallePago.Asegurado = asegurado;
                                detallePago.patronId = patron.Id;
                                detallePago.Patrone = patron;

                                detallePago.diasCotizados = int.Parse(row2["dia_cot"].ToString().Trim());
                                detallePago.sdi = decimal.Parse(row2["sal_dia"].ToString().Trim());

                                if (String.IsNullOrEmpty(row2["Dia_Inc"].ToString()))
                                {
                                    detallePago.diasIncapacidad = 0;
                                }
                                else
                                {
                                    detallePago.diasIncapacidad = int.Parse(row2["Dia_Inc"].ToString().Trim());
                                }

                                if (String.IsNullOrEmpty(row2["Dia_Aus"].ToString()))
                                {
                                    detallePago.diasAusentismo = 0;
                                }
                                else
                                {
                                    detallePago.diasAusentismo = int.Parse(row2["Dia_Aus"].ToString().Trim());
                                }

                                if (String.IsNullOrEmpty(row2["CF"].ToString()))
                                {
                                    detallePago.cuotaFija = 0;
                                }
                                else
                                {
                                    detallePago.cuotaFija = decimal.Parse(row2["CF"].ToString().Trim());
                                }

                                if (String.IsNullOrEmpty(row2["EXPA"].ToString()))
                                {
                                    detallePago.expa = 0;
                                }
                                else
                                {
                                    detallePago.expa = decimal.Parse(row2["EXPA"].ToString().Trim());
                                }

                                if (String.IsNullOrEmpty(row2["EXO"].ToString()))
                                {
                                    detallePago.exO = 0;
                                }
                                else
                                {
                                    detallePago.exO = decimal.Parse(row2["EXO"].ToString().Trim());
                                }

                                if (String.IsNullOrEmpty(row2["PDP"].ToString()))
                                {
                                    detallePago.pdp = 0;
                                }
                                else
                                {
                                    detallePago.pdp = decimal.Parse(row2["PDP"].ToString().Trim());
                                }

                                if (String.IsNullOrEmpty(row2["PDO"].ToString()))
                                {
                                    detallePago.pdo = 0;
                                }
                                else
                                {
                                    detallePago.pdo = decimal.Parse(row2["PDO"].ToString().Trim());
                                }

                                if (String.IsNullOrEmpty(row2["GMPP"].ToString()))
                                {
                                    detallePago.gmpp = 0;
                                }
                                else
                                {
                                    detallePago.gmpp = decimal.Parse(row2["GMPP"].ToString().Trim());
                                }

                                if (String.IsNullOrEmpty(row2["GMPO"].ToString()))
                                {
                                    detallePago.gmpo = 0;
                                }
                                else
                                {
                                    detallePago.gmpo = decimal.Parse(row2["GMPO"].ToString().Trim());
                                }

                                if (String.IsNullOrEmpty(row2["RT"].ToString()))
                                {
                                    detallePago.rt = 0;
                                }
                                else
                                {
                                    detallePago.rt = decimal.Parse(row2["RT"].ToString().Trim());
                                }

                                if (String.IsNullOrEmpty(row2["IVP"].ToString()))
                                {
                                    detallePago.ivp = 0;
                                }
                                else
                                {
                                    detallePago.ivp = decimal.Parse(row2["IVP"].ToString().Trim());
                                }

                                if (String.IsNullOrEmpty(row2["IVO"].ToString()))
                                {
                                    detallePago.ivo = 0;
                                }
                                else
                                {
                                    detallePago.ivo = decimal.Parse(row2["IVO"].ToString().Trim());
                                }

                                if (String.IsNullOrEmpty(row2["GPS"].ToString()))
                                {
                                    detallePago.gps = 0;
                                }
                                else
                                {
                                    detallePago.gps = decimal.Parse(row2["GPS"].ToString().Trim());
                                }


                                detallePago.patronal = detallePago.cuotaFija + detallePago.expa + detallePago.pdp + detallePago.gmpp + detallePago.rt + detallePago.ivp + detallePago.gps;
                                detallePago.obrera = detallePago.exO + detallePago.pdo + detallePago.gmpo + detallePago.ivo;
                                detallePago.imss = detallePago.patronal + detallePago.obrera;

                                if (esBimestral)
                                {

                                    // Se guardan los datos bimestrales.
                                    sSQL = "SELECT * FROM RELTRABIM " +
                                           "  WHERE Reg_Pat = '" + patron.registro + "'" +
                                           "    AND Periodo = '" + periodo + "'" +
                                           "    AND Num_Afi = '" + asegurado.numeroAfiliacion.Trim() + "'"  +
                                           "   ORDER BY Reg_Pat";

                                    DataTable dt5 = suaHelper.ejecutarSQL(sSQL);

                                    foreach (DataRow row5 in dt5.Rows)
                                    {

                                        if (String.IsNullOrEmpty(row5["Retiro"].ToString()))
                                        {
                                            detallePago.retiro = 0;
                                        }
                                        else
                                        {
                                            detallePago.retiro = decimal.Parse(row5["Retiro"].ToString().Trim());
                                        }
                                        if (String.IsNullOrEmpty(row5["CyVP"].ToString()))
                                        {
                                            detallePago.patronalBimestral = 0;
                                        }
                                        else
                                        {
                                            detallePago.patronalBimestral = decimal.Parse(row5["CyVP"].ToString().Trim());
                                        }
                                        if (String.IsNullOrEmpty(row5["CyVO"].ToString()))
                                        {
                                            detallePago.obreraBimestral = 0;
                                        }
                                        else
                                        {
                                            detallePago.obreraBimestral = decimal.Parse(row5["CyVO"].ToString().Trim());
                                        }

                                        detallePago.rcv = detallePago.retiro + detallePago.patronal + detallePago.obrera;

                                        if (String.IsNullOrEmpty(row5["Aportasc"].ToString()))
                                        {
                                            detallePago.aportacionsc = 0;
                                        }
                                        else
                                        {
                                            detallePago.aportacionsc = decimal.Parse(row5["Aportasc"].ToString().Trim());
                                        }
                                        if (String.IsNullOrEmpty(row5["Aportacc"].ToString()))
                                        {
                                            detallePago.aportacioncc = 0;
                                        }
                                        else
                                        {
                                            detallePago.aportacioncc = decimal.Parse(row5["Aportacc"].ToString().Trim());
                                        }
                                        if (String.IsNullOrEmpty(row5["Amortiza"].ToString()))
                                        {
                                            detallePago.amortizacion = 0;
                                        }
                                        else
                                        {
                                            detallePago.amortizacion = decimal.Parse(row5["Amortiza"].ToString().Trim());
                                        }

                                        detallePago.infonavit = detallePago.aportacionsc + detallePago.aportacioncc + detallePago.amortizacion;

                                    }
                                } //El periodo es bimestre



                                detallePago.usuarioId = 1;
                                detallePago.fechaCreacion = DateTime.Now;

                                try
                                {
                                    db.DetallePagoes.Add(detallePago);
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
                            }

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

                    if (!String.IsNullOrEmpty(subFolder))
                    {
                        path = Path.Combine(rutaParameter.valorString.Trim(), subFolder.Trim());
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

        [HttpGet]
        public ActionResult ResumenPagos(int id)
        {
            PagosResumenModel resumenPagoModel = new PagosResumenModel();

            Pago pago = db.Pagos.Where(p => p.id.Equals(id)).FirstOrDefault();

            List<DetallePago> detallePago = db.DetallePagoes.Where(r => r.pagoId.Equals(id)).ToList();

            resumenPagoModel.pago = pago;
            resumenPagoModel.detalle = detallePago;

            return View(resumenPagoModel);
        }

        public ActionResult actualizarPagos([Bind(Include = "id,bancoId,fechaDeposito")]Pago pago, String bancoId)
        {

            if(pago != null){
                int bancoTempId = int.Parse(bancoId); 
                Pago pagoTemp = db.Pagos.Find(pago.id);
                pagoTemp.fechaDeposito = pago.fechaDeposito;
                pagoTemp.bancoId = bancoTempId;
                db.Entry(pagoTemp).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        
        }

        public ActionResult UploadComprobantes(String id, String comprobanteId) {

            @ViewBag.id = id;
            @ViewBag.comprobanteId = comprobanteId;

            return View();
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
