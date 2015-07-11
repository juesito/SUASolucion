USE [sua]
GO

/****** Object:  StoredProcedure [dbo].[sp_createCatalogoMovimientos]    Script Date: 09/07/2015 01:41:28 p.m. ******/
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
@usuarioId                   INT    
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;


	INSERT INTO [dbo].[SumarizadoClientes]
           ([patronId],[anno],     [mes]      , [clienteId],
            [imss]    ,[rcv] ,     [infonavit],[total],
			[nt]      ,[usuarioId],[fechaCreacion])
	SELECT a.patronId, a.anno     , a.mes,  c.Id,
	       SUM(dt.imss), SUM(dt.rcv), SUM(dt.infonavit), SUM(dt.total),
		   SUM(a.nt)  , 1 ,  GETDATE()
	  from pagos a, DetallePago dt, Clientes c,
			Asegurados ac
	  where a.id = dt.pagoId
	    and ac.id = dt.aseguradoId
		and c.Id =  ac.ClienteId  
	  group by a.patronId, a.anno     , a.mes,  c.Id --, @usuarioId, GETDATE()
    
END

GO


