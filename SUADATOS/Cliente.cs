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
            this.ContratosClientes = new HashSet<ContratosCliente>();
            this.DatosAdicionalesClientes = new HashSet<DatosAdicionalesCliente>();
            this.ListaValidacionClientes = new HashSet<ListaValidacionCliente>();
            this.Solicituds = new HashSet<Solicitud>();
            this.Proyectos = new HashSet<Proyecto>();
        }
    
        public int Id { get; set; }
        public string claveCliente { get; set; }
        public string claveSua { get; set; }
        public string rfc { get; set; }
        public string descripcion { get; set; }
        public string nombre { get; set; }
        public string direccionFiscal { get; set; }
        public string contacto { get; set; }
        public string telefono { get; set; }
        public string direccionOficina { get; set; }
        public string email { get; set; }
        public string actividadPrincipal { get; set; }
        public Nullable<System.DateTime> fechaContratacion { get; set; }
        public Nullable<int> empresaFacturadoraId { get; set; }
        public Nullable<int> ejecutivoContadorId { get; set; }
        public int Plaza_id { get; set; }
        public int Grupo_id { get; set; }
        public Nullable<int> tipoClienteId { get; set; }
        public string numeroCuenta { get; set; }
        public Nullable<int> tipoServicioId { get; set; }
    
        public virtual ICollection<Acreditado> Acreditados { get; set; }
        public virtual ICollection<Asegurado> Asegurados { get; set; }
        public virtual ICollection<ContratosCliente> ContratosClientes { get; set; }
        public virtual ICollection<DatosAdicionalesCliente> DatosAdicionalesClientes { get; set; }
        public virtual Grupos Grupos { get; set; }
        public virtual ICollection<ListaValidacionCliente> ListaValidacionClientes { get; set; }
        public virtual Plaza Plaza { get; set; }
        public virtual ICollection<Solicitud> Solicituds { get; set; }
        public virtual ICollection<Proyecto> Proyectos { get; set; }
    }
}
