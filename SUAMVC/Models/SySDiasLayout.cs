using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SUAMVC.Models
{
    public class SySDiasLayout
    {
        //APATERNO	AMATERNO	NOMBRE	DIAS	TOTAL SYS	
        //GRATIFICACIONES	PRIMA_VACACIONAL	INFONAVIT	FONACOT 	
        //DESCPENSION	DESCREEMBOL	OTROSDESC	CUENTA	BANCO	CATEGORIA
        public String nombre { get; set; }
        public String apellidoMaterno { get; set; }
        public String apellidoPaterno { get; set; }
        public String diasTrabajados { get; set; }
        public String totalSyS { get; set; }
        public String gratificaciones { get; set; }
        public String primaVacacional { get; set; }
        public String fonacot { get; set; }
        public String pension { get; set; }
        public String reembolso { get; set; }
        public String otrosDescuentos { get; set; }
        public String infonavit { get; set; }
        public String cuenta { get; set; }
        public String banco { get; set; }
        public String categoria { get; set; }
    }
}