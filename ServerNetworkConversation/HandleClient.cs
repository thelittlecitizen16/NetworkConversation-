using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ServerNetworkConversation
{
    public class HandleClient
    {
        public void SendMessageToClient(TcpClient client, string message)
        {
            byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(message);
            NetworkStream clientStream = client.GetStream();
            clientStream.Write(bytesToSend, 0, bytesToSend.Length);
        }

        public string GetMessageFromClient(TcpClient client)
        {
            NetworkStream nwStream = client.GetStream();
            byte[] buffer = new byte[client.ReceiveBufferSize];

            int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);

            return Encoding.ASCII.GetString(buffer, 0, bytesRead);
        }


    }
}
