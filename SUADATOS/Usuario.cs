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
            this.ArchivoEmpleadoes = new HashSet<ArchivoEmpleado>();
            this.Bancos = new HashSet<Banco>();
            this.Conceptos = new HashSet<Concepto>();
            this.ContratosClientes = new HashSet<ContratosCliente>();
            this.CuentaEmpleadoes = new HashSet<CuentaEmpleado>();
            this.DatosAdicionalesClientes = new HashSet<DatosAdicionalesCliente>();
            this.Departamentos = new HashSet<Departamento>();
            this.DetallePagoes = new HashSet<DetallePago>();
            this.DetallePrenominas = new HashSet<DetallePrenomina>();
            this.DocumentoEmpleadoes = new HashSet<DocumentoEmpleado>();
            this.Empleados = new HashSet<Empleado>();
            this.Empresas = new HashSet<Empresa>();
            this.EsquemasPagoes = new HashSet<EsquemasPago>();
            this.EstadoCivils = new HashSet<EstadoCivil>();
            this.Estados = new HashSet<Estado>();
            this.Factores = new HashSet<Factore>();
            this.FamiliaresEmpleadoes = new HashSet<FamiliaresEmpleado>();
            this.Giros = new HashSet<Giro>();
            this.ListaValidacionClientes = new HashSet<ListaValidacionCliente>();
            this.Municipios = new HashSet<Municipio>();
            this.Pagos = new HashSet<Pago>();
            this.Paises = new HashSet<Pais>();
            this.RegimenInfonavits = new HashSet<RegimenInfonavit>();
            this.Residencias = new HashSet<Residencia>();
            this.RespuestaSolicituds = new HashSet<RespuestaSolicitud>();
            this.ResumenPagoes = new HashSet<ResumenPago>();
            this.RoleFuncions = new HashSet<RoleFuncion>();
            this.RoleModulos = new HashSet<RoleModulo>();
            this.SalarialesEmpleadoes = new HashSet<SalarialesEmpleado>();
            this.SDIs = new HashSet<SDI>();
            this.Servicios = new HashSet<Servicio>();
            this.Sexos = new HashSet<Sexo>();
            this.Solicituds = new HashSet<Solicitud>();
            this.SolicitudEmpleadoes = new HashSet<SolicitudEmpleado>();
            this.SolicitudPrenominas = new HashSet<SolicitudPrenomina>();
            this.SumarizadoClientes = new HashSet<SumarizadoCliente>();
            this.TipoContratoes = new HashSet<TipoContrato>();
            this.TipoPersonals = new HashSet<TipoPersonal>();
            this.TopicosUsuarios = new HashSet<TopicosUsuario>();
            this.TopicosUsuarios1 = new HashSet<TopicosUsuario>();
            this.ReporteConMeses = new HashSet<ReporteConMes>();
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
    
        public virtual ICollection<ArchivoEmpleado> ArchivoEmpleadoes { get; set; }
        public virtual ICollection<Banco> Bancos { get; set; }
        public virtual ICollection<Concepto> Conceptos { get; set; }
        public virtual ICollection<ContratosCliente> ContratosClientes { get; set; }
        public virtual ICollection<CuentaEmpleado> CuentaEmpleadoes { get; set; }
        public virtual ICollection<DatosAdicionalesCliente> DatosAdicionalesClientes { get; set; }
        public virtual ICollection<Departamento> Departamentos { get; set; }
        public virtual Departamento Departamento { get; set; }
        public virtual ICollection<DetallePago> DetallePagoes { get; set; }
        public virtual ICollection<DetallePrenomina> DetallePrenominas { get; set; }
        public virtual ICollection<DocumentoEmpleado> DocumentoEmpleadoes { get; set; }
        public virtual ICollection<Empleado> Empleados { get; set; }
        public virtual ICollection<Empresa> Empresas { get; set; }
        public virtual ICollection<EsquemasPago> EsquemasPagoes { get; set; }
        public virtual ICollection<EstadoCivil> EstadoCivils { get; set; }
        public virtual ICollection<Estado> Estados { get; set; }
        public virtual ICollection<Factore> Factores { get; set; }
        public virtual ICollection<FamiliaresEmpleado> FamiliaresEmpleadoes { get; set; }
        public virtual ICollection<Giro> Giros { get; set; }
        public virtual ICollection<ListaValidacionCliente> ListaValidacionClientes { get; set; }
        public virtual ICollection<Municipio> Municipios { get; set; }
        public virtual ICollection<Pago> Pagos { get; set; }
        public virtual ICollection<Pais> Paises { get; set; }
        public virtual Plaza Plaza { get; set; }
        public virtual ICollection<RegimenInfonavit> RegimenInfonavits { get; set; }
        public virtual ICollection<Residencia> Residencias { get; set; }
        public virtual ICollection<RespuestaSolicitud> RespuestaSolicituds { get; set; }
        public virtual ICollection<ResumenPago> ResumenPagoes { get; set; }
        public virtual ICollection<RoleFuncion> RoleFuncions { get; set; }
        public virtual ICollection<RoleModulo> RoleModulos { get; set; }
        public virtual Role Role { get; set; }
        public virtual ICollection<SalarialesEmpleado> SalarialesEmpleadoes { get; set; }
        public virtual ICollection<SDI> SDIs { get; set; }
        public virtual ICollection<Servicio> Servicios { get; set; }
        public virtual ICollection<Sexo> Sexos { get; set; }
        public virtual ICollection<Solicitud> Solicituds { get; set; }
        public virtual ICollection<SolicitudEmpleado> SolicitudEmpleadoes { get; set; }
        public virtual ICollection<SolicitudPrenomina> SolicitudPrenominas { get; set; }
        public virtual ICollection<SumarizadoCliente> SumarizadoClientes { get; set; }
        public virtual ICollection<TipoContrato> TipoContratoes { get; set; }
        public virtual ICollection<TipoPersonal> TipoPersonals { get; set; }
        public virtual ICollection<TopicosUsuario> TopicosUsuarios { get; set; }
        public virtual ICollection<TopicosUsuario> TopicosUsuarios1 { get; set; }
        public virtual ICollection<ReporteConMes> ReporteConMeses { get; set; }
    }
}
