using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace APICatalogo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseIISIntegration(); //Para app Asp.Net Core usar o IIS como proxy reverso
                });
    }
}
