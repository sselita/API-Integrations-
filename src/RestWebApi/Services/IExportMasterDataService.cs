using RestWebApi.Models;




using RestWebApi.Models.Items;
using RestWebApi.Models.Orders;

namespace RestWebApi.Services
{
    public interface IExportMasterDataService
    {

        ApiGetItemsResponse GetItems(ApiRequestFilter request,string companyName);
        SalesOrderResponse GetOrders(string request, string companyName);



    }
}