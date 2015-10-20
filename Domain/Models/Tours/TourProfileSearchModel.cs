using System.Collections.Generic;

namespace Domain.Models.Tours
{
    public class TourProfileSearchModel:BaseTourProfileModel
    {
        public int PaymentLowerBound { get; set; }
        public int PaymentUpperBound { get; set; }
    }
}
