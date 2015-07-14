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
    
    public partial class Pago
    {
        public Pago()
        {
            this.DetallePagoes = new HashSet<DetallePago>();
        }
    
        public int id { get; set; }
        public decimal imss { get; set; }
        public decimal rcv { get; set; }
        public decimal infonavit { get; set; }
        public decimal total { get; set; }
        public decimal recargos { get; set; }
        public decimal actualizaciones { get; set; }
        public decimal granTotal { get; set; }
        public Nullable<System.DateTime> fechaDeposito { get; set; }
        public Nullable<int> bancoId { get; set; }
        public Nullable<int> nt { get; set; }
        public string comprobantePago { get; set; }
        public string resumenLiquidacion { get; set; }
        public string cedulaAutodeterminacion { get; set; }
        public System.DateTime fechaCreacion { get; set; }
        public int usuarioId { get; set; }
        public int patronId { get; set; }
        public string mes { get; set; }
        public string anno { get; set; }
    
        public virtual Banco Banco { get; set; }
        public virtual ICollection<DetallePago> DetallePagoes { get; set; }
        public virtual Patrone Patrone { get; set; }
        public virtual Usuario Usuario { get; set; }
    }
}
