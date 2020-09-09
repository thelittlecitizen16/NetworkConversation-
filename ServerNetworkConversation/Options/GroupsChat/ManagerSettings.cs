using Common;
using ServerNetworkConversation.HandleData;
using ServerNetworkConversation.Options.HandleOptions;
using ServerNetworkConversation.Options.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerNetworkConversation.Options.GroupsChat
{
    public class  ManagerSettings : IClientOption
    {
        private TcpClient _client;
        private Data _data;
        private HandleClient _handleClient;
        private RemoveClient _removeClient;
        private Thread _thread;

        public ManagerSettings(Data data, TcpClient inClientSocket, HandleClient handleClient, RemoveClient removeClient)
        {
            _client = inClientSocket;
            _data = data;
            _handleClient = handleClient;
            _removeClient = removeClient;
        }
        public Thread Run()
        {
            _thread = new Thread(DoChat);
            _thread.Start();
            return _thread;
        }
        public void DoChat()
        {
            Guid clientGuid = _data.ClientsConnectedInServer.GetGuid(_client);

            try
            {
                AllGroupsManagedByClient(clientGuid);
                string dataReceived = _handleClient.GetMessageFromClient(_client);
                GroupChat oldGroupChat = null;

                if (dataReceived == "0")
                {
                    Console.WriteLine("client send 0");
                }
                else
                {
                    oldGroupChat = SendGroup(dataReceived);
                    SendAllConecctedClients(clientGuid);

                    GroupChat newGroupChat = WaitToGetGroupFromClient();
                    AddGroup(newGroupChat, oldGroupChat);
                } 
            }
            catch (Exception)
            {
                _removeClient.RemoveClientWhenOut(_client, clientGuid);
            }
        }

        private void SendAllConecctedClients(Guid clientGuid)
        {
            List<Guid> clientsToAdd = _data.ClientsConnectedInServer.Clients.Keys.ToList();
            clientsToAdd.Remove(clientGuid);

            Participants participants = new Participants(clientsToAdd);
            _handleClient.SendToClient(_client, participants);
        }
        private void AllGroupsManagedByClient(Guid clientGuid)
        {
            List<string> grouspName = _data.AllGroupsChat.groupsChat
               .Where(g => g.Managers.Contains(clientGuid))
               .Select(g => g.Name).ToList();

            AllGroupChat allGroupChat = new AllGroupChat(grouspName);
            _handleClient.SendToClient(_client, allGroupChat);
        }
        private GroupChat SendGroup(string groupName)
        {
            GroupChat groupChat = _data.AllGroupsChat.groupsChat.Where(g=>g.Name == groupName).First();
            _handleClient.SendToClient(_client, groupChat);

            return groupChat;
        }
        private GroupChat WaitToGetGroupFromClient()
        {
            GroupChat groupChat = (GroupChat)_handleClient.GetFromClient(_client);

            while (groupChat == null)
            {
                groupChat = (GroupChat)_handleClient.GetFromClient(_client);
            }

            return groupChat;
        }
        private void AddGroup(GroupChat newGroupChat, GroupChat oldGroupChat)
        {
            _data.AllGroupsChat.groupsChat.Remove(oldGroupChat);
            _data.AllGroupsChat.groupsChat.Add(newGroupChat);
        }
    }
}
