using SUADATOS;
using SUAMVC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;


namespace SUAMVC.Controllers
{
    public class RepCostoSocialController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: RepCostoSocials
        public ActionResult Index()
        {
            Usuario user = Session["UsuarioData"] as Usuario;
            return View();
        }


        public ActionResult LlenaReporte(String fecIni, String fecFin, String idCliente, String idPatron)
        {
            DateTime fec1 = DateTime.Parse("2015-03-01");
            DateTime fec2 = DateTime.Parse("2015-03-31");
            int clienteId = 2;
            TimeSpan dias = fec2 - fec1;
            int diasPeriodo = dias.Days + 1;

            ParametrosHelper parameterHelper = new ParametrosHelper();

            Parametro smdfParameter = parameterHelper.getParameterByKey("SMDF");
            Parametro sinfonParameter = parameterHelper.getParameterByKey("SINFON");
            Decimal smdf = Decimal.Parse(smdfParameter.valorMoneda.ToString());
            Decimal sinfon = Decimal.Parse(sinfonParameter.valorMoneda.ToString());

            //Preparamos la consulta
            var resAsegura = (from b in db.Asegurados
                              where b.fechaAlta <= fec2
                                  //                             && b.fechaBaja >= fec1
                              && b.ClienteId == clienteId
                              select b).ToList();

            if (resAsegura != null)
            {
                foreach (var aseg in resAsegura)
                {
                    Decimal salarioIMSS = Decimal.Parse("0.0");
                    if (!aseg.fechaBaja.Equals(null))
                    {
                        var movTemp2 = (from s in db.MovimientosAseguradoes
                                        where s.aseguradoId.Equals(aseg.id) &&
                                             (s.CatalogoMovimiento.tipo.Equals("01") ||
                                              s.CatalogoMovimiento.tipo.Equals("07") || s.CatalogoMovimiento.tipo.Equals("08") ||
                                              s.CatalogoMovimiento.tipo.Equals("13"))
                                              && s.fechaInicio <= fec2
                                        orderby s.fechaInicio descending
                                        select s).ToList();

                        MovimientosAsegurado movto = new MovimientosAsegurado();
                        if (movTemp2 != null && movTemp2.Count() > 0)
                        {
                            foreach (var movItem in movTemp2)
                            {
                                movto = movItem;
                                break;
                            }
                            salarioIMSS = Decimal.Parse(movto.sdi.ToString());
                        }
                    }
                    else
                    {
                        salarioIMSS = Decimal.Parse(aseg.salarioImss.ToString());
                    }
                    RepCostoSocial reporte = new RepCostoSocial();
                    int diasCotizados = diasPeriodo;
                    reporte.numeroAfiliacion = aseg.numeroAfiliacion;
                    reporte.nombre = aseg.nombreTemporal;
                    reporte.fechaAlta = aseg.fechaAlta;
                    reporte.diasCotizados = diasPeriodo;
                    reporte.salarioIMSS = Double.Parse(salarioIMSS.ToString());
                    reporte.ubicacion = aseg.Cliente.claveCliente;
                    reporte.grupo = aseg.Cliente.Grupos.claveGrupo;
                    reporte.registroPatronal = aseg.Patrone.registro;
                    reporte.nombreRegPatronal = aseg.Patrone.nombre;
                    reporte.impuestoSNomina = aseg.Patrone.porcentajeNomina;

                    if (!aseg.paginaInfo.Trim().Equals(""))
                    {
                        reporte.numeroCredito = aseg.paginaInfo;
                        DateTime date = DateTime.Now;

                        Decimal valueToCalculate = Decimal.Parse(aseg.valorDescuento.ToString());

                        Decimal newValue = Decimal.Parse("0.0");
                        if (aseg.tipoDescuento.Trim().Equals("2"))
                        {
                            // Descuento tipo cuota fija
                            newValue = valueToCalculate * 2;
                        }
                        else if (aseg.tipoDescuento.Trim().Equals("3"))
                        {
                            // Descuento tipo VSM
                            newValue = valueToCalculate * smdf * 2;
                            newValue = newValue + sinfon;
                            newValue = Math.Round(newValue, 3);
                        }

                        reporte.descuentoMensual = Math.Round(newValue / 2, 3);
                        Decimal newValue2 = Decimal.Parse(reporte.descuentoMensual.ToString()) * Decimal.Parse((7 / 30.4).ToString());
                        newValue2 = Math.Round(newValue2, 3);
                        reporte.descuentoSemanal = newValue2;

                        newValue2 = Decimal.Parse(reporte.descuentoMensual.ToString()) * Decimal.Parse((14 / 30.4).ToString());
                        newValue2 = Math.Round(newValue2, 3);
                        reporte.descuentoCatorcenal = newValue2;
                        reporte.descuentoQuincenal = Math.Round(newValue / 4, 3);
                        reporte.descuentoVeintiochonal = Math.Round(Decimal.Parse(reporte.descuentoMensual.ToString()) * Decimal.Parse((28 / 30.4).ToString()), 3);
                        reporte.descuentoDiario = Math.Round(newValue / Decimal.Parse("60.1"), 3);

                    }

                    var movimientos = (from b in db.MovimientosAseguradoes
                                      where b.aseguradoId.Equals(aseg.id)
                                      && b.fechaInicio >= fec1
                                      && b.fechaInicio <= fec2
                                      && (b.CatalogoMovimiento.tipo.Equals("02") ||
                                           b.CatalogoMovimiento.tipo.Equals("07") || b.CatalogoMovimiento.tipo.Equals("08") ||
                                           b.CatalogoMovimiento.tipo.Equals("13"))
                                      orderby (b.fechaInicio)
                                      select b).ToList();
                    if (movimientos != null && movimientos.Count() > 0)
                    {
                        foreach (var movs in movimientos)
                        {
                            salarioIMSS = Decimal.Parse(movs.sdi.ToString());
                            dias = movs.fechaInicio - fec1;
                            diasCotizados = dias.Days;
                            if (movs.movimientoId.Equals("02"))  // Baja
                            {
                                reporte.fechaBaja = movs.fechaInicio;
                            }
                            else  // Reingreso
                            {
                                reporte.fechaAlta = movs.fechaInicio;
                            }
                            reporte.diasCotizados = diasCotizados;
                            reporte.salarioIMSS = Double.Parse(salarioIMSS.ToString());
                            Decimal var1 = Decimal.Parse("0.070");
                            Decimal prestDineroPatron = salarioIMSS * diasCotizados * var1;
                            var1 = Decimal.Parse("0.025");
                            Decimal prestDineroObrero = salarioIMSS * diasCotizados * var1;
                            var1 = Decimal.Parse("0.105");
                            Decimal prestEspeciePatron = salarioIMSS * diasCotizados * var1;
                            var1 = Decimal.Parse("0.0375");
                            Decimal prestEspecieObrero = salarioIMSS * diasCotizados * var1;
                            var1 = Decimal.Parse("0.050");
                            Decimal riesgoTrabajo = salarioIMSS * diasCotizados * var1;
                            var1 = Decimal.Parse("0.175");
                            Decimal invalidezVidaPatron = salarioIMSS * diasCotizados * var1;
                            var1 = Decimal.Parse("0.0625");
                            Decimal invalidezVidaObrero = salarioIMSS * diasCotizados * var1;
                            var1 = Decimal.Parse("0.100");
                            Decimal guarderías = salarioIMSS * diasCotizados * var1;
                            reporte.IMSS = prestDineroPatron + prestDineroObrero + prestEspeciePatron + prestEspecieObrero + riesgoTrabajo + invalidezVidaPatron + invalidezVidaObrero + guarderías;

                            var1 = Decimal.Parse("0.200");
                            Decimal retiro = salarioIMSS * diasCotizados * var1;
                            var1 = Decimal.Parse("0.315");
                            Decimal cesantiaVejezPatron = salarioIMSS * diasCotizados * var1;
                            var1 = Decimal.Parse("0.1125");
                            Decimal cesantiaVejezObrero = salarioIMSS * diasCotizados * var1;
                            reporte.RCV = retiro + cesantiaVejezPatron + cesantiaVejezObrero;

                            var1 = Decimal.Parse("0.500");
                            reporte.Infonavit = salarioIMSS * diasCotizados * var1;

                            reporte.totalCosto = reporte.IMSS + reporte.RCV + reporte.Infonavit;

                            var1 = Decimal.Parse("0.300");
                            reporte.porcCotizado = salarioIMSS * diasCotizados * var1;
                            reporte.porcNomina = reporte.porcCotizado / diasCotizados;

                            reporte.totalCostoSocial = reporte.totalCosto + reporte.porcCotizado;

                            db.RepCostoSocials.Add(reporte);
                            db.SaveChanges();

                        }
                    }
                    else
                    {
                        reporte.diasCotizados = diasCotizados;
                        reporte.salarioIMSS = Double.Parse(salarioIMSS.ToString());
                        Decimal var1 = Decimal.Parse("0.0070");
                        Decimal prestDineroPatron = salarioIMSS * diasCotizados * var1;
                        var1 = Decimal.Parse("0.0025");
                        Decimal prestDineroObrero = salarioIMSS * diasCotizados * var1;
                        var1 = Decimal.Parse("0.0105");
                        Decimal prestEspeciePatron = salarioIMSS * diasCotizados * var1;
                        var1 = Decimal.Parse("0.00375");
                        Decimal prestEspecieObrero = salarioIMSS * diasCotizados * var1;
                        var1 = Decimal.Parse("0.0050");
                        Decimal riesgoTrabajo = salarioIMSS * diasCotizados * var1;
                        var1 = Decimal.Parse("0.0175");
                        Decimal invalidezVidaPatron = salarioIMSS * diasCotizados * var1;
                        var1 = Decimal.Parse("0.00625");
                        Decimal invalidezVidaObrero = salarioIMSS * diasCotizados * var1;
                        var1 = Decimal.Parse("0.0100");
                        Decimal guarderías = salarioIMSS * diasCotizados * var1;
                        reporte.IMSS = prestDineroPatron + prestDineroObrero + prestEspeciePatron + prestEspecieObrero + riesgoTrabajo + invalidezVidaPatron + invalidezVidaObrero + guarderías;

                        var1 = Decimal.Parse("0.0200");
                        Decimal retiro = salarioIMSS * diasCotizados * var1;
                        var1 = Decimal.Parse("0.0315");
                        Decimal cesantiaVejezPatron = salarioIMSS * diasCotizados * var1;
                        var1 = Decimal.Parse("0.01125");
                        Decimal cesantiaVejezObrero = salarioIMSS * diasCotizados * var1;
                        reporte.RCV = retiro + cesantiaVejezPatron + cesantiaVejezObrero;

                        var1 = Decimal.Parse("0.0500");
                        reporte.Infonavit = salarioIMSS * diasCotizados * var1;

                        reporte.totalCosto = reporte.IMSS + reporte.RCV + reporte.Infonavit;

                        var1 = Decimal.Parse("0.0300");
                        reporte.porcCotizado = salarioIMSS * diasCotizados * var1;
                        reporte.porcNomina = reporte.porcCotizado / diasCotizados;

                        reporte.totalCostoSocial = reporte.totalCosto + reporte.porcCotizado;

                        db.RepCostoSocials.Add(reporte);
                        db.SaveChanges();
                    }
                }
            }

            return RedirectToAction("UploadPagos");

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
