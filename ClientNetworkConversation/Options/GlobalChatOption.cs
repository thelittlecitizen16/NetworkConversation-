using MenuBuilder.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientNetworkConversation.Options
{
    public class GlobalChatOption : IOption
    {
        public string OptionMessage => "Enter To Global Chat";

        public void Run()
        {
            Console.WriteLine("enter to chat");
        }
    }
}
