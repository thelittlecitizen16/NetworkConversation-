using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ClientNetworkConversation
{
    public class HandleServer
    {
        public CancellationTokenSource cancellationToken;
        public void SendMessageToServer(TcpClient client, string message)
        {
            NetworkStream nwStream = client.GetStream();
            byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(message);
            nwStream.Write(bytesToSend, 0, bytesToSend.Length);
        }

        public string GetMessageFromServer(TcpClient client)
        {
            NetworkStream serverStream = client.GetStream();
            byte[] bytesToRead = new byte[client.ReceiveBufferSize];
            int bytesRead = serverStream.Read(bytesToRead, 0, client.ReceiveBufferSize);
            return Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);
        }
    }
}
