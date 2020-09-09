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

namespace ServerNetworkConversation.Options
{
    class EnterGroupChat : IClientOption
    {
        private TcpClient clientSocket;
        private Data _data;
        private HandleClient _handleClient;
        private RemoveClient _removeClient;
        private Thread _thread;
        public EnterGroupChat(Data data, TcpClient inClientSocket, HandleClient handleClient, RemoveClient removeClient)
        {
            clientSocket = inClientSocket;
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
            bool end = false;
            GroupChat group = null;
            var clientGuid = _data.ClientsConnectedInServer.GetGuid(clientSocket);

            try
            {
                List<string> grouspName = _data.AllGroupsChat.groupsChat
                    .Where(g => g.Participants.Contains(clientGuid))
                    .Select(g => g.Name).ToList();
                AllGroupChat allGroupChat = new AllGroupChat(grouspName);
                _handleClient.SendToClient(clientSocket, allGroupChat);
              


                string dataReceived = _handleClient.GetMessageFromClient(clientSocket);
                if (dataReceived == "0")
                {
                    Console.WriteLine("client send 0");
                }
                else
                {

                    group = _data.AllGroupsChat.groupsChat.Where(g => g.Name == dataReceived).First();
                    _data.AllGroupsChat.AddClientConnected(group, clientSocket);

                    while (!end)
                    {
                        dataReceived = _handleClient.GetMessageFromClient(clientSocket);

                        if (dataReceived == "0")
                        {
                            _data.AllGroupsChat.RemoveClientUnConnected(group, clientSocket);
                            Console.WriteLine("client send 0");
                            end = true;
                        }
                        else
                        {
                            Console.WriteLine("Received and Sending back: " + dataReceived);
                            SendMessageToEachClient(group, dataReceived);
                        }
                    }

                }

            }
            catch (Exception)
            {
                end = true;
                _data.AllGroupsChat.RemoveClientUnConnected(group, clientSocket);
                _removeClient.RemoveClientWhenOut(clientSocket, clientGuid);
               // RemoveClientWhenOut(group, clientSocket);
            }


            Console.WriteLine("client out thread");

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
        }
    }
}
