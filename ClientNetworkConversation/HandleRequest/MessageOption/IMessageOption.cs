using MenuBuilder.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientNetworkConversation.HandleRequest.MessageOption
{
    public interface IMessageOption
    {
        public void HandleMessage(ISystem system, object message);
    }
}
