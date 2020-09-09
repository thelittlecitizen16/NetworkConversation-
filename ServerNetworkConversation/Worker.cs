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
using ServerNetworkConversation.HandleData;
using ServerNetworkConversation.HandleOptions;
using ServerNetworkConversation.Options;
using ServerNetworkConversation.Options.HandleOptions;

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
            HandleClient handleClient = new HandleClient();
            ClientOptionsFactory clientOptionsFactory = new ClientOptionsFactory();
            RemoveClient removeClient = new RemoveClient(data);

            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddr = ipHost.AddressList[0];

            TcpListener listener = new TcpListener(ipAddr, 7777);
            listener.Start();
            Console.WriteLine("wait for first connection...");

            try
            {
                while (true)
                {
                    TcpClient tcpClient = listener.AcceptTcpClient();
                    data.ClientsConnectedInServer.AddWhenConnect(Guid.NewGuid(), tcpClient);
                    Console.WriteLine("new connection from client");

                    var manageClientOptions = new ManageClientOptions(data, tcpClient, handleClient, removeClient, clientOptionsFactory);
                    manageClientOptions.Run();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
