using Newtonsoft.Json;
using RestWebApi.Models.Orders;
using RestWebApi.Services;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace RestWebApi.Controllers
{
    [RoutePrefix("api/importdata")]
 
    public class ImportDataController : ApiController
    {
        private readonly IImportDataService _importDataService;

        public ImportDataController(IImportDataService importDataService)
        {
            _importDataService = importDataService;
        }
        [HttpPost]
        [Authorize]
        [Route("Order")]
        public HttpResponseMessage CreateOrder([FromBody] ApiOrder apiOrderCreate)
        {
            string CompanyName = CompanyService.GetCompanyName(Request);

            if (apiOrderCreate == null || !apiOrderCreate.ApiOrderLines.Any())
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Values connat be null");

            //Call the service for creating Order
            var response = _importDataService.CreateOrder(apiOrderCreate, CompanyName);

            //Manage the response
            if (!response.Success)
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);

            //If the code comes here everything is OK.
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
        [Authorize]
        [HttpPut]
        [Route("UpdateOrderStatus")]
        public HttpResponseMessage UpdateOrderStatus(ApiUpdateOrderStatus apiUpdateOrderStatus)
        {
            string CompanyName = CompanyService.GetCompanyName(Request);

            if (apiUpdateOrderStatus == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Value connat be null");

            //Call the service for updating Customer
            var response = _importDataService.UpdateOrderStatus(apiUpdateOrderStatus, CompanyName);

            //Manage the response
            if (!response.Success)
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);

            //If the code comes here everything is OK.
            return Request.CreateResponse(HttpStatusCode.OK, response);

        }
        [Authorize]
        [HttpPut]
        [Route("UpdateOrder")]
        public HttpResponseMessage UpdateOrder([FromBody] ApiUpdateOrder apiUpdateOrder)
        {
            string CompanyName = CompanyService.GetCompanyName(Request);

            if (apiUpdateOrder == null || !apiUpdateOrder.ApiOrderLines.Any())
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Values connat be null");

            //Call the service for creating Order
            var response = _importDataService.UpdateOrder(apiUpdateOrder, CompanyName);

            //Manage the response
            if (!response.Success)
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);

            //If the code comes here everything is OK.
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }


        //[Authorize]
        //[HttpPut]
        //[Route("ConfirmOrder")]
        //public HttpResponseMessage ConfirmOrder(ApiConfirmOrder apiConfirmOrder)
        //{
        //    string CompanyName = CompanyService.GetCompanyName(Request);

        //    if (apiConfirmOrder == null)
        //        return Request.CreateResponse(HttpStatusCode.BadRequest, "Value connat be null");

        //    //Call the service for updating Customer
        //    var response = _importDataService.ConfirmStatus(apiUpdateOrderStatus, CompanyName);

        //    //Manage the response
        //    if (!response.Success)
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError, response);

        //    //If the code comes here everything is OK.
        //    return Request.CreateResponse(HttpStatusCode.OK, response);

        //}
    }
}
