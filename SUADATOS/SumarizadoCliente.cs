//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SUADATOS
{
    using System;
    using System.Collections.Generic;
    
    public partial class SumarizadoCliente
    {
        public int id { get; set; }
        public int patronId { get; set; }
        public string anno { get; set; }
        public string mes { get; set; }
        public Nullable<decimal> imss { get; set; }
        public Nullable<decimal> rcv { get; set; }
        public Nullable<decimal> infonavit { get; set; }
        public Nullable<decimal> total { get; set; }
        public int nt { get; set; }
        public int usuarioId { get; set; }
        public System.DateTime fechaCreacion { get; set; }
        public int clienteId { get; set; }
    
        public virtual Cliente Cliente { get; set; }
        public virtual Patrone Patrone { get; set; }
        public virtual Usuario Usuario { get; set; }
    }
}
