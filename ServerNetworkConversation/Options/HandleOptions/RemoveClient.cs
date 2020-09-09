using ServerNetworkConversation.HandleData;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ServerNetworkConversation.Options.HandleOptions
{
    public class RemoveClient
    {
        private Data _data;
        public RemoveClient(Data data)
        {
            _data = data;
        }

        public void RemoveClientWhenOut(TcpClient client, Guid clientGuid)
        {
            client.Close();
            _data.ClientsConnectedInServer.Remove(clientGuid);
        }
    }
}
