USE [DegertAutoDemo]
GO
/****** Object:  StoredProcedure [dbo].[api_inventory]    Script Date: 4/27/2021 9:33:12 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[api_inventory]
 -- Add the parameters for the stored procedure here
    @ileTableName nvarchar(255),
    @slTableName nvarchar(255),
    @plTableName nvarchar(255),
    @tlTableName nvarchar(255),
	@locTableName nvarchar(255),
	@locationCode varchar(20)
AS
BEGIN
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
				GROUP BY [Item No_],[Location Code], loc.Name
				/*UNION ALL

				SELECT [No_] as ItemNo, SUM(Quantity) AS inv, [Location Code] as LocationCode , loc.Name as LocationName 
				FROM', @slTableName,' sl 
                    inner join ',@locTableName,' loc 
                        on sl.[Location Code] = loc.Code
				WHERE
						  [Type]=2 
						  and [Document Type] in (3,5)
				   GROUP BY [No_],[Location Code],loc.Name
				UNION ALL

				SELECT [No_] as ItemNo, -SUM(Quantity) AS inv, [Location Code] as LocationCode , loc.Name as LocationName 
				FROM ',@slTableName,' sl 
                    inner join ',@locTableName,' loc 
                        on sl.[Location Code] = loc.Code
				WHERE
						  [Type]=2 
						  and [Document Type] in (1,2)
					  GROUP BY [No_],[Location Code],loc.Name
				 UNION ALL

				SELECT [No_] as ItemNo, -SUM(Quantity) AS inv, [Location Code] as LocationCode , loc.Name as LocationName 
				FROM ',@plTableName,' pl 
                    inner join ',@locTableName,' loc 
                        on pl.[Location Code] = loc.Code
				WHERE
						  [Type]=2 
						  and [Document Type] in (3,5)
				GROUP BY [No_],[Location Code],loc.Name
				UNION ALL

				SELECT [No_] as ItemNo, SUM(Quantity) AS inv , [Location Code] as LocationCode , loc.Name as LocationName FROM
				',@plTableName,' pl 
                    inner join ',@locTableName,' loc 
                        on pl.[Location Code] = loc.Code
				WHERE
						  [Type]=2 
						  and [Document Type] in (1,2)
			    GROUP BY [No_],[Location Code],loc.Name
				UNION ALL

				SELECT [Item No_] as ItemNo, SUM(Quantity) AS inv, [Transfer-to Code] as LocationCode , loc.Name as LocationName 
				FROM ',@tlTableName,' tl 
                    inner join ',@locTableName,' loc 
                        on tl.[Transfer-to Code] = loc.Code
				GROUP BY [Item No_],[Transfer-to Code], loc.Name
				UNION ALL

				SELECT [Item No_] as ItemNo, -SUM(Quantity) AS inv, [Transfer-from Code] as LocationCode , loc.Name as LocationName 
				FROM', @tlTableName ,' tl 
                    inner join ',@locTableName,' loc 
                        on tl.[Transfer-from Code] = loc.Code
			   GROUP BY [Item No_],[Transfer-from Code], loc.Name*/ ) AS inv
					
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
						WHERE ile.[Location Code] like   (''' + @locationCode + ''')
				GROUP BY [Item No_],[Location Code], loc.Name
				/*UNION ALL

				SELECT [No_] as ItemNo, SUM(Quantity) AS inv , [Location Code] as LocationCode , loc.Name as LocationName
				FROM', @slTableName,' sl 
                    inner join ',@locTableName,' loc 
                        on sl.[Location Code] = loc.Code
						WHERE sl.[Location Code] like   (''' + @locationCode + ''') and
						  [Type]=2 
						 and  [Document Type] in (3,5)
				   GROUP BY [No_],[Location Code],loc.Name
				UNION ALL

				SELECT [No_] as ItemNo, -SUM(Quantity) AS inv, [Location Code] as LocationCode , loc.Name as LocationName
				FROM ',@slTableName,' sl 
                    inner join ',@locTableName,' loc 
                        on sl.[Location Code] = loc.Code
						WHERE sl.[Location Code] like  (''' + @locationCode + ''') and
						  [Type]=2 and
						   [Document Type] in (1,2)
					  GROUP BY [No_],[Location Code],loc.Name
				 UNION ALL

				SELECT [No_] as ItemNo, -SUM(Quantity) AS inv, [Location Code] as LocationCode , loc.Name as LocationName
				FROM ',@plTableName,' pl 
                    inner join ',@locTableName,' loc 
                        on pl.[Location Code] = loc.Code
						WHERE pl.[Location Code] like  (''' + @locationCode + ''') and
						  [Type]=2 and
						   [Document Type] in (3,5)
				GROUP BY [No_],[Location Code],loc.Name
				UNION ALL

				SELECT [No_] as ItemNo, SUM(Quantity) AS inv, [Location Code] as LocationCode , loc.Name as LocationName
				FROM ',@plTableName,' pl 
                    inner join ',@locTableName,' loc 
                        on pl.[Location Code] = loc.Code
						WHERE pl.[Location Code] like   (''' + @locationCode + ''') and
						  [Type]=2 and
						   [Document Type] in (1,2)
			    GROUP BY [No_],[Location Code],loc.Name
				UNION ALL

				SELECT [Item No_] as ItemNo, SUM(Quantity) AS inv, [Transfer-to Code] as LocationCode , loc.Name as LocationName 
				FROM ',@tlTableName,' tl 
                    inner join ',@locTableName,' loc 
                        on tl.[Transfer-to Code] = loc.Code
						WHERE tl.[Transfer-to Code] like  (''' + @locationCode + ''')
				GROUP BY [Item No_],[Transfer-to Code], loc.Name
				UNION ALL

				SELECT [Item No_] as ItemNo, -SUM(Quantity) AS inv, [Transfer-from Code] as LocationCode , loc.Name as LocationName
				FROM', @tlTableName ,' tl 
                    inner join ',@locTableName,' loc 
                        on tl.[Transfer-from Code] = loc.Code
						where tl.[Transfer-from Code] like  (''' + @locationCode + ''')
			   GROUP BY [Item No_],[Transfer-from Code], loc.Name*/ ) AS inv
					
					GROUP BY inv.ItemNo,LocationCode,LocationName
					HAVING SUM(inv)>0 and LocationCode <> '''' ' 
	)
END
EXEC  sp_executesql @sql
END
