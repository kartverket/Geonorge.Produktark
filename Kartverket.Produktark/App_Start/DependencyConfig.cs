using System.Collections.Generic;
using System.Web.Configuration;
using System.Web.Mvc;
using Autofac;
using Autofac.Core;
using Autofac.Integration.Mvc;
using GeoNorgeAPI;
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

            builder.RegisterType<GeoNorge>()
                .AsSelf()
                .WithParameters(new List<Parameter>
                {
                    new NamedParameter("geonetworkUsername", ""),
                    new NamedParameter("geonetworkPassword", ""),
                    new NamedParameter("geonetworkEndpoint", WebConfigurationManager.AppSettings["GeoNetworkUrl"])
                });

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}