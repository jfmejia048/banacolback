using Microsoft.AspNetCore.SignalR;
using PerfilacionDeCalidad.Backend.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerfilacionDeCalidad.Backend
{
    public class NotificationHub : Hub<HubHelper>
    {
        public void Send(object message)
        {
            Clients.All.BroadcastMessage(message);
        }
    }
}
