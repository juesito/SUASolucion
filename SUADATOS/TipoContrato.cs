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
    
    public partial class TipoContrato
    {
        public TipoContrato()
        {
            this.Solicituds = new HashSet<Solicitud>();
        }
    
        public int id { get; set; }
        public string descripcion { get; set; }
        public System.DateTime fechaCreacion { get; set; }
        public int usuarioId { get; set; }
    
        public virtual ICollection<Solicitud> Solicituds { get; set; }
        public virtual Usuario Usuario { get; set; }
    }
}
