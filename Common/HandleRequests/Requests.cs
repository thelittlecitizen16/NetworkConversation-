using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Common.HandleRequests
{
    public class Requests : IRequests
    {
        public void SendStringMessage(TcpClient client, string message)
        {
            byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(message);
            NetworkStream clientStream = client.GetStream();
            clientStream.Write(bytesToSend, 0, bytesToSend.Length);
        }

        public string GetStringMessage(TcpClient client)
        {
            NetworkStream nwStream = client.GetStream();
            byte[] buffer = new byte[client.ReceiveBufferSize];

            int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);

            return Encoding.ASCII.GetString(buffer, 0, bytesRead);
        }

        public void SendModelMessage(TcpClient client, object obj)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            MemoryStream memoryStream = new MemoryStream();
            binaryFormatter.Serialize(memoryStream, obj);

            byte[] bytesToSend = memoryStream.ToArray();
            NetworkStream nwStream = client.GetStream();
            nwStream.Write(bytesToSend, 0, bytesToSend.Length);
        }

        public Object GetModelMessage(TcpClient client)
        {
            NetworkStream serverStream = client.GetStream();
            byte[] bytesToRead = new byte[client.ReceiveBufferSize];
            int bytesRead = serverStream.Read(bytesToRead, 0, client.ReceiveBufferSize);

            try
            {
                using (var memStream = new MemoryStream())
                {
                    var binForm = new BinaryFormatter();
                    memStream.Write(bytesToRead, 0, client.ReceiveBufferSize);
                    memStream.Seek(0, SeekOrigin.Begin);
                    return (Object)binForm.Deserialize(memStream);
                }
            }
            catch (Exception)
            {

                return null;
            }
        }
        
    }
}
