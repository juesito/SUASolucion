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
        public ActionResult Index()
        {
            var pagos = db.Pagos.ToList();
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
                int patronTemp = int.Parse(patronesId);
                Patrone patron = db.Patrones.Find(patronTemp);
                String path = this.UploadFile(patron.direccionArchivo);


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
                            sSQL = "SELECT * FROM Registro_02" +
                                "  WHERE Registro_Patronal = '" + patron.registro + "'" +
                                "    AND Periodo_Pago = '" + periodo + "'" +
                                "ORDER BY Registro_Patronal";

                            DataTable dt = suaHelper.ejecutarSQL(sSQL);

                            foreach (DataRow row in dt.Rows)
                            {
                                ResumenPago resumenPago = new ResumenPago();
                                resumenPago.ip = row["IP"].ToString().Trim();
                                resumenPago.patronId = patron.Id;
                                resumenPago.rfc = row["RFC"].ToString().Trim();
                                resumenPago.periodoPago = periodo;
                                resumenPago.mes = periodoId;
                                resumenPago.anno = ejercicioId;
                                resumenPago.folioSUA = row["Folio_SUA"].ToString().Trim();
                                resumenPago.razonSocial = row["Razon_Social"].ToString().Trim();
                                resumenPago.calleColonia = row["Calle_Colonia"].ToString().Trim();
                                resumenPago.poblacion = row["Poblacion"].ToString().Trim();
                                resumenPago.entidadFederativa = row["Entidad_Federativa"].ToString().Trim();
                                resumenPago.codigoPostal = row["CP"].ToString().Trim();
                                resumenPago.primaRT = row["Prima_RT"].ToString().Trim();
                                resumenPago.fechaPrimaRT = row["Fecha_Prima_RT"].ToString().Trim();
                                resumenPago.actividadEconomica = row["Actividad_Economica"].ToString().Trim();
                                resumenPago.delegacionIMSS = row["Delegacion_IMSS"].ToString().Trim();
                                resumenPago.subDelegacionIMMS = row["SubDelegacion_IMSS"].ToString().Trim();
                                resumenPago.zonaEconomica = row["Zona_Economica"].ToString().Trim();
                                resumenPago.convenioReembolso = row["Convenio_Rembolso"].ToString().Trim();
                                resumenPago.tipoCotizacion = row["Tipo_Cotizacion"].ToString().Trim();
                                resumenPago.cotizantes = row["Cotizantes"].ToString().Trim();
                                resumenPago.apoPat = row["Apo_Pat"].ToString().Trim();
                                resumenPago.delSubDel = row["Del_Subdel"].ToString().Trim();
                                resumenPago.fechaCreacion = DateTime.Now;
                                resumenPago.pagoId = pago.id;
                                resumenPago.Pago = pago;

                                //Cambiar por el usuario registrado
                                resumenPago.usuarioCreacionId = 1;

                                db.ResumenPagoes.Add(resumenPago);
                                db.SaveChanges();

                                //Preparamos el query del resúmen
                                sSQL = "SELECT * FROM Registro_03";

                                DataTable dt4 = suaHelper.ejecutarSQL(sSQL);

                                foreach (DataRow row2 in dt4.Rows)
                                {

                                    DetallePago detallePago = new DetallePago();
                                    detallePago.periodo = periodo;
                                    detallePago.pagoId = pago.id;
                                    detallePago.Pago = pago;
                                    detallePago.nss = row2["nss"].ToString().Trim();
                                    detallePago.rfc = row2["rfc"].ToString().Trim();
                                    detallePago.creditoInfonavit = row2["credito_infonavit"].ToString().Trim();
                                    detallePago.fid = row2["fid"].ToString().Trim();
                                    detallePago.trabajador = row2["trabajador"].ToString().Trim();
                                    detallePago.sdi = row2["sdi"].ToString().Trim();
                                    detallePago.tipoTrabajador = row2["tipo_trabajador"].ToString().Trim();
                                    detallePago.jornadaSemanaReducida = row2["jornada_semana_reducida"].ToString().Trim();
                                    detallePago.diasCotizadosMes = row2["dias_Cotizados_Mes"].ToString().Trim();
                                    detallePago.diasIncapacidad = row2["dias_Incapacidad"].ToString().Trim();
                                    detallePago.diasAusentismo = row2["dias_Ausentismo"].ToString().Trim();
                                    detallePago.cuotaFija = decimal.Parse(row2["cuota_fija"].ToString().Trim());
                                    detallePago.cuotaExcendente = decimal.Parse(row2["Cuota_Excedente"].ToString().Trim());
                                    detallePago.prestacionesDinero = decimal.Parse(row2["prestaciones_Dinero"].ToString().Trim());
                                    detallePago.gastosMedicosPensionados = decimal.Parse(row2["gastos_Medicos_Pensionados"].ToString().Trim());
                                    detallePago.riesgoTrabajo = decimal.Parse(row2["riesgo_Trabajo"].ToString().Trim());
                                    detallePago.invalidezVida = decimal.Parse(row2["invalidez_Vida"].ToString().Trim());
                                    detallePago.guarderias = int.Parse(row2["guarderias"].ToString().Trim());
                                    detallePago.actRecargosImss = decimal.Parse(row2["act_Recargos_Imss"].ToString().Trim());
                                    detallePago.diasCotizadosBimestre = row2["dias_Cotizados_Bimestre"].ToString().Trim();
                                    detallePago.diasIncapacidadBimestre = row2["dias_Incapacidad_Bim"].ToString().Trim();
                                    detallePago.diasAusentismoBimestre = row2["dias_Ausentismo_Bim"].ToString().Trim();
                                    detallePago.retiro = int.Parse(row2["retiro"].ToString().Trim());
                                    detallePago.actRecargosRetiro = decimal.Parse(row2["act_Recargos_Retiro"].ToString().Trim());
                                    detallePago.cesantiaVejezPatronal = decimal.Parse(row2["Cesantia_Vejez_Patronal"].ToString().Trim());
                                    detallePago.cesantiaVejezObrera = decimal.Parse(row2["Cesantia_Vejez_Obrera"].ToString().Trim());
                                    detallePago.actRecargosCyV = decimal.Parse(row2["Act_Recargos_CyV"].ToString().Trim());
                                    detallePago.aportacionVoluntaria = decimal.Parse(row2["Aportacion_Voluntaria"].ToString().Trim());
                                    if (!String.IsNullOrEmpty(row2["Aportacion_Comp"].ToString()))
                                    {
                                        detallePago.aportacionComp = decimal.Parse(row2["Aportacion_Comp"].ToString().Trim());
                                    }
                                    detallePago.aportacionPatronal = decimal.Parse(row2["Aportacion_Patronal"].ToString().Trim());
                                    detallePago.amortizacion = decimal.Parse(row2["Amortizacion"].ToString().Trim());
                                    detallePago.actImss = row2["Act_IMSS"].ToString().Trim();
                                    detallePago.recImss = row2["Rec_IMSS"].ToString().Trim();
                                    detallePago.actRetiro = row2["Act_Retiro"].ToString().Trim();
                                    detallePago.recRetiro = row2["Rec_Retiro"].ToString().Trim();
                                    detallePago.actCesPat = row2["Act_CesPat"].ToString().Trim();
                                    detallePago.recCesPat = row2["Rec_CesPat"].ToString().Trim();
                                    detallePago.actCesObr = row2["Act_CesObr"].ToString().Trim();
                                    detallePago.recCesObr = row2["Rec_CesObr"].ToString().Trim();
                                    detallePago.cuotaExcObr = int.Parse(row2["Cuota_ExcObr"].ToString().Trim());
                                    if (!String.IsNullOrEmpty(row2["Cuota_PdObr"].ToString()))
                                    {
                                        detallePago.cuotaPdObr = decimal.Parse(row2["Cuota_PdObr"].ToString().Trim());
                                    }
                                    detallePago.cuotaGmpObr = decimal.Parse(row2["Cuota_GmpObr"].ToString().Trim());
                                    detallePago.cuotaIvObr = decimal.Parse(row2["Cuota_IvObr"].ToString().Trim());
                                    detallePago.actPatImss = row2["ActPat_IMSS"].ToString().Trim();
                                    detallePago.recPatImss = row2["RecPat_IMSS"].ToString().Trim();
                                    detallePago.actObrImss = row2["ActObr_IMSS"].ToString().Trim();
                                    detallePago.recObrImss = row2["RecObr_IMSS"].ToString().Trim();
                                    detallePago.usuarioId = 1;
                                    detallePago.fechaCreacion = DateTime.Now;

                                    try
                                    {
                                        db.DetallePagos.Add(detallePago);
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

            List<DetallePago> detallePago = db.DetallePagos.Where(r => r.pagoId.Equals(id)).ToList();

            resumenPagoModel.pago = pago;
            resumenPagoModel.detalle = detallePago;

            return View(resumenPagoModel);
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
