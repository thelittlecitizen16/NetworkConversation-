using Common.Enums;
using Common.HandleRequests;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Models
{
    [Serializable()]
    public class GroupChat
    {
        public string Name { get; set; }
        public List<Guid> Participants{ get; set; }
        public List<Guid> Managers { get; set; }

        public GroupChat(string name, List<Guid> participants, List<Guid> managers =null)
        {
            Name = name;
            Participants = participants;
            Managers = managers;
        }
         public GroupChat()
        {

        }
    }
}
