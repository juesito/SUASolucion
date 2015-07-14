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
    
    public partial class Usuario
    {
        public Usuario()
        {
            this.RoleModulos = new HashSet<RoleModulo>();
            this.RoleFuncions = new HashSet<RoleFuncion>();
            this.TopicosUsuarios = new HashSet<TopicosUsuario>();
            this.TopicosUsuarios1 = new HashSet<TopicosUsuario>();
            this.Factores = new HashSet<Factore>();
            this.DetallePagoes = new HashSet<DetallePago>();
            this.Pagos = new HashSet<Pago>();
            this.Bancos = new HashSet<Banco>();
            this.SumarizadoClientes = new HashSet<SumarizadoCliente>();
        }
    
        public int Id { get; set; }
        public string nombreUsuario { get; set; }
        public string contrasena { get; set; }
        public string claveUsuario { get; set; }
        public string email { get; set; }
        public string apellidoMaterno { get; set; }
        public string apellidoPaterno { get; set; }
        public string estatus { get; set; }
        public System.DateTime fechaIngreso { get; set; }
        public int roleId { get; set; }
        public int plazaId { get; set; }
        public Nullable<int> departamentoId { get; set; }
    
        public virtual Role Role { get; set; }
        public virtual ICollection<RoleModulo> RoleModulos { get; set; }
        public virtual ICollection<RoleFuncion> RoleFuncions { get; set; }
        public virtual ICollection<TopicosUsuario> TopicosUsuarios { get; set; }
        public virtual ICollection<TopicosUsuario> TopicosUsuarios1 { get; set; }
        public virtual ICollection<Factore> Factores { get; set; }
        public virtual Plaza Plaza { get; set; }
        public virtual Usuario Usuarios1 { get; set; }
        public virtual Usuario Usuario1 { get; set; }
        public virtual ICollection<DetallePago> DetallePagoes { get; set; }
        public virtual ICollection<Pago> Pagos { get; set; }
        public virtual ICollection<Banco> Bancos { get; set; }
        public virtual ICollection<SumarizadoCliente> SumarizadoClientes { get; set; }
    }
}
