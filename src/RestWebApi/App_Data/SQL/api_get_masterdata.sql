USE [DeltaPharma2017FinalObjects]
GO
/****** Object:  StoredProcedure [dbo].[api_get_masterdata]    Script Date: 9/17/2021 4:05:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,IBS, Erjon Kuka>
-- Create date: <Create Date,12/07/2020>
-- Description:	<Description,Procedure for getting master data from BC Sql Database based on some specific conditions>
-- =============================================
Create PROCEDURE [dbo].[api_get_masterdata]
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
 



    

   
   


 IF @tablename = 'Item'
    BEGIN


	Set @entityname = dbo.api_create_table_name('Item',@companyname);

	

	set @sql = CONCAT(
				'Select
				i.[No_] as ItemNo,
				i.Description,
				i.[Base Unit of Measure] as BaseUnitOfMeasure
				
				From',@entityname,' i
				
				Where i.[No_] is not null
				')
				

			  IF (@Code <> ' ')
			  BEGIN
				SET @sql =@sql +CONCAT(' and [No_] = CAST(''',@Code,''' AS NVARCHAR(20))');
			  END
	

			IF @startDateFilter IS NOT NULL 
			BEGIN
				SET @sql = @sql + CONCAT(' and [Last DateTime Modified] >= CAST(''',@startDateFilter,''' AS DATETIME2)');
			END
			IF @endDateFilter IS NOT NULL 
			BEGIN
				 SET @sql = @sql + CONCAT(' and [Last DateTime Modified] <=CAST(''',@endDateFilter,''' AS DATETIME2)');
			END
		
		
   END;
   EXEC  sp_executesql @sql

   END

--   Create FUNCTION [dbo].[api_create_table_name]
--(
--	-- Add the parameters for the function here
--	@tablename varchar(50),
--	@companyname varchar(50)
--)
--RETURNS nvarchar(100)
--AS
--BEGIN

--   DECLARE  @full_tablename nvarchar(100)=''; 
 
--   SET @full_tablename = CONCAT('[',@companyname,'$',@tablename,']');

--   RETURN @full_tablename;
--END