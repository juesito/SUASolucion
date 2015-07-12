using SUADATOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SUAMVC.Models
{
    public class PagosResumenModel
    {
        public Pago pago{set; get;}
        public List<ResumenPago> detalle{set; get;}
    }
}