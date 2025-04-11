using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Description;

namespace RestWebApi.Models
{
    public class AddRequiredHeaderParameter : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            if (operation.parameters == null)
                operation.parameters = new List<Parameter>();

            ////default token param
            //operation.parameters.Add()
            //var Header = new Header(); header
            //company name param
            operation.parameters.Add(new Parameter
            {
                name = "CompanyName",
                @in = "header",
                type = "string",
                description = "Company Name",
                required = true
            });
        }
    }
}