using JWT.Security.Models;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace RestWebApi.Models.Items
{
    /// <summary>
    /// Class used for returning the result . Inherits from abstract class BaseResponse whitch manages the Errors.
    /// </summary>
    
    public class ApiGetItemsResponse 
    {
        public ApiGetItemsResponse()
        {
            ItemList = new List<ApiGetItem>();
        }

        public List<ApiGetItem> ItemList { get; set; }

        public BaseResponse BaseResponse { get; set; }
    }
}