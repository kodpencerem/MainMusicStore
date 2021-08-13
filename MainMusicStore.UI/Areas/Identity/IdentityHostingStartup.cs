using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(MainMusicStore.UI.Areas.Identity.IdentityHostingStartup))]
namespace MainMusicStore.UI.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}