using Common;
using ServerNetworkConversation.Options.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerNetworkConversation.Options
{
    public class CreateGroupChat : IClientOption
    {
        TcpClient _client;
        Data _data;
        HandleClient _handleClient;
        Thread ctThread;

        public CreateGroupChat(Data data, TcpClient inClientSocket, HandleClient handleClient)
        {
            _client = inClientSocket;
            _data = data;
            _handleClient = handleClient;
        }
        public Thread Run()
        {
            ctThread = new Thread(DoChat);
            ctThread.Start();
            return ctThread;
        }
        public void DoChat()
        {
            Participants participants = new Participants(_data.ClientsConnected.Keys.ToList());
            _handleClient.SendToClient(_client,participants);

            GroupChat groupChat = (GroupChat)_handleClient.GetFromClient(_client);
            _data.allGroupsChat.groupsChat.Add(groupChat);
        }
    }
}
