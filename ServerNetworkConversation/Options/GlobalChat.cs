﻿using System;
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

            //CheckIfStillContect();
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
                catch (SocketException)
                {
                    clientSocket.Close();
                }
                catch (ObjectDisposedException)
                {
                    clientSocket.Close();
                }
                catch (Exception)
                {
                    clientSocket.Close();
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

        private void CheckIfStillContect()
        {
            foreach (var client in _data.ClientsInGlobalChat)
            {
                if (!client.Value.Connected)
                {
                    TcpClient clientExist;
                    _data.ClientsInGlobalChat.TryRemove(client.Key, out clientExist);
                }
            }
        }
    }
}
