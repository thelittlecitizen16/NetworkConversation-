﻿using ServerNetworkConversation.Options;
using ServerNetworkConversation.Options.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Enums;
namespace ServerNetworkConversation.HandleOptions
{
    public class ManageClientOptions
    {
        private TcpClient _clientSocket;
        private Data _data;
        private HandleClient _handleClient;
        private ClientOptionsFactory _clientOptionsFactory;
        public ManageClientOptions(Data data, TcpClient inClientSocket, HandleClient handleClient, ClientOptionsFactory clientOptionsFactory)
        {
            _clientSocket = inClientSocket;
            _data = data;
            _handleClient = handleClient;
            _clientOptionsFactory = clientOptionsFactory;
        }
        //public IClientOption AddClientOptions(int choice, TcpClient inClientSocket)
        //{
        //    switch (choice)
        //    {
        //        case 0:
        //            return new GlobalChat(_data, inClientSocket, _handleClient);
        //            break;
        //        case 1:
        //            return new PrivateChat(_data, inClientSocket, _handleClient);
        //            break;
        //        default:
        //            return null;
        //            break;
        //    }
        //}

        public void GetClientChoice()
        {
            Task.Run(() =>
             A()
            );

        }
        private void A()
        {
           // string choice = "0";
            NetworkStream serverStream = _clientSocket.GetStream();

            while (true)
            {
                if (!_clientSocket.Connected)
                {
                    break;
                }
                else
                {
                    string dataReceived = _handleClient.GetMessageFromClient(_clientSocket);
                    ClientOptions choice;

                    if (Enum.TryParse(dataReceived, out choice))
                    {
                        switch (choice)
                        {
                            case ClientOptions.GLOBAL_CHAT:
                                _data.AddClientToGlobalChat(_data.GetClientGuid(_clientSocket), _clientSocket);
                                Thread globalChat = _clientOptionsFactory.AddClientOptions(ClientOptions.GLOBAL_CHAT,_data, _clientSocket,_handleClient).Run();
                                globalChat.Join();
                                break;
                            case ClientOptions.PRIVATE_CHAT:
                                Thread privateChat = _clientOptionsFactory.AddClientOptions(ClientOptions.PRIVATE_CHAT, _data, _clientSocket, _handleClient).Run();
                                privateChat.Join();
                                break;
                            default:
                                break;
                        }

                        //if (choice == "1")
                        //{

                        //    _data.AddClientToGlobalChat(_data.GetClientGuid(_clientSocket), _clientSocket);

                        //    Thread t = AddClientOptions(0, _clientSocket).Run();
                        //    t.Join();
                        //}
                        //if (choice == "2")
                        //{

                        //    Thread t = AddClientOptions(1, _clientSocket).Run();
                        //    t.Join();
                        //}
                    }
                } 
            }
        }
    }
}