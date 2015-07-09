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
    
    public partial class Cliente
    {
        public Cliente()
        {
            this.Acreditados = new HashSet<Acreditado>();
            this.Asegurados = new HashSet<Asegurado>();
            this.Proyectos = new HashSet<Proyecto>();
        }
    
        public int Id { get; set; }
        public string claveCliente { get; set; }
        public string claveSua { get; set; }
        public string rfc { get; set; }
        public string descripcion { get; set; }
        public string ejecutivo { get; set; }
        public int Plaza_id { get; set; }
        public int Grupo_id { get; set; }
    
        public virtual ICollection<Acreditado> Acreditados { get; set; }
        public virtual ICollection<Asegurado> Asegurados { get; set; }
        public virtual Grupos Grupos { get; set; }
        public virtual Plaza Plaza { get; set; }
        public virtual ICollection<Proyecto> Proyectos { get; set; }
    }
}
