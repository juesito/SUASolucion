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

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_LimpiaReporte]
@usuarioId                   INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DELETE FROM [dbo].[RepCostoSocial]
	 WHERE usuarioId = @usuarioId

END

GO


CREATE PROCEDURE [dbo].[sp_ResumenPagosINF]
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
            [imss]    ,
			[nt]      ,[usuarioId],[fechaCreacion])
	SELECT a.patronId, a.anno     , a.mes,  c.Id, 
	       SUM(dt.amortizacion), 
		 COUNT(distinct dt.aseguradoId ),  @usuarioId ,  GETDATE()
	  from pagos a, DetallePago dt, Clientes c,
			Asegurados ac
	  where a.id = dt.pagoId
	    and ac.id = dt.aseguradoId
		and c.Id =  ac.ClienteId
		and a.patronId = ac.PatroneId
		and dt.amortizacion <> 0
		and ac.paginaInfo <> ''
	  group by a.patronId, a.anno     , a.mes,  c.Id, a.nt
    
END

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_ResumenPagosDetalleINF]
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
            [imss]    ,
			[nt]      ,[usuarioId],[fechaCreacion])
	SELECT a.patronId, a.anno     , a.mes,  c.Id, 
	       SUM(dt.amortizacion), 
		 a.nt,  @usuarioId ,  GETDATE()
	  from pagos a, DetallePago dt, Clientes c,
			Asegurados ac
	  where a.id = dt.pagoId
	    and ac.id = dt.aseguradoId
		and c.Id =  ac.ClienteId
		and ac.ClienteId = @clienteId  
		and dt.amortizacion <> 0
		and ac.paginaInfo <> ''
	  group by a.patronId, a.anno     , a.mes,  c.Id, a.nt
    
END

GO

/****** Object:  StoredProcedure [dbo].[sp_AmortizacionBimestralINF]    Script Date: 24/08/2015 12:06:26 p.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_AmortizacionBimestralINF]
@usuarioId                   INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DELETE FROM [dbo].[ReporteConMeses]
	 WHERE usuarioId = @usuarioId


	INSERT INTO [dbo].[ReporteConMeses]
           ([anno], [clienteId], [febrero], [abril],[junio],[agosto],[octubre],[diciembre],
			[total],[usuarioId],[fechaCreacion])
	SELECT a.anno, c.Id, 
		   SUM(CASE WHEN a.mes = '02' THEN amortizacion END) AS febrero,
		   SUM(CASE WHEN a.mes = '04' THEN amortizacion END) AS abril,
		   SUM(CASE WHEN a.mes = '06' THEN amortizacion END) AS junio,		   
		   SUM(CASE WHEN a.mes = '08' THEN amortizacion END) AS agosto,		   
		   SUM(CASE WHEN a.mes = '10' THEN amortizacion END) AS octubre,		   
		   SUM(CASE WHEN a.mes = '12' THEN amortizacion END) AS diciembre,		   
		   SUM(amortizacion) AS total,		   
		   @usuarioId ,  GETDATE()
	  from pagos a, DetallePago dt, Clientes c,
			Asegurados ac
	  where a.id = dt.pagoId
	    and ac.id = dt.aseguradoId
		and c.Id =  ac.ClienteId
		and dt.amortizacion <> 0
		and ac.paginaInfo <> ''
	  group by a.anno, c.Id
    
END


GO
/****** Object:  StoredProcedure [dbo].[sp_AmortizacionBimestralINFDet]    Script Date: 24/08/2015 12:06:26 p.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_AmortizacionBimestralINFDet]
@usuarioId                   INT,
@clienteId                   INT,
@anio                   INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DELETE FROM [dbo].[ReporteConMeses]
	 WHERE usuarioId = @usuarioId


	INSERT INTO [dbo].[ReporteConMeses]
           ([anno], [clienteId], [patronId], [aseguradoId], [febrero], [abril],[junio],[agosto],[octubre],[diciembre],
			[total],[usuarioId],[fechaCreacion])
	SELECT a.anno, ac.ClienteId, a.patronId, dt.aseguradoId ,
		   SUM(CASE WHEN a.mes = '02' THEN amortizacion END) AS febrero,
		   SUM(CASE WHEN a.mes = '04' THEN amortizacion END) AS abril,
		   SUM(CASE WHEN a.mes = '06' THEN amortizacion END) AS junio,		   
		   SUM(CASE WHEN a.mes = '08' THEN amortizacion END) AS agosto,		   
		   SUM(CASE WHEN a.mes = '10' THEN amortizacion END) AS octubre,		   
		   SUM(CASE WHEN a.mes = '12' THEN amortizacion END) AS diciembre,		   
		   SUM(amortizacion) AS total,		   
		   @usuarioId ,  GETDATE()
	  from pagos a, DetallePago dt, Clientes c,
			Asegurados ac
	  where a.id = dt.pagoId
	    and ac.id = dt.aseguradoId
		and ac.ClienteId = @clienteId  
		and dt.amortizacion <> 0
		and ac.paginaInfo <> ''
		and a.anno = @anio
	  group by anno, clienteId, a.patronId, aseguradoID
    
END


GO
/****** Object:  StoredProcedure [dbo].[sp_AmortizacionBimestralINFDetExcel]    Script Date: 24/08/2015 12:06:26 p.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_AmortizacionBimestralINFDetExcel]
@usuarioId                   INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DELETE FROM [dbo].[ReporteConMeses]
	 WHERE usuarioId = @usuarioId


	INSERT INTO [dbo].[ReporteConMeses]
           ([anno], [clienteId], [patronId], [aseguradoId], [febrero], [abril],[junio],[agosto],[octubre],[diciembre],
			[total],[usuarioId],[fechaCreacion])
	SELECT a.anno, c.Id, a.patronId, dt.aseguradoId ,
		   SUM(CASE WHEN a.mes = '02' THEN amortizacion END) AS febrero,
		   SUM(CASE WHEN a.mes = '04' THEN amortizacion END) AS abril,
		   SUM(CASE WHEN a.mes = '06' THEN amortizacion END) AS junio,		   
		   SUM(CASE WHEN a.mes = '08' THEN amortizacion END) AS agosto,		   
		   SUM(CASE WHEN a.mes = '10' THEN amortizacion END) AS octubre,		   
		   SUM(CASE WHEN a.mes = '12' THEN amortizacion END) AS diciembre,		   
		   SUM(amortizacion) AS total,		   
		   @usuarioId ,  GETDATE()
	  from pagos a, DetallePago dt, Clientes c,
			Asegurados ac
	  where a.id = dt.pagoId
	    and ac.id = dt.aseguradoId
		and c.Id =  ac.ClienteId
		and dt.amortizacion <> 0
		and ac.paginaInfo <> ''
	  group by a.anno, c.Id, a.patronId, dt.aseguradoID
    
END


GO