using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestWebApi.Models.Orders
{
    public class ApiUpdateOrderStatus
    {
        public string OrderNo { get; set; }
        public StatusApi OrderStatus { get; set; }
    }

    public enum StatusApi
    {
        Opened =0,
       Released =3
    }
}