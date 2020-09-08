using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ServerNetworkConversation
{
    public class Data
    {
        public ConcurrentDictionary<Guid, TcpClient> ClientsInGlobalChat { get; private set; }
        public Data()
        {
            ClientsInGlobalChat = new ConcurrentDictionary<Guid, TcpClient>();
        }

        public void AddClientToGlobalChat(Guid guid, TcpClient tcpClient)
        {
            ClientsInGlobalChat.TryAdd(guid, tcpClient);
        }
        public void RemoveClientFromGlobalChat(Guid guid)
        {
            TcpClient tcpClient;
            ClientsInGlobalChat.TryRemove(guid, out tcpClient);
        }
    }
}
