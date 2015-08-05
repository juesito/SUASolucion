USE [sua]
GO

/****** Object:  StoredProcedure [dbo].[sp_SumarizadoClientes]    Script Date: 11/07/2015 09:15:28 p.m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_SumarizadoClientes]
@usuarioId                   INT,
@clienteId                   INT    
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DELETE FROM [dbo].[SumarizadoClientes]
	 WHERE usuarioId = @usuarioId


	INSERT INTO [dbo].[SumarizadoClientes]
           ([patronId],[anno],     [mes]      , [clienteId],
            [imss]    ,[rcv] ,     [infonavit],[total],
			[nt]      ,[usuarioId],[fechaCreacion])
	SELECT a.patronId, a.anno     , a.mes,  c.Id, 
	       SUM(dt.imss), SUM(dt.rcv), SUM(dt.infonavit), SUM(dt.total),
		 a.nt,  @usuarioId ,  GETDATE()
	  from pagos a, DetallePago dt, Clientes c,
			Asegurados ac
	  where a.id = dt.pagoId
	    and ac.id = dt.aseguradoId
		and c.Id =  ac.ClienteId
		and ac.ClienteId = @clienteId  
	  group by a.patronId, a.anno     , a.mes,  c.Id, a.nt
    
END


GO


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_SumarizadoClientesTodos]
@usuarioId                   INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DELETE FROM [dbo].[SumarizadoClientes]
	 WHERE usuarioId = @usuarioId


	INSERT INTO [dbo].[SumarizadoClientes]
           ([patronId],[anno],     [mes]      , [clienteId],
            [imss]    ,[rcv] ,     [infonavit],[total],
			[nt]      ,[usuarioId],[fechaCreacion])
	SELECT a.patronId, a.anno     , a.mes,  c.Id, 
	       SUM(dt.imss), SUM(dt.rcv), SUM(dt.infonavit), SUM(dt.total),
		 COUNT(distinct dt.aseguradoId),  @usuarioId ,  GETDATE()
	  from pagos a, DetallePago dt, Clientes c,
			Asegurados ac
	  where a.id = dt.pagoId
	    and ac.id = dt.aseguradoId
		and c.Id =  ac.ClienteId
	  group by a.patronId, a.anno     , a.mes,  c.Id, a.nt
    
END




GO
