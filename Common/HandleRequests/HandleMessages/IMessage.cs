using System;
using System.Collections.Generic;
using System.Text;

namespace Common.HandleRequests.HandleMessages
{
    public interface IMessage
    {
        object  GetMessage(object message, object messageModalType);
    }
}
