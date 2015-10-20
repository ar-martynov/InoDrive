using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class WayPoint
    {
        [Key]
        public int WayPointId { get; set; }
        [ForeignKey("Tour")]
        public int TourId { get; set; }
        [ForeignKey("City")]
        public int CityId { get; set; }
        public int WayPointOrder { get; set; }
        public bool IsStart { get; set; }
        public bool IsDestination { get; set; }
        public virtual Tour Tour {get; set;}
        public virtual City City { get; set; }
    }
}
