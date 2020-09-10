using Common.HandleRequests;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Models
{
    [Serializable()]
    public class AllGroupChat
    {
        public List<string> GroupsName { get; set; }
        public AllGroupChat(List<string> groupsName)
        {
            GroupsName = groupsName;
        }
        public AllGroupChat()
        {

        }
    }
}
