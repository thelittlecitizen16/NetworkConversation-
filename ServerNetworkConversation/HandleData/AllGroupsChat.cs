using Common;
using Common.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace ServerNetworkConversation.HandleData
{
    public class AllGroupsChat
    {
        private readonly object _locker = new object();

        private List<GroupChat> _groupsChat;
        public ConcurrentDictionary<GroupChat, List<TcpClient>> ClientConnectToGroup { get; private set; }
        private ConcurrentDictionary<GroupChat, List<string>> _messagesHistory;

        public AllGroupsChat()
        {
            _groupsChat = new List<GroupChat>();
            ClientConnectToGroup = new ConcurrentDictionary<GroupChat, List<TcpClient>>();
            _messagesHistory = new ConcurrentDictionary<GroupChat, List<string>>();
        }
        public bool IsClientInAnyGroupChat(TcpClient client)
        {
            foreach (var clients in ClientConnectToGroup)
            {
                if(clients.Value.Where(c=>c ==client).Any())
                {
                    return true;
                }
            }

            return false;
        }
        public void AddMessageToHistory(GroupChat groupChat,string message)
        {
            if (_messagesHistory.ContainsKey(groupChat))
            {
                _messagesHistory[groupChat].Add(message);
            }
            else
            {
                _messagesHistory.TryAdd(groupChat, new List<string>() { message });
            }
            
        }
        public List<string> GetAllGroupHistory(GroupChat groupChat)
        {
            if (_messagesHistory.ContainsKey(groupChat))
            {
                return _messagesHistory[groupChat];
            }
            return new List<string>();
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
