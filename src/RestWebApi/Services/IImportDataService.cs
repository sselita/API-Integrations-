using RestWebApi.Models;

using RestWebApi.Models.Orders;


namespace RestWebApi.Services
{
    public interface IImportDataService
    {
        SalesOrderResponse CreateOrder(ApiOrder apiOrder, string CompanyName);
        SalesOrderResponse UpdateOrder(ApiUpdateOrder apiUpdateOrder, string CompanyName);
        BaseResponse UpdateOrderStatus(ApiUpdateOrderStatus apiUpdateOrderStatus, string CompanyName);
       
    }
}