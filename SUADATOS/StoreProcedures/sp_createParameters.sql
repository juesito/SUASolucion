-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Jesus,Armando>
-- Create date: <Martes 14 de Mayo del 2015>
-- Description:	<SP para crear los parametros basicos que se usaran en el sistema>
CREATE PROCEDURE sp_createParameters
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

   INSERT INTO Parametros
	 ( parametroId, descripcion, valorFecha, fechaCreacion)
	VALUES 
		('FACINFOSAL', 'Ultima fecha de actualizacion de datos infonavit', '09/04/2014 0:00:00', GETDATE())

	INSERT INTO Parametros
	 ( parametroId, descripcion, valorMoneda, fechaCreacion)
	VALUES 
		('SMDF', 'Salario Minimo DF', '70.10', GETDATE())

	INSERT INTO Parametros
	 ( parametroId, descripcion, valorMoneda, fechaCreacion)
	VALUES 
		('SINFON', 'Seguro Infonavit', '15.00', GETDATE())

	INSERT INTO Parametros
	 ( parametroId, descripcion, valorString, fechaCreacion)
	VALUES 
		('SUARUTA', 'Carpeta raiz para guarda SUA.mdb', 'C:\\SUA\\', GETDATE())

	INSERT INTO Parametros
	 ( parametroId, descripcion, valorString, fechaCreacion)
	VALUES 
		('IMGFOLDER', 'Ruta imágenes de los empleados.', 'C:\SUA\Empleados\Imagenes\', GETDATE())

	INSERT INTO Parametros
	 ( parametroId, descripcion, valorString, fechaCreacion)
	VALUES 
		('FOLSALTA', 'Consecutivo Folios Alta Solicitud.', '1', GETDATE())

	INSERT INTO Parametros
	 ( parametroId, descripcion, valorString, fechaCreacion)
	VALUES 
		('FOLSBAJA', 'Consecutivo Folios Baja Solicitud.', '1', GETDATE())

	INSERT INTO Parametros
	 ( parametroId, descripcion, valorString, fechaCreacion)
	VALUES 
		('FOLSMODIF', 'Consecutivo Folios Modificacion Solicitud.', '1', GETDATE())
END
GO
