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
        private List<Tuple<Guid, Guid, List<string>>> _messageHistory;

        public ClientsConnectedInChat()
        {
            _clients = new List<Tuple<Guid, Guid>>();
            _messageHistory = new List<Tuple<Guid, Guid, List<string>>>();
        }
        public bool IsClientInChat(Guid guidClient)
        {
            return _clients.Where(c => c.Item1 == guidClient).Any();
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
        public void AddMessagesToHistory(Guid clientGuid, Guid clientGuidToSend, string message)
        {
            lock (_locker)
            {
                var messages = _messageHistory.Where(c => (c.Item1 == clientGuidToSend || c.Item1 == clientGuid) && (c.Item1 == clientGuid || c.Item1 == clientGuidToSend));

                if (messages.Any())
                {
                    messages.First().Item3.Add(message);
                }
                else
                {
                    _messageHistory.Add(new Tuple<Guid, Guid, List<string>>(clientGuid, clientGuidToSend, new List<string>() { message }));
                }
            }
        }
        public List<string> GetMessagesToHistory(Guid clientGuid, Guid clientGuidToSend)
        {
            var messages = _messageHistory.Where(c => (c.Item1 == clientGuidToSend || c.Item1 == clientGuid) && (c.Item1 == clientGuid || c.Item1 == clientGuidToSend));

            if (messages.Any())
            {
                return messages.First().Item3;
            }
           
            return new List<string>();
        }
        public bool HaveConversition(Guid clientGuid, Guid clientGuidToSend)
        {

            return _clients.Where(c => c.Item1 == clientGuidToSend && c.Item2 == clientGuid).Any();
        }
    }
}
