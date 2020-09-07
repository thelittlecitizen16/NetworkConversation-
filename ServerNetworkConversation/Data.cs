using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ServerNetworkConversation
{
    public class Data
    {
        private ConcurrentDictionary<Guid, TcpClient> clientsList;
        public Data()
        {
            clientsList = new ConcurrentDictionary<Guid, TcpClient>();
        }

        public void Add(Guid guid, TcpClient tcpClient)
        {
            clientsList.TryAdd(guid, tcpClient);
        }
        public void Remove(Guid guid)
        {
            TcpClient tcpClient;
            clientsList.TryRemove(guid, out tcpClient);
        }
    }
}
