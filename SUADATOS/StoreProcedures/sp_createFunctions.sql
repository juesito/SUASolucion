USE [sua]
GO

/****** Object:  StoredProcedure [dbo].[sp_createFunctions]    Script Date: 13/05/2015 01:23:17 p.m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Jesus,Armando>
-- Create date: <Martes 12 de Mayo del 2015>
-- Description:	<SP para crear las funciones que se usaran en el sistema por modulo>
-- =============================================
CREATE PROCEDURE [dbo].[sp_createFunctions]
	-- Add the parameters for the stored procedure here
	@usuarioId                   INT    
AS
BEGIN
	DECLARE @moduloId int
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	--Buscamos el modulo de seguridad para insertar las funciones correspondientes
	SET @moduloId = (SELECT id FROM Modulos WHERE descripcionCorta = 'Seguridad')

	INSERT INTO Funcions
	( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Funciones', 'Catalogo de funciones del SIAP', 'Index', 'Funciones', 'A', @moduloId, GETDATE(), @usuarioId, 'M')

	INSERT INTO Funcions
		( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Modulos', 'Catalogo de modulos del SIAP', 'Index', 'Modulos', 'A', @moduloId, GETDATE(), @usuarioId, 'M')

	INSERT INTO Funcions
		( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Usuarios', 'Catalogo de usuarios del SIAP', 'Index', 'Usuarios', 'A', @moduloId, GETDATE(), @usuarioId, 'M')

	INSERT INTO Funcions
		( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Roles', 'Catalogo de roles del SIAP', 'Index', 'Roles', 'A', @moduloId, GETDATE(), @usuarioId, 'M')

	INSERT INTO Funcions
		( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Modulos por Rol', 'Modulos por Rol del SIAP', 'Index', 'RoleModulos', 'A', @moduloId, GETDATE(), @usuarioId, 'M')

	INSERT INTO Funcions
		( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Funciones por Rol', 'Funciones por Rol del SIAP', 'Index', 'RoleFunciones', 'A', @moduloId, GETDATE(), @usuarioId, 'M')

	INSERT INTO Funcions
		( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Topicos Por Usuario', 'Topicos Por Usuario del SIAP', 'Index', 'TopicosUsuario', 'A', @moduloId, GETDATE(), @usuarioId, 'M')
				
	--Buscamos el modulo de catalogos para insertar las funciones correspondientes
	SET @moduloId = (SELECT id FROM Modulos WHERE descripcionCorta = 'Catalogos')

	INSERT INTO Funcions
	( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Plazas', 'Catalogo de Plazas del SIAP', 'Index', 'Plazas', 'A', @moduloId, GETDATE(), @usuarioId, 'M')

	INSERT INTO Funcions
		( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Grupos', 'Catalogo de Grupos del SIAP', 'Index', 'Grupos', 'A', @moduloId, GETDATE(), @usuarioId, 'M')

	INSERT INTO Funcions
		( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Clientes', 'Catalogo de Clientes del SIAP', 'Index', 'Clientes', 'A', @moduloId, GETDATE(), @usuarioId, 'M')

	INSERT INTO Funcions
		( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Movimientos', 'Catalogo de Movimientos del SIAP', 'Index', 'CatalogoMovimientos', 'A', @moduloId, GETDATE(), @usuarioId, 'M')

	INSERT INTO Funcions
		( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Parametros', 'Modulos por Parametros del SIAP', 'Index', 'Parametros', 'A', @moduloId, GETDATE(), @usuarioId, 'M')
    
	INSERT INTO Funcions
		( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Esquemas', 'Catalogo de esquemas CIAH', 'Index', 'Esquemas', 'A', @moduloId, GETDATE(), @usuarioId, 'M')

	INSERT INTO Funcions
		( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Tipos Contratos', 'Catalogo de Tipos Contrato CIAH', 'Index', 'TipoContrato', 'A', @moduloId, GETDATE(), @usuarioId, 'M')

	INSERT INTO Funcions
		( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Regimen Infonavit', 'Catalogo de Regimen Infonavit CIAH', 'Index', 'RegimenInfonavit', 'A', @moduloId, GETDATE(), @usuarioId, 'M')

	INSERT INTO Funcions
		( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Proyectos', 'Proyectos CIAH', 'Index', 'Proyectos', 'A', @moduloId, GETDATE(), @usuarioId, 'M')

	INSERT INTO Funcions
		( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Salario Diario', 'Catalogo para SDIs CIAH', 'Index', 'SDIs', 'A', @moduloId, GETDATE(), @usuarioId, 'M')

    INSERT INTO Funcions
		( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Tipo de Personal', 'Catalogo de tipos de personal CIAH', 'Index', 'TipoPersonal', 'A', @moduloId, GETDATE(), @usuarioId, 'M')

	INSERT INTO Funcions
		( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Bancos', 'Catalogo de bancos CIAH', 'Index', 'Bancos', 'A', @moduloId, GETDATE(), @usuarioId, 'M')

	INSERT INTO Funcions
		( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Sexos', 'Catalogo de Sexos en CIAH', 'Index', 'Sexo', 'A', @moduloId, GETDATE(), @usuarioId, 'M')

	INSERT INTO Funcions
		( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Paises', 'Catalogo de Paises CIAH', 'Index', 'Paises', 'A', @moduloId, GETDATE(), @usuarioId, 'M')

	INSERT INTO Funcions
		( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Estados', 'Catalogo de Estados CIAH', 'Index', 'Estados', 'A', @moduloId, GETDATE(), @usuarioId, 'M')

	INSERT INTO Funcions
		( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Municipios', 'Catalogo de Municipios CIAH', 'Index', 'Municipios', 'A', @moduloId, GETDATE(), @usuarioId, 'M')

	INSERT INTO Funcions
		( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Departamentos', 'Catalogo de Departamentos CIAH', 'Index', 'Departamentos', 'A', @moduloId, GETDATE(), @usuarioId, 'M')

	INSERT INTO Funcions
		( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Giros', 'Catalogo de Giros CIAH', 'Index', 'Giros', 'A', @moduloId, GETDATE(), @usuarioId, 'M')

	INSERT INTO Funcions
		( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Empresas', 'Catalogo de Empresas CIAH', 'Index', 'Empresas', 'A', @moduloId, GETDATE(), @usuarioId, 'M')

	INSERT INTO Funcions
		( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Servicios', 'Catalogo de Servicios CIAH', 'Index', 'Servicios', 'A', @moduloId, GETDATE(), @usuarioId, 'M')

	INSERT INTO Funcions
		( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Estados Civiles', 'Catalogo de Estados Civiles CIAH', 'Index', 'EstadoCivil', 'A', @moduloId, GETDATE(), @usuarioId, 'M')

	INSERT INTO Funcions
		( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Conceptos', 'Catalogo de Conceptos CIAH', 'Index', 'Conceptos', 'A', @moduloId, GETDATE(), @usuarioId, 'M')


	--Buscamos el modulo de seguridad para insertar las funciones correspondientes
	SET @moduloId = (SELECT id FROM Modulos WHERE descripcionCorta = 'IMSS')

	INSERT INTO Funcions
	( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Patrones', 'Patrones cargados en el SIAP', 'Index', 'Patrones', 'A', @moduloId, GETDATE(), @usuarioId, 'M')

	INSERT INTO Funcions
		( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Asegurados', 'Asegurados registrados en el SIAP', 'Index', 'Asegurados', 'A', @moduloId, GETDATE(), @usuarioId, 'M')

	INSERT INTO Funcions
		( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Acreditados', 'Acreditados registrados en el SIAP', 'Index', 'Acreditados', 'A', @moduloId, GETDATE(), @usuarioId, 'M')

	INSERT INTO Funcions
		( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Movimientos', 'Movimientos registrados en el SIAP', 'Index', 'Movimientos', 'A', @moduloId, GETDATE(), @usuarioId, 'M')

	INSERT INTO Funcions
		( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Resumen de Pagos', 'Resumen Pagos en el SIAP', 'Index', 'Pagos', 'A', @moduloId, GETDATE(), @usuarioId, 'M')

	--Buscamos el modulo de seguridad para insertar las funciones correspondientes
	SET @moduloId = (SELECT id FROM Modulos WHERE descripcionCorta = 'Carga')

	INSERT INTO Funcions
	( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Cargar SUA', 'Cargar datos del SUA en el SIAP', 'GoAcreditados', 'Upload', 'A', @moduloId, GETDATE(), @usuarioId, 'M')

	INSERT INTO Funcions
		( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Actualizar Patrones', 'Actualizar los Patrones en el SIAP', 'Index', 'Upload', 'A', @moduloId, GETDATE(), @usuarioId, 'M')

	INSERT INTO Funcions
		( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Carga Registro de Pago', 'Cargar Registro de Pagos SUA', 'UploadPagos', 'Pagos', 'A', @moduloId, GETDATE(), @usuarioId, 'M')


	INSERT INTO Funcions
		( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Factores de Calculo', 'Administrar los factores de calculo en el SIAP', 'Index', 'Factores', 'A', @moduloId, GETDATE(), @usuarioId, 'M')


    --Buscamos el modulo de seguridad para insertar las funciones correspondientes
	SET @moduloId = (SELECT id FROM Modulos WHERE descripcionCorta = 'ACAH')
    
	INSERT INTO Funcions
		( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Alta Personal', 'Alta de solicitudes de personal en el SIAP', 'Index', 'Solicitudes', 'A', @moduloId, GETDATE(), @usuarioId, 'M')

	INSERT INTO Funcions
		( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Baja Personal', 'Baja de solicitudes de personal en el SIAP', 'Index', 'SolicitudesBaja', 'A', @moduloId, GETDATE(), @usuarioId, 'M')

	INSERT INTO Funcions
		( descripcionCorta, descripcionLarga, accion, controlador, estatus, moduloId, fechaCreacion, usuarioId, tipo)
	VALUES 
		('Modificacion Salario', 'Solicitud de modificación al salario de personal', 'Index', 'SolicitudesModificacion', 'A', @moduloId, GETDATE(), @usuarioId, 'M')

END
                

                
GO


