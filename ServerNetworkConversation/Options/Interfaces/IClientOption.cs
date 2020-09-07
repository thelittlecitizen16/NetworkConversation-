using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ServerNetworkConversation.Options.Interfaces
{
    public interface IClientOption
    {
        public Thread Run();
    }
}
