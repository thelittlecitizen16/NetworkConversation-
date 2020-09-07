using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServerNetworkConversation.Options;

namespace ServerNetworkConversation
{
    public class Worker : BackgroundService
    {
        private ConcurrentDictionary<Guid, TcpClient> clientsList = new ConcurrentDictionary<Guid, TcpClient>();
        private readonly ILogger<Worker> _logger;
        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
           

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Data data = new Data();
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddr = ipHost.AddressList[0];

            TcpListener listener = new TcpListener(ipAddr, 7777);
            listener.Start();
            Console.WriteLine("wait for first connection");

            try
            {
                while (true)
                {
                    TcpClient tcpClient = listener.AcceptTcpClient();
                    clientsList.TryAdd(Guid.NewGuid(), tcpClient);
                    Console.WriteLine("new connection from client");

                    var manageClientOptions = new ManageClientOptions(data, tcpClient, clientsList);
                  // manageClientOptions.AddClientOptions(1, tcpClient, clientsList).Run();
                     manageClientOptions.GetClientChoice();
                  //   GlobalChat client = new GlobalChat(tcpClient, clientsList);
                  // client.Run();
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            //while (!stoppingToken.IsCancellationRequested)
            //{
            //    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            //    await Task.Delay(1000, stoppingToken);
            //}
        }
    }
}
