using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Common.HandleRequests;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServerNetworkConversation.HandleData;
using ServerNetworkConversation.HandleOptions;

namespace ServerNetworkConversation
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Data data = new Data();
            ClientOptionsFactory clientOptionsFactory = new ClientOptionsFactory();
            Requests requests = new Requests();

            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddr = ipHost.AddressList[0];

            TcpListener listener = new TcpListener(ipAddr, 7777);
            listener.Start();
            _logger.LogInformation($"server wait for first connection...");

            try
            {
                while (true)
                {
                    TcpClient tcpClient = listener.AcceptTcpClient();
                    data.ClientsConnectedInServer.AddWhenConnect(Guid.NewGuid(), tcpClient);
                    _logger.LogInformation($"new client connected");

                    var manageClientOptions = new ManageClientOptions(data, tcpClient, clientOptionsFactory, _logger, requests);
                    manageClientOptions.Run();
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }
    }
}
