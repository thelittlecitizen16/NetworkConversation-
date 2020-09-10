using Common.HandleRequests;
using MenuBuilder.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ClientNetworkConversation.Options.Utils
{
    public static class Utils
    {
        public static void GetMessage(IRequests requests, ISystem system, TcpClient client)
        {
            string message = "";

            while (message != "0")
            {
                message = requests.GetStringMessage(client);
                system.Write(message);
            }
        }


    }
}
