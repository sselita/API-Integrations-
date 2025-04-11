using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestWebApi.Models.Orders
{
    public class ApiGetOrdersResponse
    {
        public ApiGetOrdersResponse()
        {
            OrderList = new List<ApiGetOrder>();
        }
        public List<ApiGetOrder> OrderList { get; set; }
        public BaseResponse BaseResponse { get; set; }
    }
}