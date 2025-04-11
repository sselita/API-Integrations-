using JWT.Security.Models;
using RestWebApi.Models;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace RestWebApi.Services.Helpers
{
    public interface ISQLConnectionHelperService
    {
        SqlDataReader ExecuteApiReader(SqlConnection sqlConnection, SqlRequestParameter sqlRequest);
        string GenerateConnectionString();
        bool ValidateConnectionString(string conString);
        int ExecuteApiNonQuery(SqlConnection sqlConnection, string procedureName, string tableName, string CompanyName, ErpResponse response);
        T ExecuteScalar<T>(SqlConnection sqlConnection, SqlRequestParameter sqlRequest);
    }
}