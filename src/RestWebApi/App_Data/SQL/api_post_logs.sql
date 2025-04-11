USE [DeltaPharma2017FinalObjects]
GO
/****** Object:  StoredProcedure [dbo].[api_post_logs]    Script Date: 9/17/2021 4:41:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,IBS, Erjon Kuka>
-- Create date: <Create Date,12/07/2020>
-- Description:	<Description,Procedure storing logs>
-- =============================================
ALTER PROCEDURE [dbo].[api_post_logs]
	-- Add the parameters for the stored procedure here
	@tablename varchar(50),
	@companyname varchar(50),
    @Xml XML
AS
BEGIN
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
   SET @entityname =  dbo.api_create_table_name(@tablename,@companyname);

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

Select * from [Pas Migrimeve-Test$Web Service Logs]
