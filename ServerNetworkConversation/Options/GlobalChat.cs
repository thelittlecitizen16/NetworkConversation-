using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Linq;
using ServerNetworkConversation.Options.Interfaces;

namespace ServerNetworkConversation.Options
{
    public class GlobalChat : IClientOption
    {
        TcpClient clientSocket;
        Data _data;
        HandleClient _handleClient;
        Thread ctThread;

        public GlobalChat(Data data, TcpClient inClientSocket, HandleClient handleClient)
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

            var guidClient = _data.ClientsInGlobalChat.Where(c => c.Value == clientSocket).Select(c => c.Key).First();
            string message = $"{guidClient} enter to global chat";
            SendMessageToEachClient(message);


            while (!end)
            {
                try
                {
                    string dataReceived = _handleClient.GetMessageFromClient(clientSocket);


                    if (dataReceived == "0")
                    {
                        _data.RemoveClientFromGlobalChat(guidClient);

                         message = $"{guidClient} exist the global chat";
                        SendMessageToEachClient(message);

                        _handleClient.SendMessageToClient(clientSocket, "0");
                        Console.WriteLine("client send 0");
                        end = true;
                    }
                    else
                    {
                        Console.WriteLine("Received and Sending back: " + dataReceived);
                         message = $"{guidClient} send: {dataReceived}";
                        SendMessageToEachClient(message);
                    }

                }
                catch (Exception)
                {
                    end = true;
                    RemoveClientWhenOut();
                }
            }

            Console.WriteLine("client out thread");

        }
        private void SendMessageToEachClient(string message)
        {
            foreach (var client in _data.ClientsInGlobalChat)
            {
                if (client.Value.Connected)
                {
                    _handleClient.SendMessageToClient(client.Value, message);
                }
            }
        }

        private void RemoveClientWhenOut()
        {
            clientSocket.Close();
            var guid = _data.GetClientGuid(clientSocket);
            TcpClient clientExist;
            _data.ClientsInGlobalChat.TryRemove(guid, out clientExist);
            _data.ClientsConnected.TryRemove(guid, out clientExist);
        }
    }
}
