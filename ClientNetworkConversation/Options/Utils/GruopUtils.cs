using Common.Models;
using MenuBuilder.Interfaces;
using System.Collections.Generic;

namespace ClientNetworkConversation.Options.Utils
{
    public static class GruopUtils
    {
        public static bool CheckGroupName(string userResponse, AllGroupChat allGroupChat)
        {
            return allGroupChat.GroupsName.Contains(userResponse);
        }
        public static void PrintString(List<string> allStrings, ISystem system)
        {
            foreach (var participant in allStrings)
            {
                system.Write(participant.ToString());
            }
        }
    }
}
