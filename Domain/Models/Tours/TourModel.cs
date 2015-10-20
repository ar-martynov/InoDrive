using System;
using System.Globalization;
using System.Collections.Generic;

namespace Domain.Models.Tours
{
    public class TourModel:BaseTourModel
    {
        public List<CityModel> WayPoints { get; set; }
        public TourProfileModel TourProfile { get; set; }
        
    }
}
