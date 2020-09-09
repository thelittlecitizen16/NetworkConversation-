using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace ServerNetworkConversation.HandleData
{
    public class ClientsInGlobalChat
    {
        public ConcurrentDictionary<Guid, TcpClient> Clients { get; private set; }
        public ClientsInGlobalChat()
        {
            Clients = new ConcurrentDictionary<Guid, TcpClient>();
        }
        public void Add(Guid guid, TcpClient tcpClient)
        {
            Clients.TryAdd(guid, tcpClient);
        }
        public void Remove(Guid guid)
        {
            TcpClient tcpClient;
            Clients.TryRemove(guid, out tcpClient);
        }
        public Guid GetClient(TcpClient client)
        {
           return Clients.Where(c => c.Value == client).Select(c => c.Key).FirstOrDefault();
        }
    }
}
