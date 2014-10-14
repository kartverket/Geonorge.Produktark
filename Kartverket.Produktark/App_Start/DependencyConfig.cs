using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Kartverket.Produktark.Models;

namespace Kartverket.Produktark
{
    public static class DependencyConfig
    {
        public static void Configure(ContainerBuilder builder)
        {
            builder.RegisterControllers(typeof(MvcApplication).Assembly).PropertiesAutowired();
            builder.RegisterModule(new AutofacWebTypesModule());

            builder.RegisterType<ProductSheetContext>().InstancePerRequest().AsSelf();
            builder.RegisterType<ProductSheetService>().As<IProductSheetService>();

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}