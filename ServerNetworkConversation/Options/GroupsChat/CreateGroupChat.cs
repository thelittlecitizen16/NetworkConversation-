using Common;
using Common.Enums;
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
using System.Text;
using System.Threading;

namespace ServerNetworkConversation.Options.GroupsChat
{
    public class CreateGroupChat : IClientOption
    {
        private TcpClient _client;
        private Data _data;
        private Thread _thread;
        private ILogger<Worker> _logger;
        private IRequests _requests;

        public CreateGroupChat(Data data, TcpClient client, ILogger<Worker> logger,  IRequests requests)
        {
            _client = client;
            _data = data;
            _logger = logger;
            _requests = requests;
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
                ChatUtils.RemoveClientWhenOut(_client, clientGuid, _data);
            }
        }

        private void SendAllConecctedClients(Guid clientGuid)
        {
            List<Guid> clientsToAdd = _data.ClientsConnectedInServer.Clients.Keys.ToList();
            clientsToAdd.Remove(clientGuid);

            Participants participants = new Participants(clientsToAdd);
            _requests.SendModelMessage(_client, participants);
        }
        private GroupChat WaitToGetGroupFromClient()
        {
           return GroupUtils.WaitToGetGroupFromClient(_client, _requests);
        }
        private void AddGroup(GroupChat groupChat, Guid clientGuid)
        {
            groupChat.Managers.Add(clientGuid);
            groupChat.Participants.Add(clientGuid);
            _data.AllGroupsChat.AddGroupChat(groupChat);
            _logger.LogInformation($"add new group {groupChat.Name}");

            SaveAlert(groupChat, clientGuid);
        }
        private void SaveAlert(GroupChat groupChat, Guid clientGuid)
        {
            Alert alert = new Alert(AlertOptions.NEW_GROUP, $"you enter to group {groupChat.Name} by {clientGuid}");
            MessageRequest messageRequestAlert = new MessageRequest(MessageKey.ALERT, alert);

            foreach (var client in groupChat.Participants)
            {
                var Client = _data.ClientsConnectedInServer.Clients.Where(c => c.Key == client).Select(c => c.Value);
                if (Client.Any())
                {
                    _data.ClientsAlerts.AddNewAlert(Client.First(), messageRequestAlert);
                }
            }
        }
    }
}
