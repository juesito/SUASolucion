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
using SUAMVC.Helpers;
using System.Web.Helpers;
using Ionic.Zip;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Microsoft;
using System.Windows;
using SUAMVC.Code52.i18n;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using System.Data.OleDb;


namespace SUAMVC.Controllers
{
    public class PagosController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: Pagos
        public ActionResult Index(String plazasId, String patronesId, String periodoId, String ejercicioId)
        {

            PagosModel pagosModel = new PagosModel();

            Usuario user = Session["UsuarioData"] as Usuario;

            var plazasAsignadas = (from x in db.TopicosUsuarios
                                   where x.usuarioId.Equals(user.Id)
                                   && x.tipo.Equals("P")
                                   select x.topicoId);

            var patronesAsignados = (from x in db.TopicosUsuarios
                                     where x.usuarioId.Equals(user.Id)
                                     && x.tipo.Equals("B")
                                     select x.topicoId);

            //Query principal
            var pagos = from s in db.Pagos
                        //                                     where plazasAsignadas.Contains(s.Patrone.Plaza_id) &&
                        where patronesAsignados.Contains(s.patronId)
                        select s;

            if (!String.IsNullOrEmpty(plazasId))
            {
                int plazaTempId = int.Parse(plazasId.Trim());
                pagos = pagos.Where(s => s.Patrone.Plaza_id.Equals(plazaTempId));
            }
            if (!String.IsNullOrEmpty(patronesId))
            {
                int patronesTempId = int.Parse(patronesId);
                pagos = pagos.Where(s => s.Patrone.Id.Equals(patronesTempId));
            }
            if (!String.IsNullOrEmpty(periodoId))
            {
                pagos = pagos.Where(s => s.mes.Trim().Equals(periodoId.Trim()));
            }
            if (!String.IsNullOrEmpty(ejercicioId))
            {
                pagos = pagos.Where(s => s.anno.Trim().Equals(ejercicioId));
            }

            pagos = pagos.OrderBy(p => p.Patrone.registro);

            pagosModel.pagos = pagos.ToList();
            PagosFooter sa = new PagosFooter();

            foreach (Pago sc in pagos)
            {
                sa.sumImss = sa.sumImss + System.Convert.ToDouble(sc.imss);
                sa.sumRcv = sa.sumRcv + System.Convert.ToDouble(sc.rcv);
                sa.sumInfonavit = sa.sumInfonavit + System.Convert.ToDouble(sc.infonavit);
                sa.sumTotal = sa.sumTotal + System.Convert.ToDouble(sc.total);
                sa.sumNt = sa.sumNt + System.Convert.ToDouble(sc.nt);
                sa.sumRecargos = sa.sumRecargos + System.Convert.ToDouble(sc.recargos);
                sa.sumActualiz = sa.sumActualiz + System.Convert.ToDouble(sc.actualizaciones);
                sa.sumGTotal = sa.sumGTotal + System.Convert.ToDouble(sc.granTotal);
            }

            pagosModel.pagosFooter = sa;

            return View(pagosModel);

        }

        public ActionResult UploadPagos()
        {
            return View();
        }

        /**
         * Se cargan los pagos por periodo y patron via carga masiva desde el SUA.mdb
         */
        [HttpPost]
        public ActionResult Upload(String patronesId, String periodoId, String ejercicioId, String usuarioId)
        {
            if (!String.IsNullOrEmpty(patronesId) && !String.IsNullOrEmpty(periodoId) && !String.IsNullOrEmpty(ejercicioId)
                && !String.IsNullOrEmpty(usuarioId))
            {
                int userId = int.Parse(usuarioId.Trim());
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

                    String sSQL = "SELECT COUNT(*) FROM RESUMEN" +
                               "  WHERE Reg_Patr = '" + patron.registro + "'" +
                               "    AND Mes_Ano = '" + periodo + "'";

                    DataTable dt3 = suaHelper.ejecutarSQL(sSQL);

                    int registros = 0;

                    foreach (DataRow rows1 in dt3.Rows)
                    {
                        registros = int.Parse(rows1[0].ToString());
                    }

                    if (registros == 0)
                    {
                        ViewBag.dbUploaded = true;
                        TempData["error"] = true;
                        TempData["viewMessage"] = "No hay datos para el periodo seleccionado!";
                    }
                    else
                    {
                        sSQL = "SELECT * FROM RESUMEN" +
                                   "  WHERE Reg_Patr = '" + patron.registro + "'" +
                                   "    AND Mes_Ano = '" + periodo + "'" +
                                   "   ORDER BY Reg_Patr";

                        DataTable dt2 = suaHelper.ejecutarSQL(sSQL);

                        foreach (DataRow rows in dt2.Rows)
                        {
                            Pago pago = new Pago();
                            pago = db.Pagos.Where(p => p.patronId.Equals(patron.Id) && p.mes.Trim().Equals(periodoId.Trim()) && p.anno.Trim().Equals(ejercicioId.Trim())).FirstOrDefault();
                            Boolean actualizar = false;

                            if (pago != null)
                            {
                                actualizar = true;
                            }
                            else
                            {
                                pago = new Pago();
                                pago.mes = periodoId;
                                pago.anno = ejercicioId;
                            }

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

                            pago.granTotal = pago.imss + pago.rcv + pago.infonavit + pago.recargos + pago.actualizaciones;

                            int newMes = mes / 2;
                            String newPeriodo = ejercicioId.Trim() + "0" + newMes.ToString();
                            if (esBimestral)
                            {
                                sSQL = "SELECT COUNT(*) FROM (SELECT DISTINCT Num_Afi FROM RELTRABIM" +
                               "  WHERE Reg_Pat = '" + patron.registro + "'" +
                               "    AND Periodo = '" + newPeriodo + "')";
                            }
                            else
                            {

                                sSQL = "SELECT COUNT(*) FROM (SELECT DISTINCT Num_Afi FROM RELTRA" +
                               "  WHERE Reg_Pat = '" + patron.registro + "'" +
                               "    AND Periodo = '" + periodo + "')";
                            }

                            dt3 = suaHelper.ejecutarSQL(sSQL);

                            foreach (DataRow rows1 in dt3.Rows)
                            {
                                pago.nt = int.Parse(rows1[0].ToString());
                            }

                            if (pago.nt == 0)
                            {
                                ViewBag.dbUploaded = true;
                                TempData["error"] = true;
                                TempData["viewMessage"] = "No hay datos para el periodo seleccionado!";
                                break;
                            }

                            pago.patronId = patron.Id;
                            pago.Patrone = patron;
                            pago.fechaCreacion = DateTime.Now;
                            pago.usuarioId = userId;

                            //                       Guardamos el pago.
                            if (actualizar)
                            {
                                db.Entry(pago).State = EntityState.Modified;
                            }
                            else
                            {
                                db.Pagos.Add(pago);
                            }
                            db.SaveChanges();
                            existe = true;

                            if (existe)
                            {

                                List<DetallePago> listDetalle = new List<DetallePago>();

                                var detalle = from d in db.DetallePagoes
                                              where d.pagoId.Equals(pago.id)
                                              select d;
                                listDetalle = detalle.ToList();

                                foreach (DetallePago detFor in listDetalle)
                                {
                                    DetallePago det = new DetallePago();
                                    det = detFor;
                                    db.DetallePagoes.Remove(det);
                                    db.SaveChanges();
                                }

                                sSQL = "SELECT * FROM RELTRA " +
                                       "  WHERE Reg_Pat = '" + patron.registro + "'" +
                                       "   AND Periodo = '" + periodo + "'" +
                                       "   ORDER BY Num_Afi ";

                                DataTable dt6 = suaHelper.ejecutarSQL(sSQL);
                                String nss2 = "0";

                                foreach (DataRow row2 in dt6.Rows)
                                {
                                    String nss = row2["Num_Afi"].ToString().Trim();
                                    DetallePago detallePago = new DetallePago();
                                    Asegurado asegurado = db.Asegurados.Where(a => a.numeroAfiliacion.Equals(nss.Trim()) && a.PatroneId.Equals(patron.Id)).FirstOrDefault();

                                    if (asegurado != null)
                                    {
                                        detallePago = new DetallePago();
                                        detallePago.pagoId = pago.id;
                                        detallePago.Pago = pago;
                                        detallePago.aseguradoId = asegurado.id;
                                        detallePago.Asegurado = asegurado;
                                        detallePago.patronId = patron.Id;
                                        detallePago.Patrone = patron;
                                        detallePago.cuotaFija = 0;
                                        detallePago.expa = 0;
                                        detallePago.pdp = 0;
                                        detallePago.gmpp = 0;
                                        detallePago.rt = 0;
                                        detallePago.ivp = 0;
                                        detallePago.gps = 0;
                                        detallePago.obrera = 0;
                                        detallePago.exO = 0;
                                        detallePago.pdo = 0;
                                        detallePago.gmpo = 0;
                                        detallePago.ivo = 0;
                                        detallePago.rcv = 0;
                                        detallePago.infonavit = 0;

                                        detallePago.diasCotizados = int.Parse(row2["dia_cot"].ToString().Trim());
                                        if (detallePago.diasCotizados != 0)
                                        {
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
                                            detallePago.usuarioId = userId;
                                            detallePago.fechaCreacion = DateTime.Now;
                                            detallePago.retiro = 0;
                                            detallePago.patronalBimestral = 0;
                                            detallePago.obreraBimestral = 0;
                                            detallePago.aportacionsc = 0;
                                            detallePago.aportacioncc = 0;
                                            detallePago.amortizacion = 0;
                                            detallePago.infonavit = 0;
                                            detallePago.diasCotizBim = 0;
                                            detallePago.rcv = 0;

                                            if (nss != nss2 && esBimestral)
                                            {

                                                // Se guardan los datos bimestrales.
                                                sSQL = "SELECT * FROM RELTRABIM " +
                                                       "  WHERE Reg_Pat = '" + patron.registro + "'" +
                                                       "    AND Periodo = '" + newPeriodo + "'" +
                                                       "    AND Num_Afi = '" + nss + "'";

                                                DataTable dt5 = suaHelper.ejecutarSQL(sSQL);

                                                foreach (DataRow row5 in dt5.Rows)
                                                {

                                                    if (String.IsNullOrEmpty(row5["dia_cot"].ToString()))
                                                    {
                                                        detallePago.diasCotizBim = detallePago.diasCotizBim + 0;
                                                    }
                                                    else
                                                    {
                                                        detallePago.diasCotizBim = detallePago.diasCotizBim + int.Parse(row5["dia_cot"].ToString().Trim());
                                                    }

                                                    if (String.IsNullOrEmpty(row5["Retiro"].ToString()))
                                                    {
                                                        detallePago.retiro = detallePago.retiro + 0;
                                                    }
                                                    else
                                                    {
                                                        detallePago.retiro = detallePago.retiro + decimal.Parse(row5["Retiro"].ToString().Trim());
                                                    }
                                                    if (String.IsNullOrEmpty(row5["CyVP"].ToString()))
                                                    {
                                                        detallePago.patronalBimestral = detallePago.patronalBimestral + 0;
                                                    }
                                                    else
                                                    {
                                                        detallePago.patronalBimestral = detallePago.patronalBimestral + decimal.Parse(row5["CyVP"].ToString().Trim());
                                                    }
                                                    if (String.IsNullOrEmpty(row5["CyVO"].ToString()))
                                                    {
                                                        detallePago.obreraBimestral = detallePago.obreraBimestral + 0;
                                                    }
                                                    else
                                                    {
                                                        detallePago.obreraBimestral = detallePago.obreraBimestral + decimal.Parse(row5["CyVO"].ToString().Trim());
                                                    }


                                                    if (String.IsNullOrEmpty(row5["Aportasc"].ToString()))
                                                    {
                                                        detallePago.aportacionsc = detallePago.aportacionsc + 0;
                                                    }
                                                    else
                                                    {
                                                        detallePago.aportacionsc = detallePago.aportacionsc + decimal.Parse(row5["Aportasc"].ToString().Trim());
                                                    }
                                                    if (String.IsNullOrEmpty(row5["Aportacc"].ToString()))
                                                    {
                                                        detallePago.aportacioncc = detallePago.aportacioncc + 0;
                                                    }
                                                    else
                                                    {
                                                        detallePago.aportacioncc = detallePago.aportacioncc + decimal.Parse(row5["Aportacc"].ToString().Trim());
                                                    }
                                                    if (String.IsNullOrEmpty(row5["Amortiza"].ToString()))
                                                    {
                                                        detallePago.amortizacion = detallePago.amortizacion + 0;
                                                    }
                                                    else
                                                    {
                                                        detallePago.amortizacion = detallePago.amortizacion + decimal.Parse(row5["Amortiza"].ToString().Trim());
                                                    }
                                                }
                                            }
                                            detallePago.infonavit = detallePago.aportacionsc + detallePago.aportacioncc + detallePago.amortizacion;
                                            detallePago.rcv = detallePago.retiro + detallePago.patronalBimestral + detallePago.obreraBimestral;
                                            detallePago.total = detallePago.imss + detallePago.rcv + detallePago.infonavit;

                                            try
                                            {
                                                nss2 = nss;
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
                                    } // else de asegurado !=null
                                    else
                                    {
                                        TempData["error"] = true;
                                        TempData["viewMessage"] = "No se terminó el proceso. Asegurados no actualizados. Favor de verificar...";
                                        break;
                                    }
                                }
                                if (esBimestral)
                                {
                                    sSQL = "SELECT * FROM RELTRABIM " +
                                          "  WHERE Reg_Pat = '" + patron.registro + "'" +
                                          "   AND Periodo = '" + newPeriodo + "'" +
                                          "   AND Num_Afi NOT IN (SELECT Num_Afi FROM RELTRA) " +
                                          "   ORDER BY Num_Afi ";

                                    DataTable dt7 = suaHelper.ejecutarSQL(sSQL);
                                    nss2 = "0";
                                    DetallePago detallePago2 = new DetallePago();

                                    foreach (DataRow row2 in dt7.Rows)
                                    {
                                        String nss = row2["Num_Afi"].ToString().Trim();
                                        Asegurado asegurado = db.Asegurados.Where(a => a.numeroAfiliacion.Equals(nss.Trim()) && a.PatroneId.Equals(patron.Id)).FirstOrDefault();

                                        if (asegurado != null)
                                        {
                                            if (nss.Equals(nss2))
                                            {
                                                if (String.IsNullOrEmpty(row2["dia_cot"].ToString()))
                                                {
                                                    detallePago2.diasCotizBim = detallePago2.diasCotizBim + 0;
                                                }
                                                else
                                                {
                                                    detallePago2.diasCotizBim = detallePago2.diasCotizBim + int.Parse(row2["dia_cot"].ToString().Trim());
                                                }
                                                if (String.IsNullOrEmpty(row2["Retiro"].ToString()))
                                                {
                                                    detallePago2.retiro = detallePago2.retiro + 0;
                                                }
                                                else
                                                {
                                                    detallePago2.retiro = detallePago2.retiro + decimal.Parse(row2["Retiro"].ToString().Trim());
                                                }
                                                if (String.IsNullOrEmpty(row2["CyVP"].ToString()))
                                                {
                                                    detallePago2.patronalBimestral = detallePago2.patronalBimestral + 0;
                                                }
                                                else
                                                {
                                                    detallePago2.patronalBimestral = detallePago2.patronalBimestral + decimal.Parse(row2["CyVP"].ToString().Trim());
                                                }
                                                if (String.IsNullOrEmpty(row2["CyVO"].ToString()))
                                                {
                                                    detallePago2.obreraBimestral = detallePago2.obreraBimestral + 0;
                                                }
                                                else
                                                {
                                                    detallePago2.obreraBimestral = detallePago2.obreraBimestral + decimal.Parse(row2["CyVO"].ToString().Trim());
                                                }


                                                if (String.IsNullOrEmpty(row2["Aportasc"].ToString()))
                                                {
                                                    detallePago2.aportacionsc = detallePago2.aportacionsc + 0;
                                                }
                                                else
                                                {
                                                    detallePago2.aportacionsc = detallePago2.aportacionsc + decimal.Parse(row2["Aportasc"].ToString().Trim());
                                                }
                                                if (String.IsNullOrEmpty(row2["Aportacc"].ToString()))
                                                {
                                                    detallePago2.aportacioncc = detallePago2.aportacioncc + 0;
                                                }
                                                else
                                                {
                                                    detallePago2.aportacioncc = detallePago2.aportacioncc + decimal.Parse(row2["Aportacc"].ToString().Trim());
                                                }
                                                if (String.IsNullOrEmpty(row2["Amortiza"].ToString()))
                                                {
                                                    detallePago2.amortizacion = detallePago2.amortizacion + 0;
                                                }
                                                else
                                                {
                                                    detallePago2.amortizacion = detallePago2.amortizacion + decimal.Parse(row2["Amortiza"].ToString().Trim());
                                                }
                                            }
                                            else
                                            {
                                                if (!nss2.Equals("0"))
                                                {
                                                    try
                                                    {
                                                        detallePago2.rcv = detallePago2.retiro + detallePago2.patronalBimestral + detallePago2.obreraBimestral;
                                                        detallePago2.infonavit = detallePago2.aportacionsc + detallePago2.aportacioncc + detallePago2.amortizacion;
                                                        detallePago2.total = detallePago2.imss + detallePago2.rcv + detallePago2.infonavit;
                                                        detallePago2.fechaCreacion = DateTime.Now;
                                                        nss2 = nss;
                                                        db.DetallePagoes.Add(detallePago2);
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
                                                detallePago2 = new DetallePago();
                                                detallePago2.pagoId = pago.id;
                                                detallePago2.Pago = pago;
                                                detallePago2.aseguradoId = asegurado.id;
                                                detallePago2.Asegurado = asegurado;
                                                detallePago2.patronId = patron.Id;
                                                detallePago2.Patrone = patron;

                                                detallePago2.diasCotizados = 0;
                                                detallePago2.sdi = decimal.Parse(row2["sal_dia"].ToString().Trim());

                                                detallePago2.usuarioId = userId;
                                                detallePago2.fechaCreacion = DateTime.Now;
                                                detallePago2.retiro = 0;
                                                detallePago2.patronalBimestral = 0;
                                                detallePago2.obreraBimestral = 0;
                                                detallePago2.aportacionsc = 0;
                                                detallePago2.aportacioncc = 0;
                                                detallePago2.amortizacion = 0;
                                                detallePago2.infonavit = 0;
                                                detallePago2.diasCotizBim = 0;
                                                detallePago2.rcv = 0;
                                                detallePago2.imss = 0;

                                                if (!String.IsNullOrEmpty(row2["dia_cot"].ToString()))
                                                {
                                                    detallePago2.diasCotizBim = detallePago2.diasCotizBim + int.Parse(row2["dia_cot"].ToString().Trim());
                                                }
                                                if (!String.IsNullOrEmpty(row2["Retiro"].ToString()))
                                                {
                                                    detallePago2.retiro = decimal.Parse(row2["Retiro"].ToString().Trim());
                                                }
                                                if (!String.IsNullOrEmpty(row2["CyVP"].ToString()))
                                                {
                                                    detallePago2.patronalBimestral = decimal.Parse(row2["CyVP"].ToString().Trim());
                                                }
                                                if (!String.IsNullOrEmpty(row2["CyVO"].ToString()))
                                                {
                                                    detallePago2.obreraBimestral = decimal.Parse(row2["CyVO"].ToString().Trim());
                                                }
                                                if (!String.IsNullOrEmpty(row2["Aportasc"].ToString()))
                                                {
                                                    detallePago2.aportacionsc = decimal.Parse(row2["Aportasc"].ToString().Trim());
                                                }
                                                if (!String.IsNullOrEmpty(row2["Aportacc"].ToString()))
                                                {
                                                    detallePago2.aportacioncc = decimal.Parse(row2["Aportacc"].ToString().Trim());
                                                }
                                                if (!String.IsNullOrEmpty(row2["Amortiza"].ToString()))
                                                {
                                                    detallePago2.amortizacion = decimal.Parse(row2["Amortiza"].ToString().Trim());
                                                }

                                                nss2 = nss;
                                            }
                                        }
                                        else
                                        {
                                            TempData["error"] = true;
                                            TempData["viewMessage"] = "No se terminó el proceso. Asegurados no actualizados. Favor de verificar...";
                                            break;
                                        }
                                    }
                                    if (!nss2.Equals("0"))
                                    {
                                        try
                                        {
                                            detallePago2.rcv = detallePago2.retiro + detallePago2.patronalBimestral + detallePago2.obreraBimestral;
                                            detallePago2.infonavit = detallePago2.aportacionsc + detallePago2.aportacioncc + detallePago2.amortizacion;
                                            detallePago2.total = detallePago2.imss + detallePago2.rcv + detallePago2.infonavit;
                                            db.DetallePagoes.Add(detallePago2);
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
                                var detPago = from s in db.DetallePagoes
                                              where s.pagoId.Equals(pago.id)
                                              select s;

                                var nt = detPago.GroupBy(x => x.aseguradoId).Count();

                                pago.nt = nt;
                                db.Entry(pago).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                    }
                    suaHelper.cerrarConexion();
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
                    ZipFile zip = ZipFile.Read(pathFinal);
                    zip.ExtractAll(path, ExtractExistingFileAction.OverwriteSilently);
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

            if (pago != null)
            {
                int bancoTempId = int.Parse(bancoId);
                Pago pagoTemp = db.Pagos.Find(pago.id);
                pagoTemp.fechaDeposito = pago.fechaDeposito;
                pagoTemp.bancoId = bancoTempId;
                db.Entry(pagoTemp).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");

        }

        public ActionResult UploadComprobantes(String id, String comprobanteId)
        {

            @ViewBag.id = id;
            @ViewBag.comprobanteId = comprobanteId;

            return View();
        }

        public ActionResult GuardarComprobantes(String id, String comprobanteId)
        {

            if (!String.IsNullOrEmpty(id))
            {
                ToolsHelper th = new ToolsHelper();
                ParametrosHelper ph = new ParametrosHelper();

                Parametro parametro = ph.getParameterByKey("COMPRUTA");
                String destino = parametro.valorString.Trim() + comprobanteId.Trim();

                String fileName = th.cargarArchivo(Request.Files, destino);

                int idTemp = int.Parse(id.Trim());

                Pago pago = db.Pagos.Find(idTemp);
                if (comprobanteId.Trim().Equals("CP"))
                {
                    pago.comprobantePago = fileName.Trim();
                }
                else
                    if (comprobanteId.Trim().Equals("RL"))
                    {
                        pago.resumenLiquidacion = fileName.Trim();
                    }
                    else
                    {
                        pago.cedulaAutodeterminacion = fileName.Trim();
                    }

                db.Entry(pago).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public ActionResult VerComprobante(String fileNameString)
        {
            if (!String.IsNullOrEmpty(fileNameString))
            {

                ParametrosHelper ph = new ParametrosHelper();

                Parametro parametro = ph.getParameterByKey("COMPRUTA");
                Parametro rutaParameter = ph.getParameterByKey("SUARUTA");
                var fileName = rutaParameter.valorString.Trim() + parametro.valorString.Trim() + fileNameString.Trim();

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

        [HttpGet]
        public void ExcelDetalle(int id)
        {

            FileStream fileStream = null;
            MemoryStream mem = new MemoryStream();
            try
            {
                Pago pago = db.Pagos.Where(p => p.id.Equals(id)).FirstOrDefault();

            List<DetallePago> detallePago = db.DetallePagoes.Where(r => r.pagoId.Equals(id)).ToList();

            List<DetallePago> allCust = new List<DetallePago>();

            allCust = detallePago;

                DateTime date = DateTime.Now;
                String path = @"C:\\SUA\\Exceles\\";
                String fileName = @"DetallePagos-" + date.ToString("ddMMyyyyHHmm") + ".xlsx";
                String fullName = path + fileName;

                ExcelHelper eh = new ExcelHelper();
                //Creamos el objeto del workbook
                SpreadsheetDocument xl = SpreadsheetDocument.Create(fullName, SpreadsheetDocumentType.Workbook);

                WorkbookPart wbp = xl.AddWorkbookPart();
                WorksheetPart wsp = wbp.AddNewPart<WorksheetPart>();
                Workbook wb = new Workbook();
                FileVersion fv = new FileVersion();
                fv.ApplicationName = "Microsoft Office Excel";

                Worksheet ws = new Worksheet();
                WorkbookStylesPart wbsp = wbp.AddNewPart<WorkbookStylesPart>();
                // add styles to sheet
                wbsp.Stylesheet = eh.CreateStylesheet();
                wbsp.Stylesheet.Save();

                SheetData sd = crearContenidoHoja2(detallePago, eh);//CreateContentRow(); 
                ws.Append(sd);
                wsp.Worksheet = ws;
                wsp.Worksheet.Save();

                Sheets sheets = new Sheets();
                Sheet sheet = new Sheet();
                sheet.Name = "Sheet1";
                sheet.SheetId = 1;
                sheet.Id = wbp.GetIdOfPart(wsp);

                sheets.Append(sheet);
                wb.Append(fv);
                wb.Append(sheets);

                xl.WorkbookPart.Workbook = wb;
                xl.WorkbookPart.Workbook.Save();
                xl.Close();

                fileStream = new FileStream(fullName, FileMode.Open);
                fileStream.Position = 0;
                mem = new MemoryStream();
                fileStream.CopyTo(mem);

                mem.Position = 0;
                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
                ToolsHelper th = new ToolsHelper();
                Response.ContentType = th.getMimeType(fullName);
                Response.BinaryWrite(mem.ToArray());

                Response.End();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());

            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Flush();
                    fileStream.Close();
                }
                mem.Flush();
                mem.Close();
            }

        }


        string[] headerColumns2 = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD", "AE", "AF", "AG" };
        public SheetData crearContenidoHoja2(List<DetallePago> detallePago, ExcelHelper eh)
        {
            SheetData sheetData = new SheetData();
            int index = 1;

            //Creamos el Header
            Row row = new Row();
            row = eh.addNewCellToRow(index, row, "Patrón", headerColumns[0] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Periodo", headerColumns[1] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Ejercicio", headerColumns[2] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "NSS", headerColumns[3] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Nombre", headerColumns[4] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Ubicación", headerColumns[5] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Días", headerColumns[6] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "S.D.I.", headerColumns[7] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Inc.", headerColumns[8] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Aus.", headerColumns[9] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "C.F.", headerColumns[10] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Ex.P", headerColumns[11] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "EX. O", headerColumns[12] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "PDP", headerColumns[13] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "GMP. Patrón", headerColumns[14] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "GMP. Obrero", headerColumns[15] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "R. T.", headerColumns[16] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "I.V.P", headerColumns[17] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "I.V.O", headerColumns[18] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "G.P.S.", headerColumns[19] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Patronal", headerColumns[20] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Obrera", headerColumns[21] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "IMSS", headerColumns[22] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Dias cotizados Bim", headerColumns[23] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Retiro", headerColumns[24] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Patronal Bim", headerColumns[25] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Obrera Bim", headerColumns[26] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "R.C.V.", headerColumns[27] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Aportación SC", headerColumns[28] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Aportación CC", headerColumns[29] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Amortización", headerColumns[30] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Infonavit", headerColumns[31] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Total", headerColumns[32] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            index++;
            //Create the cells that contain the data.
            foreach (DetallePago dp in detallePago)
            {
                int i = 0;

                row = eh.addNewCellToRow(index, row, dp.Pago.Patrone.registro, headerColumns[i] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Pago.mes, headerColumns[i + 1] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Pago.anno, headerColumns[i + 2] + index, 2U, CellValues.Number);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Asegurado.numeroAfiliacion, headerColumns[i + 3] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Asegurado.nombreTemporal, headerColumns[i + 4] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Asegurado.Cliente.claveCliente, headerColumns[i + 5] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.diasCotizados.ToString(), headerColumns[i + 6] + index, 2U, CellValues.Number);
                sheetData.AppendChild(row);

                String var1 = String.Format("{0:###,###,##0.00}", dp.sdi);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 7] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.diasIncapacidad.ToString(), headerColumns[i + 8] + index, 2U, CellValues.Number);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.diasAusentismo.ToString(), headerColumns[i + 9] + index, 2U, CellValues.Number);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.cuotaFija);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 10] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.expa);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 11] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.exO);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 12] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.pdp);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 13] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.gmpp);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 14] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.gmpo);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 15] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.rt);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 16] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.ivp);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 17] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.ivo);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 18] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.gps);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 19] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.patronal);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 20] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.obrera);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 21] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.imss);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 22] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.diasCotizBim.ToString(), headerColumns[i + 23] + index, 2U, CellValues.Number);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.retiro);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 24] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.patronalBimestral);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 25] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.obreraBimestral);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 26] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.rcv);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 27] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.aportacionsc);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 28] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.aportacioncc);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 29] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.amortizacion);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 30] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.infonavit);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 31] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.total);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 32] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                index++;
            }

            return sheetData;
        }

        [HttpGet]
        public void GetExcel(String plazasId, String patronesId, String periodoId, String ejercicioId)
        {
            FileStream fileStream = null;
            MemoryStream mem = new MemoryStream();
            try
            {
                var pagos = db.Pagos.ToList();

            if (!String.IsNullOrEmpty(plazasId))
            {
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
            List<Pago> allCust = new List<Pago>();

            allCust = pagos.ToList();

                DateTime date = DateTime.Now;
                String path = @"C:\\SUA\\Exceles\\";
                String fileName = @"Pagos-" + date.ToString("ddMMyyyyHHmm") + ".xlsx";
                String fullName = path + fileName;

                ExcelHelper eh = new ExcelHelper();
                //Creamos el objeto del workbook
                SpreadsheetDocument xl = SpreadsheetDocument.Create(fullName, SpreadsheetDocumentType.Workbook);

                WorkbookPart wbp = xl.AddWorkbookPart();
                WorksheetPart wsp = wbp.AddNewPart<WorksheetPart>();
                Workbook wb = new Workbook();
                FileVersion fv = new FileVersion();
                fv.ApplicationName = "Microsoft Office Excel";

                Worksheet ws = new Worksheet();
                WorkbookStylesPart wbsp = wbp.AddNewPart<WorkbookStylesPart>();
                // add styles to sheet
                wbsp.Stylesheet = eh.CreateStylesheet();
                wbsp.Stylesheet.Save();

                SheetData sd = crearContenidoHoja(allCust, eh);//CreateContentRow(); 
                ws.Append(sd);
                wsp.Worksheet = ws;
                wsp.Worksheet.Save();

                Sheets sheets = new Sheets();
                Sheet sheet = new Sheet();
                sheet.Name = "Sheet1";
                sheet.SheetId = 1;
                sheet.Id = wbp.GetIdOfPart(wsp);

                sheets.Append(sheet);
                wb.Append(fv);
                wb.Append(sheets);

                xl.WorkbookPart.Workbook = wb;
                xl.WorkbookPart.Workbook.Save();
                xl.Close();

                fileStream = new FileStream(fullName, FileMode.Open);
                fileStream.Position = 0;
                mem = new MemoryStream();
                fileStream.CopyTo(mem);

                mem.Position = 0;
                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
                ToolsHelper th = new ToolsHelper();
                Response.ContentType = th.getMimeType(fullName);
                Response.BinaryWrite(mem.ToArray());

                Response.End();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());

            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Flush();
                    fileStream.Close();
                }
                mem.Flush();
                mem.Close();
            }

        }


        string[] headerColumns = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD", "AE", "AF", "AG" };
        public SheetData crearContenidoHoja(List<Pago> pagos, ExcelHelper eh)
        {
            SheetData sheetData = new SheetData();
            int index = 1;

            //Creamos el Header
            Row row = new Row();
            row = eh.addNewCellToRow(index, row, "Reg. Patronal", headerColumns[0] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "ID. Empresa", headerColumns[1] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Mes", headerColumns[2] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Año", headerColumns[3] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "IMSS", headerColumns[4] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "RCV", headerColumns[5] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Infonavit", headerColumns[6] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Total", headerColumns[7] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Recargos", headerColumns[8] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Actualizaciones", headerColumns[9] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Gran Total", headerColumns[10] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Banco", headerColumns[11] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "NT", headerColumns[12] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Localidad SUA", headerColumns[13] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            index++;
            //Create the cells that contain the data.
            foreach (Pago dp in pagos)
            {
                int i = 0;

                row = eh.addNewCellToRow(index, row, dp.Patrone.registro, headerColumns[i] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Patrone.nombre, headerColumns[i + 1] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.mes, headerColumns[i + 2] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.anno, headerColumns[i + 3] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                String var1 = String.Format("{0:###,###,##0.00}", dp.imss);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 4] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.rcv);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 5] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.infonavit);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 6] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.total );
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 7] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.recargos);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 8] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.actualizaciones);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 9] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.granTotal);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 10] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.Banco);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 11] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.nt);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 12] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Patrone.Plaza.descripcion, headerColumns[i + 13] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                index++;
            }

            return sheetData;
        }


        [HttpGet]
        public void OtroExcelDetalle(int id)
        {

            Pago pago = db.Pagos.Where(p => p.id.Equals(id)).FirstOrDefault();

            List<DetallePago> detallePago = db.DetallePagoes.Where(r => r.pagoId.Equals(id)).ToList();

            List<DetallePago> allCust = new List<DetallePago>();

            allCust = detallePago;

            WebGrid grid = new WebGrid(source: allCust, canPage: false, canSort: false);


            grid.Column("Asegurado.numeroAfiliacion", "NSS", format: (item) => String.Format("{0000000000}", item.Asegurado.numeroAfiliacion));
            grid.Column("Asegurado.nombreTemporal", "Nombre", null, null, true);

            string gridData = grid.GetHtml(
                columns: grid.Columns()
                    ).ToString();

            Response.ClearContent();
            DateTime date = DateTime.Now;
            String fileName = "DetallePagos-" + date.ToString("ddMMyyyyHHmm") + ".xls";
            //            string sStyle = @" .CssText { mso-number-format:\@; } ";
            Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
            Response.ContentType = "application/excel";
            Response.ClearContent();
            Response.Write(gridData);
            Response.End();
        }

        // GET: Pagos
        public ActionResult MantPDFPagos(String plazasId, String patronesId, String periodoId, String ejercicioId)
        {

            Usuario user = Session["UsuarioData"] as Usuario;

            var plazasAsignadas = (from x in db.TopicosUsuarios
                                   where x.usuarioId.Equals(user.Id)
                                   && x.tipo.Equals("P")
                                   select x.topicoId);

            var patronesAsignados = (from x in db.TopicosUsuarios
                                     where x.usuarioId.Equals(user.Id)
                                     && x.tipo.Equals("B")
                                     select x.topicoId);

            //Query principal
            var pagos = from s in db.Pagos
                        //                                     where plazasAsignadas.Contains(s.Patrone.Plaza_id) &&
                        where patronesAsignados.Contains(s.patronId)
                        select s;

            if (!String.IsNullOrEmpty(plazasId))
            {
                int plazaTempId = int.Parse(plazasId.Trim());
                pagos = pagos.Where(s => s.Patrone.Plaza_id.Equals(plazaTempId));
            }
            if (!String.IsNullOrEmpty(patronesId))
            {
                int patronesTempId = int.Parse(patronesId);
                pagos = pagos.Where(s => s.Patrone.Id.Equals(patronesTempId));
            }
            if (!String.IsNullOrEmpty(periodoId))
            {
                pagos = pagos.Where(s => s.mes.Trim().Equals(periodoId.Trim()));
            }
            if (!String.IsNullOrEmpty(ejercicioId))
            {
                pagos = pagos.Where(s => s.anno.Trim().Equals(ejercicioId));
            }

            pagos = pagos.OrderBy(p => p.Patrone.registro);

            return View(pagos.ToList());

        }

        // GET: Movimientos/Delete/5
        public ActionResult DeletePDFs(int? id)
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

        // POST: Movimientos/Delete/5
        public ActionResult DeletePDFConf(String pagoId, IEnumerable<bool> SampleChkIntBool)
        {
            if (!pagoId.Equals(""))
            {
                Pago pagos = db.Pagos.Find(Int32.Parse(pagoId));

                string value = Request["SampleChkIntBool"];
                if (value.Substring(0, 4) == "true")
                {
                    pagos.comprobantePago = null;
                }

                value = Request["SampleChkIntBool2"];
                if (value.Substring(0, 4) == "true")
                {
                    pagos.resumenLiquidacion = null;
                }

                value = Request["SampleChkIntBool3"];
                if (value.Substring(0, 4) == "true")
                {
                    pagos.cedulaAutodeterminacion = null;
                }
                
                db.Entry(pagos).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("EliminaPDFs", "Pagos");
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
