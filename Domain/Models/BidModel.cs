using System;
using System.Collections.Generic;
using Domain.Models.Tours;

namespace Domain.Models
{
    public class BaseBidModel
    {
        public int Id { get; set; }
        public int TourId { get; set; }
        public bool IsTourCompleted { get; set; }
        public bool? IsAccepted { get; set; }
        public bool IsWatchedByOwner { get; set; }

    }
    public class BidModelWithoutOwnerProfile : BaseBidModel
    {
        public TourSearchResultModel Tour { get; set; }
    }
    public class BidModel : BaseBidModel
    {
        public TourSearchResultModel Tour { get; set; }
        public UserProfileModel User { get; set; }

    }
}
