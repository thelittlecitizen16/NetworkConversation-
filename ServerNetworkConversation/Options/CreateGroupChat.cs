﻿using Common;
using ServerNetworkConversation.HandleData;
using ServerNetworkConversation.Options.HandleOptions;
using ServerNetworkConversation.Options.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerNetworkConversation.Options
{
    public class CreateGroupChat : IClientOption
    {
        private TcpClient _client;
        private Data _data;
        private HandleClient _handleClient;
        private RemoveClient _removeClient;
        private Thread _thread;

        public CreateGroupChat(Data data, TcpClient inClientSocket, HandleClient handleClient, RemoveClient removeClient)
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
                SendAllConecctedClients(clientGuid);

                GroupChat groupChat = WaitToGetGroupFromClient();

                AddGroup(groupChat, clientGuid);
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
        private GroupChat WaitToGetGroupFromClient()
        {
            GroupChat groupChat = (GroupChat)_handleClient.GetFromClient(_client);

            while (groupChat == null)
            {
                groupChat = (GroupChat)_handleClient.GetFromClient(_client);
            }

            return groupChat;
        }
        private void AddGroup(GroupChat groupChat, Guid clientGuid)
        {
            groupChat.Managers.Add(clientGuid);
            groupChat.Participants.Add(clientGuid);
            _data.AllGroupsChat.groupsChat.Add(groupChat);
        }
    }
}
