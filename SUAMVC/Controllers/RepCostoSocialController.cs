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
using System.Web.Helpers;

namespace SUAMVC.Controllers
{
    public class RepCostoSocialController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: RepCostoSocials
        public ActionResult Index(String fechaAlta, String fechaBaja, String patronesId, String clientesId)
        {
            Usuario user = Session["UsuarioData"] as Usuario;
            var patronesAsignados = (from x in db.TopicosUsuarios
                                     where x.usuarioId.Equals(user.Id)
                                     && x.tipo.Equals("B")
                                     select x.topicoId);

            var clientesAsignados = (from x in db.TopicosUsuarios
                                     where x.usuarioId.Equals(user.Id)
                                     && x.tipo.Equals("C")
                                     select x.topicoId);

            ViewBag.patronesId = new SelectList((from s in db.Patrones.ToList()
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
            
            if (fechaAlta != null && fechaBaja != null )
            {
                LlenaReporte(fechaAlta, fechaBaja, clientesId, patronesId);
            }
            return View();
        }


        public ActionResult LlenaReporte(String fecIni, String fecFin, String idCliente, String idPatron)
        {
            DateTime fec1 = DateTime.Parse(fecIni);
            DateTime fec2 = DateTime.Parse(fecFin);
            TimeSpan dias = fec2 - fec1;
            int diasPeriodo = dias.Days + 1;

            ParametrosHelper parameterHelper = new ParametrosHelper();

            Parametro smdfParameter = parameterHelper.getParameterByKey("SMDF");
            Parametro sinfonParameter = parameterHelper.getParameterByKey("SINFON");
            Decimal smdf = Decimal.Parse(smdfParameter.valorMoneda.ToString());
            Decimal sinfon = Decimal.Parse(sinfonParameter.valorMoneda.ToString());

            //Preparamos la consulta
            var resAsegura = from b in db.Asegurados
                              where b.fechaAlta <= fec2
                              && ((b.fechaBaja >= fec1 && b.fechaBaja <= fec2) || b.fechaBaja.Equals(null))
//                              && b.ClienteId == clienteId
                              select b;

            if (idCliente != null && !idCliente.Trim().Equals("")) 
            {
                int clienteId = int.Parse(idCliente.Trim());
                resAsegura = resAsegura.Where(s => s.Cliente.Id.Equals(clienteId));
            }
            if (idPatron != null && !idPatron.Trim().Equals(""))
            {
                int patronId = int.Parse(idPatron.Trim());
                resAsegura = resAsegura.Where(s => s.PatroneId.Equals(patronId));
            }
            if (resAsegura != null)
            {
                foreach (var aseg in resAsegura.ToList())
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
                    reporte.fechaBaja = aseg.fechaBaja;
                    reporte.diasCotizados = diasPeriodo;
                    reporte.salarioIMSS = Double.Parse(salarioIMSS.ToString());
                    reporte.ubicacion = aseg.Cliente.claveCliente;
                    reporte.grupo = aseg.Cliente.Grupos.claveGrupo;
                    reporte.registroPatronal = aseg.Patrone.registro;
                    reporte.nombreRegPatronal = aseg.Patrone.nombre;
                    reporte.porcNomina = aseg.Patrone.porcentajeNomina;

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
                            dias = movs.fechaInicio - fec1;
                            diasCotizados = dias.Days + 1;
                            if (movs.movimientoId == 2)  // Baja
                            {
                                reporte.fechaBaja = movs.fechaInicio;
                            }
                            else  // Reingreso
                            {
                                salarioIMSS = Decimal.Parse(movs.sdi.ToString());
                                reporte.fechaAlta = movs.fechaInicio;
                            }
                            reporte.diasCotizados = diasCotizados;
                            reporte.salarioIMSS = Double.Parse(salarioIMSS.ToString());
                            Decimal var1 = Decimal.Parse("0.2040");
                            Decimal cuotaFija = smdf * diasCotizados * var1;
                            Decimal excedentePatronal = 0;
                            Decimal excedenteObrero = 0; ;
                            if (salarioIMSS > (smdf * 3))
                            {
                                var1 = Decimal.Parse("0.0110");
                                excedentePatronal = (salarioIMSS - (smdf * 3)) * diasCotizados * var1;
                                var1 = Decimal.Parse("0.0040");
                                excedenteObrero = (salarioIMSS - (smdf * 3)) * diasCotizados * var1;
                            }
                            var1 = Decimal.Parse("0.0070");
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
                            reporte.IMSS = cuotaFija + excedentePatronal + excedenteObrero + prestDineroPatron + prestDineroObrero +
                                           prestEspeciePatron + prestEspecieObrero + riesgoTrabajo + invalidezVidaPatron +
                                           invalidezVidaObrero + guarderías;

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
                            reporte.impuestoSNomina = reporte.porcCotizado / diasCotizados;

                            reporte.totalCostoSocial = reporte.totalCosto + reporte.porcCotizado;

                            db.RepCostoSocials.Add(reporte);
                            db.SaveChanges();

                        }
                    }
                    else
                    {
                        reporte.diasCotizados = diasCotizados;
                        reporte.salarioIMSS = Double.Parse(salarioIMSS.ToString());
                        Decimal var1 = Decimal.Parse("0.2040");
                        Decimal cuotaFija = smdf * diasCotizados * var1;
                        Decimal excedentePatronal = 0;
                        Decimal excedenteObrero = 0; ;
                        if (salarioIMSS > (smdf * 3))
                        {
                            var1 = Decimal.Parse("0.0110");
                            excedentePatronal = (salarioIMSS - (smdf * 3)) * diasCotizados * var1;
                            var1 = Decimal.Parse("0.0040");
                            excedenteObrero = (salarioIMSS - (smdf * 3)) * diasCotizados * var1;
                        }
                        var1 = Decimal.Parse("0.0070");
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
                        reporte.IMSS = cuotaFija + excedentePatronal + excedenteObrero + prestDineroPatron + prestDineroObrero + 
                                       prestEspeciePatron + prestEspecieObrero + riesgoTrabajo + invalidezVidaPatron + 
                                       invalidezVidaObrero + guarderías;

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
                        reporte.impuestoSNomina = reporte.porcCotizado / diasCotizados;

                        reporte.totalCostoSocial = reporte.totalCosto + reporte.porcCotizado;

                        db.RepCostoSocials.Add(reporte);
                        db.SaveChanges();
                    }
                }
            }

            var queryReporte = from b in db.RepCostoSocials
                               select b;
            List<RepCostoSocial> allCust = new List<RepCostoSocial>();

            allCust = queryReporte.ToList();

            WebGrid grid = new WebGrid(source: allCust, canPage: false, canSort: false);

            List<WebGridColumn> gridColumns = new List<WebGridColumn>();
            gridColumns.Add(grid.Column("numeroAfiliacion", "NSS "));
            gridColumns.Add(grid.Column("nombre", "Nombre"));
            gridColumns.Add(grid.Column("fechaAlta", "Fecha Alta", format: (item) => String.Format("{0:yyyy-MM-dd}", item.fechaAlta)));
            gridColumns.Add(grid.Column("fechaBaja", "Fecha Baja", format: (item) => item.fechaBaja != null ? String.Format("{0:yyyy-MM-dd}", item.fechaBaja) : String.Empty));
            gridColumns.Add(grid.Column("salarioImss", "Salario IMSS"));
            gridColumns.Add(grid.Column("diasCotizados", "Dias cotizados"));
            gridColumns.Add(grid.Column("ubicacion", "Ubicación"));
            gridColumns.Add(grid.Column("grupo", "Id Grupo"));
            gridColumns.Add(grid.Column("registroPatronal", "Reg. Patronal "));
            gridColumns.Add(grid.Column("nombreRegPatronal", "Nombre Reg. Patronal "));
            gridColumns.Add(grid.Column("IMSS", "IMSS"));
            gridColumns.Add(grid.Column("RCV", "RCV"));
            gridColumns.Add(grid.Column("Infonavit", "Infonavit"));
            gridColumns.Add(grid.Column("totalCosto", "Total Costo"));
            gridColumns.Add(grid.Column("impuestoSNomina", "Impuesto Sobre Nómina"));
            gridColumns.Add(grid.Column("porcNomina", "% Impuesto"));
            gridColumns.Add(grid.Column("porcCotizado", "% Cotizado"));
            gridColumns.Add(grid.Column("totalCostoSocial", "Total Costo Social"));
            gridColumns.Add(grid.Column("numeroCredito", "Número de crédito"));
            gridColumns.Add(grid.Column("descuentoMensual", "Desc. Infonavit Mensual"));
            gridColumns.Add(grid.Column("descuentoVeintiochonal", "Desc. Infonavit Veintioch."));
            gridColumns.Add(grid.Column("descuentoQuincenal", "Desc. Infonavit Quincenal"));
            gridColumns.Add(grid.Column("descuentoCatorcenal", "Desc. Infonavit Catorcenal"));
            gridColumns.Add(grid.Column("descuentoSemanal", "Desc. Infonavit Semanal"));
            gridColumns.Add(grid.Column("descuentoDiario", "Desc. Infonavit Diario"));

            string gridData = grid.GetHtml(
                columns: grid.Columns(gridColumns.ToArray())
                    ).ToString();

            Response.ClearContent();
            DateTime dateX = DateTime.Now;
            String fileName = "CostoSocial-" + dateX.ToString("ddMMyyyyHHmm") + ".xls";
            Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
            Response.ContentType = "application/excel";
            Response.Write(gridData);
            Response.End();

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
