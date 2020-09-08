using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace ServerNetworkConversation
{
    public class Data
    {
        public ConcurrentDictionary<Guid, TcpClient> ClientsInGlobalChat { get; private set; }
        public ConcurrentDictionary<Guid, TcpClient> ClientsConnected { get; private set; }
        public Data()
        {
            ClientsInGlobalChat = new ConcurrentDictionary<Guid, TcpClient>();
            ClientsConnected = new ConcurrentDictionary<Guid, TcpClient>();
        }

        public Guid GetClientGuid(TcpClient cleint)
        {
            return ClientsConnected.Keys.Where(k => ClientsConnected[k] == cleint).First();
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
        public void AddClientWhenConnect(Guid guid, TcpClient tcpClient)
        {
            ClientsConnected.TryAdd(guid, tcpClient);
        }
        public void RemoveClientWhenUnconnect(Guid guid)
        {
            TcpClient tcpClient;
            ClientsConnected.TryRemove(guid, out tcpClient);
        }
    }
}
