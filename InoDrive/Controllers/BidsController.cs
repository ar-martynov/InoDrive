using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using AutoMapper;

using Domain.Abstract;
using Domain.Abstract.EntityRepositories;
using Domain.Entities;
using Domain.Models;
using Domain.Exceptions;
using Domain.Concrete.Repositories;

using WebUI.Infrastructure;
using WebUI.Hubs;

namespace WebUI.Controllers
{

    [ApplicationExceptionFilterAttribute]
    [RoutePrefix("Api/Bids")]
    public class BidsController : BaseApiControllerWithHub<BidsHub>
    {
        public BidsController(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        // GET api/Bids/AddBid/ :id
        [HttpGet]
        [ApplicationAuthorizeAttribute]
        [Route("AddBid")]
        public IHttpActionResult AddBid(int id)
        {
            var bids = _UOW.GetRepository<IBidRepository>();
            var tours = _UOW.GetRepository<ITourRepository>();
            string uid = GetUserId();
            var user = ValidateUser(uid);
            var tour = ValidateTourBid(tours.Get(id), uid);
            ValidateTourExpiration(tour);

            var ownerTour = Mapper.Map<Tour, NewBidNotifyModel>(bids.AddBid(tour, uid));
            Hub.Clients.User(ownerTour.UserIdentityId).newBid(ownerTour.TourName);

            _UOW.Commit();
           
            return Json(new { success = true, message =  Resources.Language.BidSent});

        }

        // DELETE api/Bids/ :id
        [HttpDelete]
        [ApplicationAuthorizeAttribute]
        public IHttpActionResult DeleteBid(int id)
        {
            var bids = _UOW.GetRepository<IBidRepository>();
            var user = ValidateUser(GetUserId());
            var bid = ValidateUserBid(user, id);
            var ownerTour = Mapper.Map<Tour, NewBidNotifyModel>(bids.DeleteBid(bid));

            _UOW.Commit();
           
            Hub.Clients.User(ownerTour.UserIdentityId).rejectBid(ownerTour.TourName);
            return Json(new { success = true, message = Resources.Language.BidRejected });
        }

        // GET api/Bids/
        [HttpGet]
        [ApplicationAuthorizeAttribute]
        public IHttpActionResult GetBids(int page)
        {
            var bids = _UOW.GetRepository<IBidRepository>();

            page = (page <= 0) ? 0 : page - 1;
            int totalPages = 0;

            var user = ValidateUser(GetUserId());

            var result = Mapper.Map<IEnumerable<Bid>, List<BidModel>>(bids.GetBids(user, page, ConfigModel.PerPage, out totalPages));

           
            return Json(new { bids = result, totalPages = totalPages, page = page + 1 },
                        new JsonSerializerSettings
                        {
                            ContractResolver = new CamelCasePropertyNamesContractResolver()
                        });
        }

        // GET api/Bids/
        [HttpGet]
        [ApplicationAuthorizeAttribute]
        [Route("MyBids")]
        public IHttpActionResult GetUserBids(int page, bool showCompleted)
        {

            var bids = _UOW.GetRepository<IBidRepository>();
            page = (page <= 0) ? 0 : page - 1;
            int totalPages = 0;
            int totalBids = 0;

            var user = ValidateUser(GetUserId());

            var result = Mapper.Map<IEnumerable<Bid>, List<BidModelWithoutOwnerProfile>>(bids.GetUserBids(user, showCompleted, page, ConfigModel.PerPage, out totalPages, out totalBids));

           
            return Json(new { bids = result, totalPages = totalPages, page = page + 1, totalBids = totalBids },
                        new JsonSerializerSettings
                        {
                            ContractResolver = new CamelCasePropertyNamesContractResolver()
                        });

        }

        // PUT api/bids/ :id
        [HttpPut]
        [ApplicationAuthorizeAttribute]
        public IHttpActionResult ManageBid([FromBody]BaseBidModel model)
        {

            var bids = _UOW.GetRepository<IBidRepository>();

            var user = ValidateUser(GetUserId());
            var bid = bids.ManageBid(user, model.Id, model.TourId, (bool)model.IsAccepted);
            if (bid == null ) return BadRequest(Resources.Language.BidError);
            _UOW.Commit();

            Hub.Clients.User(bid.User.UserName).bidStatus((bool)model.IsAccepted);

            if (model.IsAccepted == true) return Json(new { message = Resources.Language.BidAccepted});
            else return Json(new { message = Resources.Language.BidRejected });

        }
    }
}
