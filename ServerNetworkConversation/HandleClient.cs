using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Linq;
namespace ServerNetworkConversation
{
    public class HandleClient
    {
        TcpClient clientSocket;
        ConcurrentDictionary<Guid, TcpClient> ClientsList;
        Thread ctThread;

        public void StartClient(TcpClient inClientSocket, ConcurrentDictionary<Guid, TcpClient> clientsList)
        {
            clientSocket = inClientSocket;
            ClientsList = clientsList;

            ctThread = new Thread(DoChat);
            ctThread.Start();
        }

        private void DoChat()
        {
            bool end = false;

            CheckIfStillContect();
            var guidClient = ClientsList.Where(c => c.Value == clientSocket).Select(c => c.Key).First();

            foreach (var client in ClientsList)
            {
                byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes($"{guidClient} enter to global chat");
                NetworkStream clientStream = client.Value.GetStream();
                clientStream.Write(bytesToSend, 0, bytesToSend.Length);
            }

            while (!end)
            {
                try
                {
                    NetworkStream nwStream = clientSocket.GetStream();
                    byte[] buffer = new byte[clientSocket.ReceiveBufferSize];

                    int bytesRead = nwStream.Read(buffer, 0, clientSocket.ReceiveBufferSize);

                    string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                    if (dataReceived == "0")
                    {
                        TcpClient clientExist;
                        ClientsList.TryRemove(guidClient, out clientExist);
                        clientSocket.Close();
                        foreach (var client in ClientsList)
                        {
                            byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes($"{guidClient} exist the global chat");
                            NetworkStream clientStream = client.Value.GetStream();
                            clientStream.Write(bytesToSend, 0, bytesToSend.Length);
                        }

                        ctThread.Abort();
                        end = true;
                    }
                    else
                    {
                        Console.WriteLine("Received and Sending back: " + dataReceived);

                        foreach (var client in ClientsList)
                        {
                            byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes($"{guidClient} send: {dataReceived}");
                            NetworkStream clientStream = client.Value.GetStream();
                            clientStream.Write(bytesToSend, 0, bytesToSend.Length);
                        }
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

        }
        private void CheckIfStillContect()
        {
            foreach (var client in ClientsList)
            {
                if (!client.Value.Connected)
                {
                    TcpClient clientExist;
                    ClientsList.TryRemove(client.Key, out clientExist);
                }
            }
        }
    }
}
