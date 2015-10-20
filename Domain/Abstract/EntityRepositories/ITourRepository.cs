using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Domain.Entities;

namespace Domain.Abstract.EntityRepositories
{
    public interface ITourRepository:IRepository<Tour>
    {
        void CreateTour(User user, Tour newtour);
        void DeleteTour(Tour tour);
        void ModifyTour(Tour originTour, Tour updatedTour);
        void Vote(User user, int tourid, int vote);
       

        IEnumerable<Tour> GetUserTours(User user, int page, int perPage, out int totalPages);
        IEnumerable<Tour> SearchTours(Tour tour, int startId, int destId, int page, int perPage, out int totalPages);
        IEnumerable<Tour> AdvancedSearchTours(Tour tour, int startId, int destId, int minPay, int maxPay, int page, int perPage, out int totalPages);
        
        TourProfile GetLastTourProfile(User user);
        
    }
}
