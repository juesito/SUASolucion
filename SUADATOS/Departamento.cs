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
    
    public partial class Departamento
    {
        public Departamento()
        {
            this.RespuestaSolicituds = new HashSet<RespuestaSolicitud>();
            this.Usuarios = new HashSet<Usuario>();
            this.RespuestaSolicituds1 = new HashSet<RespuestaSolicitud1>();
            this.UsuaNvarios = new HashSet<UsuaNvario>();
        }
    
        public int id { get; set; }
        public string descripcion { get; set; }
        public System.DateTime fechaCreacion { get; set; }
        public int usuarioId { get; set; }
        public int usuaNvarioId { get; set; }
    
        public virtual Usuario Usuario { get; set; }
        public virtual ICollection<RespuestaSolicitud> RespuestaSolicituds { get; set; }
        public virtual ICollection<Usuario> Usuarios { get; set; }
        public virtual ICollection<RespuestaSolicitud1> RespuestaSolicituds1 { get; set; }
        public virtual UsuaNvario UsuaNvario { get; set; }
        public virtual ICollection<UsuaNvario> UsuaNvarios { get; set; }
    }
}
