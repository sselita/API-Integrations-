using JWT.Security.Models;
using RestWebApi.Models;
using System;
using System.Data.SqlClient;

namespace RestWebApi.Services.Helpers
{
    /// <summary>
    /// Extension Class used for validating Values from SQL.
    /// https://blog.bitscry.com/2017/07/27/sqldatareader-null-handling/ =>Source
    /// </summary>
    public static class SqlDataReaderValidation
    {
        #region HelperMethods
        public static string SafeGetString(this SqlDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
            {
                return reader.GetString(colIndex);
            }
            else
            {
                return "";
            }
        }

        public static string SafeGetString(this SqlDataReader reader, string colName)
        {
            int colIndex = reader.GetOrdinal(colName);

            if (!reader.IsDBNull(colIndex))
            {
                return reader.GetString(colIndex);
            }
            else
            {
                return "";
            }
        }

        public static int SafeGetInt(this SqlDataReader reader, int colIndex) => !reader.IsDBNull(colIndex) ? reader.GetInt32(colIndex) : 0;

        public static int SafeGetInt(this SqlDataReader reader, string colName)
        {
            int colIndex = reader.GetOrdinal(colName);

            if (!reader.IsDBNull(colIndex))
            {
                return reader.GetInt32(colIndex);
            }
            else
            {
                return 0;
            }
        }

        public static bool SafeGetBool(this SqlDataReader reader, int colIndex) => !reader.IsDBNull(colIndex) ? reader.GetBoolean(colIndex) : false;

        public static bool SafeGetBool(this SqlDataReader reader, string colName)
        {
            int colIndex = reader.GetOrdinal(colName);

            if (!reader.IsDBNull(colIndex))
            {
                var val  = Convert.ToBoolean(reader.GetInt32(colIndex));
                return val;
            }
            else
            {
                return false;
            }
        }

        public static decimal SafeGetDecimal(this SqlDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
            {
                return reader.GetDecimal(colIndex);
            }
            else
            {
                return 0;
            }
        }

        public static decimal SafeGetDecimal(this SqlDataReader reader, string colName)
        {
            int colIndex = reader.GetOrdinal(colName);

            if (!reader.IsDBNull(colIndex))
            {
                return reader.GetDecimal(colIndex);
            }
            else
            {
                return 0;
            }
        }

        public static DateTime SafeGetDateTime(this SqlDataReader reader, string colName)
        {
            int colIndex = reader.GetOrdinal(colName);



            if (!reader.IsDBNull(colIndex))
            {
                return Convert.ToDateTime(reader.GetDateTime(colIndex));
            }
            else
            {
                return Convert.ToDateTime(BCExtension.DefaultERPDateTime);
            }
        }



        public static DateTime SafeGetDateTime(this SqlDataReader reader, int colName)
        {



            if (!reader.IsDBNull(colName))
            {
                return reader.GetDateTime(colName);
            }
            else
            {
                return Convert.ToDateTime(BCExtension.DefaultERPDateTime);
            }
        }
        public static int SafeGetByte(this SqlDataReader reader, int colIndex)
        {
            return !reader.IsDBNull(colIndex) ? reader.GetByte(colIndex) : 0;
        }

        public static int SafeGetByte(this SqlDataReader reader, string colName)
        {
            var colIndex = reader.GetOrdinal(colName);

            if (!reader.IsDBNull(colIndex))
            {
                return reader.GetByte(colIndex);
            }
            else
            {
                return 0;
            }
        }
        #endregion
    }
}

