using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace TooBigToFailBurgerShop.Ordering.API
{
    public static class Program
    {
        public static async Task<int> Main(string[] args)
        {
            Activity.DefaultIdFormat = ActivityIdFormat.W3C;

            var configuration = GetConfiguration();
            Log.Logger = CreateSerilogLogger(configuration);

            try
            {
                Log.Information("Configuring web host ({ApplicationContext})...", "Ordering.API");

                var host = CreateHostBuilder(configuration, args).Build();

                Log.Information("Starting web host ({ApplicationContext})...", "Ordering.API");

                await host.RunAsync();

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Program terminated unexpectedly ({ApplicationContext})!", "Ordering.API");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IHostBuilder CreateHostBuilder(IConfiguration configuration, string[] args)
        {

            return Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureAppConfiguration(x => x.AddConfiguration(configuration))
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseStartup<Startup>()
                        .CaptureStartupErrors(true);
                });
        }

        static IConfiguration GetConfiguration()
        {        
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env ?? "Production" }.json", optional: true)
                .AddJsonFile($"appsettings.Logging.json", optional: true)

                .AddEnvironmentVariables();

            return builder.Build();
        }

        static ILogger CreateSerilogLogger(IConfiguration configuration)
        {
            return new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }

    }
}
