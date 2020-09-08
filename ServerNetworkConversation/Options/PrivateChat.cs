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
                var clientGuid = _data.ClientsConnected.Where(c => c.Value == clientSocket).Select(c => c.Key).First();
                string message = $"the clients you can chat with: \n";

                foreach (var clientConnected in _data.ClientsConnected)
                {
                    var guidClient = _data.ClientsConnected.Where(c => c.Value == clientConnected.Value).Select(c => c.Key).First();
                    message += $"{guidClient} \n";
                }

                _handleClient.SendMessageToClient(clientSocket, message);
                string dataReceived = _handleClient.GetMessageFromClient(clientSocket);
                TcpClient clientSend;
                Guid guidToSend;
                Guid.TryParse(dataReceived, out guidToSend);

                if (_data.ClientsConnected.TryGetValue(guidToSend, out clientSend))
                {      
                    bool end = false;
                    message = $"success";
                    _handleClient.SendMessageToClient(clientSocket, message);
                    _data.ClientsConnectedInChat.Add(new Tuple<Guid, Guid>(clientGuid, guidToSend));

                    ////TODO: check if have message in this chat
                    //string messageToSend = "";

                    //if (_data.ClientHaveMessage(clientGuid, guidToSend))
                    //{
                    //    foreach (var clientMessage in _data.ClientGetMessages(clientGuid, guidToSend))
                    //    {
                    //        messageToSend += clientMessage + "\n";
                    //    }

                    //}
                    //_handleClient.SendMessageToClient(clientSocket, messageToSend);

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
                            if (_data.ClientsConnectedInChat.Where(c=>c.Item1 == guidToSend && c.Item2 == clientGuid).Any())
                            {
                                _handleClient.SendMessageToClient(clientSend, dataReceived);
                            }
                            else
                            {
                                _data.AddMessagesInPrivateChat(guidToSend, clientGuid, new List<string>() { dataReceived });
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
            catch (SocketException)
            {
                RemoveClientWhenOut();
            }
            catch (ObjectDisposedException)
            {
                RemoveClientWhenOut();
            }
            catch (Exception)
            {
                RemoveClientWhenOut();
            }

        }
        private void RemoveClientWhenOut()
        {
            clientSocket.Close();
            var guid = _data.GetClientGuid(clientSocket);
            TcpClient clientExist;
            _data.ClientsConnected.TryRemove(guid, out clientExist);
        }

    }
}
