using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace ServerNetworkConversation
{
    public class Program
    {
        public static ConcurrentDictionary<Guid, TcpClient> clientsList = new ConcurrentDictionary<Guid, TcpClient>();
        public static void Main(string[] args)
        {
            var logsFilePath = ConfigurationManager.AppSettings["logsFile"];

            Log.Logger = new LoggerConfiguration()
         .MinimumLevel.Information()
         .WriteTo.File(logsFilePath,
             rollOnFileSizeLimit: true)
         .CreateLogger();

            try
            {
                Log.Information("starting up the service");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "There Where Froblam starting the service");
                return;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                }).UseSerilog();
    }
}
