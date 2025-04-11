using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestWebApi.Models.Quotes
{
    public class ApiQuoteRequest
    {
        public ApiQuoteRequest()
        {

        }
        public string navCode { get; set; }
        public string posCode { get; set; }
    }
}