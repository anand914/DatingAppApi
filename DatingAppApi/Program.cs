using DatingAppApi.DataContext;
using DatingAppApi.DataContext.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net.Mime;

namespace DatingAppApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
           var host = CreateWebHostBuilder(args).Build();
            using( var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<DatingAppContext>();
                    context.Database.Migrate();
                    Seed.SeedUsers(context);
                }
                catch (System.Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occured during migration");
                    
                }
            }
            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
