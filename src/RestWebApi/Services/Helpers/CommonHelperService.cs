using System.IO;
using System.ServiceModel;
using System.Web.Hosting;
using JWT.Security.Models;
using Newtonsoft.Json;

namespace RestWebApi.Services.Helpers
{
    public static class CommonHelperService
    {
        /// <summary>
        /// Method that reads from JSON file.
        /// </summary>
        /// <returns></returns>
        public static Configuration GetJsonFileData()
        {          
          string Configure = File.ReadAllText(HostingEnvironment.MapPath("~/App_Data/Configuration.json"));
          Configuration config = JsonConvert.DeserializeObject<Configuration>(Configure);

          return config;
        }

        /// <summary>
        /// Method that creates bindings for connecting with ERP System
        /// </summary>
        /// <returns></returns>
        public static BasicHttpBinding CreateBindings()
        {
            BasicHttpBinding navWSBinding = new BasicHttpBinding();
            navWSBinding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
            navWSBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;

            return navWSBinding;
        }


    }
}
