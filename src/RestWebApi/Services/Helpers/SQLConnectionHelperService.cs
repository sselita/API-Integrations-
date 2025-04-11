using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Text;
using System.Xml;
using System.Linq;
using JWT.Security.Models;
using RestWebApi.Models;


namespace RestWebApi.Services.Helpers
{
    public class SQLConnectionHelperService : ISQLConnectionHelperService
    {
        //Global variable for Configuration
        private static Configuration config = CommonHelperService.GetJsonFileData();

     
        /// <summary>
        /// Method for validating the conenction string generated.
        /// </summary>
        /// <returns></returns>
        public bool ValidateConnectionString(string conString)
        {
            bool isValidConnectionString = true;
            try
            {
                var con = new SqlConnectionStringBuilder(conString);
                using (SqlConnection conn = new SqlConnection(con.ConnectionString))
                {
                    conn.Open();
                    return (conn.State == ConnectionState.Open);
                }
            }
            catch (Exception)
            {
                // can be KeyNotFoundException, FormatException, ArgumentException
                isValidConnectionString = false;
            }
            return isValidConnectionString;
        }

        /// <summary>
        /// Method for generating connection string based on some specific parameters given in JSON file.
        /// </summary>
        /// <param name="GUID">OUTPut param user for creating the string of Table Name</param>
        /// <param name="CompanyName">OUTPut param user for creating the string of Table Name</param>
        /// <returns></returns>
        public string GenerateConnectionString()
        {

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = config.SQLConnection.DataSource;
            builder.InitialCatalog = config.SQLConnection.InitialCatalog;
            builder.IntegratedSecurity = config.SQLConnection.IntegratedSecurity;
            if (!string.IsNullOrEmpty(config.SQLConnection.UserID))
            {
                builder.UserID = config.SQLConnection.UserID;
                builder.Password = config.SQLConnection.Password;
            }

            return builder.ConnectionString;
        }

        /// <summary>
        /// Helper method used for structuring the code . Since the procedure name for master Date will be always the same we use this method  to connect and read data from SQL in case of Bussiness Central.
        /// </summary>
        /// <param name="sqlConnection"></param>
        /// <param name="tableName"></param>
        /// <param name="CompanyName"></param>
        /// <param name="GUID"></param>
        /// <returns></returns>
        public SqlDataReader ExecuteApiReader(SqlConnection sqlConnection, SqlRequestParameter sqlRequest, List<TableGUIDS> GUIDS)
        {
            string entityGUID = "";
            //Filter with specific table name to get the Table GUID
            var TableGuid = GUIDS.Where(x => x.TableName.Equals(sqlRequest.tablename)).FirstOrDefault();
            if (TableGuid != null)
                entityGUID = TableGuid.GUID;

            //Gets Default GUID
            if (string.IsNullOrEmpty(entityGUID))
                entityGUID = GUIDS.Where(x => x.ID == 1).FirstOrDefault().GUID;

            //Create a list of TABLES with specific GUID different from entity from the request.
            //var OtherEntities = GUIDS.Where(x => !x.TableName.Equals(sqlRequest.tablename)).ToList();

            //Generates XML for other Entities.
            string XmlValues = CreateXML(GUIDS);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(XmlValues.ToString());

            SqlCommand cmd = new SqlCommand();

            sqlConnection.Open();
            cmd.CommandTimeout = 1800;
            cmd.CommandText = sqlRequest.procedurename;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = sqlConnection;

            //Parameter for the table that the request was made.
            cmd.Parameters.AddWithValue("@tablename", sqlRequest.tablename);
            cmd.Parameters.AddWithValue("@companyname", sqlRequest.companyname);
            //GUID of the TABLE
            cmd.Parameters.AddWithValue("@guid", entityGUID);

            //GUIDS of the tables that the request tableName may join in SQL. Parameter of type XML.
            cmd.Parameters.AddWithValue("@Xml", xmlDoc.InnerXml);
            cmd.Parameters.AddWithValue("@Code", sqlRequest.Code);
            cmd.Parameters.AddWithValue("@startDateFilter", sqlRequest.StartDateRange);
            cmd.Parameters.AddWithValue("@endDateFilter", sqlRequest.EndDateRange);
            SqlDataReader readerC = cmd.ExecuteReader();
            return readerC;
        }


        /// <summary>
        /// Helper method used for structuring the code . Since the procedure name for master Date will be always the same we use this method  to connect and read data from SQL in case of Navision.
        /// </summary>
        /// <param name="sqlConnection"></param>
        /// <param name="tableName"></param>
        /// <param name="CompanyName"></param>
        /// <returns></returns>
        public SqlDataReader ExecuteApiReader(SqlConnection sqlConnection, SqlRequestParameter sqlRequest)
        {
   
            SqlCommand cmd = new SqlCommand();

            sqlConnection.Open();
            cmd.CommandTimeout = 1800;
            cmd.CommandText = sqlRequest.procedurename;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = sqlConnection;

            //Parameter for the table that the request was made.
            cmd.Parameters.AddWithValue("@tablename", sqlRequest.tablename);
            cmd.Parameters.AddWithValue("@companyname", sqlRequest.companyname);
            cmd.Parameters.AddWithValue("@Code", sqlRequest.Code);
            cmd.Parameters.AddWithValue("@startDateFilter", sqlRequest.StartDateRange);
            cmd.Parameters.AddWithValue("@endDateFilter", sqlRequest.EndDateRange);

            SqlDataReader reader = cmd.ExecuteReader();

            return reader;
        }

        /// <summary>
        /// Helper method used for structuring the code . Since the procedure name for master Date will be always the same we use this method  to connect and read data from SQL in case of Navision.
        /// </summary>
        /// <param name="sqlConnection"></param>
        /// <param name="tableName"></param>
        /// <param name="CompanyName"></param>
        /// <returns></returns>
        public T ExecuteScalar<T>(SqlConnection sqlConnection, SqlRequestParameter sqlRequest)
        {

            SqlCommand cmd = new SqlCommand();

            sqlConnection.Open();
            
            cmd.CommandText = sqlRequest.procedurename;
            cmd.CommandTimeout = 1800;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = sqlConnection;

            //Parameter for the table that the request was made.
            cmd.Parameters.AddWithValue("@tablename", sqlRequest.tablename);
            cmd.Parameters.AddWithValue("@companyname", sqlRequest.companyname);
            cmd.Parameters.AddWithValue("@Code", sqlRequest.Code);
            cmd.Parameters.AddWithValue("@startDateFilter", sqlRequest.StartDateRange);
            cmd.Parameters.AddWithValue("@endDateFilter", sqlRequest.EndDateRange);

            var result = cmd.ExecuteScalar();
            sqlConnection.Close();
            return (T)result;
        }
        /// <summary>
        /// Helper method for execution NopQuery in SQL for Bussiness Central
        /// </summary>
        /// <param name="sqlConnection"></param>
        /// <param name="tableName"></param>
        /// <param name="CompanyName"></param>
        /// <param name="GUID"></param>
        /// <returns></returns>
        public int ExecuteApiNonQuery(SqlConnection sqlConnection, string procedureName, string tableName, string CompanyName, ErpResponse response)
        {
            //Create a list of TABLES with specific GUID different from entity from the request.
            var responseErp = response;

            //Generates XML for other Entities.
            string XmlValues = CreateXML(null, responseErp);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(XmlValues.ToString());

            SqlCommand cmd = new SqlCommand();

            sqlConnection.Open();
            cmd.CommandTimeout = 1800;
            cmd.CommandText = procedureName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = sqlConnection;

            //Parameter for the table that the request was made.
            cmd.Parameters.AddWithValue("@tablename", tableName);
            cmd.Parameters.AddWithValue("@companyname", CompanyName);

            //inner message of exception. Parameter of type XML.
            cmd.Parameters.AddWithValue("@Xml", xmlDoc.InnerXml);

            int validity = cmd.ExecuteNonQuery();
            sqlConnection.Close();
            return validity;
        }

        /// <summary>
        /// Helper method for execution NopQuery in SQL for Navision
        /// </summary>
        /// <param name="sqlConnection"></param>
        /// <param name="tableName"></param>
        /// <param name="CompanyName"></param>
        /// <param name="GUID"></param>
        /// <returns></returns>
        public int ExecuteApiNonQuery(SqlConnection sqlConnection, string procedureName, string tableName, string CompanyName)
        {
            SqlCommand cmd = new SqlCommand();

            sqlConnection.Open();
            cmd.CommandTimeout = 1800;
            cmd.CommandText = procedureName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = sqlConnection;

            //Parameter for the table that the request was made.
            cmd.Parameters.AddWithValue("@tablename", tableName);
            cmd.Parameters.AddWithValue("@companyname", CompanyName);

            int validity = cmd.ExecuteNonQuery();
            sqlConnection.Close();
            return validity;
        }


        /// <summary>
        /// Create an XML  for BC table GUID
        /// </summary>
        /// <param name="GUIDS">lIST OF GUIDS from BC.</param>
        /// <returns></returns>
        private string CreateXML(List<TableGUIDS> GUIDS=null,ErpResponse response= null)
        {
            try
            {
                var sb = new StringBuilder();
                if (GUIDS != null)
                {
                    // Save to XML string
                    XmlSerializer ser = new XmlSerializer(GUIDS.GetType());
                    using (var writer = XmlWriter.Create(sb))
                    {
                        ser.Serialize(writer, GUIDS);
                    }
                }
                else
                {
                    // Save to XML string
                    XmlSerializer ser = new XmlSerializer(response.GetType());
                    using (var writer = XmlWriter.Create(sb))
                    {
                        ser.Serialize(writer, response);
                    }
                }
                return sb.ToString();
            }
            catch(Exception e)
            {
                //Need to implement a logic for handeling errors
                throw e;
            }
        }
    }
}