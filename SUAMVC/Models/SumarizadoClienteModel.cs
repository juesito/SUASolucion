using SUADATOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SUAMVC.Models
{
    public class SumarizadoClienteModel
    {
        public SumarizadoClienteModel() {
            sumarizadoAcumulado = new SumarizadoAcumulado();
            sumarizadoCliente = new List<SumarizadoCliente>();
        }
        public List<SumarizadoCliente> sumarizadoCliente { get; set; }
        public SumarizadoAcumulado sumarizadoAcumulado { get; set; }
    }
}