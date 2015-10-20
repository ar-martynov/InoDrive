using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Linq;
using System.Threading;
using Microsoft.AspNet.Identity;


using Domain.Abstract;
using Domain.Abstract.EntityRepositories;
using Domain.Entities;
using Domain.Exceptions;

namespace WebUI.Controllers
{
    public class BaseApiController : ApiController
    {
        protected IUnitOfWork _UOW;
        public BaseApiController(IUnitOfWork uow)
        {
            this._UOW = uow;
        }


        /// <summary>
        /// Gets the user identifier.
        /// </summary>
        /// <returns></returns>
        protected string GetUserId()
        {
            var identity = (System.Security.Claims.ClaimsIdentity)User.Identity;
            return identity.FindFirst("Id").Value;
        }

        /// <summary>
        /// Gets the error result.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        protected IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("message", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        protected User ValidateUser(string userId)
        {
           var repository = _UOW.GetRepository<IUserRepository>();
           var user = repository.AppUserManager.FindById(userId);

           if (user == null) throw new CustomAppException(Resources.Language.UserNotExist);
           return user;
        }
        protected Tour ValidateTour(int tourId)
        {
            var repository = _UOW.GetRepository<ITourRepository>();
            var tour = repository.Get(tourId);

            if (tour == null) throw new CustomAppException(Resources.Language.TourNotExist);
            return tour;
        }

        protected Tour ValidateUserTour(User user, int tourId)
        {
            var tour = user.Tours.FirstOrDefault(x => x.TourId == tourId);
            if (tour == null) throw new CustomAppException(Resources.Language.BidNotExist);
            return tour;
        }

        protected Tour ValidateTourBid(Tour tour, string userId)
        {
            if (tour.Bids.FirstOrDefault(x => x.UserId == userId) != null) throw new CustomAppException(Resources.Language.SelfBidError);
            return tour;
        }
        protected void ValidateTourExpiration(Tour tour)
        {
            if (DateTime.Now > tour.ExpirationDate) throw new CustomAppException(Resources.Language.TourExpiredError);
        }
        protected bool IsTourExpired(Tour tour)
        {
            if (DateTime.Now > tour.ExpirationDate) return true;
            return false;
        }

        protected Bid ValidateUserBid(User user, int bidId)
        {
            var bid = user.Bids.FirstOrDefault(x => x.BidId == bidId);
            if (bid == null || bid.Tour == null) throw new CustomAppException(Resources.Language.BidNotExist);
            return bid;
        }


        /// <summary>
        /// Releases the unmanaged resources that are used by the object and, optionally, releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            _UOW.Dispose();
            base.Dispose(disposing);
        }
    }
}
