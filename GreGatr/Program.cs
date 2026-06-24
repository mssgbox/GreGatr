using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace YourNamespace
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            // Subscribe to unobserved task exceptions
            TaskScheduler.UnobservedTaskException += (sender, e) =>
            {
                var logger = host.Services
                    .GetRequiredService<ILogger<Program>>();

                logger.LogError(
                    e.Exception,
                    "An unobserved task exception occurred.");

                e.SetObserved();
            };

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}