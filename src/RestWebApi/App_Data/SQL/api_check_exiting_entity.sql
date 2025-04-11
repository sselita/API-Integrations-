USE [DegertAutoDemo]
GO
/****** Object:  StoredProcedure [dbo].[api_check_exiting_entity]    Script Date: 4/23/2021 12:06:55 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,IBS, Erjon Kuka>
-- Create date: <Create Date,12/07/2020>
-- Description:	<Description,Procedure for getting master data from BC Sql Database based on some specific conditions>
-- =============================================
ALTER PROCEDURE [dbo].[api_check_exiting_entity]
	-- Add the parameters for the stored procedure here
	@tablename       varchar(50), -- Specify the primary table name
	@companyname     varchar(50),--Specify the company name
	@Code            varchar(30)=' ',--Optional parameter for filter in Sales Quote from POS
	@startDateFilter datetime2 = null, --Optional parameter filter by modified date
	@endDateFilter   datetime2 =null --optional parameter filter by modified date
AS
BEGIN
   --Variable to store the Full entity name. 
   DECLARE  @entityname nvarchar(4000)='';
   --Variable to store the SQL query
   DECLARE @sql nvarchar(MAX)='';
   Declare @defaultGuid varchar(50) = '437dbf0e-84ff-417a-965d-ed2bb9650972'
   Declare @ecommerceguid varchar(50) = 'df1e71ee-a600-415c-bf99-17ad0aa3b7eb'
   Declare @otherExtensionGuid varchar(50) ='60fef34f-6f91-40be-b64d-9e20568a1d4a';
    --Case of Quote
    IF @tablename = 'Sales Header(Quote)'
     BEGIN
	   SET @tablename ='Sales Header';
	 END; 
	--Execute function which generates the full entity name of the main table.
    SET @entityname = dbo.api_create_table_name(@tablename,@companyname,@defaultGuid);
	--Get Quotes from ERP
	IF @tablename ='Sales Header'
	BEGIN

		Declare @QuoteLineEntity varchar(100) = dbo.api_create_table_name('Sales Line',@companyname,@defaultGuid); 
		Declare @SalesHeaderArchiveEntity varchar(100) =dbo.api_create_table_name('Sales Header Archive',@companyname,@defaultGuid); 
		Declare @SalesLineArchiveEntity varchar(100) =dbo.api_create_table_name('Sales Line Archive',@companyname,@defaultGuid);
		Declare @SalesHeaderTableExtensionName varchar(100) = dbo.api_create_table_name('Sales Header',@companyname,@ecommerceguid);
		Declare @SalesHeaderArchiveTableExtensionName varchar(100) = dbo.api_create_table_name('Sales Header Archive',@companyname,@ecommerceguid); 
	
	  --Get Code If Not Null

	  Declare @navCode tinyint =0;

	  Declare @formatedCode varchar(20) =' ';
	  IF (@Code <> ' ')
	  BEGIN
	    IF (SELECT SUBSTRING(@Code,1,3)) = 'NAV' 
		BEGIN
		  SET @navCode = 1;
		END;

	  SET @formatedCode = (SELECT SUBSTRING(@Code,5,LEN(@Code)));
	  END;

	DECLARE @conditionqh varchar(250) ='';
	DECLARE @conditionsha varchar(250) ='';
	IF @navCode =1 
	BEGIN
	SET @conditionqh =CONCAT('qh.[No_] =''',@formatedCode,'''');
	SET @conditionsha =CONCAT('sha.[No_] =''',@formatedCode,'''');
	END
	ELSE
	BEGIN
	
	SET @conditionqh =CONCAT('qhext.[POS No_] =''',@formatedCode,'''');
	SET @conditionsha =CONCAT('shaext.[POS No_] =''',@formatedCode,'''');
	END;

	
	SET @sql = CONCAT('WITH CTE AS(
						 SELECT COUNT(*) AS Cnt
							FROM ',@entityname,' AS qh 
							 inner join ',@SalesHeaderTableExtensionName,'as qhext
							 on qhext.[Document Type] = qh.[Document Type] 
							 and qhext.No_ = qh.No_
							 inner join ',@QuoteLineEntity,'as ql
							 on qh.[No_] = ql.[Document No_]
							  where qh.[Document Type]= 0 
							    and ',@conditionqh,'
							UNION
						    SELECT COUNT(*) AS Cnt
								from ',@SalesHeaderArchiveEntity,' sha
								 inner join ',@SalesHeaderArchiveTableExtensionName,' as shaext
								  on shaext.[Document Type] = sha.[Document Type] 
							 and shaext.No_ = sha.No_
								inner join ',@SalesLineArchiveEntity,' sla
										on sha.No_ = sla.[Document No_]
								where sha.[Document Type]= 0 
							    and ',@conditionsha,')
									SELECT SUM(Cnt) from CTE')


    --Excecute the query created.
	EXEC  sp_executesql @sql
	END;

END

