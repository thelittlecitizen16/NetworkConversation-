using Common.Enums;
using Common.HandleRequests;
using Microsoft.Extensions.Logging;
using ServerNetworkConversation.HandleData;
using ServerNetworkConversation.Interfaces;
using ServerNetworkConversation.Options;
using ServerNetworkConversation.Options.GroupsChat;
using ServerNetworkConversation.Options.Interfaces;
using System.Net.Sockets;

namespace ServerNetworkConversation.HandleOptions
{
    public class ClientOptionsFactory: IClientOptionsFactory
    {
        public IClientOption AddClientOptions(ClientOptions choice, Data data, TcpClient client,  ILogger<Worker> logger, IRequests requests)
        {
            switch (choice)
            {
                case ClientOptions.GLOBAL_CHAT:
                    return new GlobalChat(data, client, logger, requests);
                    break;

                case ClientOptions.PRIVATE_CHAT:
                    return new PrivateChat(data, client, logger, requests);
                    break;

                case ClientOptions.CREATE_GROUP_CHAT:
                    return new CreateGroupChat(data, client, logger, requests);
                    break;

                case ClientOptions.GROUP_CHAT:
                    return new EnterGroupChat(data, client, logger, requests);
                    break;

                case ClientOptions.MANAGER_SETTINGS:
                    return new ManagerSettings(data, client, logger, requests);
                    break;

                case ClientOptions.LEAVE_GROUP_CHAT:
                    return new LeaveGroupChat(data, client, logger, requests);
                    break;

                default:
                    return null;
                    break;
            }
        }
    }
}
