using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Common.HandleRequests
{
    public interface IRequests
    {
         void SendStringMessage(TcpClient client, string message);
        string GetStringMessage(TcpClient client);
        void SendModelMessage(TcpClient client, object obj);
        Object GetModelMessage(TcpClient client);
        void SendPictureMessage(TcpClient client, string url);
        void GetPictureMessage(TcpClient client);
    }
}
