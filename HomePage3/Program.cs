using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace HomePage3
{
    public class Program
    {
        public static string EmailAccount { get; set; }
        public static string EmailPsWord { get; set; }

        public static void Main (string[] args)
        {
            //Log.Logger = new LoggerConfiguration()
            //    .MinimumLevel.Debug()
            //    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            //    .Enrich.FromLogContext()
            //    .WriteTo.Console()
            //    .CreateLogger();


            CreateWebHostBuilder(args).Build().Run();
        }

        private static IWebHostBuilder CreateWebHostBuilder (string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                          .UseStartup<Startup>();
        }
    }
}

