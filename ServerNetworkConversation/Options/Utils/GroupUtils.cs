using Common.HandleRequests;
using Common.Models;
using ServerNetworkConversation.HandleData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace ServerNetworkConversation.Options.Utils
{
    public static class GroupUtils
    {

        public static void SendAllClientGroups(TcpClient client, IRequests requests, Data data, Guid clientGuid)
        {
            List<string> grouspName = data.AllGroupsChat.GetGroupsChat()
               .Where(g => g.Participants.Contains(clientGuid))
               .Select(g => g.Name).ToList();

            SendAllGroupChat(client, requests, grouspName);
        }
        public static void SendAllGroupChat(TcpClient client, IRequests requests, List<string> grouspName)
        {
            AllGroupChat allGroupChat = new AllGroupChat(grouspName);
            requests.SendModelMessage(client, allGroupChat);
        }

        public static GroupChat WaitToGetGroupFromClient(TcpClient client, IRequests requests)
        {
            GroupChat groupChat = (GroupChat)requests.GetModelMessage(client);

            while (groupChat == null)
            {
                groupChat = (GroupChat)requests.GetModelMessage(client);
            }

            return groupChat;
        }
    }
}
