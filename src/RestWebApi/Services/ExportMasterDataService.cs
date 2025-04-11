using RestWebApi.Models.Items;
using System;
using System.Data.SqlClient;
using JWT.Security.Models;
using System.Collections.Generic;

using System.Linq;
using RestWebApi.Models;
using RestWebApi.Services.Helpers;
using RestWebApi.DbFactory;
using RestWebApi.Models.Orders;
using RestWebApi.Api_SalesOrder_WebReference;
using System.Net;
using RestWebApi.Services.EndpointBehaviour;

namespace RestWebApi.Services
{
    public class ExportMasterDataService : IExportMasterDataService
    {
        #region Fields
        private static SqlConnection sqlConnection;
        private readonly ISQLConnectionHelperService _SQLConnectionHelperService;
        private readonly IManageResponseHelperService _manageResponseHelperService;
        private readonly ICustomDbFactory _customDbFactory;
        Configuration config = CommonHelperService.GetJsonFileData();
        //private readonly ICompanyService _companyService;
        #endregion

        public ExportMasterDataService(
            ISQLConnectionHelperService SQLConnectionHelperService,
            IManageResponseHelperService manageResponseHelperService,
            ICustomDbFactory customDbFactory)
        //ICompanyService companyService)
        {
            _SQLConnectionHelperService = SQLConnectionHelperService;
            _manageResponseHelperService = manageResponseHelperService;
            _customDbFactory = customDbFactory;
            //_companyService = companyService;
        }

        /// <summary>
        /// Initializes connection with SQL.
        /// </summary>
        private void InitializeConnection()
        {
            sqlConnection = new SqlConnection(_SQLConnectionHelperService.GenerateConnectionString());
        }

        /// <summary>
        /// Method used for returning items from BC.
        /// </summary>
        /// <returns></returns>
        public ApiGetItemsResponse GetItems(ApiRequestFilter request, string companyName)
        {
            //Open SQL Connection
            InitializeConnection();
            BaseResponse baseResponse = new BaseResponse();
            ErpResponse erpResponse;
            SqlRequestParameter sqlRequest = new SqlRequestParameter(BCExtension.ItemTable, companyName, BCExtension.GetMasterDataProcedure, request.Code, request.StartDate, request.EndDate);

            ApiGetItemsResponse items = new ApiGetItemsResponse();
            try
            {
                using (sqlConnection)
                {

                    //Call BC procedure and return data in Reader.
                    SqlDataReader reader = _SQLConnectionHelperService.ExecuteApiReader(sqlConnection, sqlRequest);

                    //Get all list of items from procedure
                    items.ItemList = _customDbFactory.PrepareMasterDataGroupObject<ApiGetItem>(reader).ToList();

                }

                //Manage response for successfull request   
                baseResponse = new BaseResponse(BCExtension.GetItemsMethod, "Method was successfully called.", "N/A", true);

                //Manage logs for ERP
                erpResponse = new ErpResponse(BCExtension.ItemTable, 1, $"{items.ItemList.Count} has been exposed!", "N/A", "N/A", "N/A", BCExtension.DefaultERPDateTime, 0, "N/A", BCExtension.CreateOrderMethod);//May implement sending all item codes...
                _manageResponseHelperService.WriteBCLogs(erpResponse, companyName);
            }
            catch (SqlException sqlex)
            {
                //Manage response for SQL EXCEPTION    
                baseResponse = new BaseResponse(BCExtension.GetItemsMethod, sqlex.Message.Cut(false), "N/A", false);
                //Helper method for Writing LOGS in a TEXT File in case of SQL Exceptions.
                _manageResponseHelperService.WriteLog(sqlex.Message.Cut(false), BCExtension.GetItemsMethod, sqlex.InnerException == null ? "" : sqlex.InnerException.Message.Cut(true));
            }
            catch (Exception e)
            {
                string InnerMessages = e.InnerException == null ? "N/A" : e.InnerException.Message.Cut(true);
                baseResponse = new BaseResponse(BCExtension.GetItemsMethod, e.Message.Cut(false), InnerMessages, false);

                //Manage logs for ERP
                erpResponse = new ErpResponse(BCExtension.ItemTable, 0, $"{items.ItemList.Count} has been exposed!", "N/A", "N/A", "N/A", BCExtension.DefaultERPDateTime, 0, "N/A", BCExtension.CreateOrderMethod);//May implement sending all item codes...
                _manageResponseHelperService.WriteBCLogs(erpResponse, companyName);
            }
            finally
            {
                //We always close SQL connection when job is Done.
                sqlConnection.Close();
            }

            items.BaseResponse = baseResponse;
            return items;
        }

        /// <summary>
        /// Method for Getting inserted quotes based on specific filter.
        /// </summary>
        /// <param name="apiGetQuotes"></param>
        /// <returns></returns>
        public SalesOrderResponse GetOrders(string apiOrderNo, string CompanyName)
        {
            SalesOrderApi Orders = new SalesOrderApi();
            BaseResponse baseResponse = new BaseResponse();
            //ErpResponse erpResponse;
            //SalesOrderResponse salesOrderResponse = new SalesOrderResponse();
            SalesOrderResponse salesOrderResponse = new SalesOrderResponse();


            try
            {


                using (var client = new SalesOrderApi_Service())
                {
                    SimpleEndpointBehavior simpleEndpointBehavior = new SimpleEndpointBehavior();


                    string url = client.Url.Replace(config.OldServiceName, config.NewServiceName).Replace(config.CompanyName.Replace('_', '.'), CompanyName);
                    client.UseDefaultCredentials = false;
                    client.Url = url;

                    client.Credentials = new NetworkCredential(config.ErpUser.Username, config.ErpUser.ErpPassword, config.ErpUser.Domain);

                    var OrderHeader = client.Read(apiOrderNo);


                    ApiOrderResponse apiOrderResponse = new ApiOrderResponse();


                    foreach(var sl in OrderHeader.SalesLines)
                    {
                        ApiOrderLine apiOrderLine = new ApiOrderLine
                        {
                           
                            ItemNo = sl.No,
                            Location = sl.Location_Code,
                            Quantity = sl.Quantity
                        };

                        apiOrderResponse.ApiOrderLines.Add(apiOrderLine);

                    }


                    apiOrderResponse.CustomerNo = OrderHeader.Sell_to_Customer_No;
                    apiOrderResponse.OrderDateTime = OrderHeader.Posting_Date;
                    apiOrderResponse.OrderNo = OrderHeader.No;
                    apiOrderResponse.Status = OrderHeader.Status.ToString();
                    

                    salesOrderResponse = new SalesOrderResponse
                    {
                        InnerMessage = "N / A",
                        Message = $"Order with no {OrderHeader.No} was successfully taken.",
                        MethodName = "GetOrder",
                        SalesOrder = apiOrderResponse,
                        Success = true

                    };


                }
            }
            catch (Exception e)
            {
                salesOrderResponse = new SalesOrderResponse
                {
                    InnerMessage = e.Message,
                    Message = $"An error happened when finding order with no {apiOrderNo} ",
                    MethodName = "GetOrder",
                    SalesOrder = null,
                    Success = true

                };
           
            }
            return salesOrderResponse;
        }
    }
}


