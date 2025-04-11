using System.Web.Http;
using JWT.Security.Security;
using Newtonsoft.Json.Serialization;

namespace RestWebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API routes
            config.MapHttpAttributeRoutes();

           
            // Configure the authentication filter to run on every request marked with the AuthorizeAttribute
            config.Filters.Add(new BearerAuthenticationFilter());

            // Configure the sliding expiration handler to run on every request
            config.MessageHandlers.Add(new SlidingExpirationHandler());

            // Help our JSON look professional using camelCase
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }
    }
}
