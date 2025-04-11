using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestWebApi.Models.Quotes
{
    public class ApiQuote 
    {
        public ApiQuote()
        {
            ApiQuoteLines = new List<ApiQuoteLine>();
        }

        public string No { get; set; }
        /// <summary>
        /// Specified the Customer No which is making the order
        /// </summary>
        public string CustomerNo { get; set; }

        /// <summary>
        /// Code from POS
        /// </summary>
        public string POSCode { get; set; }
        /// <summary>
        /// Sales Quote Header Description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Sales Quote payment Methop
        /// </summary>
        public string PaymentMethod { get; set; }
        public string TransactionSpecification { get; set; }
        /// <summary>
        /// Sales Quote Currency Code
        /// </summary>
        public string CurrencyCode { get; set; }
        /// <summary>
        /// Currency Rate of the Quote
        /// </summary>
        public decimal CurrencyRate { get; set; }

        /// <summary>
        /// The datetime of the order creation
        /// </summary>
        public DateTime OrderDateTime { get; set; }
        /// <summary>
        /// FromPos
        /// </summary>
        public bool FromPos { get; set; }
        /// <summary>
        /// The Type of DocumentStatus
        /// </summary>
        public string DocumentStatusTypePOS { get; set; }

        public DateTime LastDateTimeModified { get; set; }
        /// <summary>
        /// Holds all order Lines
        /// </summary>
        public List<ApiQuoteLine> ApiQuoteLines { get; set; }
    }

    #region Nested Class

    public class ApiQuoteLine 
    {
        public string DocumentNo { get; set; }
        /// <summary>
        /// Specifies the Item Number that will be inserted in ERP
        /// </summary>
        public string ItemNo { get; set; }
        /// <summary>
        /// Item Unit of measure
        /// </summary>
        public  string UnitOfMeasure { get; set; }

        /// <summary>
        /// Vat Code of the line
        /// </summary>
        public string VatCode { get; set; }
        /// <summary>
        /// Specifies the Quantity that will be inserted in ERP
        /// </summary> 

        public decimal Quantity { get; set; }

        /// <summary>
        /// Price without vat
        /// </summary>
        public decimal PriceWithoutVat { get; set; }

        /// <summary>
        /// Price including vat
        /// </summary>
        public decimal PriceWithVat { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AmountWithVat { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal AmountWithoutVat { get; set; }

        /// <summary>
        ///  Specifies the Init Price that will be inserted in ERP
        /// </summary>
        //public decimal UnitPrice { set; get; }

        /// <summary>
        /// Specifies the Line Discount Amount that will be inserted in ERP
        /// </summary>
       // public decimal LineDiscountAmount { set; get; }

        ///<summary>
        /// Specifies the Line Discount Amount that will be inserted in ERP
        ///</summary>
       // public string CurrencyCode { get; set; }

        ///<summary>
        /// Specifies the Line discount percentage that will be inserted in ERP
        ///</summary>
        public decimal LineDiscountPercentage { get; set; }
    }
    #endregion
}