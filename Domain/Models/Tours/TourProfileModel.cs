
using Domain.Models;

namespace Domain.Models.Tours
{
    public class TourProfileModel:BaseTourProfileModel
    {
        public string CarDescription { get; set; }
        public string CarImage { get; set; }
        public string CarImageExtension { get; set; }
        public string AdditionalInfo { get; set; }
        public string OwnerExperience { get; set; }
    }
}
