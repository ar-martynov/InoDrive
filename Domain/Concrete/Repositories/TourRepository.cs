using System;
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
    public class TourRepository : Repository<Tour>, ITourRepository
    {
        public TourRepository(InoDriveContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Creates the tour.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="newTour">The new tour.</param>
        public void CreateTour(User user, Tour newTour)
        {
            user.Tours.Add(newTour);
        }

        /// <summary>
        /// Deletes the tour.
        /// </summary>
        /// <param name="tour">The tour.</param>
        public void DeleteTour(Tour tour)
        {
             tour.IsDeleted = true;
        }

        /// <summary>
        /// Modifies the tour.
        /// </summary>
        /// <param name="originTour">The origin tour.</param>
        /// <param name="updatedTour">The updated tour.</param>
        public void ModifyTour(Tour originTour, Tour updatedTour)
        {
            _context.Entry(originTour).CurrentValues.SetValues(updatedTour);
            _context.Entry(originTour.TourProfile).CurrentValues.SetValues(updatedTour.TourProfile);

            foreach (var wayPoint in originTour.WayPoints.ToList())
                _context.WayPoints.Remove(wayPoint);
            foreach (var wayPoint in updatedTour.WayPoints)
                originTour.WayPoints.Add(wayPoint);
        }

        /// <summary>
        /// Votes the specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="tourId">The tour identifier.</param>
        /// <param name="vote">The vote.</param>
        public void Vote(User user, int tourId, int vote)
        {

            //if (vote > 5) vote = 5;
            //else if (vote < 1) vote = 1;

            //var like = user.Ratings.FirstOrDefault(x => x.TourId == tourId);

            //if (like != null) like.Vote = vote;
            //else
            
            var like = new Rating { TourId  = tourId, Vote = vote };
            user.Ratings.Add(like);
           
        }

        /// <summary>
        /// Gets the user tours.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="page">The page.</param>
        /// <param name="perPage">The per page.</param>
        /// <param name="totalPages">The total pages.</param>
        /// <returns></returns>
        public IEnumerable<Tour> GetUserTours(User user, int page, int perPage, out int totalPages)
        {
            var request = user.Tours.Where(x => !x.IsDeleted);
            totalPages = (int)Math.Ceiling((decimal)request.Count() / perPage);
            return request.OrderByDescending(x => x.CreationDate)
                .ThenByDescending(x => x.ExpirationDate)
                .ThenByDescending(x => x.FreeSlots)
                .ThenByDescending(x => x.Payment)
                .Skip(page * perPage).Take(perPage);
        }

        /// <summary>
        /// Searches the tours.
        /// </summary>
        /// <param name="tour">The tour.</param>
        /// <param name="startId">The start identifier.</param>
        /// <param name="destId">The dest identifier.</param>
        /// <param name="page">The page.</param>
        /// <param name="perPage">The per page.</param>
        /// <param name="totalPages">The total pages.</param>
        /// <returns></returns>
        public IEnumerable<Tour> SearchTours(Tour tour, int startId, int destId, int page, int perPage, out int totalPages)
        {
            var request = _context.Tours.Where(x => DbFunctions.TruncateTime(x.ExpirationDate) == DbFunctions.TruncateTime(tour.ExpirationDate.Date)
                                  && (DbFunctions.TruncateTime(x.ExpirationDate) >= DbFunctions.TruncateTime(DateTime.Now))
                                  && x.FreeSlots >= tour.FreeSlots
                                  && !x.IsDeleted
                                  && x.WayPoints.Any(t => t.CityId == startId && x.WayPoints.Any(c => c.CityId == destId && t.WayPointOrder < c.WayPointOrder)));

            totalPages = (int)Math.Ceiling((decimal)request.Count() / perPage);

            //.OrderByDescending(x=>x.Rating)
            return request.OrderBy(x => x.Payment)
                          .ThenBy(x => x.CreationDate)
                          .ThenBy(x => x.User.UserProfile.FirstName)
                          .Skip(page * perPage).Take(perPage);

        }

        /// <summary>
        /// Advanceds the search tours.
        /// </summary>
        /// <param name="tour">The tour.</param>
        /// <param name="startId">The start identifier.</param>
        /// <param name="destId">The dest identifier.</param>
        /// <param name="minPay">The minimum pay.</param>
        /// <param name="maxPay">The maximum pay.</param>
        /// <param name="page">The page.</param>
        /// <param name="perPage">The per page.</param>
        /// <param name="totalPages">The total pages.</param>
        /// <returns></returns>
        public IEnumerable<Tour> AdvancedSearchTours(Tour tour, int startId, int destId, int minPay, int maxPay, int page, int perPage, out int totalPages)
        {
            IEnumerable<City> cities = tour.WayPoints.Select(c => c.City);
            var request = _context.Tours.Where(x => DbFunctions.TruncateTime(x.ExpirationDate) == DbFunctions.TruncateTime(tour.ExpirationDate.Date)
                                  && (DbFunctions.TruncateTime(x.ExpirationDate) >= DbFunctions.TruncateTime(DateTime.Now))
                                  && x.FreeSlots >= tour.FreeSlots
                                  && !x.IsDeleted
                                  && x.Payment <= maxPay && x.Payment >= minPay 
                                  && x.TourProfile != null
                                  && x.TourProfile.Baggage == tour.TourProfile.Baggage
                                  && x.TourProfile.Comfort == tour.TourProfile.Comfort

                                  && (tour.TourProfile.IsDeviationAllowed == false
                                      || x.TourProfile.IsDeviationAllowed != false && x.TourProfile.IsDeviationAllowed == tour.TourProfile.IsDeviationAllowed)

                                  && (tour.TourProfile.IsDrinkAllowed == false
                                      || x.TourProfile.IsDrinkAllowed != false && x.TourProfile.IsDrinkAllowed == tour.TourProfile.IsDrinkAllowed)

                                  && (tour.TourProfile.IsFoodAllowed == false
                                      || x.TourProfile.IsFoodAllowed != false && x.TourProfile.IsFoodAllowed == tour.TourProfile.IsFoodAllowed)

                                  && (tour.TourProfile.IsMusicAllowed == false
                                      || x.TourProfile.IsMusicAllowed != false && x.TourProfile.IsMusicAllowed == tour.TourProfile.IsMusicAllowed)

                                  && (tour.TourProfile.IsPetsAllowed == false
                                      || x.TourProfile.IsPetsAllowed != false && x.TourProfile.IsPetsAllowed == tour.TourProfile.IsPetsAllowed)

                                  && (tour.TourProfile.IsSmokeAllowed == false
                                      || x.TourProfile.IsSmokeAllowed != false && x.TourProfile.IsSmokeAllowed == tour.TourProfile.IsSmokeAllowed)

                                  //&& x.WayPoints.Select(wp => wp.City).Any( c => cities.Contains(c))
                                  && x.WayPoints.Any(t => t.CityId == startId && x.WayPoints.Any(c => c.CityId == destId && t.WayPointOrder < c.WayPointOrder)));

            totalPages = (int)Math.Ceiling(
                         (decimal)request.Count() / perPage);

            //.OrderByDescending(x=>x.Rating)
            return request.OrderBy(x => x.Payment)
                          .ThenBy(x => x.CreationDate)
                          .ThenBy(x => x.User.UserProfile.FirstName)
                          .Skip(page * perPage).Take(perPage);
        }

        /// <summary>
        /// Gets the last tour profile.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public TourProfile GetLastTourProfile(User user)
        {

            var tour = user.Tours.SingleOrDefault(x => x.CreationDate == user.Tours.Max(y => y.CreationDate));
            if (tour == null) return new TourProfile();

            return tour.TourProfile;
        }
    }
}
