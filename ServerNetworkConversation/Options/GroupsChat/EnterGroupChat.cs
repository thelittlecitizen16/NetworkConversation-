using Common;
using Common.HandleRequests;
using Common.Models;
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
        private IRequests _requests;
        public EnterGroupChat(Data data, TcpClient client, HandleClient handleClient, RemoveClient removeClient, ILogger<Worker> logger, IRequests requests)
        {
            _client = client;
            _data = data;
            _handleClient = handleClient;
            _removeClient = removeClient;
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
                    _data.AllGroupsChat.AddClientConnected(group, _client);
                    _logger.LogInformation($"client {clientGuid} enter to group chat {group.Name}");

                    SendMessagesHistory(group);

                    while (!end)
                    {
                        dataReceived = _requests.GetStringMessage(_client);

                        if (dataReceived == "0")
                        {
                            _requests.SendStringMessage(_client, "0");
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
                _removeClient.RemoveClientWhenOut(_client, clientGuid);
            }


            _logger.LogInformation("client out chat");
        }
        private void SendMessagesHistory(GroupChat groupChat)
        {
            string allMessages = "";

            foreach (var message in _data.AllGroupsChat.GetAllGroupHistory(groupChat))
            {
                allMessages += message + "\n";
            }
            if (allMessages != "")
            {
                _requests.SendStringMessage(_client, allMessages);
            }
        }
        private void SendMessageToEachClient(GroupChat group, string message)
        {
            foreach (var client in _data.AllGroupsChat.ClientConnectToGroup[group])
            {
                if (client.Connected)
                {
                    _requests.SendStringMessage(client, message);
                }
            }

            _data.AllGroupsChat.AddMessageToHistory(group, message);
            _logger.LogInformation($"Received in group {group.Name} and Sending all: {message}");
        }
        private void SendAllClientGroups(Guid clientGuid)
        {
            List<string> grouspName = _data.AllGroupsChat.GetGroupsChat()
               .Where(g => g.Participants.Contains(clientGuid))
               .Select(g => g.Name).ToList();

            AllGroupChat allGroupChat = new AllGroupChat(grouspName);
            _requests.SendModelMessage(_client, allGroupChat);
        }
        private void ClientOutOfGroup(GroupChat group)
        {
            _data.AllGroupsChat.RemoveClientUnConnected(group, _client);
            _logger.LogInformation($"client{_client} out of group");
        }
    }
}
