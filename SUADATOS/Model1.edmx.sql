
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 08/25/2015 12:54:27
-- Generated from EDMX file: C:\Users\oj19edal\Source\Repos\SUASolucion\SUADATOS\Model1.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [sua];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_Acreditados_Acreditados]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Acreditados] DROP CONSTRAINT [FK_Acreditados_Acreditados];
GO
IF OBJECT_ID(N'[dbo].[FK_Acreditados_Clientes]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Acreditados] DROP CONSTRAINT [FK_Acreditados_Clientes];
GO
IF OBJECT_ID(N'[dbo].[FK_ArchivosEmpleados_Conceptos]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ArchivosEmpleados] DROP CONSTRAINT [FK_ArchivosEmpleados_Conceptos];
GO
IF OBJECT_ID(N'[dbo].[FK_ArchivosEmpleados_Empleados]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ArchivosEmpleados] DROP CONSTRAINT [FK_ArchivosEmpleados_Empleados];
GO
IF OBJECT_ID(N'[dbo].[FK_ArchivosEmpleados_Usuarios]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ArchivosEmpleados] DROP CONSTRAINT [FK_ArchivosEmpleados_Usuarios];
GO
IF OBJECT_ID(N'[dbo].[FK_Asegurados_Clientes]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Asegurados] DROP CONSTRAINT [FK_Asegurados_Clientes];
GO
IF OBJECT_ID(N'[dbo].[FK_Bancos_Bancos]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Bancos] DROP CONSTRAINT [FK_Bancos_Bancos];
GO
IF OBJECT_ID(N'[dbo].[FK_Clientes_Grupos]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Clientes] DROP CONSTRAINT [FK_Clientes_Grupos];
GO
IF OBJECT_ID(N'[dbo].[FK_Clientes_Plazas]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Clientes] DROP CONSTRAINT [FK_Clientes_Plazas];
GO
IF OBJECT_ID(N'[dbo].[FK_Clientes_Usuarios]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Clientes] DROP CONSTRAINT [FK_Clientes_Usuarios];
GO
IF OBJECT_ID(N'[dbo].[FK_Conceptos_Usuarios]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Conceptos] DROP CONSTRAINT [FK_Conceptos_Usuarios];
GO
IF OBJECT_ID(N'[dbo].[FK_ContratosCliente_Clientes]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ContratosCliente] DROP CONSTRAINT [FK_ContratosCliente_Clientes];
GO
IF OBJECT_ID(N'[dbo].[FK_ContratosCliente_Usuarios]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ContratosCliente] DROP CONSTRAINT [FK_ContratosCliente_Usuarios];
GO
IF OBJECT_ID(N'[dbo].[FK_CuentaEmpleado_Bancos]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CuentaEmpleadoes] DROP CONSTRAINT [FK_CuentaEmpleado_Bancos];
GO
IF OBJECT_ID(N'[dbo].[FK_CuentaEmpleado_Empleados]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CuentaEmpleadoes] DROP CONSTRAINT [FK_CuentaEmpleado_Empleados];
GO
IF OBJECT_ID(N'[dbo].[FK_CuentaEmpleado_Usuarios]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CuentaEmpleadoes] DROP CONSTRAINT [FK_CuentaEmpleado_Usuarios];
GO
IF OBJECT_ID(N'[dbo].[FK_DatosAdicionalesCliente_Clientes]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DatosAdicionalesCliente] DROP CONSTRAINT [FK_DatosAdicionalesCliente_Clientes];
GO
IF OBJECT_ID(N'[dbo].[FK_DatosAdicionalesCliente_Usuarios]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DatosAdicionalesCliente] DROP CONSTRAINT [FK_DatosAdicionalesCliente_Usuarios];
GO
IF OBJECT_ID(N'[dbo].[FK_Departamentos_Departamentos]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Departamentos] DROP CONSTRAINT [FK_Departamentos_Departamentos];
GO
IF OBJECT_ID(N'[dbo].[FK_DetallePago_Asegurados]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DetallePago] DROP CONSTRAINT [FK_DetallePago_Asegurados];
GO
IF OBJECT_ID(N'[dbo].[FK_DetallePago_Pagos]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DetallePago] DROP CONSTRAINT [FK_DetallePago_Pagos];
GO
IF OBJECT_ID(N'[dbo].[FK_DetallePago_Patrones]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DetallePago] DROP CONSTRAINT [FK_DetallePago_Patrones];
GO
IF OBJECT_ID(N'[dbo].[FK_DetallePago_Usuarios]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DetallePago] DROP CONSTRAINT [FK_DetallePago_Usuarios];
GO
IF OBJECT_ID(N'[dbo].[FK_documentosEmpleado_Conceptos]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DocumentosEmpleado] DROP CONSTRAINT [FK_documentosEmpleado_Conceptos];
GO
IF OBJECT_ID(N'[dbo].[FK_documentosEmpleado_Empleados]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DocumentosEmpleado] DROP CONSTRAINT [FK_documentosEmpleado_Empleados];
GO
IF OBJECT_ID(N'[dbo].[FK_documentosEmpleado_Usuarios]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DocumentosEmpleado] DROP CONSTRAINT [FK_documentosEmpleado_Usuarios];
GO
IF OBJECT_ID(N'[dbo].[FK_Empleados_Acreditados]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Empleados] DROP CONSTRAINT [FK_Empleados_Acreditados];
GO
IF OBJECT_ID(N'[dbo].[FK_Empleados_Bancos]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Empleados] DROP CONSTRAINT [FK_Empleados_Bancos];
GO
IF OBJECT_ID(N'[dbo].[FK_Empleados_EsquemasPago]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Empleados] DROP CONSTRAINT [FK_Empleados_EsquemasPago];
GO
IF OBJECT_ID(N'[dbo].[FK_Empleados_EstadoCivil]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Empleados] DROP CONSTRAINT [FK_Empleados_EstadoCivil];
GO
IF OBJECT_ID(N'[dbo].[FK_Empleados_Estados]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Empleados] DROP CONSTRAINT [FK_Empleados_Estados];
GO
IF OBJECT_ID(N'[dbo].[FK_Empleados_Municipios]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Empleados] DROP CONSTRAINT [FK_Empleados_Municipios];
GO
IF OBJECT_ID(N'[dbo].[FK_Empleados_Paises]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Empleados] DROP CONSTRAINT [FK_Empleados_Paises];
GO
IF OBJECT_ID(N'[dbo].[FK_Empleados_SDIs]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Empleados] DROP CONSTRAINT [FK_Empleados_SDIs];
GO
IF OBJECT_ID(N'[dbo].[FK_Empleados_Sexos]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Empleados] DROP CONSTRAINT [FK_Empleados_Sexos];
GO
IF OBJECT_ID(N'[dbo].[FK_Empleados_Solicitud]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Empleados] DROP CONSTRAINT [FK_Empleados_Solicitud];
GO
IF OBJECT_ID(N'[dbo].[FK_Empleados_Usuarios]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Empleados] DROP CONSTRAINT [FK_Empleados_Usuarios];
GO
IF OBJECT_ID(N'[dbo].[FK_Empresas_Empresas]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Empresas] DROP CONSTRAINT [FK_Empresas_Empresas];
GO
IF OBJECT_ID(N'[dbo].[FK_EsquemasPago_EsquemasPago]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[EsquemasPago] DROP CONSTRAINT [FK_EsquemasPago_EsquemasPago];
GO
IF OBJECT_ID(N'[dbo].[FK_EstadoCivil_EstadoCivil]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[EstadoCivil] DROP CONSTRAINT [FK_EstadoCivil_EstadoCivil];
GO
IF OBJECT_ID(N'[dbo].[FK_Estados_Paises]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Estados] DROP CONSTRAINT [FK_Estados_Paises];
GO
IF OBJECT_ID(N'[dbo].[FK_Estados_Usuarios]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Estados] DROP CONSTRAINT [FK_Estados_Usuarios];
GO
IF OBJECT_ID(N'[dbo].[FK_Factores_Usuarios]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Factores] DROP CONSTRAINT [FK_Factores_Usuarios];
GO
IF OBJECT_ID(N'[dbo].[FK_FamiliaresEmpleado_Conceptos]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[FamiliaresEmpleadoes] DROP CONSTRAINT [FK_FamiliaresEmpleado_Conceptos];
GO
IF OBJECT_ID(N'[dbo].[FK_FamiliaresEmpleado_Empleados]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[FamiliaresEmpleadoes] DROP CONSTRAINT [FK_FamiliaresEmpleado_Empleados];
GO
IF OBJECT_ID(N'[dbo].[FK_FamiliaresEmpleado_Usuarios]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[FamiliaresEmpleadoes] DROP CONSTRAINT [FK_FamiliaresEmpleado_Usuarios];
GO
IF OBJECT_ID(N'[dbo].[FK_Funciones_Modulos]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Funcions] DROP CONSTRAINT [FK_Funciones_Modulos];
GO
IF OBJECT_ID(N'[dbo].[FK_Giros_Giros]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Giros] DROP CONSTRAINT [FK_Giros_Giros];
GO
IF OBJECT_ID(N'[dbo].[FK_Incapacidades_Asegurados]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Incapacidades] DROP CONSTRAINT [FK_Incapacidades_Asegurados];
GO
IF OBJECT_ID(N'[dbo].[FK_ListaValidacionCliente_Clientes]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ListaValidacionCliente] DROP CONSTRAINT [FK_ListaValidacionCliente_Clientes];
GO
IF OBJECT_ID(N'[dbo].[FK_ListaValidacionCliente_Usuarios]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ListaValidacionCliente] DROP CONSTRAINT [FK_ListaValidacionCliente_Usuarios];
GO
IF OBJECT_ID(N'[dbo].[FK_Movimientos_Acreditados]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Movimientos] DROP CONSTRAINT [FK_Movimientos_Acreditados];
GO
IF OBJECT_ID(N'[dbo].[FK_Movimientos_Asegurados]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Movimientos] DROP CONSTRAINT [FK_Movimientos_Asegurados];
GO
IF OBJECT_ID(N'[dbo].[FK_MovimientosAsegurado_Asegurados]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[MovimientosAseguradoes] DROP CONSTRAINT [FK_MovimientosAsegurado_Asegurados];
GO
IF OBJECT_ID(N'[dbo].[FK_MovimientosAsegurado_catalogoMovimientos]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[MovimientosAseguradoes] DROP CONSTRAINT [FK_MovimientosAsegurado_catalogoMovimientos];
GO
IF OBJECT_ID(N'[dbo].[FK_MovimientosAsegurado_Incapacidades]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[MovimientosAseguradoes] DROP CONSTRAINT [FK_MovimientosAsegurado_Incapacidades];
GO
IF OBJECT_ID(N'[dbo].[FK_Municipios_Estados]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Municipios] DROP CONSTRAINT [FK_Municipios_Estados];
GO
IF OBJECT_ID(N'[dbo].[FK_Municipios_Paises]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Municipios] DROP CONSTRAINT [FK_Municipios_Paises];
GO
IF OBJECT_ID(N'[dbo].[FK_Municipios_Usuarios]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Municipios] DROP CONSTRAINT [FK_Municipios_Usuarios];
GO
IF OBJECT_ID(N'[dbo].[FK_Pagos_Bancos]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Pagos] DROP CONSTRAINT [FK_Pagos_Bancos];
GO
IF OBJECT_ID(N'[dbo].[FK_Pagos_Patrones]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Pagos] DROP CONSTRAINT [FK_Pagos_Patrones];
GO
IF OBJECT_ID(N'[dbo].[FK_Pagos_Usuarios]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Pagos] DROP CONSTRAINT [FK_Pagos_Usuarios];
GO
IF OBJECT_ID(N'[dbo].[FK_Paises_Paises]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Paises] DROP CONSTRAINT [FK_Paises_Paises];
GO
IF OBJECT_ID(N'[dbo].[FK_PatroneAcreditado]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Acreditados] DROP CONSTRAINT [FK_PatroneAcreditado];
GO
IF OBJECT_ID(N'[dbo].[FK_PatroneAsegurado]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Asegurados] DROP CONSTRAINT [FK_PatroneAsegurado];
GO
IF OBJECT_ID(N'[dbo].[FK_PlazaAcreditado]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Acreditados] DROP CONSTRAINT [FK_PlazaAcreditado];
GO
IF OBJECT_ID(N'[dbo].[FK_PlazaAsegurado]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Asegurados] DROP CONSTRAINT [FK_PlazaAsegurado];
GO
IF OBJECT_ID(N'[dbo].[FK_PlazaGrupos]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Grupos] DROP CONSTRAINT [FK_PlazaGrupos];
GO
IF OBJECT_ID(N'[dbo].[FK_PlazaPatrone]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Patrones] DROP CONSTRAINT [FK_PlazaPatrone];
GO
IF OBJECT_ID(N'[dbo].[FK_Proyectos_Clientes]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Solicitud] DROP CONSTRAINT [FK_Proyectos_Clientes];
GO
IF OBJECT_ID(N'[dbo].[FK_Proyectos_Clientes1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Proyectos] DROP CONSTRAINT [FK_Proyectos_Clientes1];
GO
IF OBJECT_ID(N'[dbo].[FK_Proyectos_EsquemasPago]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Solicitud] DROP CONSTRAINT [FK_Proyectos_EsquemasPago];
GO
IF OBJECT_ID(N'[dbo].[FK_Proyectos_SDIs]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Solicitud] DROP CONSTRAINT [FK_Proyectos_SDIs];
GO
IF OBJECT_ID(N'[dbo].[FK_Proyectos_TipoContrato]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Solicitud] DROP CONSTRAINT [FK_Proyectos_TipoContrato];
GO
IF OBJECT_ID(N'[dbo].[FK_Proyectos_TipoPersonal]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Solicitud] DROP CONSTRAINT [FK_Proyectos_TipoPersonal];
GO
IF OBJECT_ID(N'[dbo].[FK_Proyectos_Usuarios]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Solicitud] DROP CONSTRAINT [FK_Proyectos_Usuarios];
GO
IF OBJECT_ID(N'[dbo].[FK_RegimenInfonavit_RegimenInfonavit]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RegimenInfonavit] DROP CONSTRAINT [FK_RegimenInfonavit_RegimenInfonavit];
GO
IF OBJECT_ID(N'[dbo].[FK_ReporteConMese_Asegurados]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ReporteConMeses] DROP CONSTRAINT [FK_ReporteConMese_Asegurados];
GO
IF OBJECT_ID(N'[dbo].[FK_ReporteConMeses_Asegurados]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ReporteConMeses] DROP CONSTRAINT [FK_ReporteConMeses_Asegurados];
GO
IF OBJECT_ID(N'[dbo].[FK_ReporteConMeses_Patrones]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ReporteConMeses] DROP CONSTRAINT [FK_ReporteConMeses_Patrones];
GO
IF OBJECT_ID(N'[dbo].[FK_ReporteConMeses_Usuarios]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ReporteConMeses] DROP CONSTRAINT [FK_ReporteConMeses_Usuarios];
GO
IF OBJECT_ID(N'[dbo].[FK_Residencia_Residencia]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Residencia] DROP CONSTRAINT [FK_Residencia_Residencia];
GO
IF OBJECT_ID(N'[dbo].[FK_RespuestaSolicitud_Conceptos]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RespuestaSolicitud] DROP CONSTRAINT [FK_RespuestaSolicitud_Conceptos];
GO
IF OBJECT_ID(N'[dbo].[FK_RespuestaSolicitud_Departamentos]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RespuestaSolicitud] DROP CONSTRAINT [FK_RespuestaSolicitud_Departamentos];
GO
IF OBJECT_ID(N'[dbo].[FK_RespuestaSolicitud_RespuestaSolicitud]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RespuestaSolicitud] DROP CONSTRAINT [FK_RespuestaSolicitud_RespuestaSolicitud];
GO
IF OBJECT_ID(N'[dbo].[FK_RespuestaSolicitud_Usuarios]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RespuestaSolicitud] DROP CONSTRAINT [FK_RespuestaSolicitud_Usuarios];
GO
IF OBJECT_ID(N'[dbo].[FK_ResumenPago_Pagos]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ResumenPago] DROP CONSTRAINT [FK_ResumenPago_Pagos];
GO
IF OBJECT_ID(N'[dbo].[FK_ResumenPago_Patrones]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ResumenPago] DROP CONSTRAINT [FK_ResumenPago_Patrones];
GO
IF OBJECT_ID(N'[dbo].[FK_ResumenPago_Usuarios]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ResumenPago] DROP CONSTRAINT [FK_ResumenPago_Usuarios];
GO
IF OBJECT_ID(N'[dbo].[FK_RoleFunciones_Funciones]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RoleFuncions] DROP CONSTRAINT [FK_RoleFunciones_Funciones];
GO
IF OBJECT_ID(N'[dbo].[FK_RoleFunciones_Roles]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RoleFuncions] DROP CONSTRAINT [FK_RoleFunciones_Roles];
GO
IF OBJECT_ID(N'[dbo].[FK_RoleFunciones_RoleUsuarios]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RoleFuncions] DROP CONSTRAINT [FK_RoleFunciones_RoleUsuarios];
GO
IF OBJECT_ID(N'[dbo].[FK_RoleModulos_Modulos]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RoleModulos] DROP CONSTRAINT [FK_RoleModulos_Modulos];
GO
IF OBJECT_ID(N'[dbo].[FK_RoleModulos_Roles]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RoleModulos] DROP CONSTRAINT [FK_RoleModulos_Roles];
GO
IF OBJECT_ID(N'[dbo].[FK_RoleModulos_Usuarios]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RoleModulos] DROP CONSTRAINT [FK_RoleModulos_Usuarios];
GO
IF OBJECT_ID(N'[dbo].[FK_SalarialesEmpleado_Conceptos]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SalarialesEmpleadoes] DROP CONSTRAINT [FK_SalarialesEmpleado_Conceptos];
GO
IF OBJECT_ID(N'[dbo].[FK_SalarialesEmpleado_Empleados]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SalarialesEmpleadoes] DROP CONSTRAINT [FK_SalarialesEmpleado_Empleados];
GO
IF OBJECT_ID(N'[dbo].[FK_SalarialesEmpleado_Usuarios]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SalarialesEmpleadoes] DROP CONSTRAINT [FK_SalarialesEmpleado_Usuarios];
GO
IF OBJECT_ID(N'[dbo].[FK_SDIs_Clientes]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SDIs] DROP CONSTRAINT [FK_SDIs_Clientes];
GO
IF OBJECT_ID(N'[dbo].[FK_SDIs_SDIs]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SDIs] DROP CONSTRAINT [FK_SDIs_SDIs];
GO
IF OBJECT_ID(N'[dbo].[FK_Servicios_Servicios]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Servicios] DROP CONSTRAINT [FK_Servicios_Servicios];
GO
IF OBJECT_ID(N'[dbo].[FK_Sexos_Sexos]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Sexos] DROP CONSTRAINT [FK_Sexos_Sexos];
GO
IF OBJECT_ID(N'[dbo].[FK_Solicitud_Conceptos]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Solicitud] DROP CONSTRAINT [FK_Solicitud_Conceptos];
GO
IF OBJECT_ID(N'[dbo].[FK_Solicitud_Conceptos1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Solicitud] DROP CONSTRAINT [FK_Solicitud_Conceptos1];
GO
IF OBJECT_ID(N'[dbo].[FK_Solicitud_Conceptos2]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Solicitud] DROP CONSTRAINT [FK_Solicitud_Conceptos2];
GO
IF OBJECT_ID(N'[dbo].[FK_Solicitud_Conceptos3]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Solicitud] DROP CONSTRAINT [FK_Solicitud_Conceptos3];
GO
IF OBJECT_ID(N'[dbo].[FK_Solicitud_Conceptos4]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Solicitud] DROP CONSTRAINT [FK_Solicitud_Conceptos4];
GO
IF OBJECT_ID(N'[dbo].[FK_Solicitud_Conceptos5]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Solicitud] DROP CONSTRAINT [FK_Solicitud_Conceptos5];
GO
IF OBJECT_ID(N'[dbo].[FK_Solicitud_Plazas]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Solicitud] DROP CONSTRAINT [FK_Solicitud_Plazas];
GO
IF OBJECT_ID(N'[dbo].[FK_Solicitud_Proyectos]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Solicitud] DROP CONSTRAINT [FK_Solicitud_Proyectos];
GO
IF OBJECT_ID(N'[dbo].[FK_Solicitud_SDIs]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Solicitud] DROP CONSTRAINT [FK_Solicitud_SDIs];
GO
IF OBJECT_ID(N'[dbo].[FK_SolicitudEmpleado_Conceptos]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SolicitudEmpleado] DROP CONSTRAINT [FK_SolicitudEmpleado_Conceptos];
GO
IF OBJECT_ID(N'[dbo].[FK_SolicitudEmpleado_Solicitud]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SolicitudEmpleado] DROP CONSTRAINT [FK_SolicitudEmpleado_Solicitud];
GO
IF OBJECT_ID(N'[dbo].[FK_SolicitudEmpleado_SolicitudEmpleadoII]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SolicitudEmpleado] DROP CONSTRAINT [FK_SolicitudEmpleado_SolicitudEmpleadoII];
GO
IF OBJECT_ID(N'[dbo].[FK_SolicitudEmpleado_Usuarios]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SolicitudEmpleado] DROP CONSTRAINT [FK_SolicitudEmpleado_Usuarios];
GO
IF OBJECT_ID(N'[dbo].[FK_SumarizadoClientes_Asegurados]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SumarizadoClientes] DROP CONSTRAINT [FK_SumarizadoClientes_Asegurados];
GO
IF OBJECT_ID(N'[dbo].[FK_SumarizadoClientes_Patrones]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SumarizadoClientes] DROP CONSTRAINT [FK_SumarizadoClientes_Patrones];
GO
IF OBJECT_ID(N'[dbo].[FK_SumarizadoClientes_Usuarios]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SumarizadoClientes] DROP CONSTRAINT [FK_SumarizadoClientes_Usuarios];
GO
IF OBJECT_ID(N'[dbo].[FK_TipoContrato_TipoContrato]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TipoContrato] DROP CONSTRAINT [FK_TipoContrato_TipoContrato];
GO
IF OBJECT_ID(N'[dbo].[FK_TipoPersonal_TipoPersonal]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TipoPersonal] DROP CONSTRAINT [FK_TipoPersonal_TipoPersonal];
GO
IF OBJECT_ID(N'[dbo].[FK_TopicosUsuario_Usuario]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TopicosUsuarios] DROP CONSTRAINT [FK_TopicosUsuario_Usuario];
GO
IF OBJECT_ID(N'[dbo].[FK_TopicosUsuario_Usuario2]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TopicosUsuarios] DROP CONSTRAINT [FK_TopicosUsuario_Usuario2];
GO
IF OBJECT_ID(N'[dbo].[FK_Usuarios_Departamentos]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Usuarios] DROP CONSTRAINT [FK_Usuarios_Departamentos];
GO
IF OBJECT_ID(N'[dbo].[FK_Usuarios_Plazas]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Usuarios] DROP CONSTRAINT [FK_Usuarios_Plazas];
GO
IF OBJECT_ID(N'[dbo].[FK_Usuarios_Usuarios]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Usuarios] DROP CONSTRAINT [FK_Usuarios_Usuarios];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Acreditados]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Acreditados];
GO
IF OBJECT_ID(N'[dbo].[ArchivosEmpleados]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ArchivosEmpleados];
GO
IF OBJECT_ID(N'[dbo].[Asegurados]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Asegurados];
GO
IF OBJECT_ID(N'[dbo].[Bancos]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Bancos];
GO
IF OBJECT_ID(N'[dbo].[CatalogoMovimientos]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CatalogoMovimientos];
GO
IF OBJECT_ID(N'[dbo].[Clientes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Clientes];
GO
IF OBJECT_ID(N'[dbo].[Conceptos]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Conceptos];
GO
IF OBJECT_ID(N'[dbo].[ContratosCliente]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ContratosCliente];
GO
IF OBJECT_ID(N'[dbo].[CuentaEmpleadoes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CuentaEmpleadoes];
GO
IF OBJECT_ID(N'[dbo].[DatosAdicionalesCliente]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DatosAdicionalesCliente];
GO
IF OBJECT_ID(N'[dbo].[Departamentos]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Departamentos];
GO
IF OBJECT_ID(N'[dbo].[DetallePago]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DetallePago];
GO
IF OBJECT_ID(N'[dbo].[DocumentosEmpleado]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DocumentosEmpleado];
GO
IF OBJECT_ID(N'[dbo].[Empleados]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Empleados];
GO
IF OBJECT_ID(N'[dbo].[Empresas]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Empresas];
GO
IF OBJECT_ID(N'[dbo].[EsquemasPago]', 'U') IS NOT NULL
    DROP TABLE [dbo].[EsquemasPago];
GO
IF OBJECT_ID(N'[dbo].[EstadoCivil]', 'U') IS NOT NULL
    DROP TABLE [dbo].[EstadoCivil];
GO
IF OBJECT_ID(N'[dbo].[Estados]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Estados];
GO
IF OBJECT_ID(N'[dbo].[Factores]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Factores];
GO
IF OBJECT_ID(N'[dbo].[FamiliaresEmpleadoes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[FamiliaresEmpleadoes];
GO
IF OBJECT_ID(N'[dbo].[Funcions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Funcions];
GO
IF OBJECT_ID(N'[dbo].[Giros]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Giros];
GO
IF OBJECT_ID(N'[dbo].[Grupos]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Grupos];
GO
IF OBJECT_ID(N'[dbo].[Incapacidades]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Incapacidades];
GO
IF OBJECT_ID(N'[dbo].[ListaValidacionCliente]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ListaValidacionCliente];
GO
IF OBJECT_ID(N'[dbo].[Modulos]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Modulos];
GO
IF OBJECT_ID(N'[dbo].[Movimientos]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Movimientos];
GO
IF OBJECT_ID(N'[dbo].[MovimientosAseguradoes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MovimientosAseguradoes];
GO
IF OBJECT_ID(N'[dbo].[Municipios]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Municipios];
GO
IF OBJECT_ID(N'[dbo].[Pagos]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Pagos];
GO
IF OBJECT_ID(N'[dbo].[Paises]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Paises];
GO
IF OBJECT_ID(N'[dbo].[Parametros]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Parametros];
GO
IF OBJECT_ID(N'[dbo].[Patrones]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Patrones];
GO
IF OBJECT_ID(N'[dbo].[Plazas]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Plazas];
GO
IF OBJECT_ID(N'[dbo].[PrimaRT]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PrimaRT];
GO
IF OBJECT_ID(N'[dbo].[Proyectos]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Proyectos];
GO
IF OBJECT_ID(N'[dbo].[RegimenInfonavit]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RegimenInfonavit];
GO
IF OBJECT_ID(N'[dbo].[RepCostoSocial]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RepCostoSocial];
GO
IF OBJECT_ID(N'[dbo].[ReporteConMeses]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ReporteConMeses];
GO
IF OBJECT_ID(N'[dbo].[Residencia]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Residencia];
GO
IF OBJECT_ID(N'[dbo].[RespuestaSolicitud]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RespuestaSolicitud];
GO
IF OBJECT_ID(N'[dbo].[ResumenPago]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ResumenPago];
GO
IF OBJECT_ID(N'[dbo].[RoleFuncions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RoleFuncions];
GO
IF OBJECT_ID(N'[dbo].[RoleModulos]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RoleModulos];
GO
IF OBJECT_ID(N'[dbo].[Roles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Roles];
GO
IF OBJECT_ID(N'[dbo].[SalarialesEmpleadoes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SalarialesEmpleadoes];
GO
IF OBJECT_ID(N'[dbo].[SDIs]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SDIs];
GO
IF OBJECT_ID(N'[dbo].[Servicios]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Servicios];
GO
IF OBJECT_ID(N'[dbo].[Sexos]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Sexos];
GO
IF OBJECT_ID(N'[dbo].[Solicitud]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Solicitud];
GO
IF OBJECT_ID(N'[dbo].[SolicitudEmpleado]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SolicitudEmpleado];
GO
IF OBJECT_ID(N'[dbo].[SumarizadoClientes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SumarizadoClientes];
GO
IF OBJECT_ID(N'[dbo].[TipoContrato]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TipoContrato];
GO
IF OBJECT_ID(N'[dbo].[TipoPersonal]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TipoPersonal];
GO
IF OBJECT_ID(N'[dbo].[TopicosUsuarios]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TopicosUsuarios];
GO
IF OBJECT_ID(N'[dbo].[Usuarios]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Usuarios];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Parametros'
CREATE TABLE [dbo].[Parametros] (
    [id] int IDENTITY(1,1) NOT NULL,
    [parametroId] nchar(10)  NOT NULL,
    [descripcion] nchar(100)  NULL,
    [valorString] nchar(60)  NULL,
    [valorMoneda] decimal(10,2)  NULL,
    [valorFecha] datetime  NULL,
    [fechaCreacion] datetime  NOT NULL
);
GO

-- Creating table 'Acreditados'
CREATE TABLE [dbo].[Acreditados] (
    [id] int IDENTITY(1,1) NOT NULL,
    [apellidoPaterno] nchar(60)  NOT NULL,
    [apellidoMaterno] nchar(60)  NULL,
    [nombre] nchar(60)  NOT NULL,
    [nombreCompleto] nchar(100)  NULL,
    [CURP] nchar(20)  NULL,
    [RFC] nchar(15)  NOT NULL,
    [clienteId] int  NULL,
    [ocupacion] nchar(60)  NOT NULL,
    [idGrupo] nchar(20)  NULL,
    [numeroAfiliacion] nchar(15)  NOT NULL,
    [numeroCredito] nchar(15)  NULL,
    [fechaAlta] datetime  NOT NULL,
    [fechaBaja] datetime  NULL,
    [fechaInicioDescuento] datetime  NULL,
    [fechaFinDescuento] datetime  NULL,
    [smdv] float  NOT NULL,
    [sdi] float  NOT NULL,
    [sd] decimal(10,3)  NOT NULL,
    [vsm] decimal(10,3)  NOT NULL,
    [porcentaje] decimal(10,3)  NOT NULL,
    [cuotaFija] decimal(10,3)  NOT NULL,
    [descuentoBimestral] decimal(10,3)  NOT NULL,
    [descuentoMensual] decimal(10,3)  NOT NULL,
    [descuentoSemanal] decimal(10,3)  NOT NULL,
    [descuentoCatorcenal] decimal(10,3)  NOT NULL,
    [descuentoQuincenal] decimal(10,3)  NOT NULL,
    [descuentoVeintiochonal] decimal(10,3)  NOT NULL,
    [descuentoDiario] decimal(10,3)  NOT NULL,
    [acuseRetencion] nchar(20)  NULL,
    [PatroneId] int  NOT NULL,
    [Plaza_id] int  NOT NULL,
    [fechaCreacion] datetime  NULL,
    [fechaModificacion] datetime  NULL,
    [alta] nchar(10)  NULL,
    [baja] nchar(10)  NULL,
    [modificacion] nchar(10)  NULL,
    [permanente] nchar(10)  NULL,
    [fechaUltimoCalculo] datetime  NULL
);
GO

-- Creating table 'ArchivosEmpleados'
CREATE TABLE [dbo].[ArchivosEmpleados] (
    [id] int IDENTITY(1,1) NOT NULL,
    [empleadoId] int  NOT NULL,
    [archivo] nchar(100)  NOT NULL,
    [tipoArchivo] int  NOT NULL,
    [usuarioId] int  NOT NULL,
    [fechaCreacion] datetime  NOT NULL
);
GO

-- Creating table 'Asegurados'
CREATE TABLE [dbo].[Asegurados] (
    [id] int IDENTITY(1,1) NOT NULL,
    [numeroAfiliacion] nchar(11)  NOT NULL,
    [CURP] nchar(18)  NULL,
    [RFC] nchar(13)  NULL,
    [nombre] nchar(50)  NULL,
    [salarioImss] decimal(19,4)  NULL,
    [salarioInfo] decimal(19,4)  NULL,
    [fechaAlta] datetime  NOT NULL,
    [fechaBaja] datetime  NULL,
    [tipoTrabajo] nchar(1)  NOT NULL,
    [semanaJornada] nchar(1)  NOT NULL,
    [paginaInfo] nchar(10)  NOT NULL,
    [tipoDescuento] nchar(1)  NULL,
    [valorDescuento] decimal(19,4)  NULL,
    [ClienteId] int  NULL,
    [nombreTemporal] nchar(50)  NULL,
    [fechaDescuento] datetime  NULL,
    [finDescuento] datetime  NULL,
    [articulo33] nchar(1)  NULL,
    [salarioArticulo33] decimal(19,4)  NULL,
    [trapeniv] nchar(1)  NULL,
    [estado] nchar(1)  NULL,
    [claveMunicipio] nchar(3)  NULL,
    [PatroneId] int  NOT NULL,
    [extranjero] nchar(2)  NULL,
    [ocupacion] nchar(20)  NULL,
    [fechaCreacion] datetime  NULL,
    [fechaModificacion] datetime  NULL,
    [alta] nchar(60)  NULL,
    [baja] nchar(60)  NULL,
    [modificacion] nchar(60)  NULL,
    [permanente] nchar(60)  NULL,
    [Plaza_id] int  NOT NULL,
    [salarioDiario] decimal(10,3)  NULL,
    [apellidoPaterno] nchar(60)  NULL,
    [apellidoMaterno] nchar(60)  NULL,
    [nombres] nchar(60)  NULL
);
GO

-- Creating table 'Bancos'
CREATE TABLE [dbo].[Bancos] (
    [id] int IDENTITY(1,1) NOT NULL,
    [descripcion] nchar(100)  NULL,
    [fechaCreacion] datetime  NOT NULL,
    [usuarioId] int  NOT NULL
);
GO

-- Creating table 'CatalogoMovimientos'
CREATE TABLE [dbo].[CatalogoMovimientos] (
    [id] int IDENTITY(1,1) NOT NULL,
    [tipo] nchar(5)  NOT NULL,
    [descripcion] nchar(60)  NULL,
    [fechaCreacion] datetime  NOT NULL
);
GO

-- Creating table 'Clientes'
CREATE TABLE [dbo].[Clientes] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [claveCliente] nchar(15)  NOT NULL,
    [claveSua] nchar(15)  NOT NULL,
    [rfc] nchar(20)  NOT NULL,
    [descripcion] nchar(60)  NULL,
    [nombre] nchar(60)  NULL,
    [direccionFiscal] nchar(60)  NULL,
    [contacto] nchar(60)  NULL,
    [telefono] nchar(15)  NULL,
    [direccionOficina] nchar(60)  NULL,
    [email] nchar(100)  NULL,
    [actividadPrincipal] nchar(100)  NULL,
    [fechaContratacion] datetime  NULL,
    [empresaFacturadoraId] int  NULL,
    [ejecutivoContadorId] int  NULL,
    [Plaza_id] int  NOT NULL,
    [Grupo_id] int  NOT NULL,
    [tipoClienteId] int  NULL,
    [numeroCuenta] nchar(20)  NULL,
    [tipoServicioId] int  NULL
);
GO

-- Creating table 'Conceptos'
CREATE TABLE [dbo].[Conceptos] (
    [id] int IDENTITY(1,1) NOT NULL,
    [grupo] nchar(10)  NOT NULL,
    [descripcion] nchar(60)  NOT NULL,
    [fechaCreacion] datetime  NOT NULL,
    [usuarioId] int  NOT NULL
);
GO

-- Creating table 'ContratosClientes'
CREATE TABLE [dbo].[ContratosClientes] (
    [id] int IDENTITY(1,1) NOT NULL,
    [clienteId] int  NOT NULL,
    [descripcion] nchar(60)  NOT NULL,
    [archivo1] nchar(60)  NULL,
    [archivo2] nchar(60)  NULL,
    [archivo3] nchar(60)  NULL,
    [fechaFirma] datetime  NOT NULL,
    [firmo] nchar(100)  NOT NULL,
    [testigo] nchar(100)  NULL,
    [actaConstitutivaEmpresa] nchar(60)  NULL,
    [poderRepresentanteLegal] nchar(60)  NULL,
    [ifeRepresentanteLegal] nchar(60)  NULL,
    [comprobanteDomicilio] nchar(60)  NULL,
    [fechaInicioVigencia] datetime  NULL,
    [fechaFinalVigencia] datetime  NULL,
    [estatus] nchar(1)  NOT NULL,
    [fechaCreacion] datetime  NOT NULL,
    [usuarioId] int  NOT NULL
);
GO

-- Creating table 'CuentaEmpleadoes'
CREATE TABLE [dbo].[CuentaEmpleadoes] (
    [id] int IDENTITY(1,1) NOT NULL,
    [empleadoId] int  NOT NULL,
    [bancoId] int  NOT NULL,
    [cuenta] nchar(20)  NULL,
    [cuentaClabe] nchar(18)  NULL,
    [fechaCreacion] datetime  NOT NULL,
    [usuarioId] int  NOT NULL
);
GO

-- Creating table 'DatosAdicionalesClientes'
CREATE TABLE [dbo].[DatosAdicionalesClientes] (
    [id] int IDENTITY(1,1) NOT NULL,
    [clienteId] int  NOT NULL,
    [porcentajeComNomina] decimal(10,3)  NULL,
    [ivaNomina] decimal(10,3)  NULL,
    [porcentajeComIAS] decimal(10,3)  NULL,
    [ivaIAS] decimal(10,3)  NULL,
    [porcentajeComFlujo] decimal(10,3)  NULL,
    [ivaFlujo] decimal(10,3)  NULL,
    [costoSocial] decimal(10,3)  NULL,
    [conceptoFacturacion] int  NULL,
    [fechaCreacion] datetime  NOT NULL,
    [usuarioId] int  NOT NULL
);
GO

-- Creating table 'Departamentos'
CREATE TABLE [dbo].[Departamentos] (
    [id] int IDENTITY(1,1) NOT NULL,
    [descripcion] nchar(100)  NULL,
    [fechaCreacion] datetime  NOT NULL,
    [usuarioId] int  NOT NULL
);
GO

-- Creating table 'DetallePagoes'
CREATE TABLE [dbo].[DetallePagoes] (
    [id] int IDENTITY(1,1) NOT NULL,
    [pagoId] int  NOT NULL,
    [aseguradoId] int  NOT NULL,
    [diasCotizados] int  NOT NULL,
    [sdi] decimal(7,2)  NOT NULL,
    [diasIncapacidad] int  NOT NULL,
    [diasAusentismo] int  NOT NULL,
    [diaCre] int  NOT NULL,
    [cuotaFija] decimal(10,2)  NOT NULL,
    [expa] decimal(10,2)  NOT NULL,
    [exO] decimal(10,2)  NOT NULL,
    [pdp] decimal(10,2)  NOT NULL,
    [pdo] decimal(10,2)  NOT NULL,
    [gmpp] decimal(10,2)  NOT NULL,
    [gmpo] decimal(10,2)  NOT NULL,
    [rt] decimal(10,2)  NOT NULL,
    [ivp] decimal(10,2)  NOT NULL,
    [ivo] decimal(10,2)  NOT NULL,
    [gps] decimal(10,2)  NOT NULL,
    [retiro] decimal(10,2)  NULL,
    [patronal] decimal(10,2)  NULL,
    [obrera] decimal(10,2)  NULL,
    [imss] decimal(10,2)  NULL,
    [rcv] decimal(10,2)  NULL,
    [aportacionsc] decimal(10,2)  NULL,
    [aportacioncc] decimal(10,2)  NULL,
    [amortizacion] decimal(10,2)  NULL,
    [infonavit] decimal(10,2)  NULL,
    [total] decimal(10,2)  NULL,
    [patronalBimestral] decimal(10,2)  NULL,
    [imssBimestral] decimal(10,2)  NULL,
    [obreraBimestral] decimal(10,2)  NULL,
    [patronId] int  NOT NULL,
    [fechaCreacion] datetime  NOT NULL,
    [usuarioId] int  NOT NULL,
    [diasCotizBim] int  NULL
);
GO

-- Creating table 'DocumentosEmpleadoes'
CREATE TABLE [dbo].[DocumentosEmpleadoes] (
    [id] int  NOT NULL,
    [empleadoId] int  NOT NULL,
    [actividades] char(100)  NULL,
    [domicilioOficina] nchar(200)  NULL,
    [fechaAntiguedad] datetime  NOT NULL,
    [salarioVSM] decimal(10,2)  NULL,
    [jornadaLaboralId] int  NOT NULL,
    [diasDescanso] int  NULL,
    [salarioNominal] decimal(10,2)  NOT NULL,
    [diasVacaciones] int  NULL,
    [diasAguinaldo] int  NULL,
    [otros] nchar(100)  NULL,
    [telefono] nchar(10)  NULL,
    [tipoSangre] nchar(5)  NULL,
    [fechaCreacion] datetime  NOT NULL,
    [usuarioId] int  NOT NULL
);
GO

-- Creating table 'Empleados'
CREATE TABLE [dbo].[Empleados] (
    [id] int IDENTITY(1,1) NOT NULL,
    [solicitudId] int  NOT NULL,
    [folioEmpleado] nchar(30)  NULL,
    [nss] nchar(12)  NULL,
    [fechaAltaImss] datetime  NULL,
    [apellidoMaterno] nchar(60)  NOT NULL,
    [apellidoPaterno] nchar(60)  NOT NULL,
    [nombre] nchar(60)  NOT NULL,
    [nombreCompleto] nchar(180)  NOT NULL,
    [rfc] nchar(10)  NULL,
    [homoclave] nchar(5)  NULL,
    [curp] nchar(18)  NULL,
    [sexoId] int  NOT NULL,
    [sdiId] int  NULL,
    [esquemaPagoId] int  NULL,
    [salarioReal] decimal(10,3)  NOT NULL,
    [categoria] nchar(60)  NULL,
    [tieneInfonavit] int  NOT NULL,
    [creditoInfonavit] nchar(20)  NULL,
    [estadoCivilId] int  NOT NULL,
    [fechaNacimiento] datetime  NOT NULL,
    [nacionalidadId] int  NOT NULL,
    [estadoNacimientoId] int  NULL,
    [municipioNacimientoId] int  NULL,
    [calleNumero] nchar(60)  NOT NULL,
    [colonia] nchar(60)  NOT NULL,
    [edoMunicipio] nchar(60)  NOT NULL,
    [codigoPostal] nchar(8)  NULL,
    [tramitarTarjeta] int  NOT NULL,
    [bancoId] int  NOT NULL,
    [cuentaBancaria] nchar(25)  NULL,
    [cuentaClabe] nchar(18)  NULL,
    [email] nchar(100)  NULL,
    [observaciones] nchar(100)  NULL,
    [usuarioId] int  NOT NULL,
    [fechaCreacion] datetime  NOT NULL,
    [estatus] char(1)  NOT NULL,
    [fechaBaja] datetime  NULL,
    [aseguradoId] int  NULL,
    [foto] nchar(100)  NULL
);
GO

-- Creating table 'Empresas'
CREATE TABLE [dbo].[Empresas] (
    [id] int IDENTITY(1,1) NOT NULL,
    [descripcion] nchar(100)  NULL,
    [fechaCreacion] datetime  NOT NULL,
    [usuarioId] int  NOT NULL
);
GO

-- Creating table 'EsquemasPagoes'
CREATE TABLE [dbo].[EsquemasPagoes] (
    [id] int IDENTITY(1,1) NOT NULL,
    [descripcion] nchar(100)  NULL,
    [fechaCreacion] datetime  NOT NULL,
    [usuarioId] int  NOT NULL
);
GO

-- Creating table 'EstadoCivils'
CREATE TABLE [dbo].[EstadoCivils] (
    [id] int IDENTITY(1,1) NOT NULL,
    [descripcion] nchar(100)  NULL,
    [fechaCreacion] datetime  NOT NULL,
    [usuarioId] int  NOT NULL
);
GO

-- Creating table 'Estados'
CREATE TABLE [dbo].[Estados] (
    [id] int IDENTITY(1,1) NOT NULL,
    [paisId] int  NOT NULL,
    [entidad] nchar(5)  NULL,
    [descripcion] nchar(100)  NULL,
    [fechaCreacion] datetime  NOT NULL,
    [usuarioId] int  NOT NULL
);
GO

-- Creating table 'Factores'
CREATE TABLE [dbo].[Factores] (
    [id] int IDENTITY(1,1) NOT NULL,
    [anosTrabajados] int  NULL,
    [diasVacaciones] int  NULL,
    [primaVacacional] decimal(8,2)  NULL,
    [porcentaje] decimal(8,2)  NULL,
    [diasAno] int  NULL,
    [factorVacaciones] decimal(10,5)  NULL,
    [aguinaldo] int  NULL,
    [diasAnoAguinaldo] int  NULL,
    [factor] decimal(10,5)  NULL,
    [factorIntegracion] decimal(10,5)  NULL,
    [fechaRegistro] datetime  NOT NULL,
    [usuarioId] int  NOT NULL
);
GO

-- Creating table 'FamiliaresEmpleadoes'
CREATE TABLE [dbo].[FamiliaresEmpleadoes] (
    [id] int IDENTITY(1,1) NOT NULL,
    [empleadoId] int  NOT NULL,
    [parentescoId] int  NOT NULL,
    [nombre] nchar(60)  NULL,
    [apellidoMaterno] nchar(60)  NULL,
    [apellidoPaterno] nchar(60)  NULL,
    [nombreCompleto] nchar(130)  NULL,
    [telefonoCelular] nchar(10)  NULL,
    [telefonoCasa] nchar(10)  NULL,
    [email] nchar(100)  NULL,
    [fechaCreacion] datetime  NOT NULL,
    [usuarioId] int  NOT NULL
);
GO

-- Creating table 'Funcions'
CREATE TABLE [dbo].[Funcions] (
    [id] int IDENTITY(1,1) NOT NULL,
    [moduloId] int  NOT NULL,
    [descripcionCorta] nchar(60)  NOT NULL,
    [descripcionLarga] nchar(100)  NULL,
    [accion] nchar(100)  NOT NULL,
    [controlador] nchar(100)  NOT NULL,
    [estatus] nchar(2)  NOT NULL,
    [usuarioId] int  NOT NULL,
    [fechaCreacion] datetime  NOT NULL,
    [tipo] nchar(1)  NOT NULL
);
GO

-- Creating table 'Giros'
CREATE TABLE [dbo].[Giros] (
    [id] int IDENTITY(1,1) NOT NULL,
    [descripcion] nchar(100)  NULL,
    [fechaCreacion] datetime  NOT NULL,
    [usuarioId] int  NOT NULL
);
GO

-- Creating table 'Grupos'
CREATE TABLE [dbo].[Grupos] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [claveGrupo] nchar(15)  NOT NULL,
    [nombre] nchar(60)  NOT NULL,
    [nombreCorto] nchar(30)  NULL,
    [Plaza_id] int  NOT NULL,
    [posicion] nchar(10)  NULL,
    [estatus] nchar(1)  NOT NULL
);
GO

-- Creating table 'Incapacidades'
CREATE TABLE [dbo].[Incapacidades] (
    [id] int IDENTITY(1,1) NOT NULL,
    [aseguradoId] int  NOT NULL,
    [fechaAcc] datetime  NOT NULL,
    [folioIncapacidad] nchar(15)  NOT NULL,
    [grupoIncapacidad] nchar(20)  NOT NULL,
    [recRev] nchar(2)  NULL,
    [consecuencia] nchar(1)  NULL,
    [tieRie] nchar(25)  NULL,
    [ramSeq] nchar(25)  NULL,
    [secuela] nchar(25)  NULL,
    [conInc] nchar(25)  NULL,
    [diaSub] int  NOT NULL,
    [porcentajeIncapacidad] decimal(10,4)  NOT NULL,
    [indDef] nchar(2)  NOT NULL,
    [fecTer] datetime  NOT NULL,
    [tipoIncapacidad] nchar(1)  NULL,
    [alta] nchar(60)  NULL
);
GO

-- Creating table 'ListaValidacionClientes'
CREATE TABLE [dbo].[ListaValidacionClientes] (
    [id] int IDENTITY(1,1) NOT NULL,
    [clienteId] int  NOT NULL,
    [validador] nchar(60)  NOT NULL,
    [emailValidador] nchar(100)  NOT NULL,
    [autorizador] nchar(60)  NOT NULL,
    [emailAutorizador] nchar(100)  NOT NULL,
    [listaEmailAux] char(500)  NULL,
    [fechaCreacion] datetime  NOT NULL,
    [usuarioId] int  NOT NULL
);
GO

-- Creating table 'Modulos'
CREATE TABLE [dbo].[Modulos] (
    [id] int IDENTITY(1,1) NOT NULL,
    [descripcionCorta] nchar(10)  NULL,
    [descripcionLarga] nchar(100)  NULL,
    [fechaCreacion] datetime  NOT NULL,
    [estatus] nchar(1)  NOT NULL
);
GO

-- Creating table 'Movimientos'
CREATE TABLE [dbo].[Movimientos] (
    [id] int IDENTITY(1,1) NOT NULL,
    [aseguradoId] int  NULL,
    [acreditadoId] int  NULL,
    [lote] nchar(50)  NOT NULL,
    [fechaTransaccion] datetime  NOT NULL,
    [tipo] nchar(2)  NOT NULL,
    [nombreArchivo] nchar(100)  NOT NULL,
    [fechaCreacion] datetime  NOT NULL
);
GO

-- Creating table 'MovimientosAseguradoes'
CREATE TABLE [dbo].[MovimientosAseguradoes] (
    [id] int IDENTITY(1,1) NOT NULL,
    [aseguradoId] int  NOT NULL,
    [movimientoId] int  NOT NULL,
    [fechaInicio] datetime  NOT NULL,
    [sdi] nchar(10)  NULL,
    [numeroDias] int  NULL,
    [folio] nchar(10)  NULL,
    [incapacidadId] int  NOT NULL,
    [credito] nchar(10)  NULL,
    [estatus] nchar(1)  NOT NULL
);
GO

-- Creating table 'Municipios'
CREATE TABLE [dbo].[Municipios] (
    [id] int IDENTITY(1,1) NOT NULL,
    [paisId] int  NOT NULL,
    [estadoId] int  NOT NULL,
    [entidad] nchar(3)  NULL,
    [municipio1] nchar(5)  NULL,
    [descripcion] nchar(100)  NULL,
    [tipo] nchar(1)  NULL,
    [nomTipo] nchar(200)  NULL,
    [region] nchar(200)  NULL,
    [gradoMarginacion] nchar(15)  NULL,
    [fechaCreacion] datetime  NOT NULL,
    [usuarioId] int  NOT NULL
);
GO

-- Creating table 'Pagos'
CREATE TABLE [dbo].[Pagos] (
    [id] int IDENTITY(1,1) NOT NULL,
    [imss] decimal(10,2)  NOT NULL,
    [rcv] decimal(10,2)  NOT NULL,
    [infonavit] decimal(10,2)  NOT NULL,
    [total] decimal(10,2)  NOT NULL,
    [recargos] decimal(10,2)  NOT NULL,
    [actualizaciones] decimal(10,2)  NOT NULL,
    [granTotal] decimal(10,2)  NOT NULL,
    [fechaDeposito] datetime  NULL,
    [bancoId] int  NULL,
    [nt] int  NULL,
    [comprobantePago] nchar(100)  NULL,
    [resumenLiquidacion] nchar(100)  NULL,
    [cedulaAutodeterminacion] nchar(100)  NULL,
    [fechaCreacion] datetime  NOT NULL,
    [usuarioId] int  NOT NULL,
    [patronId] int  NOT NULL,
    [mes] nchar(2)  NOT NULL,
    [anno] nchar(4)  NOT NULL
);
GO

-- Creating table 'Paises'
CREATE TABLE [dbo].[Paises] (
    [id] int IDENTITY(1,1) NOT NULL,
    [descripcion] nchar(100)  NULL,
    [clavePais] nchar(5)  NULL,
    [naturalez] nchar(1)  NULL,
    [fechaCreacion] datetime  NOT NULL,
    [usuarioId] int  NOT NULL
);
GO

-- Creating table 'Patrones'
CREATE TABLE [dbo].[Patrones] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [registro] nvarchar(max)  NOT NULL,
    [rfc] nchar(15)  NOT NULL,
    [nombre] nchar(50)  NOT NULL,
    [actividad] nchar(50)  NOT NULL,
    [domicilio] nchar(50)  NOT NULL,
    [municipio] nchar(50)  NOT NULL,
    [codigoPostal] nchar(7)  NOT NULL,
    [entidad] nchar(2)  NOT NULL,
    [telefono] nchar(15)  NOT NULL,
    [remision] nchar(1)  NOT NULL,
    [zona] nchar(1)  NOT NULL,
    [delegacion] nchar(4)  NOT NULL,
    [carEnt] nchar(30)  NOT NULL,
    [numeroDelegacion] int  NOT NULL,
    [carDel] nchar(25)  NOT NULL,
    [numSub] int  NOT NULL,
    [tipoConvenio] decimal(19,4)  NOT NULL,
    [convenio] nchar(3)  NULL,
    [inicioAfiliacion] nchar(6)  NULL,
    [patRep] nchar(50)  NULL,
    [clase] nchar(3)  NULL,
    [fraccion] nchar(4)  NULL,
    [STyPS] nchar(2)  NULL,
    [Plaza_id] int  NOT NULL,
    [direccionArchivo] nchar(100)  NULL,
    [porcentajeNomina] int  NULL
);
GO

-- Creating table 'Plazas'
CREATE TABLE [dbo].[Plazas] (
    [id] int IDENTITY(1,1) NOT NULL,
    [descripcion] nchar(50)  NOT NULL,
    [cveCorta] nchar(10)  NULL,
    [indicador] nchar(1)  NULL
);
GO

-- Creating table 'PrimaRTs'
CREATE TABLE [dbo].[PrimaRTs] (
    [id] int IDENTITY(1,1) NOT NULL,
    [registroPatronal] int  NOT NULL,
    [anio] int  NOT NULL,
    [mes] int  NOT NULL,
    [primaRT1] decimal(8,5)  NOT NULL,
    [nomMes] nchar(10)  NOT NULL
);
GO

-- Creating table 'Proyectos'
CREATE TABLE [dbo].[Proyectos] (
    [id] int IDENTITY(1,1) NOT NULL,
    [clienteId] int  NOT NULL,
    [descripcion] nchar(60)  NOT NULL,
    [fechaCreacion] datetime  NOT NULL,
    [usuarioId] int  NOT NULL
);
GO

-- Creating table 'RegimenInfonavits'
CREATE TABLE [dbo].[RegimenInfonavits] (
    [id] int IDENTITY(1,1) NOT NULL,
    [descripcion] nchar(100)  NULL,
    [fechaCreacion] datetime  NOT NULL,
    [usuarioId] int  NOT NULL
);
GO

-- Creating table 'RepCostoSocials'
CREATE TABLE [dbo].[RepCostoSocials] (
    [id] int IDENTITY(1,1) NOT NULL,
    [numeroAfiliacion] nchar(15)  NULL,
    [nombre] nchar(100)  NULL,
    [fechaAlta] datetime  NULL,
    [fechaBaja] datetime  NULL,
    [diasCotizados] int  NULL,
    [salarioIMSS] float  NULL,
    [ubicacion] nchar(15)  NULL,
    [grupo] nchar(15)  NULL,
    [registroPatronal] nchar(15)  NULL,
    [nombreRegPatronal] nchar(50)  NULL,
    [IMSS] decimal(10,3)  NULL,
    [RCV] decimal(10,3)  NULL,
    [Infonavit] decimal(10,3)  NULL,
    [totalCosto] decimal(10,3)  NULL,
    [descuentoMensual] decimal(10,3)  NULL,
    [impuestoSNomina] decimal(7,3)  NULL,
    [porcNomina] decimal(7,3)  NULL,
    [porcCotizado] decimal(7,3)  NULL,
    [totalCostoSocial] decimal(10,3)  NULL,
    [numeroCredito] nchar(15)  NULL,
    [descuentoCatorcenal] decimal(10,3)  NULL,
    [descuentoQuincenal] decimal(10,3)  NULL,
    [descuentoVeintiochonal] decimal(10,3)  NULL,
    [descuentoDiario] decimal(10,3)  NULL,
    [descuentoSemanal] decimal(10,3)  NULL,
    [usuarioId] int  NULL
);
GO

-- Creating table 'ReporteConMeses'
CREATE TABLE [dbo].[ReporteConMeses] (
    [id] int IDENTITY(1,1) NOT NULL,
    [patronId] int  NULL,
    [aseguradoId] int  NULL,
    [anno] nchar(4)  NOT NULL,
    [mes] nchar(2)  NULL,
    [enero] decimal(10,2)  NULL,
    [febrero] decimal(10,2)  NULL,
    [marzo] decimal(10,2)  NULL,
    [abril] decimal(10,2)  NULL,
    [mayo] decimal(10,2)  NULL,
    [junio] decimal(10,2)  NULL,
    [julio] decimal(10,2)  NULL,
    [agosto] decimal(10,2)  NULL,
    [septiembre] decimal(10,2)  NULL,
    [octubre] decimal(10,2)  NULL,
    [noviembre] decimal(10,2)  NULL,
    [diciembre] decimal(10,2)  NULL,
    [total] decimal(10,2)  NULL,
    [nt] int  NULL,
    [usuarioId] int  NOT NULL,
    [fechaCreacion] datetime  NOT NULL,
    [clienteId] int  NOT NULL
);
GO

-- Creating table 'Residencias'
CREATE TABLE [dbo].[Residencias] (
    [id] int IDENTITY(1,1) NOT NULL,
    [descripcion] nchar(100)  NULL,
    [fechaCreacion] datetime  NOT NULL,
    [usuarioId] int  NOT NULL
);
GO

-- Creating table 'RespuestaSolicituds'
CREATE TABLE [dbo].[RespuestaSolicituds] (
    [id] int IDENTITY(1,1) NOT NULL,
    [solicitudId] int  NOT NULL,
    [departamentoId] int  NOT NULL,
    [estatusId] int  NOT NULL,
    [observaciones] nchar(300)  NULL,
    [usuarioId] int  NOT NULL,
    [fechaCreacion] datetime  NOT NULL
);
GO

-- Creating table 'ResumenPagoes'
CREATE TABLE [dbo].[ResumenPagoes] (
    [id] int IDENTITY(1,1) NOT NULL,
    [pagoId] int  NOT NULL,
    [ip] nchar(15)  NULL,
    [patronId] int  NOT NULL,
    [rfc] nchar(13)  NULL,
    [periodoPago] nchar(6)  NULL,
    [mes] nchar(2)  NULL,
    [anno] nchar(4)  NULL,
    [folioSUA] nchar(9)  NULL,
    [razonSocial] nchar(50)  NULL,
    [calleColonia] nchar(40)  NULL,
    [poblacion] nchar(40)  NULL,
    [entidadFederativa] nchar(2)  NULL,
    [codigoPostal] nchar(5)  NULL,
    [primaRT] nchar(7)  NULL,
    [fechaPrimaRT] nchar(6)  NULL,
    [actividadEconomica] nchar(40)  NULL,
    [delegacionIMSS] nchar(2)  NULL,
    [subDelegacionIMMS] nchar(2)  NULL,
    [zonaEconomica] nchar(1)  NULL,
    [convenioReembolso] nchar(1)  NULL,
    [tipoCotizacion] nchar(1)  NULL,
    [cotizantes] nchar(9)  NULL,
    [apoPat] nchar(4)  NULL,
    [delSubDel] nchar(4)  NULL,
    [fechaCreacion] datetime  NOT NULL,
    [usuarioCreacionId] int  NOT NULL
);
GO

-- Creating table 'RoleFuncions'
CREATE TABLE [dbo].[RoleFuncions] (
    [id] int IDENTITY(1,1) NOT NULL,
    [roleId] int  NOT NULL,
    [funcionId] int  NOT NULL,
    [usuarioCreacionId] int  NOT NULL,
    [fechaCreacion] datetime  NOT NULL
);
GO

-- Creating table 'RoleModulos'
CREATE TABLE [dbo].[RoleModulos] (
    [id] int IDENTITY(1,1) NOT NULL,
    [roleId] int  NOT NULL,
    [moduloId] int  NOT NULL,
    [usuarioCreacionId] int  NOT NULL,
    [fechaCreacion] datetime  NOT NULL
);
GO

-- Creating table 'Roles'
CREATE TABLE [dbo].[Roles] (
    [id] int IDENTITY(1,1) NOT NULL,
    [descripcion] nchar(60)  NULL,
    [fechaCreacion] datetime  NOT NULL,
    [estatus] char(2)  NOT NULL
);
GO

-- Creating table 'SalarialesEmpleadoes'
CREATE TABLE [dbo].[SalarialesEmpleadoes] (
    [id] int IDENTITY(1,1) NOT NULL,
    [empleadoId] int  NOT NULL,
    [salarioMensual] decimal(10,2)  NULL,
    [salarioHrsExtra] decimal(10,2)  NULL,
    [descuentos] int  NULL,
    [montoInfonavit] decimal(10,2)  NULL,
    [creditoFonacot] nchar(20)  NULL,
    [importeFonacot] decimal(10,2)  NULL,
    [fonacotDescuentos] int  NULL,
    [numeroPrestamo] nchar(20)  NULL,
    [importePrestamo] decimal(10,2)  NULL,
    [prestamoDescuentos] decimal(10,2)  NULL,
    [porcientoPension] decimal(10,2)  NULL,
    [importePension] decimal(10,2)  NULL,
    [periodoId] int  NULL,
    [fechaCaptura] datetime  NOT NULL,
    [usuarioId] int  NOT NULL
);
GO

-- Creating table 'SDIs'
CREATE TABLE [dbo].[SDIs] (
    [id] int IDENTITY(1,1) NOT NULL,
    [descripcion] nchar(100)  NULL,
    [fechaCreacion] datetime  NOT NULL,
    [usuarioId] int  NOT NULL,
    [clienteId] int  NOT NULL
);
GO

-- Creating table 'Servicios'
CREATE TABLE [dbo].[Servicios] (
    [id] int IDENTITY(1,1) NOT NULL,
    [descripcion] nchar(100)  NULL,
    [fechaCreacion] datetime  NOT NULL,
    [usuarioId] int  NOT NULL
);
GO

-- Creating table 'Sexos'
CREATE TABLE [dbo].[Sexos] (
    [id] int IDENTITY(1,1) NOT NULL,
    [descripcion] nchar(100)  NULL,
    [fechaCreacion] datetime  NOT NULL,
    [usuarioId] int  NOT NULL
);
GO

-- Creating table 'Solicituds'
CREATE TABLE [dbo].[Solicituds] (
    [id] int IDENTITY(1,1) NOT NULL,
    [folioSolicitud] nchar(15)  NULL,
    [clienteId] int  NOT NULL,
    [plazaId] int  NOT NULL,
    [fechaSolicitud] datetime  NOT NULL,
    [esquemaId] int  NULL,
    [sdiId] int  NULL,
    [contratoId] int  NULL,
    [fechaTerminoContrato] datetime  NULL,
    [fechaInicial] datetime  NULL,
    [fechaFinal] datetime  NULL,
    [fechaModificacion] datetime  NULL,
    [fechaBaja] datetime  NULL,
    [tipoPersonalId] int  NULL,
    [solicita] nchar(60)  NULL,
    [valida] nchar(60)  NULL,
    [autoriza] nchar(60)  NULL,
    [noTrabajadores] int  NOT NULL,
    [observaciones] nchar(100)  NULL,
    [estatusSolicitud] int  NOT NULL,
    [estatusNomina] int  NULL,
    [estatusAfiliado] int  NULL,
    [estatusJuridico] int  NULL,
    [estatusTarjeta] int  NULL,
    [usuarioId] int  NOT NULL,
    [proyectoId] int  NOT NULL,
    [fechaEnvio] datetime  NULL,
    [tipoSolicitud] int  NOT NULL,
    [conceptoBaja] int  NULL
);
GO

-- Creating table 'SolicitudEmpleadoes'
CREATE TABLE [dbo].[SolicitudEmpleadoes] (
    [id] int IDENTITY(1,1) NOT NULL,
    [solicitudId] int  NOT NULL,
    [empleadoId] int  NOT NULL,
    [tipoId] int  NOT NULL,
    [estatus] nchar(1)  NOT NULL,
    [fechaCreacion] datetime  NOT NULL,
    [usuarioId] int  NOT NULL
);
GO

-- Creating table 'SumarizadoClientes'
CREATE TABLE [dbo].[SumarizadoClientes] (
    [id] int IDENTITY(1,1) NOT NULL,
    [patronId] int  NOT NULL,
    [anno] nchar(4)  NOT NULL,
    [mes] nchar(2)  NOT NULL,
    [imss] decimal(10,2)  NULL,
    [rcv] decimal(10,2)  NULL,
    [infonavit] decimal(10,2)  NULL,
    [total] decimal(10,2)  NULL,
    [nt] int  NOT NULL,
    [usuarioId] int  NOT NULL,
    [fechaCreacion] datetime  NOT NULL,
    [clienteId] int  NOT NULL
);
GO

-- Creating table 'TipoContratoes'
CREATE TABLE [dbo].[TipoContratoes] (
    [id] int IDENTITY(1,1) NOT NULL,
    [descripcion] nchar(100)  NULL,
    [fechaCreacion] datetime  NOT NULL,
    [usuarioId] int  NOT NULL
);
GO

-- Creating table 'TipoPersonals'
CREATE TABLE [dbo].[TipoPersonals] (
    [id] int IDENTITY(1,1) NOT NULL,
    [descripcion] nchar(100)  NULL,
    [fechaCreacion] datetime  NOT NULL,
    [usuarioId] int  NOT NULL
);
GO

-- Creating table 'TopicosUsuarios'
CREATE TABLE [dbo].[TopicosUsuarios] (
    [id] int IDENTITY(1,1) NOT NULL,
    [topicoId] int  NOT NULL,
    [usuarioId] int  NOT NULL,
    [tipo] nchar(2)  NOT NULL,
    [usuarioCreacionId] int  NOT NULL,
    [fechaCreacion] datetime  NOT NULL
);
GO

-- Creating table 'Usuarios'
CREATE TABLE [dbo].[Usuarios] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [nombreUsuario] nchar(60)  NOT NULL,
    [contrasena] nchar(20)  NOT NULL,
    [claveUsuario] nchar(30)  NOT NULL,
    [email] nchar(100)  NULL,
    [apellidoMaterno] nchar(60)  NULL,
    [apellidoPaterno] nchar(60)  NULL,
    [estatus] nchar(2)  NOT NULL,
    [fechaIngreso] datetime  NOT NULL,
    [roleId] int  NOT NULL,
    [plazaId] int  NOT NULL,
    [departamentoId] int  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [id] in table 'Parametros'
ALTER TABLE [dbo].[Parametros]
ADD CONSTRAINT [PK_Parametros]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'Acreditados'
ALTER TABLE [dbo].[Acreditados]
ADD CONSTRAINT [PK_Acreditados]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'ArchivosEmpleados'
ALTER TABLE [dbo].[ArchivosEmpleados]
ADD CONSTRAINT [PK_ArchivosEmpleados]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'Asegurados'
ALTER TABLE [dbo].[Asegurados]
ADD CONSTRAINT [PK_Asegurados]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'Bancos'
ALTER TABLE [dbo].[Bancos]
ADD CONSTRAINT [PK_Bancos]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'CatalogoMovimientos'
ALTER TABLE [dbo].[CatalogoMovimientos]
ADD CONSTRAINT [PK_CatalogoMovimientos]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [Id] in table 'Clientes'
ALTER TABLE [dbo].[Clientes]
ADD CONSTRAINT [PK_Clientes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [id] in table 'Conceptos'
ALTER TABLE [dbo].[Conceptos]
ADD CONSTRAINT [PK_Conceptos]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'ContratosClientes'
ALTER TABLE [dbo].[ContratosClientes]
ADD CONSTRAINT [PK_ContratosClientes]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'CuentaEmpleadoes'
ALTER TABLE [dbo].[CuentaEmpleadoes]
ADD CONSTRAINT [PK_CuentaEmpleadoes]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'DatosAdicionalesClientes'
ALTER TABLE [dbo].[DatosAdicionalesClientes]
ADD CONSTRAINT [PK_DatosAdicionalesClientes]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'Departamentos'
ALTER TABLE [dbo].[Departamentos]
ADD CONSTRAINT [PK_Departamentos]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'DetallePagoes'
ALTER TABLE [dbo].[DetallePagoes]
ADD CONSTRAINT [PK_DetallePagoes]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'DocumentosEmpleadoes'
ALTER TABLE [dbo].[DocumentosEmpleadoes]
ADD CONSTRAINT [PK_DocumentosEmpleadoes]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'Empleados'
ALTER TABLE [dbo].[Empleados]
ADD CONSTRAINT [PK_Empleados]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'Empresas'
ALTER TABLE [dbo].[Empresas]
ADD CONSTRAINT [PK_Empresas]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'EsquemasPagoes'
ALTER TABLE [dbo].[EsquemasPagoes]
ADD CONSTRAINT [PK_EsquemasPagoes]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'EstadoCivils'
ALTER TABLE [dbo].[EstadoCivils]
ADD CONSTRAINT [PK_EstadoCivils]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'Estados'
ALTER TABLE [dbo].[Estados]
ADD CONSTRAINT [PK_Estados]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'Factores'
ALTER TABLE [dbo].[Factores]
ADD CONSTRAINT [PK_Factores]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'FamiliaresEmpleadoes'
ALTER TABLE [dbo].[FamiliaresEmpleadoes]
ADD CONSTRAINT [PK_FamiliaresEmpleadoes]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'Funcions'
ALTER TABLE [dbo].[Funcions]
ADD CONSTRAINT [PK_Funcions]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'Giros'
ALTER TABLE [dbo].[Giros]
ADD CONSTRAINT [PK_Giros]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [Id] in table 'Grupos'
ALTER TABLE [dbo].[Grupos]
ADD CONSTRAINT [PK_Grupos]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [id] in table 'Incapacidades'
ALTER TABLE [dbo].[Incapacidades]
ADD CONSTRAINT [PK_Incapacidades]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'ListaValidacionClientes'
ALTER TABLE [dbo].[ListaValidacionClientes]
ADD CONSTRAINT [PK_ListaValidacionClientes]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'Modulos'
ALTER TABLE [dbo].[Modulos]
ADD CONSTRAINT [PK_Modulos]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'Movimientos'
ALTER TABLE [dbo].[Movimientos]
ADD CONSTRAINT [PK_Movimientos]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'MovimientosAseguradoes'
ALTER TABLE [dbo].[MovimientosAseguradoes]
ADD CONSTRAINT [PK_MovimientosAseguradoes]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'Municipios'
ALTER TABLE [dbo].[Municipios]
ADD CONSTRAINT [PK_Municipios]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'Pagos'
ALTER TABLE [dbo].[Pagos]
ADD CONSTRAINT [PK_Pagos]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'Paises'
ALTER TABLE [dbo].[Paises]
ADD CONSTRAINT [PK_Paises]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [Id] in table 'Patrones'
ALTER TABLE [dbo].[Patrones]
ADD CONSTRAINT [PK_Patrones]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [id] in table 'Plazas'
ALTER TABLE [dbo].[Plazas]
ADD CONSTRAINT [PK_Plazas]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'PrimaRTs'
ALTER TABLE [dbo].[PrimaRTs]
ADD CONSTRAINT [PK_PrimaRTs]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'Proyectos'
ALTER TABLE [dbo].[Proyectos]
ADD CONSTRAINT [PK_Proyectos]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'RegimenInfonavits'
ALTER TABLE [dbo].[RegimenInfonavits]
ADD CONSTRAINT [PK_RegimenInfonavits]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'RepCostoSocials'
ALTER TABLE [dbo].[RepCostoSocials]
ADD CONSTRAINT [PK_RepCostoSocials]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'ReporteConMeses'
ALTER TABLE [dbo].[ReporteConMeses]
ADD CONSTRAINT [PK_ReporteConMeses]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'Residencias'
ALTER TABLE [dbo].[Residencias]
ADD CONSTRAINT [PK_Residencias]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'RespuestaSolicituds'
ALTER TABLE [dbo].[RespuestaSolicituds]
ADD CONSTRAINT [PK_RespuestaSolicituds]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'ResumenPagoes'
ALTER TABLE [dbo].[ResumenPagoes]
ADD CONSTRAINT [PK_ResumenPagoes]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'RoleFuncions'
ALTER TABLE [dbo].[RoleFuncions]
ADD CONSTRAINT [PK_RoleFuncions]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'RoleModulos'
ALTER TABLE [dbo].[RoleModulos]
ADD CONSTRAINT [PK_RoleModulos]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'Roles'
ALTER TABLE [dbo].[Roles]
ADD CONSTRAINT [PK_Roles]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'SalarialesEmpleadoes'
ALTER TABLE [dbo].[SalarialesEmpleadoes]
ADD CONSTRAINT [PK_SalarialesEmpleadoes]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'SDIs'
ALTER TABLE [dbo].[SDIs]
ADD CONSTRAINT [PK_SDIs]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'Servicios'
ALTER TABLE [dbo].[Servicios]
ADD CONSTRAINT [PK_Servicios]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'Sexos'
ALTER TABLE [dbo].[Sexos]
ADD CONSTRAINT [PK_Sexos]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'Solicituds'
ALTER TABLE [dbo].[Solicituds]
ADD CONSTRAINT [PK_Solicituds]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'SolicitudEmpleadoes'
ALTER TABLE [dbo].[SolicitudEmpleadoes]
ADD CONSTRAINT [PK_SolicitudEmpleadoes]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'SumarizadoClientes'
ALTER TABLE [dbo].[SumarizadoClientes]
ADD CONSTRAINT [PK_SumarizadoClientes]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'TipoContratoes'
ALTER TABLE [dbo].[TipoContratoes]
ADD CONSTRAINT [PK_TipoContratoes]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'TipoPersonals'
ALTER TABLE [dbo].[TipoPersonals]
ADD CONSTRAINT [PK_TipoPersonals]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'TopicosUsuarios'
ALTER TABLE [dbo].[TopicosUsuarios]
ADD CONSTRAINT [PK_TopicosUsuarios]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [Id] in table 'Usuarios'
ALTER TABLE [dbo].[Usuarios]
ADD CONSTRAINT [PK_Usuarios]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [id] in table 'Acreditados'
ALTER TABLE [dbo].[Acreditados]
ADD CONSTRAINT [FK_Acreditados_Acreditados]
    FOREIGN KEY ([id])
    REFERENCES [dbo].[Acreditados]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [clienteId] in table 'Acreditados'
ALTER TABLE [dbo].[Acreditados]
ADD CONSTRAINT [FK_Acreditados_Clientes]
    FOREIGN KEY ([clienteId])
    REFERENCES [dbo].[Clientes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Acreditados_Clientes'
CREATE INDEX [IX_FK_Acreditados_Clientes]
ON [dbo].[Acreditados]
    ([clienteId]);
GO

-- Creating foreign key on [aseguradoId] in table 'Empleados'
ALTER TABLE [dbo].[Empleados]
ADD CONSTRAINT [FK_Empleados_Acreditados]
    FOREIGN KEY ([aseguradoId])
    REFERENCES [dbo].[Acreditados]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Empleados_Acreditados'
CREATE INDEX [IX_FK_Empleados_Acreditados]
ON [dbo].[Empleados]
    ([aseguradoId]);
GO

-- Creating foreign key on [acreditadoId] in table 'Movimientos'
ALTER TABLE [dbo].[Movimientos]
ADD CONSTRAINT [FK_Movimientos_Acreditados]
    FOREIGN KEY ([acreditadoId])
    REFERENCES [dbo].[Acreditados]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Movimientos_Acreditados'
CREATE INDEX [IX_FK_Movimientos_Acreditados]
ON [dbo].[Movimientos]
    ([acreditadoId]);
GO

-- Creating foreign key on [PatroneId] in table 'Acreditados'
ALTER TABLE [dbo].[Acreditados]
ADD CONSTRAINT [FK_PatroneAcreditado]
    FOREIGN KEY ([PatroneId])
    REFERENCES [dbo].[Patrones]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_PatroneAcreditado'
CREATE INDEX [IX_FK_PatroneAcreditado]
ON [dbo].[Acreditados]
    ([PatroneId]);
GO

-- Creating foreign key on [Plaza_id] in table 'Acreditados'
ALTER TABLE [dbo].[Acreditados]
ADD CONSTRAINT [FK_PlazaAcreditado]
    FOREIGN KEY ([Plaza_id])
    REFERENCES [dbo].[Plazas]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_PlazaAcreditado'
CREATE INDEX [IX_FK_PlazaAcreditado]
ON [dbo].[Acreditados]
    ([Plaza_id]);
GO

-- Creating foreign key on [tipoArchivo] in table 'ArchivosEmpleados'
ALTER TABLE [dbo].[ArchivosEmpleados]
ADD CONSTRAINT [FK_ArchivosEmpleados_Conceptos]
    FOREIGN KEY ([tipoArchivo])
    REFERENCES [dbo].[Conceptos]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ArchivosEmpleados_Conceptos'
CREATE INDEX [IX_FK_ArchivosEmpleados_Conceptos]
ON [dbo].[ArchivosEmpleados]
    ([tipoArchivo]);
GO

-- Creating foreign key on [empleadoId] in table 'ArchivosEmpleados'
ALTER TABLE [dbo].[ArchivosEmpleados]
ADD CONSTRAINT [FK_ArchivosEmpleados_Empleados]
    FOREIGN KEY ([empleadoId])
    REFERENCES [dbo].[Empleados]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ArchivosEmpleados_Empleados'
CREATE INDEX [IX_FK_ArchivosEmpleados_Empleados]
ON [dbo].[ArchivosEmpleados]
    ([empleadoId]);
GO

-- Creating foreign key on [usuarioId] in table 'ArchivosEmpleados'
ALTER TABLE [dbo].[ArchivosEmpleados]
ADD CONSTRAINT [FK_ArchivosEmpleados_Usuarios]
    FOREIGN KEY ([usuarioId])
    REFERENCES [dbo].[Usuarios]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ArchivosEmpleados_Usuarios'
CREATE INDEX [IX_FK_ArchivosEmpleados_Usuarios]
ON [dbo].[ArchivosEmpleados]
    ([usuarioId]);
GO

-- Creating foreign key on [ClienteId] in table 'Asegurados'
ALTER TABLE [dbo].[Asegurados]
ADD CONSTRAINT [FK_Asegurados_Clientes]
    FOREIGN KEY ([ClienteId])
    REFERENCES [dbo].[Clientes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Asegurados_Clientes'
CREATE INDEX [IX_FK_Asegurados_Clientes]
ON [dbo].[Asegurados]
    ([ClienteId]);
GO

-- Creating foreign key on [aseguradoId] in table 'DetallePagoes'
ALTER TABLE [dbo].[DetallePagoes]
ADD CONSTRAINT [FK_DetallePago_Asegurados]
    FOREIGN KEY ([aseguradoId])
    REFERENCES [dbo].[Asegurados]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_DetallePago_Asegurados'
CREATE INDEX [IX_FK_DetallePago_Asegurados]
ON [dbo].[DetallePagoes]
    ([aseguradoId]);
GO

-- Creating foreign key on [aseguradoId] in table 'Incapacidades'
ALTER TABLE [dbo].[Incapacidades]
ADD CONSTRAINT [FK_Incapacidades_Asegurados]
    FOREIGN KEY ([aseguradoId])
    REFERENCES [dbo].[Asegurados]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Incapacidades_Asegurados'
CREATE INDEX [IX_FK_Incapacidades_Asegurados]
ON [dbo].[Incapacidades]
    ([aseguradoId]);
GO

-- Creating foreign key on [aseguradoId] in table 'Movimientos'
ALTER TABLE [dbo].[Movimientos]
ADD CONSTRAINT [FK_Movimientos_Asegurados]
    FOREIGN KEY ([aseguradoId])
    REFERENCES [dbo].[Asegurados]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Movimientos_Asegurados'
CREATE INDEX [IX_FK_Movimientos_Asegurados]
ON [dbo].[Movimientos]
    ([aseguradoId]);
GO

-- Creating foreign key on [aseguradoId] in table 'MovimientosAseguradoes'
ALTER TABLE [dbo].[MovimientosAseguradoes]
ADD CONSTRAINT [FK_MovimientosAsegurado_Asegurados]
    FOREIGN KEY ([aseguradoId])
    REFERENCES [dbo].[Asegurados]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_MovimientosAsegurado_Asegurados'
CREATE INDEX [IX_FK_MovimientosAsegurado_Asegurados]
ON [dbo].[MovimientosAseguradoes]
    ([aseguradoId]);
GO

-- Creating foreign key on [PatroneId] in table 'Asegurados'
ALTER TABLE [dbo].[Asegurados]
ADD CONSTRAINT [FK_PatroneAsegurado]
    FOREIGN KEY ([PatroneId])
    REFERENCES [dbo].[Patrones]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_PatroneAsegurado'
CREATE INDEX [IX_FK_PatroneAsegurado]
ON [dbo].[Asegurados]
    ([PatroneId]);
GO

-- Creating foreign key on [Plaza_id] in table 'Asegurados'
ALTER TABLE [dbo].[Asegurados]
ADD CONSTRAINT [FK_PlazaAsegurado]
    FOREIGN KEY ([Plaza_id])
    REFERENCES [dbo].[Plazas]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_PlazaAsegurado'
CREATE INDEX [IX_FK_PlazaAsegurado]
ON [dbo].[Asegurados]
    ([Plaza_id]);
GO

-- Creating foreign key on [aseguradoId] in table 'ReporteConMeses'
ALTER TABLE [dbo].[ReporteConMeses]
ADD CONSTRAINT [FK_ReporteConMese_Asegurados]
    FOREIGN KEY ([aseguradoId])
    REFERENCES [dbo].[Asegurados]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ReporteConMese_Asegurados'
CREATE INDEX [IX_FK_ReporteConMese_Asegurados]
ON [dbo].[ReporteConMeses]
    ([aseguradoId]);
GO

-- Creating foreign key on [usuarioId] in table 'Bancos'
ALTER TABLE [dbo].[Bancos]
ADD CONSTRAINT [FK_Bancos_Bancos]
    FOREIGN KEY ([usuarioId])
    REFERENCES [dbo].[Usuarios]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Bancos_Bancos'
CREATE INDEX [IX_FK_Bancos_Bancos]
ON [dbo].[Bancos]
    ([usuarioId]);
GO

-- Creating foreign key on [bancoId] in table 'CuentaEmpleadoes'
ALTER TABLE [dbo].[CuentaEmpleadoes]
ADD CONSTRAINT [FK_CuentaEmpleado_Bancos]
    FOREIGN KEY ([bancoId])
    REFERENCES [dbo].[Bancos]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_CuentaEmpleado_Bancos'
CREATE INDEX [IX_FK_CuentaEmpleado_Bancos]
ON [dbo].[CuentaEmpleadoes]
    ([bancoId]);
GO

-- Creating foreign key on [bancoId] in table 'Empleados'
ALTER TABLE [dbo].[Empleados]
ADD CONSTRAINT [FK_Empleados_Bancos]
    FOREIGN KEY ([bancoId])
    REFERENCES [dbo].[Bancos]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Empleados_Bancos'
CREATE INDEX [IX_FK_Empleados_Bancos]
ON [dbo].[Empleados]
    ([bancoId]);
GO

-- Creating foreign key on [bancoId] in table 'Pagos'
ALTER TABLE [dbo].[Pagos]
ADD CONSTRAINT [FK_Pagos_Bancos]
    FOREIGN KEY ([bancoId])
    REFERENCES [dbo].[Bancos]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Pagos_Bancos'
CREATE INDEX [IX_FK_Pagos_Bancos]
ON [dbo].[Pagos]
    ([bancoId]);
GO

-- Creating foreign key on [movimientoId] in table 'MovimientosAseguradoes'
ALTER TABLE [dbo].[MovimientosAseguradoes]
ADD CONSTRAINT [FK_MovimientosAsegurado_catalogoMovimientos]
    FOREIGN KEY ([movimientoId])
    REFERENCES [dbo].[CatalogoMovimientos]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_MovimientosAsegurado_catalogoMovimientos'
CREATE INDEX [IX_FK_MovimientosAsegurado_catalogoMovimientos]
ON [dbo].[MovimientosAseguradoes]
    ([movimientoId]);
GO

-- Creating foreign key on [Grupo_id] in table 'Clientes'
ALTER TABLE [dbo].[Clientes]
ADD CONSTRAINT [FK_Clientes_Grupos]
    FOREIGN KEY ([Grupo_id])
    REFERENCES [dbo].[Grupos]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Clientes_Grupos'
CREATE INDEX [IX_FK_Clientes_Grupos]
ON [dbo].[Clientes]
    ([Grupo_id]);
GO

-- Creating foreign key on [Plaza_id] in table 'Clientes'
ALTER TABLE [dbo].[Clientes]
ADD CONSTRAINT [FK_Clientes_Plazas]
    FOREIGN KEY ([Plaza_id])
    REFERENCES [dbo].[Plazas]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Clientes_Plazas'
CREATE INDEX [IX_FK_Clientes_Plazas]
ON [dbo].[Clientes]
    ([Plaza_id]);
GO

-- Creating foreign key on [ejecutivoContadorId] in table 'Clientes'
ALTER TABLE [dbo].[Clientes]
ADD CONSTRAINT [FK_Clientes_Usuarios]
    FOREIGN KEY ([ejecutivoContadorId])
    REFERENCES [dbo].[Usuarios]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Clientes_Usuarios'
CREATE INDEX [IX_FK_Clientes_Usuarios]
ON [dbo].[Clientes]
    ([ejecutivoContadorId]);
GO

-- Creating foreign key on [clienteId] in table 'ContratosClientes'
ALTER TABLE [dbo].[ContratosClientes]
ADD CONSTRAINT [FK_ContratosCliente_Clientes]
    FOREIGN KEY ([clienteId])
    REFERENCES [dbo].[Clientes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ContratosCliente_Clientes'
CREATE INDEX [IX_FK_ContratosCliente_Clientes]
ON [dbo].[ContratosClientes]
    ([clienteId]);
GO

-- Creating foreign key on [clienteId] in table 'DatosAdicionalesClientes'
ALTER TABLE [dbo].[DatosAdicionalesClientes]
ADD CONSTRAINT [FK_DatosAdicionalesCliente_Clientes]
    FOREIGN KEY ([clienteId])
    REFERENCES [dbo].[Clientes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_DatosAdicionalesCliente_Clientes'
CREATE INDEX [IX_FK_DatosAdicionalesCliente_Clientes]
ON [dbo].[DatosAdicionalesClientes]
    ([clienteId]);
GO

-- Creating foreign key on [clienteId] in table 'ListaValidacionClientes'
ALTER TABLE [dbo].[ListaValidacionClientes]
ADD CONSTRAINT [FK_ListaValidacionCliente_Clientes]
    FOREIGN KEY ([clienteId])
    REFERENCES [dbo].[Clientes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ListaValidacionCliente_Clientes'
CREATE INDEX [IX_FK_ListaValidacionCliente_Clientes]
ON [dbo].[ListaValidacionClientes]
    ([clienteId]);
GO

-- Creating foreign key on [clienteId] in table 'Solicituds'
ALTER TABLE [dbo].[Solicituds]
ADD CONSTRAINT [FK_Proyectos_Clientes]
    FOREIGN KEY ([clienteId])
    REFERENCES [dbo].[Clientes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Proyectos_Clientes'
CREATE INDEX [IX_FK_Proyectos_Clientes]
ON [dbo].[Solicituds]
    ([clienteId]);
GO

-- Creating foreign key on [clienteId] in table 'Proyectos'
ALTER TABLE [dbo].[Proyectos]
ADD CONSTRAINT [FK_Proyectos_Clientes1]
    FOREIGN KEY ([clienteId])
    REFERENCES [dbo].[Clientes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Proyectos_Clientes1'
CREATE INDEX [IX_FK_Proyectos_Clientes1]
ON [dbo].[Proyectos]
    ([clienteId]);
GO

-- Creating foreign key on [clienteId] in table 'ReporteConMeses'
ALTER TABLE [dbo].[ReporteConMeses]
ADD CONSTRAINT [FK_ReporteConMeses_Asegurados]
    FOREIGN KEY ([clienteId])
    REFERENCES [dbo].[Clientes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ReporteConMeses_Asegurados'
CREATE INDEX [IX_FK_ReporteConMeses_Asegurados]
ON [dbo].[ReporteConMeses]
    ([clienteId]);
GO

-- Creating foreign key on [clienteId] in table 'SDIs'
ALTER TABLE [dbo].[SDIs]
ADD CONSTRAINT [FK_SDIs_Clientes]
    FOREIGN KEY ([clienteId])
    REFERENCES [dbo].[Clientes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SDIs_Clientes'
CREATE INDEX [IX_FK_SDIs_Clientes]
ON [dbo].[SDIs]
    ([clienteId]);
GO

-- Creating foreign key on [clienteId] in table 'SumarizadoClientes'
ALTER TABLE [dbo].[SumarizadoClientes]
ADD CONSTRAINT [FK_SumarizadoClientes_Asegurados]
    FOREIGN KEY ([clienteId])
    REFERENCES [dbo].[Clientes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SumarizadoClientes_Asegurados'
CREATE INDEX [IX_FK_SumarizadoClientes_Asegurados]
ON [dbo].[SumarizadoClientes]
    ([clienteId]);
GO

-- Creating foreign key on [usuarioId] in table 'Conceptos'
ALTER TABLE [dbo].[Conceptos]
ADD CONSTRAINT [FK_Conceptos_Usuarios]
    FOREIGN KEY ([usuarioId])
    REFERENCES [dbo].[Usuarios]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Conceptos_Usuarios'
CREATE INDEX [IX_FK_Conceptos_Usuarios]
ON [dbo].[Conceptos]
    ([usuarioId]);
GO

-- Creating foreign key on [jornadaLaboralId] in table 'DocumentosEmpleadoes'
ALTER TABLE [dbo].[DocumentosEmpleadoes]
ADD CONSTRAINT [FK_documentosEmpleado_Conceptos]
    FOREIGN KEY ([jornadaLaboralId])
    REFERENCES [dbo].[Conceptos]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_documentosEmpleado_Conceptos'
CREATE INDEX [IX_FK_documentosEmpleado_Conceptos]
ON [dbo].[DocumentosEmpleadoes]
    ([jornadaLaboralId]);
GO

-- Creating foreign key on [parentescoId] in table 'FamiliaresEmpleadoes'
ALTER TABLE [dbo].[FamiliaresEmpleadoes]
ADD CONSTRAINT [FK_FamiliaresEmpleado_Conceptos]
    FOREIGN KEY ([parentescoId])
    REFERENCES [dbo].[Conceptos]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_FamiliaresEmpleado_Conceptos'
CREATE INDEX [IX_FK_FamiliaresEmpleado_Conceptos]
ON [dbo].[FamiliaresEmpleadoes]
    ([parentescoId]);
GO

-- Creating foreign key on [estatusId] in table 'RespuestaSolicituds'
ALTER TABLE [dbo].[RespuestaSolicituds]
ADD CONSTRAINT [FK_RespuestaSolicitud_Conceptos]
    FOREIGN KEY ([estatusId])
    REFERENCES [dbo].[Conceptos]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_RespuestaSolicitud_Conceptos'
CREATE INDEX [IX_FK_RespuestaSolicitud_Conceptos]
ON [dbo].[RespuestaSolicituds]
    ([estatusId]);
GO

-- Creating foreign key on [periodoId] in table 'SalarialesEmpleadoes'
ALTER TABLE [dbo].[SalarialesEmpleadoes]
ADD CONSTRAINT [FK_SalarialesEmpleado_Conceptos]
    FOREIGN KEY ([periodoId])
    REFERENCES [dbo].[Conceptos]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SalarialesEmpleado_Conceptos'
CREATE INDEX [IX_FK_SalarialesEmpleado_Conceptos]
ON [dbo].[SalarialesEmpleadoes]
    ([periodoId]);
GO

-- Creating foreign key on [estatusSolicitud] in table 'Solicituds'
ALTER TABLE [dbo].[Solicituds]
ADD CONSTRAINT [FK_Solicitud_Conceptos]
    FOREIGN KEY ([estatusSolicitud])
    REFERENCES [dbo].[Conceptos]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Solicitud_Conceptos'
CREATE INDEX [IX_FK_Solicitud_Conceptos]
ON [dbo].[Solicituds]
    ([estatusSolicitud]);
GO

-- Creating foreign key on [estatusNomina] in table 'Solicituds'
ALTER TABLE [dbo].[Solicituds]
ADD CONSTRAINT [FK_Solicitud_Conceptos1]
    FOREIGN KEY ([estatusNomina])
    REFERENCES [dbo].[Conceptos]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Solicitud_Conceptos1'
CREATE INDEX [IX_FK_Solicitud_Conceptos1]
ON [dbo].[Solicituds]
    ([estatusNomina]);
GO

-- Creating foreign key on [estatusJuridico] in table 'Solicituds'
ALTER TABLE [dbo].[Solicituds]
ADD CONSTRAINT [FK_Solicitud_Conceptos2]
    FOREIGN KEY ([estatusJuridico])
    REFERENCES [dbo].[Conceptos]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Solicitud_Conceptos2'
CREATE INDEX [IX_FK_Solicitud_Conceptos2]
ON [dbo].[Solicituds]
    ([estatusJuridico]);
GO

-- Creating foreign key on [estatusAfiliado] in table 'Solicituds'
ALTER TABLE [dbo].[Solicituds]
ADD CONSTRAINT [FK_Solicitud_Conceptos3]
    FOREIGN KEY ([estatusAfiliado])
    REFERENCES [dbo].[Conceptos]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Solicitud_Conceptos3'
CREATE INDEX [IX_FK_Solicitud_Conceptos3]
ON [dbo].[Solicituds]
    ([estatusAfiliado]);
GO

-- Creating foreign key on [estatusTarjeta] in table 'Solicituds'
ALTER TABLE [dbo].[Solicituds]
ADD CONSTRAINT [FK_Solicitud_Conceptos4]
    FOREIGN KEY ([estatusTarjeta])
    REFERENCES [dbo].[Conceptos]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Solicitud_Conceptos4'
CREATE INDEX [IX_FK_Solicitud_Conceptos4]
ON [dbo].[Solicituds]
    ([estatusTarjeta]);
GO

-- Creating foreign key on [tipoSolicitud] in table 'Solicituds'
ALTER TABLE [dbo].[Solicituds]
ADD CONSTRAINT [FK_Solicitud_Conceptos5]
    FOREIGN KEY ([tipoSolicitud])
    REFERENCES [dbo].[Conceptos]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Solicitud_Conceptos5'
CREATE INDEX [IX_FK_Solicitud_Conceptos5]
ON [dbo].[Solicituds]
    ([tipoSolicitud]);
GO

-- Creating foreign key on [tipoId] in table 'SolicitudEmpleadoes'
ALTER TABLE [dbo].[SolicitudEmpleadoes]
ADD CONSTRAINT [FK_SolicitudEmpleado_Conceptos]
    FOREIGN KEY ([tipoId])
    REFERENCES [dbo].[Conceptos]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SolicitudEmpleado_Conceptos'
CREATE INDEX [IX_FK_SolicitudEmpleado_Conceptos]
ON [dbo].[SolicitudEmpleadoes]
    ([tipoId]);
GO

-- Creating foreign key on [usuarioId] in table 'ContratosClientes'
ALTER TABLE [dbo].[ContratosClientes]
ADD CONSTRAINT [FK_ContratosCliente_Usuarios]
    FOREIGN KEY ([usuarioId])
    REFERENCES [dbo].[Usuarios]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ContratosCliente_Usuarios'
CREATE INDEX [IX_FK_ContratosCliente_Usuarios]
ON [dbo].[ContratosClientes]
    ([usuarioId]);
GO

-- Creating foreign key on [empleadoId] in table 'CuentaEmpleadoes'
ALTER TABLE [dbo].[CuentaEmpleadoes]
ADD CONSTRAINT [FK_CuentaEmpleado_Empleados]
    FOREIGN KEY ([empleadoId])
    REFERENCES [dbo].[Empleados]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_CuentaEmpleado_Empleados'
CREATE INDEX [IX_FK_CuentaEmpleado_Empleados]
ON [dbo].[CuentaEmpleadoes]
    ([empleadoId]);
GO

-- Creating foreign key on [usuarioId] in table 'CuentaEmpleadoes'
ALTER TABLE [dbo].[CuentaEmpleadoes]
ADD CONSTRAINT [FK_CuentaEmpleado_Usuarios]
    FOREIGN KEY ([usuarioId])
    REFERENCES [dbo].[Usuarios]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_CuentaEmpleado_Usuarios'
CREATE INDEX [IX_FK_CuentaEmpleado_Usuarios]
ON [dbo].[CuentaEmpleadoes]
    ([usuarioId]);
GO

-- Creating foreign key on [usuarioId] in table 'DatosAdicionalesClientes'
ALTER TABLE [dbo].[DatosAdicionalesClientes]
ADD CONSTRAINT [FK_DatosAdicionalesCliente_Usuarios]
    FOREIGN KEY ([usuarioId])
    REFERENCES [dbo].[Usuarios]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_DatosAdicionalesCliente_Usuarios'
CREATE INDEX [IX_FK_DatosAdicionalesCliente_Usuarios]
ON [dbo].[DatosAdicionalesClientes]
    ([usuarioId]);
GO

-- Creating foreign key on [usuarioId] in table 'Departamentos'
ALTER TABLE [dbo].[Departamentos]
ADD CONSTRAINT [FK_Departamentos_Departamentos]
    FOREIGN KEY ([usuarioId])
    REFERENCES [dbo].[Usuarios]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Departamentos_Departamentos'
CREATE INDEX [IX_FK_Departamentos_Departamentos]
ON [dbo].[Departamentos]
    ([usuarioId]);
GO

-- Creating foreign key on [departamentoId] in table 'RespuestaSolicituds'
ALTER TABLE [dbo].[RespuestaSolicituds]
ADD CONSTRAINT [FK_RespuestaSolicitud_Departamentos]
    FOREIGN KEY ([departamentoId])
    REFERENCES [dbo].[Departamentos]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_RespuestaSolicitud_Departamentos'
CREATE INDEX [IX_FK_RespuestaSolicitud_Departamentos]
ON [dbo].[RespuestaSolicituds]
    ([departamentoId]);
GO

-- Creating foreign key on [departamentoId] in table 'Usuarios'
ALTER TABLE [dbo].[Usuarios]
ADD CONSTRAINT [FK_Usuarios_Departamentos]
    FOREIGN KEY ([departamentoId])
    REFERENCES [dbo].[Departamentos]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Usuarios_Departamentos'
CREATE INDEX [IX_FK_Usuarios_Departamentos]
ON [dbo].[Usuarios]
    ([departamentoId]);
GO

-- Creating foreign key on [pagoId] in table 'DetallePagoes'
ALTER TABLE [dbo].[DetallePagoes]
ADD CONSTRAINT [FK_DetallePago_Pagos]
    FOREIGN KEY ([pagoId])
    REFERENCES [dbo].[Pagos]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_DetallePago_Pagos'
CREATE INDEX [IX_FK_DetallePago_Pagos]
ON [dbo].[DetallePagoes]
    ([pagoId]);
GO

-- Creating foreign key on [patronId] in table 'DetallePagoes'
ALTER TABLE [dbo].[DetallePagoes]
ADD CONSTRAINT [FK_DetallePago_Patrones]
    FOREIGN KEY ([patronId])
    REFERENCES [dbo].[Patrones]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_DetallePago_Patrones'
CREATE INDEX [IX_FK_DetallePago_Patrones]
ON [dbo].[DetallePagoes]
    ([patronId]);
GO

-- Creating foreign key on [usuarioId] in table 'DetallePagoes'
ALTER TABLE [dbo].[DetallePagoes]
ADD CONSTRAINT [FK_DetallePago_Usuarios]
    FOREIGN KEY ([usuarioId])
    REFERENCES [dbo].[Usuarios]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_DetallePago_Usuarios'
CREATE INDEX [IX_FK_DetallePago_Usuarios]
ON [dbo].[DetallePagoes]
    ([usuarioId]);
GO

-- Creating foreign key on [empleadoId] in table 'DocumentosEmpleadoes'
ALTER TABLE [dbo].[DocumentosEmpleadoes]
ADD CONSTRAINT [FK_documentosEmpleado_Empleados]
    FOREIGN KEY ([empleadoId])
    REFERENCES [dbo].[Empleados]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_documentosEmpleado_Empleados'
CREATE INDEX [IX_FK_documentosEmpleado_Empleados]
ON [dbo].[DocumentosEmpleadoes]
    ([empleadoId]);
GO

-- Creating foreign key on [usuarioId] in table 'DocumentosEmpleadoes'
ALTER TABLE [dbo].[DocumentosEmpleadoes]
ADD CONSTRAINT [FK_documentosEmpleado_Usuarios]
    FOREIGN KEY ([usuarioId])
    REFERENCES [dbo].[Usuarios]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_documentosEmpleado_Usuarios'
CREATE INDEX [IX_FK_documentosEmpleado_Usuarios]
ON [dbo].[DocumentosEmpleadoes]
    ([usuarioId]);
GO

-- Creating foreign key on [esquemaPagoId] in table 'Empleados'
ALTER TABLE [dbo].[Empleados]
ADD CONSTRAINT [FK_Empleados_EsquemasPago]
    FOREIGN KEY ([esquemaPagoId])
    REFERENCES [dbo].[EsquemasPagoes]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Empleados_EsquemasPago'
CREATE INDEX [IX_FK_Empleados_EsquemasPago]
ON [dbo].[Empleados]
    ([esquemaPagoId]);
GO

-- Creating foreign key on [estadoCivilId] in table 'Empleados'
ALTER TABLE [dbo].[Empleados]
ADD CONSTRAINT [FK_Empleados_EstadoCivil]
    FOREIGN KEY ([estadoCivilId])
    REFERENCES [dbo].[EstadoCivils]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Empleados_EstadoCivil'
CREATE INDEX [IX_FK_Empleados_EstadoCivil]
ON [dbo].[Empleados]
    ([estadoCivilId]);
GO

-- Creating foreign key on [estadoNacimientoId] in table 'Empleados'
ALTER TABLE [dbo].[Empleados]
ADD CONSTRAINT [FK_Empleados_Estados]
    FOREIGN KEY ([estadoNacimientoId])
    REFERENCES [dbo].[Estados]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Empleados_Estados'
CREATE INDEX [IX_FK_Empleados_Estados]
ON [dbo].[Empleados]
    ([estadoNacimientoId]);
GO

-- Creating foreign key on [municipioNacimientoId] in table 'Empleados'
ALTER TABLE [dbo].[Empleados]
ADD CONSTRAINT [FK_Empleados_Municipios]
    FOREIGN KEY ([municipioNacimientoId])
    REFERENCES [dbo].[Municipios]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Empleados_Municipios'
CREATE INDEX [IX_FK_Empleados_Municipios]
ON [dbo].[Empleados]
    ([municipioNacimientoId]);
GO

-- Creating foreign key on [nacionalidadId] in table 'Empleados'
ALTER TABLE [dbo].[Empleados]
ADD CONSTRAINT [FK_Empleados_Paises]
    FOREIGN KEY ([nacionalidadId])
    REFERENCES [dbo].[Paises]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Empleados_Paises'
CREATE INDEX [IX_FK_Empleados_Paises]
ON [dbo].[Empleados]
    ([nacionalidadId]);
GO

-- Creating foreign key on [sdiId] in table 'Empleados'
ALTER TABLE [dbo].[Empleados]
ADD CONSTRAINT [FK_Empleados_SDIs]
    FOREIGN KEY ([sdiId])
    REFERENCES [dbo].[SDIs]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Empleados_SDIs'
CREATE INDEX [IX_FK_Empleados_SDIs]
ON [dbo].[Empleados]
    ([sdiId]);
GO

-- Creating foreign key on [sexoId] in table 'Empleados'
ALTER TABLE [dbo].[Empleados]
ADD CONSTRAINT [FK_Empleados_Sexos]
    FOREIGN KEY ([sexoId])
    REFERENCES [dbo].[Sexos]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Empleados_Sexos'
CREATE INDEX [IX_FK_Empleados_Sexos]
ON [dbo].[Empleados]
    ([sexoId]);
GO

-- Creating foreign key on [solicitudId] in table 'Empleados'
ALTER TABLE [dbo].[Empleados]
ADD CONSTRAINT [FK_Empleados_Solicitud]
    FOREIGN KEY ([solicitudId])
    REFERENCES [dbo].[Solicituds]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Empleados_Solicitud'
CREATE INDEX [IX_FK_Empleados_Solicitud]
ON [dbo].[Empleados]
    ([solicitudId]);
GO

-- Creating foreign key on [usuarioId] in table 'Empleados'
ALTER TABLE [dbo].[Empleados]
ADD CONSTRAINT [FK_Empleados_Usuarios]
    FOREIGN KEY ([usuarioId])
    REFERENCES [dbo].[Usuarios]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Empleados_Usuarios'
CREATE INDEX [IX_FK_Empleados_Usuarios]
ON [dbo].[Empleados]
    ([usuarioId]);
GO

-- Creating foreign key on [empleadoId] in table 'FamiliaresEmpleadoes'
ALTER TABLE [dbo].[FamiliaresEmpleadoes]
ADD CONSTRAINT [FK_FamiliaresEmpleado_Empleados]
    FOREIGN KEY ([empleadoId])
    REFERENCES [dbo].[Empleados]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_FamiliaresEmpleado_Empleados'
CREATE INDEX [IX_FK_FamiliaresEmpleado_Empleados]
ON [dbo].[FamiliaresEmpleadoes]
    ([empleadoId]);
GO

-- Creating foreign key on [empleadoId] in table 'SalarialesEmpleadoes'
ALTER TABLE [dbo].[SalarialesEmpleadoes]
ADD CONSTRAINT [FK_SalarialesEmpleado_Empleados]
    FOREIGN KEY ([empleadoId])
    REFERENCES [dbo].[Empleados]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SalarialesEmpleado_Empleados'
CREATE INDEX [IX_FK_SalarialesEmpleado_Empleados]
ON [dbo].[SalarialesEmpleadoes]
    ([empleadoId]);
GO

-- Creating foreign key on [empleadoId] in table 'SolicitudEmpleadoes'
ALTER TABLE [dbo].[SolicitudEmpleadoes]
ADD CONSTRAINT [FK_SolicitudEmpleado_SolicitudEmpleadoII]
    FOREIGN KEY ([empleadoId])
    REFERENCES [dbo].[Empleados]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SolicitudEmpleado_SolicitudEmpleadoII'
CREATE INDEX [IX_FK_SolicitudEmpleado_SolicitudEmpleadoII]
ON [dbo].[SolicitudEmpleadoes]
    ([empleadoId]);
GO

-- Creating foreign key on [usuarioId] in table 'Empresas'
ALTER TABLE [dbo].[Empresas]
ADD CONSTRAINT [FK_Empresas_Empresas]
    FOREIGN KEY ([usuarioId])
    REFERENCES [dbo].[Usuarios]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Empresas_Empresas'
CREATE INDEX [IX_FK_Empresas_Empresas]
ON [dbo].[Empresas]
    ([usuarioId]);
GO

-- Creating foreign key on [usuarioId] in table 'EsquemasPagoes'
ALTER TABLE [dbo].[EsquemasPagoes]
ADD CONSTRAINT [FK_EsquemasPago_EsquemasPago]
    FOREIGN KEY ([usuarioId])
    REFERENCES [dbo].[Usuarios]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_EsquemasPago_EsquemasPago'
CREATE INDEX [IX_FK_EsquemasPago_EsquemasPago]
ON [dbo].[EsquemasPagoes]
    ([usuarioId]);
GO

-- Creating foreign key on [esquemaId] in table 'Solicituds'
ALTER TABLE [dbo].[Solicituds]
ADD CONSTRAINT [FK_Proyectos_EsquemasPago]
    FOREIGN KEY ([esquemaId])
    REFERENCES [dbo].[EsquemasPagoes]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Proyectos_EsquemasPago'
CREATE INDEX [IX_FK_Proyectos_EsquemasPago]
ON [dbo].[Solicituds]
    ([esquemaId]);
GO

-- Creating foreign key on [usuarioId] in table 'EstadoCivils'
ALTER TABLE [dbo].[EstadoCivils]
ADD CONSTRAINT [FK_EstadoCivil_EstadoCivil]
    FOREIGN KEY ([usuarioId])
    REFERENCES [dbo].[Usuarios]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_EstadoCivil_EstadoCivil'
CREATE INDEX [IX_FK_EstadoCivil_EstadoCivil]
ON [dbo].[EstadoCivils]
    ([usuarioId]);
GO

-- Creating foreign key on [paisId] in table 'Estados'
ALTER TABLE [dbo].[Estados]
ADD CONSTRAINT [FK_Estados_Paises]
    FOREIGN KEY ([paisId])
    REFERENCES [dbo].[Paises]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Estados_Paises'
CREATE INDEX [IX_FK_Estados_Paises]
ON [dbo].[Estados]
    ([paisId]);
GO

-- Creating foreign key on [usuarioId] in table 'Estados'
ALTER TABLE [dbo].[Estados]
ADD CONSTRAINT [FK_Estados_Usuarios]
    FOREIGN KEY ([usuarioId])
    REFERENCES [dbo].[Usuarios]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Estados_Usuarios'
CREATE INDEX [IX_FK_Estados_Usuarios]
ON [dbo].[Estados]
    ([usuarioId]);
GO

-- Creating foreign key on [estadoId] in table 'Municipios'
ALTER TABLE [dbo].[Municipios]
ADD CONSTRAINT [FK_Municipios_Estados]
    FOREIGN KEY ([estadoId])
    REFERENCES [dbo].[Estados]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Municipios_Estados'
CREATE INDEX [IX_FK_Municipios_Estados]
ON [dbo].[Municipios]
    ([estadoId]);
GO

-- Creating foreign key on [usuarioId] in table 'Factores'
ALTER TABLE [dbo].[Factores]
ADD CONSTRAINT [FK_Factores_Usuarios]
    FOREIGN KEY ([usuarioId])
    REFERENCES [dbo].[Usuarios]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Factores_Usuarios'
CREATE INDEX [IX_FK_Factores_Usuarios]
ON [dbo].[Factores]
    ([usuarioId]);
GO

-- Creating foreign key on [usuarioId] in table 'FamiliaresEmpleadoes'
ALTER TABLE [dbo].[FamiliaresEmpleadoes]
ADD CONSTRAINT [FK_FamiliaresEmpleado_Usuarios]
    FOREIGN KEY ([usuarioId])
    REFERENCES [dbo].[Usuarios]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_FamiliaresEmpleado_Usuarios'
CREATE INDEX [IX_FK_FamiliaresEmpleado_Usuarios]
ON [dbo].[FamiliaresEmpleadoes]
    ([usuarioId]);
GO

-- Creating foreign key on [moduloId] in table 'Funcions'
ALTER TABLE [dbo].[Funcions]
ADD CONSTRAINT [FK_Funciones_Modulos]
    FOREIGN KEY ([moduloId])
    REFERENCES [dbo].[Modulos]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Funciones_Modulos'
CREATE INDEX [IX_FK_Funciones_Modulos]
ON [dbo].[Funcions]
    ([moduloId]);
GO

-- Creating foreign key on [funcionId] in table 'RoleFuncions'
ALTER TABLE [dbo].[RoleFuncions]
ADD CONSTRAINT [FK_RoleFunciones_Funciones]
    FOREIGN KEY ([funcionId])
    REFERENCES [dbo].[Funcions]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_RoleFunciones_Funciones'
CREATE INDEX [IX_FK_RoleFunciones_Funciones]
ON [dbo].[RoleFuncions]
    ([funcionId]);
GO

-- Creating foreign key on [usuarioId] in table 'Giros'
ALTER TABLE [dbo].[Giros]
ADD CONSTRAINT [FK_Giros_Giros]
    FOREIGN KEY ([usuarioId])
    REFERENCES [dbo].[Usuarios]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Giros_Giros'
CREATE INDEX [IX_FK_Giros_Giros]
ON [dbo].[Giros]
    ([usuarioId]);
GO

-- Creating foreign key on [Plaza_id] in table 'Grupos'
ALTER TABLE [dbo].[Grupos]
ADD CONSTRAINT [FK_PlazaGrupos]
    FOREIGN KEY ([Plaza_id])
    REFERENCES [dbo].[Plazas]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_PlazaGrupos'
CREATE INDEX [IX_FK_PlazaGrupos]
ON [dbo].[Grupos]
    ([Plaza_id]);
GO

-- Creating foreign key on [incapacidadId] in table 'MovimientosAseguradoes'
ALTER TABLE [dbo].[MovimientosAseguradoes]
ADD CONSTRAINT [FK_MovimientosAsegurado_Incapacidades]
    FOREIGN KEY ([incapacidadId])
    REFERENCES [dbo].[Incapacidades]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_MovimientosAsegurado_Incapacidades'
CREATE INDEX [IX_FK_MovimientosAsegurado_Incapacidades]
ON [dbo].[MovimientosAseguradoes]
    ([incapacidadId]);
GO

-- Creating foreign key on [usuarioId] in table 'ListaValidacionClientes'
ALTER TABLE [dbo].[ListaValidacionClientes]
ADD CONSTRAINT [FK_ListaValidacionCliente_Usuarios]
    FOREIGN KEY ([usuarioId])
    REFERENCES [dbo].[Usuarios]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ListaValidacionCliente_Usuarios'
CREATE INDEX [IX_FK_ListaValidacionCliente_Usuarios]
ON [dbo].[ListaValidacionClientes]
    ([usuarioId]);
GO

-- Creating foreign key on [moduloId] in table 'RoleModulos'
ALTER TABLE [dbo].[RoleModulos]
ADD CONSTRAINT [FK_RoleModulos_Modulos]
    FOREIGN KEY ([moduloId])
    REFERENCES [dbo].[Modulos]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_RoleModulos_Modulos'
CREATE INDEX [IX_FK_RoleModulos_Modulos]
ON [dbo].[RoleModulos]
    ([moduloId]);
GO

-- Creating foreign key on [paisId] in table 'Municipios'
ALTER TABLE [dbo].[Municipios]
ADD CONSTRAINT [FK_Municipios_Paises]
    FOREIGN KEY ([paisId])
    REFERENCES [dbo].[Paises]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Municipios_Paises'
CREATE INDEX [IX_FK_Municipios_Paises]
ON [dbo].[Municipios]
    ([paisId]);
GO

-- Creating foreign key on [usuarioId] in table 'Municipios'
ALTER TABLE [dbo].[Municipios]
ADD CONSTRAINT [FK_Municipios_Usuarios]
    FOREIGN KEY ([usuarioId])
    REFERENCES [dbo].[Usuarios]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Municipios_Usuarios'
CREATE INDEX [IX_FK_Municipios_Usuarios]
ON [dbo].[Municipios]
    ([usuarioId]);
GO

-- Creating foreign key on [patronId] in table 'Pagos'
ALTER TABLE [dbo].[Pagos]
ADD CONSTRAINT [FK_Pagos_Patrones]
    FOREIGN KEY ([patronId])
    REFERENCES [dbo].[Patrones]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Pagos_Patrones'
CREATE INDEX [IX_FK_Pagos_Patrones]
ON [dbo].[Pagos]
    ([patronId]);
GO

-- Creating foreign key on [usuarioId] in table 'Pagos'
ALTER TABLE [dbo].[Pagos]
ADD CONSTRAINT [FK_Pagos_Usuarios]
    FOREIGN KEY ([usuarioId])
    REFERENCES [dbo].[Usuarios]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Pagos_Usuarios'
CREATE INDEX [IX_FK_Pagos_Usuarios]
ON [dbo].[Pagos]
    ([usuarioId]);
GO

-- Creating foreign key on [pagoId] in table 'ResumenPagoes'
ALTER TABLE [dbo].[ResumenPagoes]
ADD CONSTRAINT [FK_ResumenPago_Pagos]
    FOREIGN KEY ([pagoId])
    REFERENCES [dbo].[Pagos]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ResumenPago_Pagos'
CREATE INDEX [IX_FK_ResumenPago_Pagos]
ON [dbo].[ResumenPagoes]
    ([pagoId]);
GO

-- Creating foreign key on [usuarioId] in table 'Paises'
ALTER TABLE [dbo].[Paises]
ADD CONSTRAINT [FK_Paises_Paises]
    FOREIGN KEY ([usuarioId])
    REFERENCES [dbo].[Usuarios]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Paises_Paises'
CREATE INDEX [IX_FK_Paises_Paises]
ON [dbo].[Paises]
    ([usuarioId]);
GO

-- Creating foreign key on [Plaza_id] in table 'Patrones'
ALTER TABLE [dbo].[Patrones]
ADD CONSTRAINT [FK_PlazaPatrone]
    FOREIGN KEY ([Plaza_id])
    REFERENCES [dbo].[Plazas]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_PlazaPatrone'
CREATE INDEX [IX_FK_PlazaPatrone]
ON [dbo].[Patrones]
    ([Plaza_id]);
GO

-- Creating foreign key on [patronId] in table 'ReporteConMeses'
ALTER TABLE [dbo].[ReporteConMeses]
ADD CONSTRAINT [FK_ReporteConMeses_Patrones]
    FOREIGN KEY ([patronId])
    REFERENCES [dbo].[Patrones]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ReporteConMeses_Patrones'
CREATE INDEX [IX_FK_ReporteConMeses_Patrones]
ON [dbo].[ReporteConMeses]
    ([patronId]);
GO

-- Creating foreign key on [patronId] in table 'ResumenPagoes'
ALTER TABLE [dbo].[ResumenPagoes]
ADD CONSTRAINT [FK_ResumenPago_Patrones]
    FOREIGN KEY ([patronId])
    REFERENCES [dbo].[Patrones]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ResumenPago_Patrones'
CREATE INDEX [IX_FK_ResumenPago_Patrones]
ON [dbo].[ResumenPagoes]
    ([patronId]);
GO

-- Creating foreign key on [patronId] in table 'SumarizadoClientes'
ALTER TABLE [dbo].[SumarizadoClientes]
ADD CONSTRAINT [FK_SumarizadoClientes_Patrones]
    FOREIGN KEY ([patronId])
    REFERENCES [dbo].[Patrones]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SumarizadoClientes_Patrones'
CREATE INDEX [IX_FK_SumarizadoClientes_Patrones]
ON [dbo].[SumarizadoClientes]
    ([patronId]);
GO

-- Creating foreign key on [plazaId] in table 'Solicituds'
ALTER TABLE [dbo].[Solicituds]
ADD CONSTRAINT [FK_Solicitud_Plazas]
    FOREIGN KEY ([plazaId])
    REFERENCES [dbo].[Plazas]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Solicitud_Plazas'
CREATE INDEX [IX_FK_Solicitud_Plazas]
ON [dbo].[Solicituds]
    ([plazaId]);
GO

-- Creating foreign key on [plazaId] in table 'Usuarios'
ALTER TABLE [dbo].[Usuarios]
ADD CONSTRAINT [FK_Usuarios_Plazas]
    FOREIGN KEY ([plazaId])
    REFERENCES [dbo].[Plazas]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Usuarios_Plazas'
CREATE INDEX [IX_FK_Usuarios_Plazas]
ON [dbo].[Usuarios]
    ([plazaId]);
GO

-- Creating foreign key on [proyectoId] in table 'Solicituds'
ALTER TABLE [dbo].[Solicituds]
ADD CONSTRAINT [FK_Solicitud_Proyectos]
    FOREIGN KEY ([proyectoId])
    REFERENCES [dbo].[Proyectos]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Solicitud_Proyectos'
CREATE INDEX [IX_FK_Solicitud_Proyectos]
ON [dbo].[Solicituds]
    ([proyectoId]);
GO

-- Creating foreign key on [usuarioId] in table 'RegimenInfonavits'
ALTER TABLE [dbo].[RegimenInfonavits]
ADD CONSTRAINT [FK_RegimenInfonavit_RegimenInfonavit]
    FOREIGN KEY ([usuarioId])
    REFERENCES [dbo].[Usuarios]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_RegimenInfonavit_RegimenInfonavit'
CREATE INDEX [IX_FK_RegimenInfonavit_RegimenInfonavit]
ON [dbo].[RegimenInfonavits]
    ([usuarioId]);
GO

-- Creating foreign key on [usuarioId] in table 'ReporteConMeses'
ALTER TABLE [dbo].[ReporteConMeses]
ADD CONSTRAINT [FK_ReporteConMeses_Usuarios]
    FOREIGN KEY ([usuarioId])
    REFERENCES [dbo].[Usuarios]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ReporteConMeses_Usuarios'
CREATE INDEX [IX_FK_ReporteConMeses_Usuarios]
ON [dbo].[ReporteConMeses]
    ([usuarioId]);
GO

-- Creating foreign key on [usuarioId] in table 'Residencias'
ALTER TABLE [dbo].[Residencias]
ADD CONSTRAINT [FK_Residencia_Residencia]
    FOREIGN KEY ([usuarioId])
    REFERENCES [dbo].[Usuarios]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Residencia_Residencia'
CREATE INDEX [IX_FK_Residencia_Residencia]
ON [dbo].[Residencias]
    ([usuarioId]);
GO

-- Creating foreign key on [id] in table 'RespuestaSolicituds'
ALTER TABLE [dbo].[RespuestaSolicituds]
ADD CONSTRAINT [FK_RespuestaSolicitud_RespuestaSolicitud]
    FOREIGN KEY ([id])
    REFERENCES [dbo].[Solicituds]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [usuarioId] in table 'RespuestaSolicituds'
ALTER TABLE [dbo].[RespuestaSolicituds]
ADD CONSTRAINT [FK_RespuestaSolicitud_Usuarios]
    FOREIGN KEY ([usuarioId])
    REFERENCES [dbo].[Usuarios]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_RespuestaSolicitud_Usuarios'
CREATE INDEX [IX_FK_RespuestaSolicitud_Usuarios]
ON [dbo].[RespuestaSolicituds]
    ([usuarioId]);
GO

-- Creating foreign key on [usuarioCreacionId] in table 'ResumenPagoes'
ALTER TABLE [dbo].[ResumenPagoes]
ADD CONSTRAINT [FK_ResumenPago_Usuarios]
    FOREIGN KEY ([usuarioCreacionId])
    REFERENCES [dbo].[Usuarios]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ResumenPago_Usuarios'
CREATE INDEX [IX_FK_ResumenPago_Usuarios]
ON [dbo].[ResumenPagoes]
    ([usuarioCreacionId]);
GO

-- Creating foreign key on [roleId] in table 'RoleFuncions'
ALTER TABLE [dbo].[RoleFuncions]
ADD CONSTRAINT [FK_RoleFunciones_Roles]
    FOREIGN KEY ([roleId])
    REFERENCES [dbo].[Roles]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_RoleFunciones_Roles'
CREATE INDEX [IX_FK_RoleFunciones_Roles]
ON [dbo].[RoleFuncions]
    ([roleId]);
GO

-- Creating foreign key on [usuarioCreacionId] in table 'RoleFuncions'
ALTER TABLE [dbo].[RoleFuncions]
ADD CONSTRAINT [FK_RoleFunciones_RoleUsuarios]
    FOREIGN KEY ([usuarioCreacionId])
    REFERENCES [dbo].[Usuarios]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_RoleFunciones_RoleUsuarios'
CREATE INDEX [IX_FK_RoleFunciones_RoleUsuarios]
ON [dbo].[RoleFuncions]
    ([usuarioCreacionId]);
GO

-- Creating foreign key on [roleId] in table 'RoleModulos'
ALTER TABLE [dbo].[RoleModulos]
ADD CONSTRAINT [FK_RoleModulos_Roles]
    FOREIGN KEY ([roleId])
    REFERENCES [dbo].[Roles]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_RoleModulos_Roles'
CREATE INDEX [IX_FK_RoleModulos_Roles]
ON [dbo].[RoleModulos]
    ([roleId]);
GO

-- Creating foreign key on [usuarioCreacionId] in table 'RoleModulos'
ALTER TABLE [dbo].[RoleModulos]
ADD CONSTRAINT [FK_RoleModulos_Usuarios]
    FOREIGN KEY ([usuarioCreacionId])
    REFERENCES [dbo].[Usuarios]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_RoleModulos_Usuarios'
CREATE INDEX [IX_FK_RoleModulos_Usuarios]
ON [dbo].[RoleModulos]
    ([usuarioCreacionId]);
GO

-- Creating foreign key on [roleId] in table 'Usuarios'
ALTER TABLE [dbo].[Usuarios]
ADD CONSTRAINT [FK_Usuarios_Usuarios]
    FOREIGN KEY ([roleId])
    REFERENCES [dbo].[Roles]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Usuarios_Usuarios'
CREATE INDEX [IX_FK_Usuarios_Usuarios]
ON [dbo].[Usuarios]
    ([roleId]);
GO

-- Creating foreign key on [usuarioId] in table 'SalarialesEmpleadoes'
ALTER TABLE [dbo].[SalarialesEmpleadoes]
ADD CONSTRAINT [FK_SalarialesEmpleado_Usuarios]
    FOREIGN KEY ([usuarioId])
    REFERENCES [dbo].[Usuarios]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SalarialesEmpleado_Usuarios'
CREATE INDEX [IX_FK_SalarialesEmpleado_Usuarios]
ON [dbo].[SalarialesEmpleadoes]
    ([usuarioId]);
GO

-- Creating foreign key on [sdiId] in table 'Solicituds'
ALTER TABLE [dbo].[Solicituds]
ADD CONSTRAINT [FK_Proyectos_SDIs]
    FOREIGN KEY ([sdiId])
    REFERENCES [dbo].[SDIs]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Proyectos_SDIs'
CREATE INDEX [IX_FK_Proyectos_SDIs]
ON [dbo].[Solicituds]
    ([sdiId]);
GO

-- Creating foreign key on [usuarioId] in table 'SDIs'
ALTER TABLE [dbo].[SDIs]
ADD CONSTRAINT [FK_SDIs_SDIs]
    FOREIGN KEY ([usuarioId])
    REFERENCES [dbo].[Usuarios]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SDIs_SDIs'
CREATE INDEX [IX_FK_SDIs_SDIs]
ON [dbo].[SDIs]
    ([usuarioId]);
GO

-- Creating foreign key on [sdiId] in table 'Solicituds'
ALTER TABLE [dbo].[Solicituds]
ADD CONSTRAINT [FK_Solicitud_SDIs]
    FOREIGN KEY ([sdiId])
    REFERENCES [dbo].[SDIs]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Solicitud_SDIs'
CREATE INDEX [IX_FK_Solicitud_SDIs]
ON [dbo].[Solicituds]
    ([sdiId]);
GO

-- Creating foreign key on [usuarioId] in table 'Servicios'
ALTER TABLE [dbo].[Servicios]
ADD CONSTRAINT [FK_Servicios_Servicios]
    FOREIGN KEY ([usuarioId])
    REFERENCES [dbo].[Usuarios]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Servicios_Servicios'
CREATE INDEX [IX_FK_Servicios_Servicios]
ON [dbo].[Servicios]
    ([usuarioId]);
GO

-- Creating foreign key on [usuarioId] in table 'Sexos'
ALTER TABLE [dbo].[Sexos]
ADD CONSTRAINT [FK_Sexos_Sexos]
    FOREIGN KEY ([usuarioId])
    REFERENCES [dbo].[Usuarios]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Sexos_Sexos'
CREATE INDEX [IX_FK_Sexos_Sexos]
ON [dbo].[Sexos]
    ([usuarioId]);
GO

-- Creating foreign key on [contratoId] in table 'Solicituds'
ALTER TABLE [dbo].[Solicituds]
ADD CONSTRAINT [FK_Proyectos_TipoContrato]
    FOREIGN KEY ([contratoId])
    REFERENCES [dbo].[TipoContratoes]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Proyectos_TipoContrato'
CREATE INDEX [IX_FK_Proyectos_TipoContrato]
ON [dbo].[Solicituds]
    ([contratoId]);
GO

-- Creating foreign key on [tipoPersonalId] in table 'Solicituds'
ALTER TABLE [dbo].[Solicituds]
ADD CONSTRAINT [FK_Proyectos_TipoPersonal]
    FOREIGN KEY ([tipoPersonalId])
    REFERENCES [dbo].[TipoPersonals]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Proyectos_TipoPersonal'
CREATE INDEX [IX_FK_Proyectos_TipoPersonal]
ON [dbo].[Solicituds]
    ([tipoPersonalId]);
GO

-- Creating foreign key on [usuarioId] in table 'Solicituds'
ALTER TABLE [dbo].[Solicituds]
ADD CONSTRAINT [FK_Proyectos_Usuarios]
    FOREIGN KEY ([usuarioId])
    REFERENCES [dbo].[Usuarios]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Proyectos_Usuarios'
CREATE INDEX [IX_FK_Proyectos_Usuarios]
ON [dbo].[Solicituds]
    ([usuarioId]);
GO

-- Creating foreign key on [solicitudId] in table 'SolicitudEmpleadoes'
ALTER TABLE [dbo].[SolicitudEmpleadoes]
ADD CONSTRAINT [FK_SolicitudEmpleado_Solicitud]
    FOREIGN KEY ([solicitudId])
    REFERENCES [dbo].[Solicituds]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SolicitudEmpleado_Solicitud'
CREATE INDEX [IX_FK_SolicitudEmpleado_Solicitud]
ON [dbo].[SolicitudEmpleadoes]
    ([solicitudId]);
GO

-- Creating foreign key on [usuarioId] in table 'SolicitudEmpleadoes'
ALTER TABLE [dbo].[SolicitudEmpleadoes]
ADD CONSTRAINT [FK_SolicitudEmpleado_Usuarios]
    FOREIGN KEY ([usuarioId])
    REFERENCES [dbo].[Usuarios]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SolicitudEmpleado_Usuarios'
CREATE INDEX [IX_FK_SolicitudEmpleado_Usuarios]
ON [dbo].[SolicitudEmpleadoes]
    ([usuarioId]);
GO

-- Creating foreign key on [usuarioId] in table 'SumarizadoClientes'
ALTER TABLE [dbo].[SumarizadoClientes]
ADD CONSTRAINT [FK_SumarizadoClientes_Usuarios]
    FOREIGN KEY ([usuarioId])
    REFERENCES [dbo].[Usuarios]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SumarizadoClientes_Usuarios'
CREATE INDEX [IX_FK_SumarizadoClientes_Usuarios]
ON [dbo].[SumarizadoClientes]
    ([usuarioId]);
GO

-- Creating foreign key on [usuarioId] in table 'TipoContratoes'
ALTER TABLE [dbo].[TipoContratoes]
ADD CONSTRAINT [FK_TipoContrato_TipoContrato]
    FOREIGN KEY ([usuarioId])
    REFERENCES [dbo].[Usuarios]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TipoContrato_TipoContrato'
CREATE INDEX [IX_FK_TipoContrato_TipoContrato]
ON [dbo].[TipoContratoes]
    ([usuarioId]);
GO

-- Creating foreign key on [usuarioId] in table 'TipoPersonals'
ALTER TABLE [dbo].[TipoPersonals]
ADD CONSTRAINT [FK_TipoPersonal_TipoPersonal]
    FOREIGN KEY ([usuarioId])
    REFERENCES [dbo].[Usuarios]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TipoPersonal_TipoPersonal'
CREATE INDEX [IX_FK_TipoPersonal_TipoPersonal]
ON [dbo].[TipoPersonals]
    ([usuarioId]);
GO

-- Creating foreign key on [usuarioCreacionId] in table 'TopicosUsuarios'
ALTER TABLE [dbo].[TopicosUsuarios]
ADD CONSTRAINT [FK_TopicosUsuario_Usuario]
    FOREIGN KEY ([usuarioCreacionId])
    REFERENCES [dbo].[Usuarios]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TopicosUsuario_Usuario'
CREATE INDEX [IX_FK_TopicosUsuario_Usuario]
ON [dbo].[TopicosUsuarios]
    ([usuarioCreacionId]);
GO

-- Creating foreign key on [usuarioId] in table 'TopicosUsuarios'
ALTER TABLE [dbo].[TopicosUsuarios]
ADD CONSTRAINT [FK_TopicosUsuario_Usuario2]
    FOREIGN KEY ([usuarioId])
    REFERENCES [dbo].[Usuarios]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TopicosUsuario_Usuario2'
CREATE INDEX [IX_FK_TopicosUsuario_Usuario2]
ON [dbo].[TopicosUsuarios]
    ([usuarioId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------