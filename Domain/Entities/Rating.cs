using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Rating
    {
        public int RatingId { get; set; }
        public string UserId { get; set; }
        public int TourId { get; set; }
        public int Vote { get; set; }

        public virtual User User { get; set; }
        public virtual Tour Tour { get; set; }
    }
}
