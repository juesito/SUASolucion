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
        public int usuaNvarioId { get; set; }
    
        public virtual Cliente Cliente { get; set; }
        public virtual Patrone Patrone { get; set; }
        public virtual Usuario Usuario { get; set; }
        public virtual UsuaNvario UsuaNvario { get; set; }
    }
}
