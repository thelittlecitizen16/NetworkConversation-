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
        private ConcurrentDictionary<Guid, TcpClient> _clientsList;
        public ManageClientOptions(TcpClient inClientSocket, ConcurrentDictionary<Guid, TcpClient> clientsList)
        {
            _inClientSocket = inClientSocket;
            _clientsList = clientsList;
            _clientOptions = new ConcurrentDictionary<int, IClientOption>();
        }
        public IClientOption AddClientOptions(int choice, TcpClient inClientSocket, ConcurrentDictionary<Guid, TcpClient> clientsList)
        {
            switch (choice)
            {
                case 1:
                    return new GlobalChat(inClientSocket, clientsList);
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

            while (true)
            {

                NetworkStream serverStream = _inClientSocket.GetStream();
                byte[] bytesToRead = new byte[_inClientSocket.ReceiveBufferSize];
                int bytesRead = serverStream.Read(bytesToRead, 0, _inClientSocket.ReceiveBufferSize);
                string dataReceived = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);
                choice = dataReceived;

                if (choice == "1")
                {
                    Task t = Task.Run(() => AddClientOptions(1, _inClientSocket, _clientsList).Run());
                    Task.WaitAll(t);
                }
                // Thread t = AddClientOptions(1, _inClientSocket, _clientsList).Run();
            }
        }
    }
}
