using System.Collections.Generic;
using Domain.Entities;

namespace Domain.Abstract.EntityRepositories
{
    public interface IBidRepository:IRepository<Bid>
    {
        Tour AddBid(Tour tour, string userId);
        Tour DeleteBid(Bid bid);
        IEnumerable<Bid> GetBids(User user, int page, int perPage, out int totalPages);
        IEnumerable<Bid> GetUserBids(User user, bool showCompleted, int page, int perPage, out int totalPages, out int totalBids);
        Bid ManageBid(User user, int bidId, int tourId, bool isAccept);
        int GetCountOfUserBids(User user);
        int GetCountOfUserToursBids(User user);
    }
}
