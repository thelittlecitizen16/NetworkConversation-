using Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerNetworkConversation.HandleData
{
    public class AllGroupsChat
    {
        public List<GroupChat> groupsChat { get; private set; }

        public AllGroupsChat()
        {
            groupsChat = new List<GroupChat>();
        }
    }
}
