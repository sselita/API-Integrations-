using JWT.Security.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestWebApi.Models
{
    public class CompanyResponse
    {
        public CompanyResponse()
        {
            Companies = new List<string>();
            baseResponse = new BaseResponse();
        }
         public List<string> Companies { get; set; }

        public BaseResponse baseResponse { get; set; }
    }
}