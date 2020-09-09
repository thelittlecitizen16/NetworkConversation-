using ServerNetworkConversation.HandleData;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace ServerNetworkConversation.HandleData
{
    public class Data
    {
        public AllGroupsChat AllGroupsChat {  get; private set; }
        public ClientsConnectedInChat ClientsConnectedInChat { get; private set; }
        public ClientsInGlobalChat ClientsInGlobalChat { get; private set; }
        public ClientsConnectedInServer ClientsConnectedInServer { get; private set; }

        public Data()
        {
            AllGroupsChat = new AllGroupsChat();
            ClientsConnectedInChat = new ClientsConnectedInChat();
            ClientsInGlobalChat = new ClientsInGlobalChat();
            ClientsConnectedInServer = new ClientsConnectedInServer();
        }
    }
}
