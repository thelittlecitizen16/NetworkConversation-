using ServerNetworkConversation.HandleData;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace ServerNetworkConversation.HandleData
{
    public class Data
    {
        public AllGroupsChat AllGroupsChat {  get; private set; }
        public ClientsConnectedInChat ClientsConnectedInChat { get; private set; }
        public ClientsInGlobalChat ClientsInGlobalChat { get; private set; }
        public ClientsConnectedInServer ClientsConnectedInServer { get; private set; }
        //  public List<Tuple<Guid, Guid, List<string>>> MessagesInPrivateChat { get; private set; }

        public Data()
        {
         //   MessagesInPrivateChat = new List<Tuple<Guid, Guid, List<string>>>();
            AllGroupsChat = new AllGroupsChat();
            ClientsConnectedInChat = new ClientsConnectedInChat();
            ClientsInGlobalChat = new ClientsInGlobalChat();
            ClientsConnectedInServer = new ClientsConnectedInServer();
        }
        
        //public bool ClientHaveMessage(Guid reciver, Guid sender)
        //{
        //    return MessagesInPrivateChat.Where(c => c.Item1 == reciver && c.Item2 == sender).Any();
        //}
        //public List<string> ClientGetMessages(Guid reciver, Guid sender)
        //{
        //    return MessagesInPrivateChat.Where(c => c.Item1 == reciver && c.Item2 == sender).First().Item3;
        //}
        //public void AddMessagesInPrivateChat(Guid reciver, Guid sender, List<string> messages)
        //{
        //    var existMessages = MessagesInPrivateChat.Where(c => c.Item1 == reciver && c.Item2 == sender);
        //    if (existMessages.Any())
        //    {
        //        existMessages.First().Item3.AddRange(messages);
        //    }
        //    else
        //    {
        //        MessagesInPrivateChat.Add(new Tuple<Guid, Guid, List<string>>(reciver, sender, messages));
        //    }
        //}
        //public void RemoveClientWhenUnconnect(Guid guid)
        //{
        //    TcpClient tcpClient;
        //    ClientsConnected.TryRemove(guid, out tcpClient);
        //}
    }
}
