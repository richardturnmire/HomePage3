using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace HomePage3
{
    public class Program
    {
        public static string EmailAccount { get; set;}
        public static string EmailPsWord { get; set;}
      
        public static void Main(string[] args)
        {
            //Log.Logger = new LoggerConfiguration()
            //    .MinimumLevel.Debug()
            //    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            //    .Enrich.FromLogContext()
            //    .WriteTo.Console()
            //    .CreateLogger();


            BuildWebHost(args).Run();
        }
 
        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
        
        
    }
}

