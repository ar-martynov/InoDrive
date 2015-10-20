using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models.Tours
{
    public class BaseTourModel
    {
        public int Id { get; set; }
        [Required]
        public int FreeSlots { get; set; }
        public decimal? Payment { get; set; }
        [Required]
        public string ExpirationDate { get; set; }
    }
}
