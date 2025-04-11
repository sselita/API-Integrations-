using System.IO;
using System.Text;
using System.Web.Hosting;
using JWT.Security.Models;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace JWT.Security.Security
{
    public static class SecurityConfiguration
    {
        // Stand-in for our users table
        private static string ConfigJson = File.ReadAllText(HostingEnvironment.MapPath("~/App_Data/Configuration.json"));
        private static Configuration conf = JsonConvert.DeserializeObject<Configuration>(ConfigJson);

        public static string SigningKey = conf.TokenModel.SigningKey;

        public static string TokenIssuer = conf.TokenModel.TokenIssuer;
        public static string TokenAudience = conf.TokenModel.TokenAudience;
        public static int Lifetime = int.Parse(conf.TokenModel.TokenLifetimeInMinutes);


        public static SecurityKey SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SigningKey));
        public static SigningCredentials SigningCredentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
        
    }
}
