using ServerNetworkConversation.Options;
using ServerNetworkConversation.Options.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Common.Enums;
using System.Text;

namespace ServerNetworkConversation.HandleOptions
{
    public class ClientOptionsFactory
    {
        public IClientOption AddClientOptions(ClientOptions choice, Data data, TcpClient client, HandleClient handleClient)
        {
            switch (choice)
            {
                case ClientOptions.GLOBAL_CHAT:
                    return new GlobalChat(data, client, handleClient);
                    break;
                case ClientOptions.PRIVATE_CHAT:
                    return new PrivateChat(data, client, handleClient);
                    break;
                default:
                    return null;
                    break;
            }
        }
    }
}
