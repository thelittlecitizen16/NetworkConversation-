﻿using Common;
using Microsoft.Extensions.Logging;
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
    public class CreateGroupChat : IClientOption
    {
        private TcpClient _client;
        private Data _data;
        private HandleClient _handleClient;
        private RemoveClient _removeClient;
        private Thread _thread;
        private ILogger<Worker> _logger;

        public CreateGroupChat(Data data, TcpClient client, HandleClient handleClient, RemoveClient removeClient, ILogger<Worker> logger)
        {
            _client = client;
            _data = data;
            _handleClient = handleClient;
            _removeClient = removeClient;
            _logger = logger;
        }
        public Thread Run()
        {
            _thread = new Thread(CreateGroup);
            _thread.Start();
            return _thread;
        }
        public void CreateGroup()
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
            _data.AllGroupsChat.AddGroupChat(groupChat);
            _logger.LogInformation($"add new group {groupChat.Name}");
        }
    }
}
