using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace ServerNetworkConversation.HandleData
{
    public class ClientsConnectedInServer
    {
        public ConcurrentDictionary<Guid, TcpClient> Clients { get; private set; }
        public ClientsConnectedInServer()
        {
            Clients = new ConcurrentDictionary<Guid, TcpClient>();
        }

        public Guid GetGuid(TcpClient cleint)
        {
            return Clients.Keys.Where(k => Clients[k] == cleint).First();
        }
        public void AddWhenConnect(Guid guid, TcpClient tcpClient)
        {
            Clients.TryAdd(guid, tcpClient);
        }
        public void Remove(Guid guid)
        {
            TcpClient client;
            Clients.TryRemove(guid, out client);
        }
    }
}
