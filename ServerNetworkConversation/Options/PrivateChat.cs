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
        TcpClient clientSocket;
        Data _data;
        HandleClient _handleClient;
        Thread ctThread;
        public PrivateChat(Data data, TcpClient inClientSocket, HandleClient handleClient)
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
            try
            {
                var clientGuid = _data.ClientsConnectedInServer.GetGuid(clientSocket);
                string message = $"the clients you can chat with: \n";

                foreach (var clientConnected in _data.ClientsConnectedInServer.Clients)
                {
                    var guidClient = _data.ClientsConnectedInServer.GetGuid(clientConnected.Value);
                    message += $"{guidClient} \n";
                }

                _handleClient.SendMessageToClient(clientSocket, message);
                string dataReceived = _handleClient.GetMessageFromClient(clientSocket);
                TcpClient clientSend;
                Guid guidToSend;
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
                            _handleClient.SendMessageToClient(clientSocket,"0");
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
                RemoveClientWhenOut();
            }
        }

        private void RemoveClientWhenOut()
        {
            clientSocket.Close();
            var guid = _data.ClientsConnectedInServer.GetGuid(clientSocket);
            _data.ClientsConnectedInServer.Remove(guid);
        }

    }
}
