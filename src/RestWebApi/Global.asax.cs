using System;
using System.Web.Http;
using SimpleInjector;
using RestWebApi.Infrastructure;
using RestWebApi.Services.Helpers;
using RestWebApi.Services;
using RestWebApi.DbFactory;

namespace RestWebApi
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            var container = new Container();
            container.Register<Services.IExportMasterDataService, Services.ExportMasterDataService>();
            container.Register<ISQLConnectionHelperService, SQLConnectionHelperService>();
            container.Register<IManageResponseHelperService, ManageResponseHelperService>();
            container.Register<IImportDataService, ImportDataService>();
            container.Register<ICustomDbFactory, CustomDbFactory>();

            container.RegisterWebApiControllers(
                              GlobalConfiguration.Configuration,
                               System.Reflection.Assembly.GetExecutingAssembly()
                                      );
            container.Verify();

            System.Web.Mvc.DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));

            GlobalConfiguration.Configuration.DependencyResolver = new SimpleInjectorDependencyResolver(container);
        }
    }
}
