using Common;
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
        private HandleClient _handleClient;
        private RemoveClient _removeClient;
        private Thread _thread;

        public LeaveGroupChat(Data data, TcpClient inClientSocket, HandleClient handleClient, RemoveClient removeClient)
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
        private void DoChat()
        {
            var clientGuid = _data.ClientsConnectedInServer.GetGuid(_client);

            try
            {
                SendAllClientGroups(clientGuid);

                string dataReceived = _handleClient.GetMessageFromClient(_client);
                if (dataReceived == "0")
                {
                    Console.WriteLine("client send 0");
                }
                else
                {
                    _data.AllGroupsChat.groupsChat.Where(g => g.Name == dataReceived).First().Participants.Remove(clientGuid);
                }
            }
            catch (Exception)
            {
                _removeClient.RemoveClientWhenOut(_client, clientGuid);
            }


            Console.WriteLine("client out thread");
        }

        private void SendAllClientGroups(Guid clientGuid)
        {
            List<string> grouspName = _data.AllGroupsChat.groupsChat
               .Where(g => g.Participants.Contains(clientGuid))
               .Select(g => g.Name).ToList();

            AllGroupChat allGroupChat = new AllGroupChat(grouspName);
            _handleClient.SendToClient(_client, allGroupChat);
        }
    }
}
