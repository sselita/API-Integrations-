using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestWebApi.Models.Orders
{
    public class ApiUpdateOrder
    {
        public ApiUpdateOrder()
        {
            ApiOrderLines = new List<ApiOrderLine>();
        }
        public string CustomerNo { get; set; }


        public string OrderNo { get; set; }

        //public string ItemNo { get; set; }
        //public decimal Quantity { get; set; }
        //public string Location { get; set; }

        public List<ApiOrderLine> ApiOrderLines { get; set; }
    }
}