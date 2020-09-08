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
                string message = "";
                foreach (var clientConnected in _data.ClientsConnected)
                {
                    var guidClient = _data.ClientsConnected.Where(c => c.Value == clientConnected.Value).Select(c => c.Key).First();
                     message = $"the clients you can chat with: \n";
                    message += $"{guidClient} \n";
                }

                _handleClient.SendMessageToClient(clientSocket, message);
                string dataReceived = _handleClient.GetMessageFromClient(clientSocket);
                TcpClient clientSend;
                Guid guid;
                Guid.TryParse(dataReceived, out guid);
                if (_data.ClientsConnected.TryGetValue(guid, out clientSend))
                {
                    bool end = false;
                    message = $"success";
                    _handleClient.SendMessageToClient(clientSocket, message);

                    while (!end)
                    {
                       
                        dataReceived = _handleClient.GetMessageFromClient(clientSocket);

                        if (dataReceived == "0")
                        {
                            end = true;
                        }
                        else
                        {
                            Console.WriteLine("Received and Sending back: " + dataReceived);
                             message = $"{dataReceived}";
                            _handleClient.SendMessageToClient(clientSend , message);
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
