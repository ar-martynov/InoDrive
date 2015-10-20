using System;
using System.Collections.Generic;
using Domain.Models.Tours;

namespace Domain.Models
{
    public class NewBidNotifyModel
    {
        public string UserIdentityId { get; set; }
        public string TourName { get; set; }
    }
}
