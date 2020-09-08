using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public class Participants
    {
        public List<Guid> AllParticipants { get; set; }
        public Participants(List<Guid> allParticipants)
        {
            AllParticipants = allParticipants;
        }
    }
}
