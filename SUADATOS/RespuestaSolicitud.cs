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
    
    public partial class RespuestaSolicitud
    {
        public int id { get; set; }
        public int solicitudId { get; set; }
        public int departamentoId { get; set; }
        public int estatusId { get; set; }
        public string observaciones { get; set; }
        public int usuarioId { get; set; }
        public System.DateTime fechaCreacion { get; set; }
    
        public virtual Concepto Concepto { get; set; }
        public virtual Departamento Departamento { get; set; }
        public virtual Solicitud Solicitud { get; set; }
        public virtual Usuario Usuario { get; set; }
    }
}
