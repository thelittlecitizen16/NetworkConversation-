using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerNetworkConversation.HandleData
{
    public class ClientsConnectedInChat
    {
        public List<Tuple<Guid, Guid>> Clients { get; private set; }

        public ClientsConnectedInChat()
        {
            Clients = new List<Tuple<Guid, Guid>>();
        }

        public void Add(Guid clientGuid, Guid clientGuidToSend)
        {
            Clients.Add(new Tuple<Guid, Guid>(clientGuid, clientGuidToSend));
        }
        public bool HaveConversition(Guid clientGuid, Guid clientGuidToSend)
        {
            return Clients.Where(c => c.Item1 == clientGuidToSend && c.Item2 == clientGuid).Any();
        }
    }
}
