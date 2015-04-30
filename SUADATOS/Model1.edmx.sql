
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 04/20/2015 17:37:45
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

IF OBJECT_ID(N'[dbo].[FK_Acreditados_Clientes]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Acreditados] DROP CONSTRAINT [FK_Acreditados_Clientes];
GO
IF OBJECT_ID(N'[dbo].[FK_Asegurados_Clientes]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Asegurados] DROP CONSTRAINT [FK_Asegurados_Clientes];
GO
IF OBJECT_ID(N'[dbo].[FK_Incapacidades_Asegurados]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Incapacidades] DROP CONSTRAINT [FK_Incapacidades_Asegurados];
GO
IF OBJECT_ID(N'[dbo].[FK_Movimientos_Asegurados]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Movimientos] DROP CONSTRAINT [FK_Movimientos_Asegurados];
GO
IF OBJECT_ID(N'[dbo].[FK_MovimientosAsegurado_Asegurados]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[MovimientosAsegurado] DROP CONSTRAINT [FK_MovimientosAsegurado_Asegurados];
GO
IF OBJECT_ID(N'[dbo].[FK_MovimientosAsegurado_catalogoMovimientos]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[MovimientosAsegurado] DROP CONSTRAINT [FK_MovimientosAsegurado_catalogoMovimientos];
GO
IF OBJECT_ID(N'[dbo].[FK_MovimientosAsegurado_Incapacidades]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[MovimientosAsegurado] DROP CONSTRAINT [FK_MovimientosAsegurado_Incapacidades];
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
IF OBJECT_ID(N'[dbo].[FK_PlazaPatrone]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Patrones] DROP CONSTRAINT [FK_PlazaPatrone];
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
IF OBJECT_ID(N'[dbo].[catalogoMovimientos]', 'U') IS NOT NULL
    DROP TABLE [dbo].[catalogoMovimientos];
GO
IF OBJECT_ID(N'[dbo].[Clientes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Clientes];
GO
IF OBJECT_ID(N'[dbo].[Incapacidades]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Incapacidades];
GO
IF OBJECT_ID(N'[dbo].[Movimientos]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Movimientos];
GO
IF OBJECT_ID(N'[dbo].[MovimientosAsegurado]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MovimientosAsegurado];
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
IF OBJECT_ID(N'[dbo].[Usuarios]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Usuarios];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Usuarios'
CREATE TABLE [dbo].[Usuarios] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [nombreUsuario] nchar(10)  NOT NULL,
    [contrasena] nchar(10)  NOT NULL,
    [claveUsuario] nchar(10)  NOT NULL,
    [email] nchar(100)  NULL,
    [apellidoMaterno] nchar(60)  NULL,
    [apellidoPaterno] nchar(60)  NULL,
    [estatus] nchar(2)  NOT NULL,
    [fechaIngreso] datetime  NOT NULL,
    [tipo] nchar(2)  NOT NULL
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
    [fecTer] datetime  NOT NULL
);
GO

-- Creating table 'Movimientos'
CREATE TABLE [dbo].[Movimientos] (
    [id] int IDENTITY(1,1) NOT NULL,
    [aseguradoId] int  NOT NULL,
    [lote] nchar(50)  NOT NULL,
    [fechaTransaccion] datetime  NOT NULL,
    [tipo] nchar(2)  NOT NULL,
    [nombreArchivo] nchar(100)  NOT NULL
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
    [descripcion] nchar(50)  NOT NULL
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
    [Plaza_id] int  NOT NULL
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

-- Creating table 'Clientes'
CREATE TABLE [dbo].[Clientes] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [claveCliente] nchar(15)  NOT NULL,
    [claveSua] nchar(15)  NOT NULL,
    [rfc] nchar(20)  NOT NULL,
    [descripcion] nchar(60)  NULL,
    [recidencia] nchar(30)  NULL,
    [ejecutivo] nchar(100)  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Usuarios'
ALTER TABLE [dbo].[Usuarios]
ADD CONSTRAINT [PK_Usuarios]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [id] in table 'catalogoMovimientos'
ALTER TABLE [dbo].[catalogoMovimientos]
ADD CONSTRAINT [PK_catalogoMovimientos]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'Incapacidades'
ALTER TABLE [dbo].[Incapacidades]
ADD CONSTRAINT [PK_Incapacidades]
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

-- Creating primary key on [id] in table 'Asegurados'
ALTER TABLE [dbo].[Asegurados]
ADD CONSTRAINT [PK_Asegurados]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'Acreditados'
ALTER TABLE [dbo].[Acreditados]
ADD CONSTRAINT [PK_Acreditados]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [Id] in table 'Clientes'
ALTER TABLE [dbo].[Clientes]
ADD CONSTRAINT [PK_Clientes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

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

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------