using System;

namespace RestWebApi.Models.Items
{
    public partial class ApiGetItem 
    {
        /// <summary>
        /// Item No from Bussiness Central
        /// </summary>
        public string ItemNo { get; set; }
        //public string ItemNo2 { get; set; }      
        public string Description { get; set; }
        public string BaseUnitOfMeasure { get; set; }
        //public string VendorNo { get; set; }
        //public string Referencat { get; set; }
        //public string VendorName { get; set; }
        //public bool Blocked { get; set; }
        //public DateTime LastDateTimeModified { get; set; }
        //public string ItemId { get; set; }
        
        
        
    }
}