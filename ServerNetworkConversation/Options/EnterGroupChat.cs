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
    class EnterGroupChat : IClientOption
    {
        TcpClient clientSocket;
        Data _data;
        HandleClient _handleClient;
        Thread ctThread;
        public EnterGroupChat(Data data, TcpClient inClientSocket, HandleClient handleClient)
        {
            clientSocket = inClientSocket;
            _data = data;
            _handleClient = handleClient;
        }
        public Thread Run()
        {
            ctThread = new Thread(DoChat);
            ctThread.Start();
            return ctThread;
        }
        private void DoChat()
        {
            bool end = false;
            GroupChat group = null;

            try
            {
                var guidClient = _data.ClientsConnected.Where(c => c.Value == clientSocket).Select(c => c.Key).First();
                List<string> grouspName = _data.allGroupsChat.groupsChat
                    .Where(g => g.Participants.Contains(guidClient))
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

                    group = _data.allGroupsChat.groupsChat.Where(g => g.Name == dataReceived).First();
                    _data.allGroupsChat.AddClientConnected(group, clientSocket);

                    while (!end)
                    {
                        dataReceived = _handleClient.GetMessageFromClient(clientSocket);

                        if (dataReceived == "0")
                        {
                            Console.WriteLine("client send 0");
                            end = true;
                            _data.allGroupsChat.RemoveClientUnConnected(group, clientSocket);
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
                RemoveClientWhenOut(group, clientSocket);
            }


            Console.WriteLine("client out thread");

        }
        private void SendMessageToEachClient(GroupChat group, string message)
        {
            foreach (var client in _data.allGroupsChat.ClientConnectToGroup[group])
            {
                if (client.Connected)
                {
                    _handleClient.SendMessageToClient(client, message);
                }
            }
        }

        private void RemoveClientWhenOut(GroupChat group, TcpClient client)
        {
            clientSocket.Close();
            var guid = _data.GetClientGuid(clientSocket);
            TcpClient clientExist;
            _data.allGroupsChat.RemoveClientUnConnected(group, client);
            _data.ClientsConnected.TryRemove(guid, out clientExist);
        }
    }
}
