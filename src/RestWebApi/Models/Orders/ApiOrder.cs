using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestWebApi.Models.Orders
{
    public enum OrderStatus
    {
       Ordered , Confirm , Cancel
    }


    public class ApiOrder
    {
       
        public ApiOrder()
        {
            ApiOrderLines = new List<ApiOrderLine>();
        }
        /// <summary>
        ///  get or set the DocNoOnline 
        /// </summary>
        //public string DocNoOnline { get; set; }
        /// <summary>
        /// Specifiy the Customer No which is making the order
        /// </summary>
        public string CustomerNo { get; set; }
         public string Status { get; set; }
     
        /// <summary>
        /// The datetime of the order creation
        /// </summary>
        public DateTime OrderDateTime { get; set; }

        //public OrderStatus OrderStatus { get; set; }
        /// <summary>
        /// get or sets the salespersonCode
        /// </summary>
        //public string  SalesPersonCode { get; set; }
        /// <summary>
        /// Holds all order Lines
        /// </summary>
        public List<ApiOrderLine> ApiOrderLines { get; set; }
    }
}
