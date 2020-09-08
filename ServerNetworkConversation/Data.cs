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
        // public ConcurrentDictionary<Guid, ConcurrentDictionary<Guid, List<string>>> MessagesInPrivateChat { get; private set; }
        public List<Tuple<Guid, Guid, List<string>>> MessagesInPrivateChat { get; private set; }

        public Data()
        {
            ClientsInGlobalChat = new ConcurrentDictionary<Guid, TcpClient>();
            ClientsConnected = new ConcurrentDictionary<Guid, TcpClient>();
            MessagesInPrivateChat = new List<Tuple<Guid, Guid, List<string>>>();
        }
        public bool ClientHaveMessage(Guid reciver, Guid sender)
        {
            return MessagesInPrivateChat.Where(c => c.Item1 == reciver && c.Item2 == sender).Any();
        }
        public List<string> ClientGetMessages(Guid reciver, Guid sender)
        {
            return MessagesInPrivateChat.Where(c => c.Item1 == reciver && c.Item2 == sender).First().Item3;
        }
        public void AddMessagesInPrivateChat(Guid reciver, Guid sender, List<string> messages)
        {
            var existMessages = MessagesInPrivateChat.Where(c => c.Item1 == reciver && c.Item2 == sender);
            if (existMessages.Any())
            {
                existMessages.First().Item3.AddRange(messages);
            }
            else
            {
                MessagesInPrivateChat.Add(new Tuple<Guid, Guid, List<string>>(reciver, sender, messages));
            }
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
