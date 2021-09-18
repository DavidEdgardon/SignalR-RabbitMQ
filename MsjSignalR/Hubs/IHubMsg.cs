using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MsjSignalR.Hubs
{
    public interface IHubMsg
    {
        Task MessageUpdated(string message);
    }
}
