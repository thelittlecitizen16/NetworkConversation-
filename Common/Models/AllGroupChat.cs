using System;
using System.Collections.Generic;

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
