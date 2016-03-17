using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SUAMVC.Models
{
    public class PrenominaExcelLayout
    {
        public String nombre { get; set; }
        public String apellidoMaterno { get; set; }
        public String apellidoPaterno { get; set; }
        public String nss { get; set; }
        public Int32 dt { get; set; }
        public decimal ingresos { get; set; }
        public decimal gratificacion { get; set; }
        public decimal primaVacacional { get; set; }
        public decimal aguinaldo { get; set; }
        public decimal descuentoInfonavit { get; set; }
        public decimal descuentoFonacot { get; set; }
        public decimal descuentoPension { get; set; }
        public decimal otrosDescuentos { get; set; }
        public decimal isr { get; set; }
    }
}