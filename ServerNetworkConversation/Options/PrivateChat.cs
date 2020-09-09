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
    public class PrivateChat : IClientOption
    {
        private TcpClient clientSocket;
        private Data _data;
        private HandleClient _handleClient;
        private RemoveClient _removeClient;
        private Thread _thread;
        public PrivateChat(Data data, TcpClient inClientSocket, HandleClient handleClient, RemoveClient removeClient)
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
            var clientGuid = _data.ClientsConnectedInServer.GetGuid(clientSocket);
            Guid guidToSend;

            try
            {
                string message = $"the clients you can chat with: \n";

                foreach (var clientConnected in _data.ClientsConnectedInServer.Clients)
                {
                    var guidClient = _data.ClientsConnectedInServer.GetGuid(clientConnected.Value);
                    message += $"{guidClient} \n";
                }

                _handleClient.SendMessageToClient(clientSocket, message);
                string dataReceived = _handleClient.GetMessageFromClient(clientSocket);
                TcpClient clientSend;
              
                Guid.TryParse(dataReceived, out guidToSend);

                if (_data.ClientsConnectedInServer.Clients.TryGetValue(guidToSend, out clientSend))
                {      
                    bool end = false;
                    message = $"success";
                    _handleClient.SendMessageToClient(clientSocket, message);
                    _data.ClientsConnectedInChat.Add(clientGuid, guidToSend);

                    while (!end)
                    {
                        dataReceived = _handleClient.GetMessageFromClient(clientSocket);

                        if (dataReceived == "0")
                        {
                            _handleClient.SendMessageToClient(clientSocket , "0");
                            _data.ClientsConnectedInChat.Remove(clientGuid, guidToSend);
                            end = true;
                        }
                        else
                        {
                            Console.WriteLine("Received and Sending back: " + dataReceived);

                            if (_data.ClientsConnectedInChat.HaveConversition(clientGuid, guidToSend))
                            {
                                _handleClient.SendMessageToClient(clientSend, dataReceived);
                            }
                            else
                            {
                               // _data.AddMessagesInPrivateChat(guidToSend, clientGuid, new List<string>() { dataReceived });
                            }
                        }
                    }          
                }
                else
                {
                     message = $"fail";
                    _handleClient.SendMessageToClient(clientSocket, message);
                }
            }
            catch (Exception)
            {
                _removeClient.RemoveClientWhenOut(clientSocket,clientGuid);
            }
        }
    }
}
