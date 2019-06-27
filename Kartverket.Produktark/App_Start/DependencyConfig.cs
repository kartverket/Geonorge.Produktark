using System.Collections.Generic;
using System.Web.Configuration;
using System.Web.Mvc;
using Autofac;
using Autofac.Core;
using Autofac.Integration.Mvc;
using GeoNorgeAPI;
using Kartverket.Geonorge.Utilities;
using Kartverket.Geonorge.Utilities.Organization;
using Kartverket.Produktark.Models;
using Autofac.Core.Activators.Reflection;
using Geonorge.AuthLib.NetFull;


namespace Kartverket.Produktark
{
    public static class DependencyConfig
    {
        public static IContainer Configure(ContainerBuilder builder)
        {
            builder.RegisterControllers(typeof(MvcApplication).Assembly).PropertiesAutowired();
            builder.RegisterModule(new AutofacWebTypesModule());
            builder.RegisterModule<GeonorgeAuthenticationModule>();

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

            builder.RegisterType<HttpClientFactory>().As<IHttpClientFactory>();
            builder.RegisterType<OrganizationService>().As<IOrganizationService>().WithParameters(new List<Parameter>
            {
                new NamedParameter("registryUrl", WebConfigurationManager.AppSettings["RegistryUrl"]),
                new AutowiringParameter()
            });

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            return container;
        }
    }
}