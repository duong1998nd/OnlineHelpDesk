using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineHelpDesk.Hubs
{
    public class ChatHub : Hub 
    {
        public async Task SendMess(int UserId, string mess)
        {
            await Clients.All.SendAsync("ReceiveMess", UserId, mess);
        }
    }
}
