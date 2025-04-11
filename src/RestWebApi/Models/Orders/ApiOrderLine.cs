using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestWebApi.Models.Orders
{


    public class ApiOrderLine
    {
        /// <summary>
        /// Specifies the Item Number that will be inserted in ERP
        /// </summary>
        ///
        public string ItemNo{ get; set; }

        public string Location { get; set; }
        /// <summary>
        /// Specifies the Quantity that will be inserted in ERP
        /// </summary>
        public decimal Quantity { get; set; }
        
               /// <summary>
        ///  Specifies the Init Price that will be inserted in ERP
        /// </summary>
        //public decimal UnitPriceIncludingVat { set; get; }
        ///// <summary>
        ///// Line No
        ///// </summary>
        //public string LineNo { get; set; }
        //public string  DocNoOnline { get; set; }
    }
}