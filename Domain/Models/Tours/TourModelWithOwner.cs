using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Tours
{
    public class TourModelWithOwner:TourModel
    {
        public UserRatedProfileModel UserProfile { get; set; }
       
        public new string Payment { get; set; }
        public bool IsOwner { get; set; }
        public bool AlreadyBet { get; set; }
        public bool IsExpired { get; set; }
    }
}
