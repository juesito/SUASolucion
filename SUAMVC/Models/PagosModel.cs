using SUADATOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SUAMVC.Models
{
    public class PagosModel
    {
        public PagosModel() {
            pagosFooter = new PagosFooter();
            pagos = new List<Pago>();
        }
        public List<Pago> pagos { get; set; }
        public PagosFooter pagosFooter { get; set; }
    }
}