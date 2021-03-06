﻿using Common.Enums;
using Common.HandleRequests;
using Common.Models;
using Microsoft.Extensions.Logging;
using ServerNetworkConversation.HandleData;
using ServerNetworkConversation.Options.Interfaces;
using ServerNetworkConversation.Options.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace ServerNetworkConversation.Options.GroupsChat
{
    class EnterGroupChat : IClientOption
    {
        private TcpClient _client;
        private Data _data;
        private Thread _thread;
        private ILogger<Worker> _logger;
        private IRequests _requests;
        public EnterGroupChat(Data data, TcpClient client, ILogger<Worker> logger, IRequests requests)
        {
            _client = client;
            _data = data;
            _logger = logger;
            _requests = requests;
        }
        public Thread Run()
        {
            _thread = new Thread(StartGroupChat);
            _thread.Start();
            return _thread;
        }

        private void StartGroupChat()
        {
            bool end = false;
            GroupChat group = null;
            var clientGuid = _data.ClientsConnectedInServer.GetGuid(_client);

            try
            {
                SendAllClientGroups(clientGuid);

                string dataReceived = _requests.GetStringMessage(_client);
                if (dataReceived == "0")
                {
                    _logger.LogInformation("client dont want to enter chat in any group");
                }
                else
                {

                    group = _data.AllGroupsChat.GetGroupsChat().Where(g => g.Name == dataReceived).First();
                    _logger.LogInformation($"client {clientGuid} enter to group chat {group.Name}");
                    _data.AllGroupsChat.AddClientConnected(group, _client);
                    SendMessagesHistory(group);

                    while (!end)
                    {
                        dataReceived = _requests.GetStringMessage(_client);

                        if (dataReceived == "0")
                        {
                            MessageRequest messageRequest = new MessageRequest(MessageKey.Exit, "0");
                            _requests.SendModelMessage(_client, messageRequest);

                            ClientOutOfGroup(group);
                            _data.AllGroupsChat.RemoveClientUnConnected(group, _client);
                            end = true;
                        }
                        else
                        {
                            string message = $"{clientGuid} send:  {dataReceived}";
                            SendMessageToEachClient(group, message);
                        }
                    }

                }

            }
            catch (Exception)
            {
                end = true;
                _data.AllGroupsChat.RemoveClientUnConnected(group, _client);
                ChatUtils.RemoveClientWhenOut(_client, clientGuid, _data);
            }


            _logger.LogInformation("client out chat");
        }
        private void SendMessagesHistory(GroupChat groupChat)
        {
            ChatUtils.SendMessagesHistory(_data.AllGroupsChat.GetAllGroupHistory(groupChat), _client, _requests);
        }

        private void SendMessageToEachClient(GroupChat group, string message)
        {
            ChatUtils.SendMessageToEachClient(message, _data.AllGroupsChat.ClientConnectToGroup[group], _requests);

            List<TcpClient> allClientsNotConnected = GetAllUnConnectedClients(group);

            SaveAlerts(allClientsNotConnected, group);

            _data.AllGroupsChat.AddMessageToHistory(group, message);
            _logger.LogInformation($"Received in group {group.Name} and Sending all: {message}");
        }

        private List<TcpClient>  GetAllUnConnectedClients(GroupChat group)
        {
            List<TcpClient> allClientsNotConnected = new List<TcpClient>();
            foreach (var participant in group.Participants)
            {
                TcpClient client = _data.ClientsConnectedInServer.Clients.Where(c => c.Key == participant).Select(c => c.Value).First();

                if (!_data.AllGroupsChat.ClientConnectToGroup[group].Contains(client))
                {
                    allClientsNotConnected.Add(client);
                }
            }

            return allClientsNotConnected;
        }

        private void SaveAlerts(List<TcpClient> allClientsNotConnected, GroupChat group)
        {
            Alert alert = new Alert(AlertOptions.NEW_MESSAGE, $"you have message in group {group.Name}");
            MessageRequest messageRequestAlert = new MessageRequest(MessageKey.ALERT, alert);

            foreach (var client in allClientsNotConnected)
            {
                _data.ClientsAlerts.AddNewAlert(client, messageRequestAlert);
            }
        }

        private void SendAllClientGroups(Guid clientGuid)
        {
            GroupUtils.SendAllClientGroups(_client, _requests, _data, clientGuid);
        }

        private void ClientOutOfGroup(GroupChat group)
        {
            _data.AllGroupsChat.RemoveClientUnConnected(group, _client);
            _logger.LogInformation($"client{_client} out of group");
        }
    }
}
