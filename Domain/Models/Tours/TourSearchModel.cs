using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Domain.Models;
using Domain.Abstract;

namespace Domain.Models.Tours
{
    public class TourSearchModel : IPagingModel
    {
        [Required]
        public CityModel StartCity { get; set; }
        [Required]
        public CityModel DestCity { get; set; }
        [Required]
        public string ExpirationDate { get; set; }
        [Required]
        public int FreeSlots { get; set; }
        public int Page { get; set; }
    }
}
