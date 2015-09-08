﻿//------------------------------------------------------------------------------
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
        public virtual DbSet<DetallePrenomina> DetallePrenominas { get; set; }
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
        public virtual DbSet<PrimaRT> PrimaRTs { get; set; }
        public virtual DbSet<Proyecto> Proyectos { get; set; }
        public virtual DbSet<RegimenInfonavit> RegimenInfonavits { get; set; }
        public virtual DbSet<RepCostoSocial> RepCostoSocials { get; set; }
        public virtual DbSet<ReporteConMes> ReporteConMeses { get; set; }
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
        public virtual DbSet<SolicitudPrenomina> SolicitudPrenominas { get; set; }
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
    
        public virtual int sp_AmortizacionBimestralINF(Nullable<int> usuarioId)
        {
            var usuarioIdParameter = usuarioId.HasValue ?
                new ObjectParameter("usuarioId", usuarioId) :
                new ObjectParameter("usuarioId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_AmortizacionBimestralINF", usuarioIdParameter);
        }
    
        public virtual int sp_LimpiaReporte(Nullable<int> usuarioId)
        {
            var usuarioIdParameter = usuarioId.HasValue ?
                new ObjectParameter("usuarioId", usuarioId) :
                new ObjectParameter("usuarioId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_LimpiaReporte", usuarioIdParameter);
        }
    
        public virtual int sp_ResumenPagosDetalleINF(Nullable<int> usuarioId, Nullable<int> clienteId)
        {
            var usuarioIdParameter = usuarioId.HasValue ?
                new ObjectParameter("usuarioId", usuarioId) :
                new ObjectParameter("usuarioId", typeof(int));
    
            var clienteIdParameter = clienteId.HasValue ?
                new ObjectParameter("clienteId", clienteId) :
                new ObjectParameter("clienteId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_ResumenPagosDetalleINF", usuarioIdParameter, clienteIdParameter);
        }
    
        public virtual int sp_ResumenPagosINF(Nullable<int> usuarioId)
        {
            var usuarioIdParameter = usuarioId.HasValue ?
                new ObjectParameter("usuarioId", usuarioId) :
                new ObjectParameter("usuarioId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_ResumenPagosINF", usuarioIdParameter);
        }
    
        public virtual int sp_AmortizacionBimestralINFDet(Nullable<int> usuarioId, Nullable<int> clienteId, Nullable<int> anio)
        {
            var usuarioIdParameter = usuarioId.HasValue ?
                new ObjectParameter("usuarioId", usuarioId) :
                new ObjectParameter("usuarioId", typeof(int));
    
            var clienteIdParameter = clienteId.HasValue ?
                new ObjectParameter("clienteId", clienteId) :
                new ObjectParameter("clienteId", typeof(int));
    
            var anioParameter = anio.HasValue ?
                new ObjectParameter("anio", anio) :
                new ObjectParameter("anio", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_AmortizacionBimestralINFDet", usuarioIdParameter, clienteIdParameter, anioParameter);
        }
    
        public virtual int sp_AmortizacionBimestralINFDetExcel(Nullable<int> usuarioId)
        {
            var usuarioIdParameter = usuarioId.HasValue ?
                new ObjectParameter("usuarioId", usuarioId) :
                new ObjectParameter("usuarioId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_AmortizacionBimestralINFDetExcel", usuarioIdParameter);
        }
    
        public virtual int sp_AmortizacionBimestralINF1(Nullable<int> usuarioId)
        {
            var usuarioIdParameter = usuarioId.HasValue ?
                new ObjectParameter("usuarioId", usuarioId) :
                new ObjectParameter("usuarioId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_AmortizacionBimestralINF1", usuarioIdParameter);
        }
    
        public virtual int sp_AmortizacionBimestralINFDet1(Nullable<int> usuarioId, Nullable<int> clienteId, Nullable<int> anio)
        {
            var usuarioIdParameter = usuarioId.HasValue ?
                new ObjectParameter("usuarioId", usuarioId) :
                new ObjectParameter("usuarioId", typeof(int));
    
            var clienteIdParameter = clienteId.HasValue ?
                new ObjectParameter("clienteId", clienteId) :
                new ObjectParameter("clienteId", typeof(int));
    
            var anioParameter = anio.HasValue ?
                new ObjectParameter("anio", anio) :
                new ObjectParameter("anio", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_AmortizacionBimestralINFDet1", usuarioIdParameter, clienteIdParameter, anioParameter);
        }
    
        public virtual int sp_AmortizacionBimestralINFDetExcel1(Nullable<int> usuarioId)
        {
            var usuarioIdParameter = usuarioId.HasValue ?
                new ObjectParameter("usuarioId", usuarioId) :
                new ObjectParameter("usuarioId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_AmortizacionBimestralINFDetExcel1", usuarioIdParameter);
        }
    
        public virtual int sp_createCatalogoMovimientos1()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_createCatalogoMovimientos1");
        }
    
        public virtual int sp_createConcepts1(Nullable<int> usuarioId)
        {
            var usuarioIdParameter = usuarioId.HasValue ?
                new ObjectParameter("usuarioId", usuarioId) :
                new ObjectParameter("usuarioId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_createConcepts1", usuarioIdParameter);
        }
    
        public virtual int sp_createFunctions1(Nullable<int> usuarioId)
        {
            var usuarioIdParameter = usuarioId.HasValue ?
                new ObjectParameter("usuarioId", usuarioId) :
                new ObjectParameter("usuarioId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_createFunctions1", usuarioIdParameter);
        }
    
        public virtual int sp_createModules1()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_createModules1");
        }
    
        public virtual int sp_createParameters1()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_createParameters1");
        }
    
        public virtual int sp_createRoles1()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_createRoles1");
        }
    
        public virtual int sp_LimpiaReporte1(Nullable<int> usuarioId)
        {
            var usuarioIdParameter = usuarioId.HasValue ?
                new ObjectParameter("usuarioId", usuarioId) :
                new ObjectParameter("usuarioId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_LimpiaReporte1", usuarioIdParameter);
        }
    
        public virtual int sp_ResumenPagosDetalleINF1(Nullable<int> usuarioId, Nullable<int> clienteId)
        {
            var usuarioIdParameter = usuarioId.HasValue ?
                new ObjectParameter("usuarioId", usuarioId) :
                new ObjectParameter("usuarioId", typeof(int));
    
            var clienteIdParameter = clienteId.HasValue ?
                new ObjectParameter("clienteId", clienteId) :
                new ObjectParameter("clienteId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_ResumenPagosDetalleINF1", usuarioIdParameter, clienteIdParameter);
        }
    
        public virtual int sp_ResumenPagosINF1(Nullable<int> usuarioId)
        {
            var usuarioIdParameter = usuarioId.HasValue ?
                new ObjectParameter("usuarioId", usuarioId) :
                new ObjectParameter("usuarioId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_ResumenPagosINF1", usuarioIdParameter);
        }
    
        public virtual int sp_SumarizadoClientes1(Nullable<int> usuarioId, Nullable<int> clienteId)
        {
            var usuarioIdParameter = usuarioId.HasValue ?
                new ObjectParameter("usuarioId", usuarioId) :
                new ObjectParameter("usuarioId", typeof(int));
    
            var clienteIdParameter = clienteId.HasValue ?
                new ObjectParameter("clienteId", clienteId) :
                new ObjectParameter("clienteId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_SumarizadoClientes1", usuarioIdParameter, clienteIdParameter);
        }
    
        public virtual int sp_SumarizadoClientesTodos1(Nullable<int> usuarioId)
        {
            var usuarioIdParameter = usuarioId.HasValue ?
                new ObjectParameter("usuarioId", usuarioId) :
                new ObjectParameter("usuarioId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_SumarizadoClientesTodos1", usuarioIdParameter);
        }
    
        public virtual int spCreateActionFunctions1(Nullable<int> usuarioId)
        {
            var usuarioIdParameter = usuarioId.HasValue ?
                new ObjectParameter("usuarioId", usuarioId) :
                new ObjectParameter("usuarioId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("spCreateActionFunctions1", usuarioIdParameter);
        }
    
        public virtual int spCreateFactorss1(Nullable<int> usuarioId)
        {
            var usuarioIdParameter = usuarioId.HasValue ?
                new ObjectParameter("usuarioId", usuarioId) :
                new ObjectParameter("usuarioId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("spCreateFactorss1", usuarioIdParameter);
        }
    
        public virtual int sp_CostoSocialAnual(Nullable<int> usuarioId)
        {
            var usuarioIdParameter = usuarioId.HasValue ?
                new ObjectParameter("usuarioId", usuarioId) :
                new ObjectParameter("usuarioId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_CostoSocialAnual", usuarioIdParameter);
        }
    }
}
