using System;
using System.Data.Objects;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity;

using Domain.Contexts;
using Domain.Models;
using Domain.Entities;
using Domain.Abstract;
using Domain.Abstract.EntityRepositories;

namespace Domain.Concrete.Repositories
{
    public class BidRepository : Repository<Bid>, IBidRepository
    {
        public BidRepository(InoDriveContext context)
            : base(context) { }


        /// <summary>
        /// Adds the bid.
        /// </summary>
        /// <param name="tour">The tour.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public Tour AddBid(Tour tour, string userId)
        {
            tour.Bids.Add(new Bid { UserId = userId, CreationDate = DateTime.Now });

            return tour;
        }

        /// <summary>
        /// Deletes the bid.
        /// </summary>
        /// <param name="bid">The bid.</param>
        /// <returns></returns>
        public Tour DeleteBid(Bid bid)
        {
            var tour = bid.Tour;

            if (bid.IsAccepted == true && !tour.IsDeleted) bid.Tour.FreeSlots += 1;
            
            _context.Bids.Remove(bid);
            return tour;
        }

        /// <summary>
        /// Gets the bids.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="page">The page.</param>
        /// <param name="perPage">The per page.</param>
        /// <param name="totalPages">The total pages.</param>
        /// <returns></returns>
        public IEnumerable<Bid> GetBids(User user, int page, int perPage, out int totalPages)
        {

            var request = user.Tours.Where(x => x.ExpirationDate >= DateTime.Now).SelectMany(x => x.Bids);

            totalPages = (int)Math.Ceiling((decimal)request.Count() / perPage);

            return request.OrderByDescending(x => x.CreationDate)
                          .Skip(page * perPage).Take(perPage);
        }

        /// <summary>
        /// Gets the user bids.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="showCompleted">if set to <c>true</c> [show completed].</param>
        /// <param name="page">The page.</param>
        /// <param name="perPage">The per page.</param>
        /// <param name="totalPages">The total pages.</param>
        /// <param name="totalBids">The total bids.</param>
        /// <returns></returns>
        public IEnumerable<Bid> GetUserBids(User user, bool showCompleted, int page, int perPage, out int totalPages, out int totalBids)
        {
            var request = showCompleted ? user.Bids : user.Bids.Where(x => x.Tour.ExpirationDate >= DateTime.Now);

            totalBids = request.Count();
            totalPages = (int)Math.Ceiling((decimal)totalBids / perPage);

            return request.OrderByDescending(x => x.CreationDate)
                          .Skip(page * perPage).Take(perPage);
        }

        /// <summary>
        /// Manages the bid.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="bidId">The bid identifier.</param>
        /// <param name="tourId">The tour identifier.</param>
        /// <param name="isAccept">if set to <c>true</c> [is accept].</param>
        /// <returns></returns>
        public Bid ManageBid(User user, int bidId, int tourId, bool isAccept)
        {

            var tour = user.Tours.FirstOrDefault(x => x.TourId == tourId);
            if (tour == null || tour.IsDeleted) return null;

            int bidsCount = tour.Bids.Where(x => x.IsAccepted == true).Count();
            int slotsCount = tour.TotalSlots;

            if ((slotsCount - bidsCount) <= 0 && isAccept) return null;

            var bid = tour.Bids.FirstOrDefault(x => x.BidId == bidId);
            if (bid == null) return null;

            if (!isAccept) tour.FreeSlots += 1;
            else tour.FreeSlots -= 1;

            bid.IsWatchedByOwner = true;
            bid.IsAccepted = isAccept;

            return bid;
        }

        /// <summary>
        /// Gets the count of user bids.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public int GetCountOfUserBids(User user)
        {
            //var user = ValidateUser(userId);

            return user.Bids.Count(x => x.IsAccepted != null && !x.IsWatchedByOwner);
        }

        /// <summary>
        /// Gets the count of user tours bids.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public int GetCountOfUserToursBids(User user)
        {
            //var user = ValidateUser(userId);

            return user.Tours.SelectMany(x => x.Bids).Count(x => x.IsAccepted == null);
        }
    }
}
