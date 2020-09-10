using Common.HandleRequests;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Models
{
    [Serializable()]
    public class Participants
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<Guid> AllParticipants { get; set; }
        public Participants(List<Guid> allParticipants)
        {
            AllParticipants = allParticipants;
        }
        public Participants()
        {

        }
    }
}
