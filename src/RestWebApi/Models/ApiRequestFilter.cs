using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestWebApi.Models
{
    public class ApiRequestFilter
    {
        public ApiRequestFilter()
        {
           // Code = string.Empty;
        }
        public ApiRequestFilter(string code)
        {
            Code = code;
        }
        public ApiRequestFilter(DateTime? start, DateTime? end)
        {
            StartDate = start;
            EndDate = end;
        }
        public string Code { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}