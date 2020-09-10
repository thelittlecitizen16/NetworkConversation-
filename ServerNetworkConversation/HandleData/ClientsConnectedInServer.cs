using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Sockets;

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
            var clients = Clients.Keys.Where(k => Clients[k] == cleint);
            if (clients.Any())
            {
                return clients.First();
            }

            return new Guid();
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
