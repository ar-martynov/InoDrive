using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

using Domain.Abstract;


namespace WebUI.Controllers
{
    public abstract class BaseApiControllerWithHub<THub> : BaseApiController
        where THub : IHub
    {
        public BaseApiControllerWithHub(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        protected readonly Lazy<IHubContext> _hub = new Lazy<IHubContext>(
            () => GlobalHost.ConnectionManager.GetHubContext<THub>()
        );

        protected IHubContext Hub
        {
            get { return _hub.Value; }
        }
    }
}