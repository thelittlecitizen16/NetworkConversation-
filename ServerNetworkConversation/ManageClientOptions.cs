using ServerNetworkConversation.Options;
using ServerNetworkConversation.Options.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerNetworkConversation
{
    public class ManageClientOptions
    {
        private ConcurrentDictionary<int, IClientOption> _clientOptions;
        private TcpClient _inClientSocket;
        private Data _data;
        private HandleClient _handleClient;
        public ManageClientOptions(Data data, TcpClient inClientSocket, HandleClient handleClient)
        {
            _inClientSocket = inClientSocket;
            _data = data;
            _handleClient = handleClient;
            _clientOptions = new ConcurrentDictionary<int, IClientOption>();
        }
        public IClientOption AddClientOptions(int choice, TcpClient inClientSocket)
        {
            switch (choice)
            {
                case 1:
                    return new GlobalChat(_data,inClientSocket, _handleClient);
                    break;
                default:
                    return null;
                    break;
            }
        }

        public void GetClientChoice()
        {
            Task.Run(() =>
             A()
            );

        }
        private void A()
        {
            string choice = "0";
            NetworkStream serverStream = _inClientSocket.GetStream();

            while (true)
            {
                if (!_inClientSocket.Connected)
                {
                    break;
                }
                else
                {
                    string dataReceived = _handleClient.GetMessageFromClient(_inClientSocket);
                    choice = dataReceived;

                    if (choice == "1")
                    {

                        _data.AddClientToGlobalChat(_data.GetClientGuid(_inClientSocket), _inClientSocket);

                        Thread t = AddClientOptions(1, _inClientSocket).Run();
                        t.Join();
                    }
                } 
            }
        }
    }
}
