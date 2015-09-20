using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SUAMVC.Models
{
    public class DetallePrenominaEmpleado
    {
        public int id { get; set; }
        public int diasLaborados { get; set; }
        public double gratificacion { get; set; }
        public double primaVacacional { get; set; }
        public double descuentoInfonavit { get; set; }
        public double descuentoFonacot { get; set; }
        public double descuentoPension { get; set; }
        public double otrosDescuentos { get; set; }
        public double netoPagar { get; set; }


    }
}