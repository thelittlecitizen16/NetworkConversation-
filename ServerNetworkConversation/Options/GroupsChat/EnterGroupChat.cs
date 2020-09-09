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
    class EnterGroupChat : IClientOption
    {
        private TcpClient _client;
        private Data _data;
        private HandleClient _handleClient;
        private RemoveClient _removeClient;
        private Thread _thread;
        private ILogger<Worker> _logger;

        public EnterGroupChat(Data data, TcpClient client, HandleClient handleClient, RemoveClient removeClient, ILogger<Worker> logger)
        {
            _client = client;
            _data = data;
            _handleClient = handleClient;
            _removeClient = removeClient;
            _logger = logger;
        }
        public Thread Run()
        {
            _thread = new Thread(DoChat);
            _thread.Start();
            return _thread;
        }

        private void DoChat()
        {
            bool end = false;
            GroupChat group = null;
            var clientGuid = _data.ClientsConnectedInServer.GetGuid(_client);

            try
            {
                SendAllClientGroups(clientGuid);

                string dataReceived = _handleClient.GetMessageFromClient(_client);
                if (dataReceived == "0")
                {
                    _logger.LogInformation("client dont want to enter chat in any group");
                }
                else
                {

                    group = _data.AllGroupsChat.GroupsChat.Where(g => g.Name == dataReceived).First();
                    _data.AllGroupsChat.AddClientConnected(group, _client);
                    _logger.LogInformation($"client {clientGuid} enter to group chat {group.Name}");

                    while (!end)
                    {
                        dataReceived = _handleClient.GetMessageFromClient(_client);

                        if (dataReceived == "0")
                        {
                            _handleClient.SendMessageToClient(_client, "0");
                            ClientOutOfGroup(group);
                            _data.AllGroupsChat.RemoveClientUnConnected(group, _client);
                            end = true;
                        }
                        else
                        {
                            SendMessageToEachClient(group, dataReceived);
                        }
                    }

                }

            }
            catch (Exception)
            {
                end = true;
                _data.AllGroupsChat.RemoveClientUnConnected(group, _client);
                _removeClient.RemoveClientWhenOut(_client, clientGuid);
            }


            _logger.LogInformation("client out chat");
        }
        private void SendMessageToEachClient(GroupChat group, string message)
        {
            foreach (var client in _data.AllGroupsChat.ClientConnectToGroup[group])
            {
                if (client.Connected)
                {
                    _handleClient.SendMessageToClient(client, message);
                }
            }

            _logger.LogInformation($"Received in group {group.Name} and Sending all: {message}");
        }
        private void SendAllClientGroups(Guid clientGuid)
        {
            List<string> grouspName = _data.AllGroupsChat.GroupsChat
               .Where(g => g.Participants.Contains(clientGuid))
               .Select(g => g.Name).ToList();

            AllGroupChat allGroupChat = new AllGroupChat(grouspName);
            _handleClient.SendToClient(_client, allGroupChat);
        }
        private void ClientOutOfGroup(GroupChat group)
        {
            _data.AllGroupsChat.RemoveClientUnConnected(group, _client);
            _logger.LogInformation($"client{_client} out of group");
        }
    }
}
