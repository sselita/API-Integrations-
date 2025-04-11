USE [DegertAutoDemo]
GO
/****** Object:  StoredProcedure [dbo].[api_inventory]    Script Date: 4/27/2021 10:23:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[api_inventory]
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


