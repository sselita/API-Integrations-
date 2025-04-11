
using System.Net;
using System.ServiceModel;
using RestWebApi.Api_CustomerService;

using RestWebApi.Models.Orders;

using System;
using System.Linq;
using RestWebApi.Services.EndpointBehaviour;

using System.Threading.Tasks;
using RestWebApi.Models;
using RestWebApi.Services.Helpers;
using JWT.Security.Models;
using System.Data.SqlClient;
using RestWebApi.Models.Exceptions;
using RestWebApi.Api_ControlQuantity_WebReference;

using RestWebApi.Api_SalesOrder_WebReference;

namespace RestWebApi.Services
{

    public class ImportDataService : IImportDataService
    {

        private static readonly string _createOrderMethod = "CreateOrder";
        //Global variable for Configuration
        Configuration config = CommonHelperService.GetJsonFileData();
        private readonly IManageResponseHelperService _manageErrorsHelperService;
        private readonly ISQLConnectionHelperService _SQLConnectionHelperService;

        public object Api_SalesOrderService { get; private set; }

        public ImportDataService(IManageResponseHelperService manageErrorsHelperService, ISQLConnectionHelperService SQLConnectionHelperService)
        {
            _manageErrorsHelperService = manageErrorsHelperService;
            _SQLConnectionHelperService = SQLConnectionHelperService;
        }


        public SalesOrderResponse CreateOrder(ApiOrder apiOrder, string CompanyName)
        {
            BaseResponse baseResponse = new BaseResponse();
            ErpResponse erpResponse;
            SalesOrderResponse salesOrderResponse = new SalesOrderResponse();
            bool headerInserted = false;
            bool lineInserted = false;


            try
            {

                using (var client = new SalesOrderApi_Service())
                {
                    SimpleEndpointBehavior simpleEndpointBehavior = new SimpleEndpointBehavior();


                    string url = client.Url.Replace(config.OldServiceName, config.NewServiceName).Replace(config.CompanyName.Replace('_', '.'), CompanyName);
                    client.UseDefaultCredentials = false;
                    client.Url = url;

                    client.Credentials = new NetworkCredential(config.ErpUser.Username, config.ErpUser.ErpPassword, config.ErpUser.Domain);

                    SalesOrderApi OrderHeader = new SalesOrderApi();

                    try
                    {
                        client.Create(ref OrderHeader);

                        var locCode = string.Empty;
                        if (!string.IsNullOrWhiteSpace(CommonHelperService.GetJsonFileData().DefaultInventoryCode))
                            locCode = CommonHelperService.GetJsonFileData().DefaultInventoryCode;

                        //Map all Order Heade Fields
                        OrderHeader.Sell_to_Customer_No = apiOrder.CustomerNo;
                        //OrderHeader.Doc_No_Online = apiOrder.DocNoOnline;
                        OrderHeader.Order_Date = apiOrder.OrderDateTime;
                        //OrderHeader.Salesperson_Code = apiOrder.SalesPersonCode;
                        OrderHeader.Posting_Date = apiOrder.OrderDateTime;


                        client.Update(ref OrderHeader);
                        headerInserted = client.IsUpdated(OrderHeader.Key);
                        headerInserted = true;

                    }
                    catch (Exception e)
                    {
                        //Response for Api
                        baseResponse = new BaseResponse(BCExtension.CreateOrderMethod, e.Message.Cut(false), e.InnerException == null ? $"Header was not created" : e.InnerException.Message.ToString().Cut(true), false);

                        //Response for ERP
                        erpResponse = new ErpResponse(BCExtension.SalesHeaderTable, 0, e.Message.Cut(false), "Header was not created because an error happened.",
                                                                                apiOrder.CustomerNo, "N/A", BCExtension.DefaultERPDateTime, 0, "N/A", BCExtension.CreateOrderMethod);

                        _manageErrorsHelperService.WriteBCLogs(erpResponse, CompanyName);
                    }

                    if (headerInserted)

                    {


                        int i = 0;
                        OrderHeader.SalesLines = apiOrder.ApiOrderLines.Select(x => new Sales_Order_Line()).ToArray();

                        foreach (var ol in apiOrder.ApiOrderLines)
                        {
                            decimal navquantity = decimal.Zero;

                            using (var quantity = new ControlQuantity())

                            {
                                quantity.Credentials = new NetworkCredential(config.ErpUser.Username, config.ErpUser.ErpPassword, config.ErpUser.Domain);
                                navquantity = quantity.CheckQuantityForItem(ol.ItemNo, ol.Location);
                            }

                            if (ol.Quantity > navquantity)
                            {
                                ol.Quantity = navquantity;
                            }


                            try
                            {
                                //OrderHeader.SalesLines[i].Doc_No_Online = ol.DocNoOnline;
                                OrderHeader.SalesLines[i].Type = Api_SalesOrder_WebReference.Type.Item;
                                OrderHeader.SalesLines[i].TypeSpecified = true;
                                //OrderHeader.SalesLines[i].No = ol.ItemId;
                                OrderHeader.SalesLines[i].No = ol.ItemNo;
                                OrderHeader.SalesLines[i].Unit_PriceSpecified = true;
                                OrderHeader.SalesLines[i].Quantity = ol.Quantity;
                                OrderHeader.SalesLines[i].Location_Code = "1";
                                //OrderHeader.SalesLines[i].Unit_Price = ol.UnitPriceIncludingVat;
                                OrderHeader.SalesLines[i].SalesLineDiscExistsSpecified = false;
                                OrderHeader.SalesLines[i].TotalSalesLine_Line_AmountSpecified = false;
                                OrderHeader.SalesLines[i].Total_Amount_Excl_VATSpecified = false;
                                OrderHeader.SalesLines[i].Total_Amount_Incl_VATSpecified = false;
                                OrderHeader.SalesLines[i].SalesPriceExistSpecified = false;
                                OrderHeader.SalesLines[i].Total_Amount_Excl_VATSpecified = false;
                                OrderHeader.SalesLines[i].Total_Amount_Incl_VATSpecified = false;
                                OrderHeader.SalesLines[i].Total_VAT_AmountSpecified = false;
                                //OrderHeader.SalesLines[i].OrderStatus = false;


                                client.Update(ref OrderHeader);
                                //Manage response for ERP
                                erpResponse = new ErpResponse(BCExtension.SalesLineTable, 1, $"Sales Line with Document No {OrderHeader.No} was created successfully!", "N/A",
                                                                    OrderHeader.Sell_to_Customer_No, OrderHeader.No, OrderHeader.Order_Date.ToString(), OrderHeader.SalesLines[i].Line_No,
                                                                                                                                OrderHeader.SalesLines[i].No, BCExtension.CreateOrderMethod);
                                _manageErrorsHelperService.WriteBCLogs(erpResponse, CompanyName);

                                i++;

                                lineInserted = true;
                            }
                            catch (Exception e)
                            {

                                // Response for ERP
                                erpResponse = new ErpResponse(BCExtension.SalesHeaderTable, 0, e.Message.Cut(false), "Lines were not created because an error happened and order was deleted.",
                                                                                apiOrder.CustomerNo, OrderHeader.No, BCExtension.DefaultERPDateTime, 0, "N/A", BCExtension.CreateOrderMethod);
                                baseResponse = new BaseResponse(BCExtension.CreateOrderMethod, e.Message.Cut(false), e.InnerException == null ? $"Line  was not created" : e.InnerException.Message.ToString().Cut(true), false);
                                _manageErrorsHelperService.WriteBCLogs(erpResponse, CompanyName);

                                lineInserted = false;
                            }
                        }

                    }

                    if (headerInserted && lineInserted)
                    {
                        //Manage response for api call
                        baseResponse = new BaseResponse(BCExtension.CreateOrderMethod, $"Order with NO: {OrderHeader.No} was created successfully!",
                                                                        "N/A", true);
                        //Manage response for ERP
                        erpResponse = new ErpResponse(BCExtension.SalesHeaderTable + "|" + BCExtension.SalesLineTable, 1, $"Order with no {OrderHeader.No} was created successfully!", "N/A",
                                                                                                         OrderHeader.Sell_to_Customer_No, OrderHeader.No, OrderHeader.Order_Date.ToString(), 0, "N/A", BCExtension.CreateOrderMethod);

                        _manageErrorsHelperService.WriteBCLogs(erpResponse, CompanyName);
                        
                        ApiOrderResponse apiOrderResponse = new ApiOrderResponse();

                        apiOrderResponse.ApiOrderLines = apiOrder.ApiOrderLines;
                        apiOrderResponse.CustomerNo = apiOrder.CustomerNo;
                        apiOrderResponse.OrderDateTime = apiOrder.OrderDateTime;
                        apiOrderResponse.OrderNo = OrderHeader.No;

                        salesOrderResponse = new SalesOrderResponse
                        {
                            InnerMessage = "N / A",
                            Message = "Porosia u krye me sukses",
                            MethodName = "CreateOrder",
                            SalesOrder = apiOrderResponse,
                            Success = true

                        };
                    }
                    else
                    {
                        client.Delete(OrderHeader.Key);
                        //Response for ERP
                        erpResponse = new ErpResponse(BCExtension.SalesHeaderTable, 0, "Lines were not created because an error happened and order was deleted.",
                                                            "Lines were not created because an error happened and order was deleted.", apiOrder.CustomerNo, "N/A", "N/A", 0, "N/A", BCExtension.CreateOrderMethod);

                        baseResponse = new BaseResponse(BCExtension.CreateOrderMethod, "Lines were not created because an error happened and order was deleted.", "N/A", false);
                    }

                }
            }

            catch (Exception e)
            {

                baseResponse = new BaseResponse(BCExtension.CreateOrderMethod, e.Message.Cut(false), e.InnerException == null ? "N/A" : e.InnerException.Message.ToString().Cut(true), false);
                //Response for ERP
                erpResponse = new ErpResponse(BCExtension.SalesHeaderTable, 0, e.Message.Cut(false), "Header was not created because an error happened.", apiOrder.CustomerNo, "N/A", BCExtension.DefaultERPDateTime, 0, "N/A", BCExtension.CreateOrderMethod);

                _manageErrorsHelperService.WriteBCLogs(erpResponse, CompanyName);
            }
            return salesOrderResponse;
        }
        public BaseResponse UpdateOrderStatus(ApiUpdateOrderStatus apiUpdateOrderStatus, string CompanyName)
        {
            BaseResponse baseResponse;

            try
            {
                using (var client = new SalesOrderApi_Service())
                {
                    SimpleEndpointBehavior simpleEndpointBehavior = new SimpleEndpointBehavior();


                    string url = client.Url.Replace(config.OldServiceName, config.NewServiceName).Replace(config.CompanyName.Replace('_', '.'), CompanyName);
                    client.UseDefaultCredentials = false;
                    client.Url = url;

                    client.Credentials = new NetworkCredential(config.ErpUser.Username, config.ErpUser.ErpPassword, config.ErpUser.Domain);

                    SalesOrderApi OrderLine = new SalesOrderApi();

                    var OrderErp = client.Read(apiUpdateOrderStatus.OrderNo);
                    if (OrderErp != null)
                    {
                        OrderErp.StatusSpecified = false;
                        OrderErp.StatusApiSpecified = true;
                        OrderErp.StatusApi = (Api_SalesOrder_WebReference.StatusApi)apiUpdateOrderStatus.OrderStatus;

                        client.Update(ref OrderErp);
                    }


                    baseResponse = new BaseResponse(BCExtension.CreateOrderMethod, $"Order with NO: {OrderErp.No} was updated successfully!", "N/A", true);

                    //  _manageErrorsHelperService.WriteBCLogs(baseResponse);
                }
            }
            catch (FaultException e)
            {
                baseResponse = new BaseResponse(BCExtension.CreateOrderMethod, e.Message.Cut(false), e.InnerException == null ? "N/A" : e.InnerException.Message.ToString().Cut(true), false);
                // _manageErrorsHelperService.WriteBCLogs(baseResponse);
            }

            return baseResponse;
        }

        public SalesOrderResponse UpdateOrder(ApiUpdateOrder apiOrder, string CompanyName)
        {
            BaseResponse baseResponse = new BaseResponse();
            //ErpResponse erpResponse;
            //SalesOrderResponse salesOrderResponse = new SalesOrderResponse();
            SalesOrderResponse salesOrderResponse = new SalesOrderResponse();
            var OldOrderHeader = new Api_SalesOrder_WebReference.SalesOrderApi();

            var NewOrderHeader = new Api_SalesOrder_WebReference.SalesOrderApi();

            bool headerInserted = false;
            bool lineInserted = false;
            int counter = 0;
            int innerCnt = 0;
            try
            {

                using (var client = new SalesOrderApi_Service())
                {
                    //SimpleEndpointBehavior simpleEndpointBehavior = new SimpleEndpointBehavior();


                    //string url = client.Url.Replace(config.OldServiceName, config.NewServiceName).Replace(config.CompanyName.Replace('_', '.'), CompanyName);
                    //client.UseDefaultCredentials = false;
                    //client.Url = url;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(config.ErpUser.Username, config.ErpUser.ErpPassword, config.ErpUser.Domain);


                    

                    try
                    {
                        OldOrderHeader = client.Read(apiOrder.OrderNo);
                        //counter = OldOrderHeader.SalesLines.Count();
                        headerInserted = true;

                    }
                    catch (Exception e)
                    {
                        baseResponse = new BaseResponse("UpdateOrder", e.Message.Cut(false), e.InnerException == null ? $"Header was not Found" : e.InnerException.Message.ToString().Cut(true), false);
                    }

                    if (headerInserted)

                    {
                        foreach (var sl in OldOrderHeader.SalesLines)
                        {
                            client.Delete_SalesLines(sl.Key);
                        }

                        int i = 0;
                        OldOrderHeader.SalesLines = apiOrder.ApiOrderLines.Select(x => new Sales_Order_Line()).ToArray();

                        foreach (var ol in apiOrder.ApiOrderLines)
                        {
                            decimal navquantity = decimal.Zero;

                            using (var quantity = new ControlQuantity())

                            {
                                quantity.Credentials = new NetworkCredential(config.ErpUser.Username, config.ErpUser.ErpPassword, config.ErpUser.Domain);
                                navquantity = quantity.CheckQuantityForItem("", "");
                            }

                            if (ol.Quantity > navquantity)
                            {
                                ol.Quantity = navquantity;
                            }


                            try
                            {   

                                    //OrderHeader.SalesLines[i].Doc_No_Online = ol.DocNoOnline;
                                    OldOrderHeader.SalesLines[counter].Type = Api_SalesOrder_WebReference.Type.Item;
                                    OldOrderHeader.SalesLines[counter].TypeSpecified = true;
                                    //OrderHeader.SalesLines[i].No = ol.ItemId;
                                    OldOrderHeader.SalesLines[counter].No = ol.ItemNo;
                                    OldOrderHeader.SalesLines[counter].Unit_PriceSpecified = true;
                                    OldOrderHeader.SalesLines[counter].Quantity = ol.Quantity;
                                    OldOrderHeader.SalesLines[counter].Location_Code = "1";
                                    //OrderHeader.SalesLines[i].Unit_Price = ol.UnitPriceIncludingVat;
                                    OldOrderHeader.SalesLines[counter].SalesLineDiscExistsSpecified = false;
                                    OldOrderHeader.SalesLines[counter].TotalSalesLine_Line_AmountSpecified = false;
                                    OldOrderHeader.SalesLines[counter].Total_Amount_Excl_VATSpecified = false;
                                    OldOrderHeader.SalesLines[counter].Total_Amount_Incl_VATSpecified = false;
                                    OldOrderHeader.SalesLines[counter].SalesPriceExistSpecified = false;
                                    OldOrderHeader.SalesLines[counter].Total_Amount_Incl_VATSpecified = false;
                                    OldOrderHeader.SalesLines[counter].Total_VAT_AmountSpecified = false;
                                    //OrderHeader.SalesLines[i].OrderStatus = false;
                                    lineInserted = true;

                                    counter++;
                          
                                client.Update(ref OldOrderHeader);

                            }
                            catch (Exception e)
                            {

                                // Response for ERP
                                baseResponse = new BaseResponse("UpdateOrder", e.Message.Cut(false), e.InnerException == null ? $"Line  was not created" : e.InnerException.Message.ToString().Cut(true), false);
                                lineInserted = false;
                            }
                         
                        }

                        if (headerInserted && lineInserted)
                        {
                            baseResponse = new BaseResponse("UpdateOrder", $"Order with NO: {OldOrderHeader.No} was updated successfully!",
                                                                                 "N/A", true);
                          
                            ApiOrderResponse apiOrderResponse = new ApiOrderResponse();

                            apiOrderResponse.ApiOrderLines = apiOrder.ApiOrderLines;
                            apiOrderResponse.CustomerNo = apiOrder.CustomerNo;
                            apiOrderResponse.OrderDateTime = OldOrderHeader.Posting_Date;
                            apiOrderResponse.OrderNo = OldOrderHeader.No;

                            salesOrderResponse = new SalesOrderResponse
                            {
                                InnerMessage = "N / A",
                                Message = "Porosia u krye me sukses",
                                MethodName = "CreateOrder",
                                SalesOrder = apiOrderResponse,
                                Success = true

                            };
                        }
                        else
                        {
                            baseResponse = new BaseResponse("UpdateOrder", "Lines were not created because an error happened!.", "N/A", false);
                        }
                    }
                }
            }

            catch (Exception e)
            {
                baseResponse = new BaseResponse("UpdateOrder", e.Message.Cut(false), e.InnerException == null ? "N/A" : e.InnerException.Message.ToString().Cut(true), false);
            }
            return salesOrderResponse;


           

        }
    }
}
