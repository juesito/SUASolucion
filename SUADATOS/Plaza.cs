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
    
    public partial class Plaza
    {
        public Plaza()
        {
            this.Acreditados = new HashSet<Acreditado>();
            this.Asegurados = new HashSet<Asegurado>();
            this.Clientes = new HashSet<Cliente>();
            this.Grupos = new HashSet<Grupos>();
            this.Patrones = new HashSet<Patrone>();
            this.Solicituds = new HashSet<Solicitud>();
            this.Usuarios = new HashSet<Usuario>();
            this.Solicituds1 = new HashSet<Solicitud1>();
            this.UsuaNvarios = new HashSet<UsuaNvario>();
        }
    
        public int id { get; set; }
        public string descripcion { get; set; }
        public string cveCorta { get; set; }
        public string indicador { get; set; }
    
        public virtual ICollection<Acreditado> Acreditados { get; set; }
        public virtual ICollection<Asegurado> Asegurados { get; set; }
        public virtual ICollection<Cliente> Clientes { get; set; }
        public virtual ICollection<Grupos> Grupos { get; set; }
        public virtual ICollection<Patrone> Patrones { get; set; }
        public virtual ICollection<Solicitud> Solicituds { get; set; }
        public virtual ICollection<Usuario> Usuarios { get; set; }
        public virtual ICollection<Solicitud1> Solicituds1 { get; set; }
        public virtual ICollection<UsuaNvario> UsuaNvarios { get; set; }
    }
}
