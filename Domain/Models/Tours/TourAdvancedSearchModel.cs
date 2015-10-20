using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Domain.Models;
using Domain.Abstract;

namespace Domain.Models.Tours
{
    public class TourAdvancedSearchModel : TourSearchModel,IPagingModel
    {
        public List<CityModel> WayPoints { get; set; }
        public TourProfileSearchModel AdvancedSearchProfile { get; set; }
    }
}
