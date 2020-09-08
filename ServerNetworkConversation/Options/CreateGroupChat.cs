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
                Guid clientGuid = _data.ClientsConnected.Where(c => c.Value == _client).Select(c => c.Key).First();
                List<Guid> clientsToAdd = _data.ClientsConnected.Keys.ToList();
                clientsToAdd.Remove(clientGuid);

                Participants participants = new Participants(clientsToAdd);
                _handleClient.SendToClient(_client, participants);

                GroupChat groupChat = (GroupChat)_handleClient.GetFromClient(_client);

                while (groupChat == null)
                {
                    groupChat = (GroupChat)_handleClient.GetFromClient(_client);
                }
                var guidClient = _data.ClientsConnected.Where(c => c.Value == _client).Select(c => c.Key).First();

                groupChat.Managers.Add(guidClient);
                groupChat.Participants.Add(guidClient);
                _data.allGroupsChat.groupsChat.Add(groupChat);
            }
            catch (Exception)
            {
                var guid = _data.GetClientGuid(_client);
                TcpClient clientExist;
                _data.ClientsConnected.TryRemove(guid, out clientExist);
                _client.Close();

            }
        }
    }
}
