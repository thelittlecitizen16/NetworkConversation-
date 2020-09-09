using Common;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ServerNetworkConversation.HandleData
{
    public class AllGroupsChat
    {
        public List<GroupChat> GroupsChat { get; private set; }
        public Dictionary<GroupChat, List<TcpClient>> ClientConnectToGroup { get; private set; }

        public AllGroupsChat()
        {
            GroupsChat = new List<GroupChat>();
            ClientConnectToGroup = new Dictionary<GroupChat, List<TcpClient>>();
        }
        public void AddClientConnected(GroupChat group, TcpClient client)
        {
            List<TcpClient> connectedUsers;
            if (ClientConnectToGroup.TryGetValue(group, out connectedUsers))
            {
                connectedUsers.Add(client);
            }
            else
            {
                ClientConnectToGroup.Add(group, new List<TcpClient>() { client });
            }
        }
        public void RemoveClientUnConnected(GroupChat group, TcpClient user)
        {
            if (group!=null)
            {
                ClientConnectToGroup[group].Remove(user);
            }
            
        }
    }
}
