using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestWebApi.Models.Orders
{
    public class ApiGetOrder
    {
        public ApiGetOrder()
        {
            OrderLines = new List<ApiOrderLine>();
        }
        public string OrderNo { get; set; }
        public string ItemNo { get; set; }
        public string Quantity { get; set; }
        public DateTime PostingDate { get; set; }
        //public string DocNoOnline { get; set; }
        public List<ApiOrderLine> OrderLines { get; set; }
    }
}