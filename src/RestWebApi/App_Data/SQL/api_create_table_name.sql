USE [DegertAutoDemo]
GO
/****** Object:  UserDefinedFunction [dbo].[api_create_table_name]    Script Date: 4/23/2021 12:07:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,IBS>
-- Create date: <Create Date, 12/07/2020>
-- Description:	<Description,Function to be called inside Procedure for retrieving the database full table name.>
-- =============================================
ALTER FUNCTION [dbo].[api_create_table_name]
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
