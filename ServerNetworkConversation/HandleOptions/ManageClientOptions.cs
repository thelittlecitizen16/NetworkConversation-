using ServerNetworkConversation.Options;
using ServerNetworkConversation.Options.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Enums;
using ServerNetworkConversation.HandleData;
using ServerNetworkConversation.Options.HandleOptions;

namespace ServerNetworkConversation.HandleOptions
{
    public class ManageClientOptions
    {
        private TcpClient _clientSocket;
        private Data _data;
        private HandleClient _handleClient;
        private  RemoveClient _removeClient;
        private ClientOptionsFactory _clientOptionsFactory;
        public ManageClientOptions(Data data, TcpClient inClientSocket, HandleClient handleClient, RemoveClient removeClient, ClientOptionsFactory clientOptionsFactory)
        {
            _clientSocket = inClientSocket;
            _data = data;
            _handleClient = handleClient;
            _removeClient = removeClient;
            _clientOptionsFactory = clientOptionsFactory;
        }

        public void Run()
        {
            Task.Run(() =>
             GetClientChoice()
            );

        }
        private void GetClientChoice()
        {
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
                                _data.ClientsInGlobalChat.Add(_data.ClientsConnectedInServer.GetGuid(_clientSocket), _clientSocket);
                                Thread globalChat = _clientOptionsFactory.AddClientOptions(ClientOptions.GLOBAL_CHAT,_data, _clientSocket,_handleClient, _removeClient).Run();
                                globalChat.Join();
                                break;

                            case ClientOptions.PRIVATE_CHAT:
                                Thread privateChat = _clientOptionsFactory.AddClientOptions(ClientOptions.PRIVATE_CHAT, _data, _clientSocket, _handleClient, _removeClient).Run();
                                privateChat.Join();
                                break;

                            case ClientOptions.CREATE_GROUP_CHAT:
                                Thread CreateGroupChat = _clientOptionsFactory.AddClientOptions(ClientOptions.CREATE_GROUP_CHAT, _data, _clientSocket, _handleClient, _removeClient).Run();
                                CreateGroupChat.Join();
                                break;
                            case ClientOptions.GROUP_CHAT:
                                Thread EnterGroupChat = _clientOptionsFactory.AddClientOptions(ClientOptions.GROUP_CHAT, _data, _clientSocket, _handleClient, _removeClient).Run();
                                EnterGroupChat.Join();
                                break;
                            case ClientOptions.MANAGER_SETTINGS:
                                Thread managerSettings = _clientOptionsFactory.AddClientOptions(ClientOptions.MANAGER_SETTINGS, _data, _clientSocket, _handleClient, _removeClient).Run();
                                managerSettings.Join();
                                break;
                            case ClientOptions.LEAVE_GROUP_CHAT:
                                Thread leavrGroupChat = _clientOptionsFactory.AddClientOptions(ClientOptions.LEAVE_GROUP_CHAT, _data, _clientSocket, _handleClient, _removeClient).Run();
                                leavrGroupChat.Join();
                                break;
                            default:
                                break;
                        }
                    }
                } 
            }
        }
    }
}
