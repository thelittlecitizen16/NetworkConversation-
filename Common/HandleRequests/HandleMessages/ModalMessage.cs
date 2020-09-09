using System;
using System.Collections.Generic;
using System.Text;

namespace Common.HandleRequests.HandleMessages
{
    public  class ModalMessage : IMessage
    {
        public object GetMessage(object message, object messageModalType)
        {
            return Convert.ChangeType(message, messageModalType.GetType());
        }
    }
}
