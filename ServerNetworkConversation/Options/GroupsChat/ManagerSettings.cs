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
    public class  ManagerSettings : IClientOption
    {
        private TcpClient _client;
        private Data _data;
        private Thread _thread;
        private ILogger<Worker> _logger;
        private IRequests _requests;

        public ManagerSettings(Data data, TcpClient client,  ILogger<Worker> logger, IRequests requests)
        {
            _client = client;
            _data = data;
            _logger = logger;
            _requests = requests;
        }
        public Thread Run()
        {
            _thread = new Thread(StartManagerSettings);
            _thread.Start();
            return _thread;
        }
        public void StartManagerSettings()
        {
            Guid clientGuid = _data.ClientsConnectedInServer.GetGuid(_client);

            try
            {
                AllGroupsManagedByClient(clientGuid);
                string dataReceived = _requests.GetStringMessage(_client);
                GroupChat oldGroupChat = null;

                if (dataReceived == "0")
                {
                    _logger.LogInformation($"client dont want to enter manager settings to any group");
                }
                else
                {
                    oldGroupChat = SendGroup(dataReceived);
                    SendAllConecctedClients(clientGuid);

                    GroupChat newGroupChat = WaitToGetGroupFromClient();
                    AddGroup(newGroupChat, oldGroupChat, clientGuid);
                    _logger.LogInformation($"client {clientGuid} change settings group {newGroupChat.Name}");
                } 
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
        private void AllGroupsManagedByClient(Guid clientGuid)
        {
            List<string> grouspName = _data.AllGroupsChat.GetGroupsChat()
               .Where(g => g.Managers.Contains(clientGuid))
               .Select(g => g.Name).ToList();

            GroupUtils.SendAllGroupChat(_client, _requests, grouspName);
        }
        private GroupChat SendGroup(string groupName)
        {
            GroupChat groupChat = _data.AllGroupsChat.GetGroupsChat().Where(g=>g.Name == groupName).First();
            _requests.SendModelMessage(_client, groupChat);

            return groupChat;
        }
        private GroupChat WaitToGetGroupFromClient()
        {
            return GroupUtils.WaitToGetGroupFromClient(_client, _requests);
        }
        private void AddGroup(GroupChat newGroupChat, GroupChat oldGroupChat, Guid clientGuid)
        {
            _data.AllGroupsChat.RemoveGroupChat(oldGroupChat);
            _data.AllGroupsChat.AddGroupChat(newGroupChat);
            List<Guid> removeClients = GetRemoveClients(newGroupChat, oldGroupChat);

            SaveAlert(removeClients,oldGroupChat, clientGuid);
        }
        private List<Guid> GetRemoveClients(GroupChat newGroupChat, GroupChat oldGroupChat)
        {
            List<Guid> removeClients = new List<Guid>();

            foreach (var participant in oldGroupChat.Participants)
            {
                if (!newGroupChat.Participants.Contains(participant))
                {
                    removeClients.Add(participant);
                }
            }
            return removeClients;
        }

        private void SaveAlert(List<Guid> removeClients,GroupChat oldGroupChat, Guid clientGuid)
        {
            Alert alert = new Alert(AlertOptions.EXIT_GROUP, $"you exit  group {oldGroupChat.Name} by {clientGuid}");
            MessageRequest messageRequestAlert = new MessageRequest(MessageKey.ALERT, alert);

            foreach (var client in removeClients)
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
