using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CoinRate.Startup))]
namespace CoinRate
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
