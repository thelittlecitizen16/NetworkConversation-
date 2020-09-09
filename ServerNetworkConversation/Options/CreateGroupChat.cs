using Common;
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
        TcpClient _client;
        Data _data;
        HandleClient _handleClient;
        Thread ctThread;

        public CreateGroupChat(Data data, TcpClient inClientSocket, HandleClient handleClient)
        {
            _client = inClientSocket;
            _data = data;
            _handleClient = handleClient;
        }
        public Thread Run()
        {
            ctThread = new Thread(DoChat);
            ctThread.Start();
            return ctThread;
        }
        public void DoChat()
        {
            try
            {
                Guid clientGuid = _data.ClientsConnectedInServer.GetGuid(_client);
                List<Guid> clientsToAdd = _data.ClientsConnectedInServer.Clients.Keys.ToList();
                clientsToAdd.Remove(clientGuid);

                Participants participants = new Participants(clientsToAdd);
                _handleClient.SendToClient(_client, participants);

                GroupChat groupChat = (GroupChat)_handleClient.GetFromClient(_client);

                while (groupChat == null)
                {
                    groupChat = (GroupChat)_handleClient.GetFromClient(_client);
                }
                var guidClient = _data.ClientsConnectedInServer.GetGuid(_client);

                groupChat.Managers.Add(guidClient);
                groupChat.Participants.Add(guidClient);
                _data.AllGroupsChat.groupsChat.Add(groupChat);
            }
            catch (Exception)
            {
                var guid = _data.ClientsConnectedInServer.GetGuid(_client);
                _data.ClientsConnectedInServer.Remove(guid);
                _client.Close();
            }
        }
    }
}
