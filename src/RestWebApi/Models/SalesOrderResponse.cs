
using RestWebApi.Models.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestWebApi.Models
{
    public class SalesOrderResponse : BaseResponse
    {

        public ApiOrderResponse SalesOrder { get; set; }
      

    } 
}
