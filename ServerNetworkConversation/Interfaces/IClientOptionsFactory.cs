using Common.Enums;
using Common.HandleRequests;
using Microsoft.Extensions.Logging;
using ServerNetworkConversation.HandleData;
using ServerNetworkConversation.Options.HandleOptions;
using ServerNetworkConversation.Options.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ServerNetworkConversation.Interfaces
{
    public interface IClientOptionsFactory
    {
        public IClientOption AddClientOptions(ClientOptions choice, Data data, TcpClient client, RemoveClient removeClient, ILogger<Worker> logger, IRequests requests);
    }
}
