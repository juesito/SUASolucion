//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SUADATOS
{
    using System;
    using System.Collections.Generic;
    
    public partial class ResumenPago
    {
        public int id { get; set; }
        public int pagoId { get; set; }
        public string ip { get; set; }
        public int patronId { get; set; }
        public string rfc { get; set; }
        public string periodoPago { get; set; }
        public string mes { get; set; }
        public string anno { get; set; }
        public string folioSUA { get; set; }
        public string razonSocial { get; set; }
        public string calleColonia { get; set; }
        public string poblacion { get; set; }
        public string entidadFederativa { get; set; }
        public string codigoPostal { get; set; }
        public string primaRT { get; set; }
        public string fechaPrimaRT { get; set; }
        public string actividadEconomica { get; set; }
        public string delegacionIMSS { get; set; }
        public string subDelegacionIMMS { get; set; }
        public string zonaEconomica { get; set; }
        public string convenioReembolso { get; set; }
        public string tipoCotizacion { get; set; }
        public string cotizantes { get; set; }
        public string apoPat { get; set; }
        public string delSubDel { get; set; }
        public System.DateTime fechaCreacion { get; set; }
        public int usuarioCreacionId { get; set; }
    
        public virtual Pago Pago { get; set; }
    }
}
