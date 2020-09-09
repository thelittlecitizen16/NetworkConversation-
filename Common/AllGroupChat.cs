using System;
using System.Collections.Generic;
using System.Text;

namespace Common
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
