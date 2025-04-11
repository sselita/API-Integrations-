using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestWebApi.Models.Quotes
{
    public class ApiGetQuotesResponse
    {
        public ApiGetQuotesResponse()
        {
            Quotes = new List<ApiQuote>();
        }
        public List<ApiQuote> Quotes { get; set; }

        public BaseResponse BaseResponse { get; set; }
    }
}