﻿using ServerNetworkConversation.Options;
using ServerNetworkConversation.Options.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Common.Enums;
using System.Text;
using ServerNetworkConversation.HandleData;
using ServerNetworkConversation.Options.HandleOptions;
using ServerNetworkConversation.Options.GroupsChat;

namespace ServerNetworkConversation.HandleOptions
{
    public class ClientOptionsFactory
    {
        public IClientOption AddClientOptions(ClientOptions choice, Data data, TcpClient client, HandleClient handleClient, RemoveClient removeClient)
        {
            switch (choice)
            {
                case ClientOptions.GLOBAL_CHAT:
                    return new GlobalChat(data, client, handleClient, removeClient);
                    break;
                case ClientOptions.PRIVATE_CHAT:
                    return new PrivateChat(data, client, handleClient, removeClient);
                    break;
                case ClientOptions.CREATE_GROUP_CHAT:
                    return new CreateGroupChat(data, client, handleClient,removeClient);
                    break;
                case ClientOptions.GROUP_CHAT:
                    return new EnterGroupChat(data, client, handleClient, removeClient);
                    break;
                case ClientOptions.MANAGER_SETTINGS:
                    return new ManagerSettings(data, client, handleClient, removeClient);
                    break;
                default:
                    return null;
                    break;
            }
        }
    }
}
