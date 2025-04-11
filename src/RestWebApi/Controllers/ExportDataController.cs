using RestWebApi.Models;
using RestWebApi.Services;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace RestWebApi.Controllers
{
    [RoutePrefix("api/exportdata")]

    public class ExportDataController : ApiController
    {

        #region Properties and Ctor
        private readonly IExportMasterDataService _exportMasterDataService;
        //private static string companyName = GetCompanyName();
        public ExportDataController(IExportMasterDataService exportMasterDataService)
        {
            this._exportMasterDataService = exportMasterDataService;
        }
        #endregion

        #region ApiMethods
        [Authorize]
        [CheckCompany]
        [Route("items")]
        public HttpResponseMessage GetItems([FromUri] ApiRequestFilter apiRequest)
        {
            if (apiRequest == null)
                apiRequest = new ApiRequestFilter();

            string companyName = CompanyService.GetCompanyName(Request);
            //Get all items from the service.
            var items = _exportMasterDataService.GetItems(apiRequest, companyName);

            //Manage success response
            if (items.BaseResponse.Success)
                return Request.CreateResponse(HttpStatusCode.OK, items);

            //Manage failure response and descibe the type of error.
            return Request.CreateResponse(HttpStatusCode.InternalServerError, items);
        }

        [Authorize]
        [CheckCompany]
        [Route("orders")]
        public HttpResponseMessage GetOrders([FromUri] string  apiRequest)
        {

            SalesOrderResponse salesOrderResponse = new SalesOrderResponse();
            string companyName = CompanyService.GetCompanyName(Request);
            //Get all items from the service.
            var orders = _exportMasterDataService.GetOrders(apiRequest, companyName);

            //Manage success response
            if (orders.Success)
                return Request.CreateResponse(HttpStatusCode.OK, orders);

            //Manage failure response and descibe the type of error.
            return Request.CreateResponse(HttpStatusCode.InternalServerError, orders);
        }


        #endregion

    }
}
