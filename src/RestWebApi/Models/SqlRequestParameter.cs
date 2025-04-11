using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RestWebApi.Models
{
    public class SqlRequestParameter
    {
        public SqlRequestParameter(string tableName,string companyName,string procedureName, string code, DateTime? start,DateTime? end)
        {
            tablename = tableName;
            companyname = companyName;
            procedurename = procedureName;
            Code = code;
            StartDateRange = start;
            EndDateRange = end;
        }
        public string  tablename { get; set; }
        public string  companyname { get; set; }
        public string  procedurename { get; set; }

        //Optional Parameters
        public string Code { get; set; }
        public DateTime? StartDateRange { get; set; }

        public DateTime? EndDateRange { get; set; }
    }

}
