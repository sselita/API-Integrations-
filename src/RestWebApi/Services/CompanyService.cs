using JWT.Security.Models;
using RestWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using RestWebApi.Services.Helpers;
using System.Web;
using System.Web.Http.Controllers;

namespace RestWebApi.Services
{
    public static class CompanyService 
    {
        /// <summary>
        /// Method for getting Company Name from Request
        /// </summary>
        /// <param name="httpActionContext"></param>
        /// <returns></returns>
        public static string GetCompanyName(HttpRequestMessage httpActionContext)
        {
         
            string CompanyName = "";
            var headers = httpActionContext.Headers;
            

            if (headers.Contains("CompanyName"))
            {
                CompanyName = headers.GetValues("CompanyName").First();

                CompanyName = CompanyName.Replace('.', '_');
            }

            

            return CompanyName;
        }

        /// <summary>
        /// Method for getting
        /// </summary>
        /// <returns></returns>
        public static CompanyResponse GetAllCompanies()
        {
            //Global variable for Configuration
            Configuration config = CommonHelperService.GetJsonFileData();
            CompanyResponse companyResponse = new CompanyResponse();
            try
            {
                // Creates instance of service and set credentials.  
                var systemService = new SystemService.SystemService();

                systemService.UseDefaultCredentials = false;
                systemService.Credentials = new NetworkCredential(config.ErpUser.Username, config.ErpUser.ErpPassword, config.ErpUser.Domain);


                // Loads all companies into an array.  
                var companies = systemService.Companies();

                companyResponse.Companies = companies.ToList();
                companyResponse.baseResponse.Message = BCExtension.GetCompaniesMethod + " was called successfully!";
                companyResponse.baseResponse.InnerMessage = "N/A";
                companyResponse.baseResponse.Success = true;
                companyResponse.baseResponse.MethodName = BCExtension.GetCompaniesMethod;


                return companyResponse;

            }
            catch (Exception e)
            {
                companyResponse.Companies = new List<string>();
                companyResponse.baseResponse.Message = BCExtension.GetCompaniesMethod + " an error happened!";
                companyResponse.baseResponse.InnerMessage = e.Message;
                companyResponse.baseResponse.Success = false;
                companyResponse.baseResponse.MethodName = BCExtension.GetCompaniesMethod;
                return companyResponse;
            }
        }

        /// <summary>
        /// Find if Company is same as the request and give suggestions
        /// </summary>
        /// <param name="CompanyName"></param>
        /// <returns></returns>
        public static (string ,bool) CheckCompanyName(string CompanyName)
        {
            string result = "";
            try
            {
                var allCompanies = GetAllCompanies();
                if (allCompanies.Companies.Any())
                {
                    result = allCompanies.Companies.Where(x => x.Replace('.', '_').Equals(CompanyName)).FirstOrDefault();


                    if (string.IsNullOrEmpty(result))
                    {
                        result = "Do you mean: ";

                        Dictionary<string, int> SearchResult = new Dictionary<string, int>();
                        var contresult = allCompanies.Companies.Where(y => y.ToLower().Contains(CompanyName.ToLower()) || CompanyName.ToLower().Contains(y.ToLower())).FirstOrDefault();
                        if (contresult != null)
                        {
                            SearchResult.Add(contresult, 0);
                        }
                        else
                        {
                            foreach (var x in allCompanies.Companies)
                            {
                                int res = Compute(CompanyName.ToLower(), x.ToLower());
                                SearchResult.Add(x, res);
                            }
                        }

                        var bestresult = SearchResult.OrderBy(x => x.Value).FirstOrDefault();
                        result += bestresult.Key.Replace('.', '_');
                        return (result, false);
                    }

                    return (result, true);
                }
                else
                {
                    return (allCompanies.baseResponse.MethodName +":" +allCompanies.baseResponse.InnerMessage, false);
                }
            }catch(Exception e)
            {
                return (e.Message, false);
            }


        }

        /// <summary>
        /// Find shortest distance betwen 2 words
        /// </summary>
        /// <param name="input"></param>
        /// <param name="dbval"></param>
        /// <returns></returns>
        public static int Compute(string input, string dbval)
        {
            int n = input.Length;
            int m = dbval.Length;

            int[,] d = new int[n + 1, m + 1];

            if (n == 0)
            {
                return m;
            }
            if (m == 0)
            {
                return n;
            }
            for (int i = 0; i <= n; d[i, 0] = i++)
                ;
            for (int j = 0; j <= m; d[0, j] = j++)
                ;
            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (dbval[j - 1] == input[i - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            return d[n, m];

        }
    }
}