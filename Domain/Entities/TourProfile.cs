using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class TourProfile
    {
        [Key, ForeignKey("Tour")]
        public int TourId { get; set; }

        public bool IsDeviationAllowed { get; set; }
        public bool IsPetsAllowed { get; set; }
        public bool IsMusicAllowed { get; set; }
        public bool IsFoodAllowed { get; set; }
        public bool IsDrinkAllowed { get; set; }
        public bool IsSmokeAllowed { get; set; }

        public string CarDescription { get; set; }
        public string CarImage { get; set; }
        public string CarImageExtension { get; set; }
        public int Comfort { get; set; } // some gradations
        public int Baggage { get; set; } // some gradations
        public string AdditionalInfo { get; set; }
        public string OwnerExperience { get; set; }

        public virtual Tour Tour { get; set; }
    }
}
