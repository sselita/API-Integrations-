using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RestWebApi.Models
{
    [XmlRoot("Results")]
    public   class BaseResponse
    {
        public BaseResponse()
        {

        }
        public BaseResponse(string methodName, string message, string innerMessage, bool success)
        {
            Success = success;
            Message = message;
            InnerMessage = innerMessage;
            MethodName = methodName;
        }

        [XmlElement()]
        public bool Success { get; set; }
        [XmlElement()]
        public string  Message { get; set; }
        [XmlElement()]
        public string  InnerMessage { get; set; }
        [XmlElement()]
        public string MethodName { get; set; }

        
    }
}
