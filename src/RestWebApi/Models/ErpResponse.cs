using System.Xml.Serialization;

namespace RestWebApi.Models
{
    [XmlRoot(nameof(ErpResponse))]
    public class ErpResponse
    {
        public ErpResponse()
        {
                
        }
        public ErpResponse(string tablename, int status,string message, string fullmessage , string customerno,string orderno, string orderdate, int lineno, string itemno,string methodname)
        {
            TableName=tablename ;
            CustomerNo=customerno ;
            OrderNo= orderno ;
            OrderDate=orderdate;
            Message= message;
            FullMessage= fullmessage;
            Status =status;
            LineNo= lineno;
            ItemNo =itemno;
            MethodName = methodname;

        }
        /// <summary>
        /// Table name may be OrderHeader or OrderLine
        /// </summary>
        [XmlElement()]
        public string TableName { get; set; }
        /// <summary>
        /// Value of Customer Number
        /// </summary>
        [XmlElement()]
        public string CustomerNo { get; set; }
        /// <summary>
        /// Value Of Order Number
        /// </summary>
        [XmlElement()]
        public string OrderNo { get; set; }
        /// <summary>
        /// Value Of Order Date
        /// </summary>
        [XmlElement()]
        public string OrderDate { get; set; }
        /// <summary>
        /// Value of Status Code
        /// 0-> False; 1-> True
        /// </summary>
        [XmlElement()]
        public int Status { get; set; }
        /// <summary>
        /// Information Message
        /// </summary>
        [XmlElement()]
        public string Message { get; set; }
        /// <summary>
        /// Full Log Message
        /// </summary>
        [XmlElement()]
        public string FullMessage { get; set; }
        /// <summary>
        /// Line number of lines inserted
        /// </summary>
        [XmlElement()]
        public int LineNo { get; set; }
        /// <summary>
        /// Item No Value
        /// </summary>
        [XmlElement()]
        public string ItemNo { get; set; }
        /// <summary>
        /// Name of the method 
        /// </summary>
        [XmlElement()]
        public string MethodName { get; set; }
    }
}