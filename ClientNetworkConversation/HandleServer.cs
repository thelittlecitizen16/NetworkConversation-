using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
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
        public void SendToServer(TcpClient client, object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            byte[] bytesToSend;
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                bytesToSend = ms.ToArray();
            }

            NetworkStream nwStream = client.GetStream();
            nwStream.Write(bytesToSend, 0, bytesToSend.Length);
        }
        public Object GetFromServer(TcpClient client)
        {
            NetworkStream serverStream = client.GetStream();
            byte[] bytesToRead = new byte[client.ReceiveBufferSize];
            int bytesRead = serverStream.Read(bytesToRead, 0, client.ReceiveBufferSize);

            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(bytesToRead, 0, client.ReceiveBufferSize);
                memStream.Seek(0, SeekOrigin.Begin);
               return  binForm.Deserialize(memStream);   
            }
        }
    }
}
