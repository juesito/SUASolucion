﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class suaEntities : DbContext
    {
        public suaEntities()
            : base("name=suaEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Parametro> Parametros { get; set; }
        public virtual DbSet<Acreditado> Acreditados { get; set; }
        public virtual DbSet<ArchivosEmpleado> ArchivosEmpleados { get; set; }
        public virtual DbSet<Asegurado> Asegurados { get; set; }
        public virtual DbSet<Banco> Bancos { get; set; }
        public virtual DbSet<CatalogoMovimiento> CatalogoMovimientos { get; set; }
        public virtual DbSet<Cliente> Clientes { get; set; }
        public virtual DbSet<Concepto> Conceptos { get; set; }
        public virtual DbSet<ContratosCliente> ContratosClientes { get; set; }
        public virtual DbSet<CuentaEmpleado> CuentaEmpleadoes { get; set; }
        public virtual DbSet<DatosAdicionalesCliente> DatosAdicionalesClientes { get; set; }
        public virtual DbSet<Departamento> Departamentos { get; set; }
        public virtual DbSet<DetallePago> DetallePagoes { get; set; }
        public virtual DbSet<DocumentosEmpleado> DocumentosEmpleadoes { get; set; }
        public virtual DbSet<Empleado> Empleados { get; set; }
        public virtual DbSet<Empresa> Empresas { get; set; }
        public virtual DbSet<EsquemasPago> EsquemasPagoes { get; set; }
        public virtual DbSet<EstadoCivil> EstadoCivils { get; set; }
        public virtual DbSet<Estado> Estados { get; set; }
        public virtual DbSet<Factore> Factores { get; set; }
        public virtual DbSet<FamiliaresEmpleado> FamiliaresEmpleadoes { get; set; }
        public virtual DbSet<Funcion> Funcions { get; set; }
        public virtual DbSet<Giro> Giros { get; set; }
        public virtual DbSet<Grupos> Grupos { get; set; }
        public virtual DbSet<Incapacidade> Incapacidades { get; set; }
        public virtual DbSet<ListaValidacionCliente> ListaValidacionClientes { get; set; }
        public virtual DbSet<Modulo> Modulos { get; set; }
        public virtual DbSet<Movimiento> Movimientos { get; set; }
        public virtual DbSet<MovimientosAsegurado> MovimientosAseguradoes { get; set; }
        public virtual DbSet<Municipio> Municipios { get; set; }
        public virtual DbSet<Pago> Pagos { get; set; }
        public virtual DbSet<Pais> Paises { get; set; }
        public virtual DbSet<Patrone> Patrones { get; set; }
        public virtual DbSet<Plaza> Plazas { get; set; }
        public virtual DbSet<Proyecto> Proyectos { get; set; }
        public virtual DbSet<RegimenInfonavit> RegimenInfonavits { get; set; }
        public virtual DbSet<Residencia> Residencias { get; set; }
        public virtual DbSet<RespuestaSolicitud> RespuestaSolicituds { get; set; }
        public virtual DbSet<ResumenPago> ResumenPagoes { get; set; }
        public virtual DbSet<RoleFuncion> RoleFuncions { get; set; }
        public virtual DbSet<RoleModulo> RoleModulos { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<SalarialesEmpleado> SalarialesEmpleadoes { get; set; }
        public virtual DbSet<SDI> SDIs { get; set; }
        public virtual DbSet<Servicio> Servicios { get; set; }
        public virtual DbSet<Sexo> Sexos { get; set; }
        public virtual DbSet<Solicitud> Solicituds { get; set; }
        public virtual DbSet<SolicitudEmpleado> SolicitudEmpleadoes { get; set; }
        public virtual DbSet<SumarizadoCliente> SumarizadoClientes { get; set; }
        public virtual DbSet<TipoContrato> TipoContratoes { get; set; }
        public virtual DbSet<TipoPersonal> TipoPersonals { get; set; }
        public virtual DbSet<TopicosUsuario> TopicosUsuarios { get; set; }
        public virtual DbSet<Usuario> Usuarios { get; set; }
    
        public virtual int sp_createCatalogoMovimientos()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_createCatalogoMovimientos");
        }
    
        public virtual int sp_createConcepts(Nullable<int> usuarioId)
        {
            var usuarioIdParameter = usuarioId.HasValue ?
                new ObjectParameter("usuarioId", usuarioId) :
                new ObjectParameter("usuarioId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_createConcepts", usuarioIdParameter);
        }
    
        public virtual int sp_createFunctions(Nullable<int> usuarioId)
        {
            var usuarioIdParameter = usuarioId.HasValue ?
                new ObjectParameter("usuarioId", usuarioId) :
                new ObjectParameter("usuarioId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_createFunctions", usuarioIdParameter);
        }
    
        public virtual int sp_createModules()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_createModules");
        }
    
        public virtual int sp_createParameters()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_createParameters");
        }
    
        public virtual int sp_createRoles()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_createRoles");
        }
    
        public virtual int sp_SumarizadoClientes(Nullable<int> usuarioId, Nullable<int> clienteId)
        {
            var usuarioIdParameter = usuarioId.HasValue ?
                new ObjectParameter("usuarioId", usuarioId) :
                new ObjectParameter("usuarioId", typeof(int));
    
            var clienteIdParameter = clienteId.HasValue ?
                new ObjectParameter("clienteId", clienteId) :
                new ObjectParameter("clienteId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_SumarizadoClientes", usuarioIdParameter, clienteIdParameter);
        }
    
        public virtual int sp_SumarizadoClientesTodos(Nullable<int> usuarioId)
        {
            var usuarioIdParameter = usuarioId.HasValue ?
                new ObjectParameter("usuarioId", usuarioId) :
                new ObjectParameter("usuarioId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_SumarizadoClientesTodos", usuarioIdParameter);
        }
    
        public virtual int spCreateActionFunctions(Nullable<int> usuarioId)
        {
            var usuarioIdParameter = usuarioId.HasValue ?
                new ObjectParameter("usuarioId", usuarioId) :
                new ObjectParameter("usuarioId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("spCreateActionFunctions", usuarioIdParameter);
        }
    
        public virtual int spCreateFactorss(Nullable<int> usuarioId)
        {
            var usuarioIdParameter = usuarioId.HasValue ?
                new ObjectParameter("usuarioId", usuarioId) :
                new ObjectParameter("usuarioId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("spCreateFactorss", usuarioIdParameter);
        }
    }
}
