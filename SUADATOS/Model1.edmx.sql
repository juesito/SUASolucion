
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 06/19/2015 15:42:03
-- Generated from EDMX file: C:\Users\Jesus Quiñones\Documents\Visual Studio 2013\Projects\SUASolucion\SUADATOS\Model1.edmx
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
IF OBJECT_ID(N'[dbo].[FK_Asegurados_Clientes]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Asegurados] DROP CONSTRAINT [FK_Asegurados_Clientes];
GO
IF OBJECT_ID(N'[dbo].[FK_Bancos_Bancos]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Bancos] DROP CONSTRAINT [FK_Bancos_Bancos];
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
IF OBJECT_ID(N'[dbo].[FK_Funciones_Modulos]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Funcions] DROP CONSTRAINT [FK_Funciones_Modulos];
GO
IF OBJECT_ID(N'[dbo].[FK_GrupoClientes]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Clientes] DROP CONSTRAINT [FK_GrupoClientes];
GO
IF OBJECT_ID(N'[dbo].[FK_Incapacidades_Asegurados]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Incapacidades] DROP CONSTRAINT [FK_Incapacidades_Asegurados];
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
IF OBJECT_ID(N'[dbo].[FK_Pagos_Asegurados]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Pagos] DROP CONSTRAINT [FK_Pagos_Asegurados];
GO
IF OBJECT_ID(N'[dbo].[FK_Pagos_ResumenPago]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Pagos] DROP CONSTRAINT [FK_Pagos_ResumenPago];
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
IF OBJECT_ID(N'[dbo].[FK_PlazaClientes]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Clientes] DROP CONSTRAINT [FK_PlazaClientes];
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
IF OBJECT_ID(N'[dbo].[FK_Proyectos_Residencia]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Solicitud] DROP CONSTRAINT [FK_Proyectos_Residencia];
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
IF OBJECT_ID(N'[dbo].[FK_Residencia_Residencia]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Residencia] DROP CONSTRAINT [FK_Residencia_Residencia];
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
IF OBJECT_ID(N'[dbo].[FK_SDIs_SDIs]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SDIs] DROP CONSTRAINT [FK_SDIs_SDIs];
GO
IF OBJECT_ID(N'[dbo].[FK_Sexos_Sexos]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Sexos] DROP CONSTRAINT [FK_Sexos_Sexos];
GO
IF OBJECT_ID(N'[dbo].[FK_Solicitud_Proyectos]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Solicitud] DROP CONSTRAINT [FK_Solicitud_Proyectos];
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
IF OBJECT_ID(N'[dbo].[Asegurados]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Asegurados];
GO
IF OBJECT_ID(N'[dbo].[Bancos]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Bancos];
GO
IF OBJECT_ID(N'[dbo].[catalogoMovimientos]', 'U') IS NOT NULL
    DROP TABLE [dbo].[catalogoMovimientos];
GO
IF OBJECT_ID(N'[dbo].[Clientes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Clientes];
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
IF OBJECT_ID(N'[dbo].[Funcions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Funcions];
GO
IF OBJECT_ID(N'[dbo].[Grupos]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Grupos];
GO
IF OBJECT_ID(N'[dbo].[Incapacidades]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Incapacidades];
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
IF OBJECT_ID(N'[dbo].[Proyectos]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Proyectos];
GO
IF OBJECT_ID(N'[dbo].[RegimenInfonavit]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RegimenInfonavit];
GO
IF OBJECT_ID(N'[dbo].[Residencia]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Residencia];
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
IF OBJECT_ID(N'[dbo].[SDIs]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SDIs];
GO
IF OBJECT_ID(N'[dbo].[Sexos]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Sexos];
GO
IF OBJECT_ID(N'[dbo].[Solicitud]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Solicitud];
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
    [ocupacion] nchar(60)  NULL,
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

-- Creating table 'Asegurados'
CREATE TABLE [dbo].[Asegurados] (
    [id] int IDENTITY(1,1) NOT NULL,
    [numeroAfiliacion] nchar(11)  NOT NULL,
    [CURP] nchar(18)  NULL,
    [RFC] nchar(13)  NULL,
    [nombre] nchar(50)  NULL,
    [apellidoMaterno] nchar(60)  NULL,
    [apellidoPaterno] nchar(60)  NULL,
    [nombres] nchar(60)  NULL,
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
    [salarioDiario] decimal(10,3)  NULL
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

-- Creating table 'catalogoMovimientos'
CREATE TABLE [dbo].[catalogoMovimientos] (
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
    [ejecutivo] nchar(100)  NULL,
    [Plaza_id] int  NOT NULL,
    [Grupo_id] int  NOT NULL
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
    [fechaCreacion] datetime  NULL
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
    [id] int  NOT NULL,
    [paisId] int  NOT NULL,
    [estadoId] int  NOT NULL,
    [descripcion] nchar(100)  NULL,
    [fechaCreacion] datetime  NOT NULL,
    [usuarioId] int  NOT NULL
);
GO

-- Creating table 'Pagos'
CREATE TABLE [dbo].[Pagos] (
    [id] int  NOT NULL,
    [resumenPagoId] int  NOT NULL,
    [ip] nchar(15)  NULL,
    [NSS] nchar(11)  NULL,
    [RFC] nchar(13)  NULL,
    [CURP] nchar(18)  NULL,
    [creditoInfonavit] nchar(10)  NULL,
    [fid] nchar(8)  NULL,
    [trabajador] nchar(50)  NULL,
    [sdi] decimal(7,2)  NULL,
    [tipoTrabajador] nchar(1)  NULL,
    [jornadaSemanaReducida] nchar(1)  NULL,
    [diasCotizadosMes] int  NULL,
    [diasIncapacidad] int  NULL,
    [diasAusentismo] int  NULL,
    [cuotaFija] decimal(8,2)  NULL,
    [cuotaExcedente] decimal(8,2)  NULL,
    [prestacionesDinero] decimal(8,2)  NULL,
    [gastosMedicosPensionado] decimal(8,2)  NULL,
    [riesgoTrabajo] decimal(8,2)  NULL,
    [invalidezVida] decimal(8,2)  NULL,
    [guarderias] decimal(8,2)  NULL,
    [actRecargosIMSS] nchar(8)  NULL,
    [diasCotizadosBimestre] int  NULL,
    [diasIncapacidadBimestre] int  NULL,
    [diasAusentismoBimestre] int  NULL,
    [retiro] decimal(7,2)  NULL,
    [actRecargosRetiro] nchar(8)  NULL,
    [cesantiaVejezPatronal] decimal(8,2)  NULL,
    [cesantiaVejezObrera] decimal(8,2)  NULL,
    [actRecargosCyV] decimal(8,2)  NULL,
    [aportacionVoluntaria] decimal(8,2)  NULL,
    [aportacionComp] decimal(8,2)  NULL,
    [aportacionPatronal] decimal(8,2)  NULL,
    [amortizacion] decimal(10,2)  NULL,
    [actIMSS] decimal(10,2)  NULL,
    [recIMSS] decimal(10,2)  NULL,
    [actRetiro] decimal(10,2)  NULL,
    [recRetiro] decimal(10,2)  NULL,
    [actCesPat] decimal(10,2)  NULL,
    [recCesPat] decimal(10,2)  NULL,
    [actCesObr] decimal(10,2)  NULL,
    [recCesObr] decimal(10,2)  NULL,
    [cuotaExcObr] decimal(10,2)  NULL,
    [cuotaPdObr] decimal(10,2)  NULL,
    [cuotaGmpObr] decimal(10,2)  NULL,
    [cuotaIvObr] decimal(10,2)  NULL,
    [actPatIMSS] decimal(10,2)  NULL,
    [recPatIMSS] decimal(10,2)  NULL,
    [actObrIMSS] decimal(10,2)  NULL,
    [recObrIMSS] decimal(10,2)  NULL,
    [trabajadorId] int  NOT NULL,
    [anoPago] int  NOT NULL,
    [mesPago] int  NOT NULL
);
GO

-- Creating table 'Paises'
CREATE TABLE [dbo].[Paises] (
    [id] int IDENTITY(1,1) NOT NULL,
    [descripcion] nchar(100)  NULL,
    [naturalez] nchar(1)  NULL,
    [fechaCreacion] datetime  NOT NULL,
    [usuarioId] int  NOT NULL
);
GO

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
    [direccionArchivo] nchar(100)  NULL
);
GO

-- Creating table 'Plazas'
CREATE TABLE [dbo].[Plazas] (
    [id] int IDENTITY(1,1) NOT NULL,
    [descripcion] nchar(50)  NOT NULL,
    [cveCorta] nchar(10)  NULL,
    [indicador] char(1)  NULL
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

-- Creating table 'Residencias'
CREATE TABLE [dbo].[Residencias] (
    [id] int IDENTITY(1,1) NOT NULL,
    [descripcion] nchar(100)  NULL,
    [fechaCreacion] datetime  NOT NULL,
    [usuarioId] int  NOT NULL
);
GO

-- Creating table 'ResumenPagoes'
CREATE TABLE [dbo].[ResumenPagoes] (
    [id] int  NOT NULL,
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

-- Creating table 'SDIs'
CREATE TABLE [dbo].[SDIs] (
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
    [clienteId] int  NOT NULL,
    [residenciaId] int  NOT NULL,
    [fechaSolicitud] datetime  NOT NULL,
    [esquemaId] int  NOT NULL,
    [sdiId] int  NOT NULL,
    [contratoId] int  NOT NULL,
    [fechaInicial] datetime  NOT NULL,
    [fechaFinal] datetime  NOT NULL,
    [tipoPersonalId] int  NOT NULL,
    [solicita] nchar(60)  NULL,
    [valida] nchar(60)  NULL,
    [autoriza] nchar(60)  NULL,
    [noTrabajadores] int  NOT NULL,
    [observaciones] nchar(100)  NULL,
    [estatusSolicitud] nchar(1)  NOT NULL,
    [estatusNomina] nchar(1)  NULL,
    [estatusAfiliado] nchar(1)  NULL,
    [estatusJuridico] nchar(1)  NULL,
    [estatusTarjeta] nchar(1)  NULL,
    [usuarioId] int  NOT NULL,
    [proyectoId] int  NOT NULL
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
    [contrasena] nchar(10)  NOT NULL,
    [claveUsuario] nchar(30)  NOT NULL,
    [email] nchar(100)  NULL,
    [apellidoMaterno] nchar(60)  NULL,
    [apellidoPaterno] nchar(60)  NULL,
    [estatus] nchar(2)  NOT NULL,
    [fechaIngreso] datetime  NOT NULL,
    [roleId] int  NOT NULL,
    [plazaId] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [id] in table 'Acreditados'
ALTER TABLE [dbo].[Acreditados]
ADD CONSTRAINT [PK_Acreditados]
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

-- Creating primary key on [id] in table 'catalogoMovimientos'
ALTER TABLE [dbo].[catalogoMovimientos]
ADD CONSTRAINT [PK_catalogoMovimientos]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [Id] in table 'Clientes'
ALTER TABLE [dbo].[Clientes]
ADD CONSTRAINT [PK_Clientes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
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

-- Creating primary key on [id] in table 'Funcions'
ALTER TABLE [dbo].[Funcions]
ADD CONSTRAINT [PK_Funcions]
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

-- Creating primary key on [id] in table 'Parametros'
ALTER TABLE [dbo].[Parametros]
ADD CONSTRAINT [PK_Parametros]
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

-- Creating primary key on [id] in table 'Residencias'
ALTER TABLE [dbo].[Residencias]
ADD CONSTRAINT [PK_Residencias]
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

-- Creating primary key on [id] in table 'SDIs'
ALTER TABLE [dbo].[SDIs]
ADD CONSTRAINT [PK_SDIs]
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

-- Creating foreign key on [trabajadorId] in table 'Pagos'
ALTER TABLE [dbo].[Pagos]
ADD CONSTRAINT [FK_Pagos_Asegurados]
    FOREIGN KEY ([trabajadorId])
    REFERENCES [dbo].[Asegurados]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Pagos_Asegurados'
CREATE INDEX [IX_FK_Pagos_Asegurados]
ON [dbo].[Pagos]
    ([trabajadorId]);
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

-- Creating foreign key on [movimientoId] in table 'MovimientosAseguradoes'
ALTER TABLE [dbo].[MovimientosAseguradoes]
ADD CONSTRAINT [FK_MovimientosAsegurado_catalogoMovimientos]
    FOREIGN KEY ([movimientoId])
    REFERENCES [dbo].[catalogoMovimientos]
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
ADD CONSTRAINT [FK_GrupoClientes]
    FOREIGN KEY ([Grupo_id])
    REFERENCES [dbo].[Grupos]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_GrupoClientes'
CREATE INDEX [IX_FK_GrupoClientes]
ON [dbo].[Clientes]
    ([Grupo_id]);
GO

-- Creating foreign key on [Plaza_id] in table 'Clientes'
ALTER TABLE [dbo].[Clientes]
ADD CONSTRAINT [FK_PlazaClientes]
    FOREIGN KEY ([Plaza_id])
    REFERENCES [dbo].[Plazas]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_PlazaClientes'
CREATE INDEX [IX_FK_PlazaClientes]
ON [dbo].[Clientes]
    ([Plaza_id]);
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

-- Creating foreign key on [resumenPagoId] in table 'Pagos'
ALTER TABLE [dbo].[Pagos]
ADD CONSTRAINT [FK_Pagos_ResumenPago]
    FOREIGN KEY ([resumenPagoId])
    REFERENCES [dbo].[ResumenPagoes]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Pagos_ResumenPago'
CREATE INDEX [IX_FK_Pagos_ResumenPago]
ON [dbo].[Pagos]
    ([resumenPagoId]);
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

-- Creating foreign key on [residenciaId] in table 'Solicituds'
ALTER TABLE [dbo].[Solicituds]
ADD CONSTRAINT [FK_Proyectos_Residencia]
    FOREIGN KEY ([residenciaId])
    REFERENCES [dbo].[Residencias]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Proyectos_Residencia'
CREATE INDEX [IX_FK_Proyectos_Residencia]
ON [dbo].[Solicituds]
    ([residenciaId]);
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