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
    public class LeaveGroupChat : IClientOption
    {
        private TcpClient _client;
        private Data _data;
        private RemoveClient _removeClient;
        private Thread _thread;
        private ILogger<Worker> _logger;
        private IRequests _requests;

        public LeaveGroupChat(Data data, TcpClient client, RemoveClient removeClient, ILogger<Worker> logger, IRequests requests)
        {
            _client = client;
            _data = data;
            _removeClient = removeClient;
            _logger = logger;
            _requests = requests;
        }

        public Thread Run()
        {
            _thread = new Thread(LeaveGroup);
            _thread.Start();
            return _thread;
        }
        private void LeaveGroup()
        {
            var clientGuid = _data.ClientsConnectedInServer.GetGuid(_client);

            try
            {
                SendAllClientGroups(clientGuid);

                string dataReceived = _requests.GetStringMessage(_client);

                if (dataReceived == "0")
                {
                    _logger.LogInformation($"client {clientGuid} dont want to leave any group");
                }
                else
                {
                    _data.AllGroupsChat.GetGroupsChat().Where(g => g.Name == dataReceived).First().Participants.Remove(clientGuid);
                    _logger.LogInformation($"client {clientGuid} leave group {dataReceived}");
                }
            }
            catch (Exception)
            {
                _removeClient.RemoveClientWhenOut(_client, clientGuid);
            }
        }

        private void SendAllClientGroups(Guid clientGuid)
        {
            List<string> grouspName = _data.AllGroupsChat.GetGroupsChat()
               .Where(g => g.Participants.Contains(clientGuid))
               .Select(g => g.Name).ToList();

            AllGroupChat allGroupChat = new AllGroupChat(grouspName);
            _requests.SendModelMessage(_client, allGroupChat);
        }
    }
}
