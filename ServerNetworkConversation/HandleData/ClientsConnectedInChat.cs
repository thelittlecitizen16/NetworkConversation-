using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerNetworkConversation.HandleData
{
    public class ClientsConnectedInChat
    {
        private readonly object _locker = new object();
        private List<Tuple<Guid, Guid>> _clients;

        public ClientsConnectedInChat()
        {
            _clients = new List<Tuple<Guid, Guid>>();
        }

        public void Add(Guid clientGuid, Guid clientGuidToSend)
        {
            lock (_locker)
            {
                _clients.Add(new Tuple<Guid, Guid>(clientGuid, clientGuidToSend));
            }

        }
        public void Remove(Guid clientGuid, Guid clientGuidToSend)
        {
            lock (_locker)
            {
                _clients.Remove(new Tuple<Guid, Guid>(clientGuid, clientGuidToSend));
            }
        }
        public bool HaveConversition(Guid clientGuid, Guid clientGuidToSend)
        {

            return _clients.Where(c => c.Item1 == clientGuidToSend && c.Item2 == clientGuid).Any();
        }
    }
}
