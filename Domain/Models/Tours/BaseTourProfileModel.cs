using Domain.Models;

namespace Domain.Models.Tours
{
    public class BaseTourProfileModel
    {
        public bool IsDeviationAllowed { get; set; }
        public bool IsPetsAllowed { get; set; }
        public bool IsMusicAllowed { get; set; }
        public bool IsFoodAllowed { get; set; }
        public bool IsDrinkAllowed { get; set; }
        public bool IsSmokeAllowed { get; set; }
     
        public ComfortTypes Comfort { get; set; } // some gradations
        public BaggageTypes Baggage { get; set; } // some gradations
    }
}
