using JWT.Security.Models;
using RestWebApi.Infrastructure;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using RestWebApi.Models;
using System.Web.Mvc;

namespace RestWebApi.Services
{
    public class CheckCompany : ActionFilterAttribute
    {
        private readonly bool _ignore;


        public CheckCompany(bool ignore = false)
        {
            _ignore = ignore;
        }

        public override void OnActionExecuting(HttpActionContext filterContext)
        {
           if (filterContext == null )
                return;

            if (_ignore)
                return;

            if (filterContext.Request == null)
                return;



            string actionName = filterContext.ActionDescriptor.ActionName;
            if (String.IsNullOrEmpty(actionName))
                return;

            

            string controllerName = filterContext.ControllerContext.ToString();
            if (String.IsNullOrEmpty(controllerName))
                return;

            string CompanyName = "";
            var re = filterContext.Request;
            var headers = re.Headers;

            if (headers.Contains("CompanyName"))
            {
                CompanyName = headers.GetValues("CompanyName").First();
            }

            if (string.IsNullOrEmpty(CompanyName))
            {
                filterContext.Response = new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent("Company must have a name in header section")};
            }
            else
            {
                CompanyName = CompanyName.Replace('.', '_');

                var result = CompanyService.CheckCompanyName(CompanyName);

                BaseResponse baseResponse = new BaseResponse();
                if (!result.Item2)
                {
                    //quoteResponse.Quotes = new List<ApiQuote>();
                    baseResponse = new BaseResponse(BCExtension.GetQuoteMethod, "Company Name is Invalid!", result.Item1, false);
                    // Create the response.
                    var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent(baseResponse.InnerMessage)
                    };


                    filterContext.Response = response;
                }
            }
           

        }
    }
}