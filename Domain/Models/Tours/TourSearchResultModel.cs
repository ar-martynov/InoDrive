using System;

namespace Domain.Models.Tours
{
    public class TourSearchResultModel
    {
        public int Id { get; set; }
        public string Owner { get; set; }
        public string StartCity { get; set; }
        public string DestCity { get; set; }
        public string ExpirationDate { get; set; }
        public bool IsExpired { get; set; }
        public int FreeSlots { get; set; }
        public string Payment { get; set; }

    }
}
