using ClientNetworkConversation.HandleRequest.MessageOption;
using Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientNetworkConversation.HandleRequest
{
    public static class MessageRequestFactory
    {
        public static IMessageOption GetMessageOptionByKey(MessageKey messageKey)
        {
            switch (messageKey)
            {
                case MessageKey.STRING:
                    return new StringMessage();
                    break;
                case MessageKey.ALERT:
                    return new AlertMessage();
                    break;
                default:
                    return null;
                    break;
            }
        }
    }
}
