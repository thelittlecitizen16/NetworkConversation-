using Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Models
{
    [Serializable()]
    public class MessageRequest
    {
        public MessageKey Key { get; set; }
        public object Value { get; set; }

        public MessageRequest(MessageKey key, object value)
        {
            Key = key;
            Value = value;
        }
    }
}
