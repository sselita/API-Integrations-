using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestWebApi.Models.Orders
{
    public class ApiOrderResponse : ApiOrder
    {
        public string  OrderNo { get; set; }
       
    }
}