namespace RestWebApi.Models
{
    public static class BCExtension
    {
        /// <summary>
        /// Property values in this section MUST be the same as in the Configuration.JSON file. Otherwise errors will be thrown.
        /// </summary>
        public static string ItemTable = "Item";
        public static string SalesPriceTable = "Sales Price";
        public static string ItemGroupTable = "Item Category";
        public static string InventoryTable = "Item Ledger Entry";
        //public static string CustomerTable = "Customer";
        public static string SalesHeaderTable = "Sales Header";
        public static string SalesLineTable = "Sales Line";
        //public static string SalesHeaderQuoteTable = "Sales Header(Quote)";
        //public static string SalesLineQuoteTable = "Sales Line(Quote)";
        //public static string CashBank = "Gen_ Journal Line";
        public static string LocationTable = "Location";
        public static string ManufacturerTable = "Manufacturer";
        public static string LogTable = "Web Service Logs";
        public static string CompanyTable = "Companies";
        //public static string Agents = "Shipping Agent";
        //public static string Vendors = "Vendor";


        //MethodNames
        public static string GetItemsMethod = "GetItems";
        public static string GetOrdersMethod = "GetOrders";

        //public static string GetSalesPriceMethod = "GetPrices";
        //public static string GetItemGroupMethod = "GetItemGroup";
        //public static string GetCashBankMethod = "GetCashBank";
        //public static string GetInventoryMethod = "ItemLedgerEntry";
        //public static string GetLocationsMethod = "GetLocation";
        //public static string GetManufacturersMethod = "GetManufacturers";
        //public static string GetCustomerMethod = "GetCustomer";
        //public static string CreateCustomerMethod = "CreateCustomer";
        public static string CreateOrderMethod = "CreateOrder";
        public static string UpdateOrderMethod = "UpdateOrder";
        //public static string CreateQuoteMethod = "CreateQuote";
        public static string GetQuoteMethod = "GetQuotes";
        public static string GetCompaniesMethod = "GetCompanies";
        //public static string GetAgentsMethod = "GetAgents";
        //public static string GetVendorsMethod = "GetVendors";

        //Procedure Name
        public static string GetMasterDataProcedure = "api_get_masterdata";
        public static string PostLogsProcedure = "api_post_logs";
        public static string CheckExistingEntityProcedure = "api_check_exiting_entity";

        //Static Fields

        public static string DefaultERPDateTime = "1753-01-01 00:00:00.000";
    }
}
