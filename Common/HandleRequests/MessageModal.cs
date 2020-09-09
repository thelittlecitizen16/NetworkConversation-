using Common.Enums;
using System;

namespace Common.HandleRequests
{
    [Serializable()]
    public class MessageModal
    { 
        public MessageTypes MessageTypes { get; private set; }
        //public object Message { get; private set; }
        //public object MessageModalType { get; private set; }
        //public MessageModal(MessageTypes messageTypes, object message, object messageModalType)
        //{
        //    MessageTypes = messageTypes;
        //    Message = message;
        //    MessageModalType = messageModalType;
        //}
    }
}
