using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Kartverket.Produktark.Startup))]
namespace Kartverket.Produktark
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            
        }
    }
}
