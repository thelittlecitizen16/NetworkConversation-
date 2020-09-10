using ServerNetworkConversation.Options;
using ServerNetworkConversation.Options.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Common.Enums;
using System.Text;
using ServerNetworkConversation.HandleData;
using ServerNetworkConversation.Options.HandleOptions;
using ServerNetworkConversation.Options.GroupsChat;
using Microsoft.Extensions.Logging;
using ServerNetworkConversation.Interfaces;
using Common.HandleRequests;

namespace ServerNetworkConversation.HandleOptions
{
    public class ClientOptionsFactory: IClientOptionsFactory
    {
        public IClientOption AddClientOptions(ClientOptions choice, Data data, TcpClient client, RemoveClient removeClient,  ILogger<Worker> logger, IRequests requests)
        {
            switch (choice)
            {
                case ClientOptions.GLOBAL_CHAT:
                    return new GlobalChat(data, client, removeClient, logger, requests);
                    break;

                case ClientOptions.PRIVATE_CHAT:
                    return new PrivateChat(data, client, removeClient, logger, requests);
                    break;

                case ClientOptions.CREATE_GROUP_CHAT:
                    return new CreateGroupChat(data, client,removeClient, logger, requests);
                    break;

                case ClientOptions.GROUP_CHAT:
                    return new EnterGroupChat(data, client, removeClient, logger, requests);
                    break;

                case ClientOptions.MANAGER_SETTINGS:
                    return new ManagerSettings(data, client, removeClient, logger, requests);
                    break;

                case ClientOptions.LEAVE_GROUP_CHAT:
                    return new LeaveGroupChat(data, client, removeClient, logger, requests);
                    break;

                default:
                    return null;
                    break;
            }
        }
    }
}
