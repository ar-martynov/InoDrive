using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class Bid
    {
        public int BidId { get; set; }
        public string UserId { get; set; }
        public int TourId { get; set; }
        public DateTime CreationDate { get; set; }
        public bool? IsAccepted { get; set; }
        public bool IsWatchedByOwner { get; set; }
        public bool IsWatchedByUser { get; set; }
        

        public virtual User User { get; set; }
        public virtual Tour Tour { get; set; }
    }
}
