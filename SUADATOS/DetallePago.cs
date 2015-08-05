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
    
    public partial class DetallePago
    {
        public int id { get; set; }
        public int pagoId { get; set; }
        public int aseguradoId { get; set; }
        public int diasCotizados { get; set; }
        public decimal sdi { get; set; }
        public int diasIncapacidad { get; set; }
        public int diasAusentismo { get; set; }
        public int diaCre { get; set; }
        public decimal cuotaFija { get; set; }
        public decimal expa { get; set; }
        public decimal exO { get; set; }
        public decimal pdp { get; set; }
        public decimal pdo { get; set; }
        public decimal gmpp { get; set; }
        public decimal gmpo { get; set; }
        public decimal rt { get; set; }
        public decimal ivp { get; set; }
        public decimal ivo { get; set; }
        public decimal gps { get; set; }
        public Nullable<decimal> retiro { get; set; }
        public Nullable<decimal> patronal { get; set; }
        public Nullable<decimal> obrera { get; set; }
        public Nullable<decimal> imss { get; set; }
        public Nullable<decimal> rcv { get; set; }
        public Nullable<decimal> aportacionsc { get; set; }
        public Nullable<decimal> aportacioncc { get; set; }
        public Nullable<decimal> amortizacion { get; set; }
        public Nullable<decimal> infonavit { get; set; }
        public Nullable<decimal> total { get; set; }
        public Nullable<decimal> patronalBimestral { get; set; }
        public Nullable<decimal> imssBimestral { get; set; }
        public Nullable<decimal> obreraBimestral { get; set; }
        public int patronId { get; set; }
        public System.DateTime fechaCreacion { get; set; }
        public int usuarioId { get; set; }
        public Nullable<int> diasCotizBim { get; set; }
    
        public virtual Asegurado Asegurado { get; set; }
        public virtual Pago Pago { get; set; }
        public virtual Patrone Patrone { get; set; }
        public virtual Usuario Usuario { get; set; }
    }
}
