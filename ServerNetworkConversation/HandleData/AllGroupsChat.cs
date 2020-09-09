using Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ServerNetworkConversation.HandleData
{
    public class AllGroupsChat
    {
        private readonly object _locker = new object();

        private List<GroupChat> _groupsChat;
        public ConcurrentDictionary<GroupChat, List<TcpClient>> ClientConnectToGroup { get; private set; }

        public AllGroupsChat()
        {
            _groupsChat = new List<GroupChat>();
            ClientConnectToGroup = new ConcurrentDictionary<GroupChat, List<TcpClient>>();
        }
        public List<GroupChat> GetGroupsChat()
        {
            lock (_locker)
            {
                return _groupsChat;
            }
        }
        public void AddGroupChat(GroupChat groupChat)
        {
            lock (_locker)
            {
                _groupsChat.Add(groupChat);
            }
        }
        public void RemoveGroupChat(GroupChat groupChat)
        {
            lock (_locker)
            {
                _groupsChat.Remove(groupChat);
            }
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
                ClientConnectToGroup.TryAdd(group, new List<TcpClient>() { client });
            }
        }
        public void RemoveClientUnConnected(GroupChat group, TcpClient user)
        {
            if (group != null)
            {
                ClientConnectToGroup[group].Remove(user);
            }

        }
    }
}
