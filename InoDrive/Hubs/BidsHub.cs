using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

using WebUI.Infrastructure;

namespace WebUI.Hubs
{
    [Authorize]
    [HubName("Bids")]
    public class BidsHub : Hub
    {
        private readonly static HubConnectionsStorage<string> _connections =
            new HubConnectionsStorage<string>();

        //Web sockets doesn't support custom http headers :'(    
        public override Task OnConnected()
        {
            if (!_connections.GetConnections(Context.User.Identity.Name).Contains(Context.ConnectionId))
            {
                _connections.Add(Context.User.Identity.Name, Context.ConnectionId);
            }

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            _connections.Remove(Context.User.Identity.Name, Context.ConnectionId);

            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {

            if (!_connections.GetConnections(Context.User.Identity.Name).Contains(Context.ConnectionId))
            {
                _connections.Add(Context.User.Identity.Name, Context.ConnectionId);
            }

            return base.OnReconnected();
        }
    }
}