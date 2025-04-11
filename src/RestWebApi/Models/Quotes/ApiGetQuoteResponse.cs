using JWT.Security.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestWebApi.Models.Quotes
{
    public class ApiGetQuoteResponse
    {
        public ApiQuote Quote { get; set; }

        public BaseResponse BaseResponse { get; set; }
    }
}