USE DegertAutoBC15
GO
/****** Object:  StoredProcedure [dbo].[api_check_exiting_entity]    Script Date: 4/23/2021 12:06:55 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[api_create_table_name]
(
	-- Add the parameters for the function here
	@tablename varchar(50),
	@companyname varchar(50),
    @guid varchar(50)
)
RETURNS nvarchar(100)
AS
BEGIN

   DECLARE  @full_tablename nvarchar(100)=''; 
 
   SET @full_tablename = CONCAT('[',@companyname,'$',@tablename, '$',@guid, ']');

   RETURN @full_tablename;
END

GO
CREATE PROCEDURE [dbo].[api_inventory]
 -- Add the parameters for the stored procedure here
    @ileTableName nvarchar(255),
    --@slTableName nvarchar(255),
    --@plTableName nvarchar(255),
    --@tlTableName nvarchar(255),
	@locTableName nvarchar(255),
	@locationCode varchar(20)= NULL,
	@startDate datetime2
AS
PRINT @locationCode
BEGIN
Declare @condition varchar(250) = 'WHERE 1=1 ';

IF @startDate IS NOT NULL  
BEGIN
	SET @condition =@condition+CONCAT('AND [Item No_] IN (SELECT ile2.[Item No_] FROM ',@ileTableName,' ile2 where ile2.[Posting Date]  >= CAST(''',@startDate,''' AS DATETIME2))');
END;

IF(@locationCode <> '')
BEGIN
	SET @condition =@condition+' AND ile.[Location Code] like   (''' + @locationCode + ''')';	
END;

Declare @sql as nvarchar(MAX)

IF(@locationCode is null or @locationCode like '')
BEGIN

set @sql = CONCAT('
	SELECT inv.ItemNo, inv.LocationCode, inv.LocationName,
       Cast(SUM(inv) AS INT) AS [Inventory] FROM
            (
			   SELECT [Item No_] as ItemNo, SUM([Remaining Quantity]) AS inv, [Location Code] as LocationCode , loc.Name as LocationName 
			   FROM', @ileTableName,' ile
				  inner join ',@locTableName,' loc 
                        on ile.[Location Code] = loc.Code
						',@condition,'
				GROUP BY [Item No_],[Location Code], loc.Name
				) AS inv
					
					GROUP BY inv.ItemNo , inv.LocationCode, inv.LocationName
					HAVING SUM(inv) > 0 and LocationCode <> '''' '
	)
END
ELSE 
BEGIN 
set @sql = CONCAT('
	SELECT inv.ItemNo, inv.LocationCode, inv.LocationName,
       Cast(SUM(inv) AS INT) AS [Inventory] FROM
            (
			   SELECT [Item No_] as ItemNo, SUM([Remaining Quantity]) AS inv , [Location Code] as LocationCode , loc.Name as LocationName 
			   FROM', @ileTableName,' ile
				  inner join ',@locTableName,' loc 
                        on ile.[Location Code] = loc.Code
						',@condition,'
				GROUP BY [Item No_],[Location Code], loc.Name
				) AS inv
					
					GROUP BY inv.ItemNo,LocationCode,LocationName
					HAVING SUM(inv)>0 and LocationCode <> '''' ' 
	)
END

EXEC  sp_executesql @sql
END
GO
-- =============================================
-- Author:		<Author,,IBS, Erjon Kuka>
-- Create date: <Create Date,12/07/2020>
-- Description:	<Description,Procedure for getting master data from BC Sql Database based on some specific conditions>
-- =============================================
CREATE PROCEDURE [dbo].[api_get_masterdata]
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

   --Create a temporary table to save GUID of table that may join with main Table.
   Create table #TableConfig
	(
		ID INT NOT NULL,
		TableName varchar(50) NOT NULL, 
		TableGuid Varchar(100) NOT NULL
	)

    --Case of Quote
    IF @tablename = 'Sales Header(Quote)'
     BEGIN
	   SET @tablename ='Sales Header';
	 END; 

    --Execute function which generates the full entity name of the main table.
    SET @entityname = dbo.api_create_table_name(@tablename,@companyname,@defaultGuid);

    --Get data from Item Table
    IF @tablename = 'Item'
    BEGIN
	DECLARE @salesprice_entity nvarchar(4000) ='';
	--Get the Sales Price Table GUID extension saved in the configuration.JSON on the Rest API.
	DECLARE @itemExtname nvarchar(4000) = dbo.api_create_table_name(@tablename,@companyname,@otherExtensionGuid);
	--Get the Sales Price Table GUID extension saved in the configuration.JSON on the Rest API.
	set @sql = CONCAT(
				'Select
				i.[No_] as ItemNo,
				i.[No_ 2] as ItemNo2,
				i.Description,
				i.[Base Unit of Measure] as BaseUnitOfMeasure,
				i.Blocked,
				i.[Vendor No_] as VendorNo,
				iext.Referencat as Referencat,
				i.[Last DateTime Modified] as LastDateTimeModified
				From',@entityname,' i
				INNER JOIN ',@itemExtname,' iext
				on i.[No_] = iext.[No_]
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
			PRINT @Code
			PRINT @sql
		
   END;

    --Get data from Sales Price Table
    IF @tablename = 'Sales Price'
   BEGIN

    Declare @salesPriceExt varchar(100) = dbo.api_create_table_name(@tablename,@companyname, @ecommerceguid); 
	

	set @sql = Concat('
					 
					Select 
					sl.[Item No_] as ItemNo,
					sl.[Sales Code] as CustomerGroup,
					sl.[Minimum Quantity] as MinimumQuantity,
					sl.[Unit of Measure Code] as UnitOfMeasureCode,
					(CASE WHEN sl.[Currency Code] = '' '' THEN ''LEK'' ELSE sl.[Currency Code] END) AS CurrencyCode,
					sl.[Unit Price] as UnitPrice ,
					sl.[Allow Invoice Disc_] as AllowInvoiceDiscount,
					sl.[Allow Line Disc_] as AllowLineDiscount,
					sl.[Price Includes VAT] as PriceIncludesVAT,
					sl.[Starting Date] as StartingDate,
					sl.[Ending Date] as EndingDate,
					slx.[Last Date Time Modified] as LastDateTimeModified
					From'
					,@entityname,' as sl
					inner join ',@salesPriceExt,' as slx
					on sl.[Item No_] =slx.[Item No_] 
							and sl.[Sales Code] = slx.[Sales Code]
							and sl.[Sales Type] = slx.[Sales Type] 
							and sl.[Starting Date] = slx.[Starting Date] 
							and sl.[Currency Code] = slx.[Currency Code] 
							and sl.[Variant Code] = slx.[Variant Code]
							and sl.[Minimum Quantity] = slx.[Minimum Quantity]
							and sl.[Unit of Measure Code] = slx.[Unit of Measure Code]
					where sl.[Sales Type] = 1')
					PRINT @sql
					
	IF @startDateFilter IS NOT NULL 
			BEGIN
				SET @sql = @sql + CONCAT(' and slx.[Last Date Time Modified] >=CAST(''',@startDateFilter,''' AS DATETIME2)');
			END
			IF @endDateFilter IS NOT NULL 
			BEGIN
				 SET @sql = @sql + CONCAT(' and slx.[Last Date Time Modified] <=CAST(''',@endDateFilter,''' AS DATETIME2)');
			END

   END

   --Get data from Customer Table
    IF @tablename = 'Customer'
    BEGIN 

	 --Create Full table Name For Customer Ledger Entry
     Declare @CustLegEntryname varchar(250)  = dbo.api_create_table_name('Cust_ Ledger Entry',@companyname, @defaultGuid); 

     --Create Full table Name For Detailed Customer Ledger Entry
	 Declare @DetCustLegEntryname varchar(250)  = dbo.api_create_table_name('Detailed Cust_ Ledg_ Entry',@companyname, @defaultGuid); 

	 --Extension to get Total Balance
	 Declare @customerOtherExt varchar(250) =dbo.api_create_table_name('Customer',@companyname, @otherExtensionGuid);

	SET @sql =CONCAT('SELECT 
						  c.No_ as CustomerNo,
						  c.Blocked,
						  c.[Credit Limit (LCY)] AS CreditLimitLCY,
						  c.Blocked as Active,
						  (CASE WHEN c.[Currency Code] = '' '' THEN ''LEK'' ELSE c.[Currency Code] END) AS CurrencyCode,
						  c.[VAT Registration No_] as NUIS,
						  c.[Phone No_] AS PhoneNr,
						  c.[Address],
						  c.[Payment Method Code] PaymentMethodCode,
						  c.[Payment Terms Code] PaymentTermsCode,
						  c.[Shipping Agent Code] as AgentCode,
						  (SELECT SUM(dtle.[Amount (LCY)]) 
							   FROM ',@CustLegEntryname ,' as cle 
								   inner join ',@DetCustLegEntryname,' as dtle
									 on cle.[Entry No_] = dtle.[Cust_ Ledger Entry No_]
										where cle.[Customer No_] = c.No_ AND cle.[Open] =1 ) AS BalanceLCY,

						  (SELECT SUM(dtle.[Amount (LCY)]) 
							   FROM ',@CustLegEntryname ,' as cle 
								   inner join ',@DetCustLegEntryname,' as dtle
									 on cle.[Entry No_] = dtle.[Cust_ Ledger Entry No_]
										where cle.[Customer No_] = c.No_ AND cle.[Due Date] < GETDATE()) AS RemainingAmountLCY,
		
							c.[Name] as Name,
							c.[E-Mail] as Email,
							c.[Customer Price Group] as CustomerPriceGroup,
							c.[Last Modified Date Time] as LastModifiedDateTime,
							c.Blocked,
							cext.[Total Balanca] as TotalBalance
                        FROM ', @entityname,' c
						inner join ',@customerOtherExt,' cext
						  on c.No_ = cext.No_
						  where c.No_ <> '''' ');

		IF (@Code <> ' ')
		  BEGIN
		    SET @sql =@sql +CONCAT(' and c.[No_] = CAST(''',@Code,''' AS NVARCHAR(20))');
		  END
	
		IF @startDateFilter IS NOT NULL 
			BEGIN
				SET @sql =@sql +CONCAT(' and c.[Last Modified Date Time] >= CAST(''',@startDateFilter,''' AS DATETIME2)');
			END

		IF @endDateFilter IS NOT NULL 
			BEGIN
				 SET @sql =@sql +CONCAT(' and c.[Last Modified Date Time] <= CAST(''',@endDateFilter,''' AS DATETIME2)');
			END
        
    END;	
	
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

	IF @Code =' '
	BEGIN 
	SET @conditionqh ='1 = 1';
	SET @conditionsha ='1 = 1';
	END
	--TOP(1)

	SET @sql = CONCAT('WITH CTE AS 
							(SELECT 
							qh.[No_] as No,
							(CASE WHEN qh.[Currency Code] = '' '' THEN ''LEK'' ELSE qh.[Currency Code] END) AS CurrencyCode,
							qh.[Currency Factor]as CurrencyRate,
							qh.[Sell-to Customer No_] as CustomerNo,
							qh.[Posting Date] as OrderDateTime,
							qh.[Payment Method Code] as PaymentMethod,
							qhext.[POS No_] as POSCode,
							qhext.[From POS] as FromPos,
							qhext.[Document Status Type POS] as DocumentStatusTypePOS,
							ql.[Document No_] as DocumentNo,
							ql.[No_] as ItemNo,
							ql.Quantity as Quantity,
							ql.[Line Discount _] as LineDiscountPercentage,
							ql.[Unit of Measure] as UnitOfMeasure,
							ql.[VAT Prod_ Posting Group] as VatCode,
							ql.[Line Amount] as AmountWithoutVat,
							ql.[Amount Including VAT] as AmountWithVat, 
							qhext.[Last Date Time Modified] as LastDateTimeModified
							FROM ',@entityname,' AS qh 
							 inner join ',@SalesHeaderTableExtensionName,'as qhext
							 on qhext.[Document Type] = qh.[Document Type] 
							 and qhext.No_ = qh.No_
							 inner join ',@QuoteLineEntity,'as ql
							 on qh.[No_] = ql.[Document No_]
							  where qh.[Document Type]= 0 and qhext.[From POS] =1
							    and ',@conditionqh,'
							UNION ALL
						   Select 
							sha.[No_] as No,
							(CASE WHEN sha.[Currency Code] = '' '' THEN ''LEK'' ELSE sha.[Currency Code] END) AS CurrencyCode,
							sha.[Currency Factor]as CurrencyRate,
							sha.[Sell-to Customer No_] as CustomerNo,
							sha.[Posting Date] as OrderDateTime,
							sha.[Payment Method Code] as PaymentMethod,
							shaext.[POS No_] as POSCode,
							shaext.[From POS] as FromPos,
							shaext.[Document Status Type POS] as DocumentStatusTypePOS,
							sla.[Document No_] as DocumentNo,
							sla.[No_] as ItemNo,
							sla.Quantity as Quantity,
							sla.[Line Discount _] as LineDiscountPercentage,
							sla.[Unit of Measure] as UnitOfMeasure,
							sla.[VAT Prod_ Posting Group] as VatCode,
							sla.[Line Amount] as AmountWithoutVat,
							sla.[Amount Including VAT] as AmountWithVat,
							CAST(sha.[Date Archived] AS DATETIME) + CAST(CAST(sha.[Time Archived] AS time) AS DATETIME) as LastDateTimeModified
								from ',@SalesHeaderArchiveEntity,' sha
								 inner join ',@SalesHeaderArchiveTableExtensionName,' as shaext
								  on shaext.[Document Type] = sha.[Document Type] 
							 and shaext.No_ = sha.No_
								inner join ',@SalesLineArchiveEntity,' sla
										on sha.No_ = sla.[Document No_]
								where sha.[Document Type]= 0 and shaext.[From POS] =1
							    and ',@conditionsha,'
								) SELECT * FROM CTE WHERE 1=1')

	IF @startDateFilter IS NOT NULL 
			BEGIN
				SET @sql =@sql +CONCAT(' and CTE.LastDateTimeModified >=CAST(''',@startDateFilter,''' AS DATETIME2)');--date modified
			END

		IF @endDateFilter IS NOT NULL 
			BEGIN
				 SET @sql =@sql +CONCAT(' and CTE.LastDateTimeModified <=CAST(''',@endDateFilter,''' AS DATETIME2)');--date modified
			END

	END;

	--Get Data from Gen Journal Line (Needs Word)
	IF @tablename = 'Gen_ Journal Line' --ARKA BANKA
	BEGIN

		Declare @GenJLGUIDEntity varchar(100) = dbo.api_create_table_name('Gen_ Journal Line',@companyname,@defaultguid); 

		
		SET @sql = CONCAT('SELECT 
							*
							FROM ',@GenJLGUIDEntity, ' as gjl 
							')


		--IF @startDateFilter IS NOT NULL 
		--	BEGIN
		--		SET @sql =@sql +CONCAT(' and gjl.[Posting Date] >=',@startDateFilter,' ');
		--	END

		--IF @endDateFilter IS NOT NULL 
		--	BEGIN
		--		 SET @sql =@sql +CONCAT(' and gjl.[Posting Date] <=',@endDateFilter,' ');
		--	END

	END

	--Get data from Location
	IF @tablename = 'Location' --Warehouse
	BEGIN

	Declare @LocationEntityext varchar(100) = dbo.api_create_table_name('Location',@companyname,@ecommerceguid); 
	
		SET @sql = CONCAT('SELECT 
							l.Code as Code,
							l.Name as Name,
							lx.[Last Date Time Modified] as LastDateTimeModified
							FROM ' ,@entityname,' as l
							inner join ',@LocationEntityext,'as lx
							on l.Code= lx.Code
							WHERE 1 = 1')

		IF (@Code <> ' ')
		  BEGIN
		    SET @sql =@sql +CONCAT(' and l.Code = CAST(''',@Code,''' AS NVARCHAR(20))');
		  END

		IF @startDateFilter IS NOT NULL 
			BEGIN
				SET @sql =@sql +CONCAT(' and lx.[Last Date Time Modified] >=CAST(''',@startDateFilter,''' AS DATETIME2)');--date modified
			END

		IF @endDateFilter IS NOT NULL 
			BEGIN
				 SET @sql =@sql +CONCAT(' and lx.[Last Date Time Modified] <=CAST(''',@endDateFilter,''' AS DATETIME2)');--date modified
			END

	END

	--Get Data from Item Category (May need work if filter by date is required)
	IF @tablename = 'Item Category'
	BEGIN

	
		Declare @ItemGroupEntity varchar(100) = dbo.api_create_table_name('Item Group',@companyname,@otherExtensionGuid); 


		SET @sql = CONCAT('SELECT 
							Code,
							[Parent Category] as ParentCategory,
							Description,
							[Has Children] as HasChildren
							FROM ' ,@entityname,
							'UNION ALL
							SELECT 
							Code,
							'''',
							Description,
							0
							FROM' ,@ItemGroupEntity,'
							')


		--IF @startDateFilter IS NOT NULL 
		--	BEGIN
		--		SET @sql =@sql +CONCAT(' and [Posting Date] >=',@startDateFilter,' ');
		--	END

		--IF @endDateFilter IS NOT NULL 
		--	BEGIN
		--		 SET @sql =@sql +CONCAT(' and [Posting Date] <=',@endDateFilter,' ');
		--	END
	END

	--Get Manufaturers
	IF @tablename = 'Manufacturer'
	BEGIN
		SET @sql = CONCAT('SELECT 
							Code,
							Name
							FROM ' ,@entityname)
	END
	
	--Get Inventory from ILE
	IF @tablename = 'Item Ledger Entry'
	BEGIN
		-- Get Inventory Data from procedure
		--Declare @sle varchar(100) = dbo.api_create_table_name('Sales Line',@companyname,@defaultguid); 
		--Declare @ple varchar(100) = dbo.api_create_table_name('Purchase Line',@companyname,@defaultguid); 
		--Declare @tle varchar(100) = dbo.api_create_table_name('Transfer Line',@companyname,@defaultguid); 
		Declare @loce varchar(100) = dbo.api_create_table_name('Location',@companyname,@defaultguid); 

		Exec dbo.api_inventory @entityname,@loce, @Code,@startDateFilter
	END

	IF @tablename ='Shipping Agent'
	BEGIN
	Declare @AgentEntityext varchar(100) = dbo.api_create_table_name('Shipping Agent',@companyname,@ecommerceguid); 
	SET @sql = CONCAT('SELECT 
							a.Code,
							a.Name,
							a.[Internet Address] as InternetAddress,
							aext.[WS Blocked] as Blocked
							FROM ' ,@entityname,' a
							INNER JOIN ',@AgentEntityext,' aext
							ON a.Code = aext.Code')
		IF (@Code <> ' ')
		  BEGIN
		    SET @sql =@sql +CONCAT(' WHERE [Code] = CAST(''',@Code,''' AS NVARCHAR(20))');
		  END
	END;

	IF @tablename ='Vendor'
	BEGIN
	Declare @VendorEntityext varchar(100) = dbo.api_create_table_name('Vendor',@companyname,@ecommerceguid); 
	SET @sql = CONCAT('SELECT 
							v.No_ as No,
							v.Name,
							v.Address,
							v.City,
							vext.[WS Blocked] as Blocked,
							v.[Territory Code] as TerritoryCode,
							(CASE WHEN v.[Currency Code] = '' '' THEN ''LEK'' ELSE v.[Currency Code] END) AS CurrencyCode,
							v.[Last Modified Date Time] as DateTimeModified
							FROM ' ,@entityname,' v
							INNER JOIN ',@VendorEntityext,' vext
							on v.No_ =vext.No_
							WHERE 1=1')
		IF (@Code <> ' ')
		  BEGIN
		    SET @sql =@sql +CONCAT(' AND [No_] = CAST(''',@Code,''' AS NVARCHAR(20))');
		  END

		  IF @startDateFilter IS NOT NULL 
			BEGIN
				SET @sql =@sql +CONCAT(' and [Last Modified Date Time] >= CAST(''',@startDateFilter,''' AS DATETIME2)');
			END

		IF @endDateFilter IS NOT NULL 
			BEGIN
				 SET @sql =@sql +CONCAT(' and [Last Modified Date Time] <= CAST(''',@endDateFilter,''' AS DATETIME2)');
			END
	END;

	--Excecute the query created.
	EXEC  sp_executesql @sql

	DROP TABLE #TableConfig
END


GO
-- =============================================
-- Author:		<Author,,IBS, Erjon Kuka>
-- Create date: <Create Date,12/07/2020>
-- Description:	<Description,Procedure storing logs>
-- =============================================
CREATE PROCEDURE [dbo].[api_post_logs]
	-- Add the parameters for the stored procedure here
	@tablename varchar(50),
	@companyname varchar(50),
    @Xml XML
AS
BEGIN
   Declare @guid varchar(50) = 'df1e71ee-a600-415c-bf99-17ad0aa3b7eb'
   Declare @defaultguid varchar(50) = '437dbf0e-84ff-417a-965d-ed2bb9650972'
   --Variable to store the Full entity name. 
   DECLARE  @entityname nvarchar(4000)='';
   --Variable to store the SQL query
   DECLARE @sql nvarchar(MAX)='';
   
   --Create a temporary table to save message Response in XML Format.
   Create table #Logs
	(
	    TableName varchar(50) null,
		CustomerNo varchar(50)  NULL, 
		[Status] INT NOT NULL,	
		[Message] varchar(500) null,	
		FullMessage Varchar(2048)  NULL,
		OrderNo varchar(50)  NULL, 
		OrderDate datetime null,
		[LineNo] int  NULL ,
		ItemNo varchar(20) null,
		MethodName varchar(30) null,
	   [Entry Date Time] datetime not null
	)
	  --Generate the full Name of 
   SET @entityname =  dbo.api_create_table_name(@tablename,@companyname, @guid);
   PRINT @entityname
	--Insertin the values from the XML document to the temporary table.
   Insert into #Logs 
	SELECT 
      Config.value('(TableName/text())[1]','VARCHAR(50)') AS TableName, 
	  Config.value('(CustomerNo/text())[1]','VARCHAR(20)') AS CustomerNo,
	   CASE 
	  WHEN Config.value('(Status/text())[1]','VARCHAR(10)') ='1' THEN 1
	   ELSE 0 
	   END,
	   Config.value('(Message/text())[1]','VARCHAR(500)') AS [Message],
	    Config.value('(FullMessage/text())[1]','VARCHAR(2048)') AS FullMessage, --TAG
	  Config.value('(OrderNo/text())[1]','VARCHAR(20)') AS OrderNo,
	  CAST(Config.value('(OrderDate/text())[1]','VARCHAR(50)') AS datetime) as OrderDate,	     
	  CAST(Config.value('(LineNo/text())[1]','VARCHAR(10)')as int )AS [LineNo],
	  Config.value('(ItemNo/text())[1]','VARCHAR(20)') AS ItemNo,--TAG  
	  Config.value('(MethodName/text())[1]','VARCHAR(20)') AS MethodName,--TAG  
	  GETDATE() AS [Entry Date Time]
      FROM
      @Xml.nodes('/ErpResponse')AS TEMPTABLE(Config)


	select * from #Logs
 

   --Create the SQL query for writing logs.
   SET @sql = 'INSERT INTO '+@entityname+'
     ([ID],[Table Name],[Customer No_],[Status],Message,[Full Message],[Order No],[Order Date],
	 [Line No],[Item No],[Method Name],[Entry DateTime]) SELECT (select ISNULL(MAX(ID)+1,1)
		from '+@entityname+'), * FROM #Logs';

	--Excecute the query created.
	EXEC  sp_executesql @sql

	DROP TABLE #Logs;

END
GO
-- =============================================
-- Author:		<Author,,IBS, Erjon Kuka>
-- Create date: <Create Date,12/07/2020>
-- Description:	<Description,Procedure for getting master data from BC Sql Database based on some specific conditions>
-- =============================================
CREATE PROCEDURE [dbo].[api_check_exiting_entity]
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