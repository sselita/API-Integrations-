using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace RestWebApi.Models.Response.Quotes
{
    [XmlRoot("Results")]

    public class QuoteBaseResponse
    {
        public QuoteBaseResponse()
        {

        }
        public QuoteBaseResponse(string methodName, string message, string innerMessage, bool success, string orderDate,string posCode= "",bool existent=false)
        {
            Success = success;
            Message = message;
            InnerMessage = innerMessage;
            MethodName = methodName;
            PosCode = posCode;
            Existent = existent;
            OrderDate = orderDate;
        }

        [XmlElement()]
        public bool Success { get; set; }
        [XmlElement()]
        public string Message { get; set; }
        [XmlElement()]
        public string PosCode { get; set; }
        [XmlElement()]
        public bool Existent { get; set; }
        [XmlElement()]
        public string OrderDate { get; set; }
        [XmlElement()]
        public string InnerMessage { get; set; }
        [XmlElement()]
        public string MethodName { get; set; }
    }
}