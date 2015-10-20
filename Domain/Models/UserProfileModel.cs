using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class UserProfileModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PublicEmail { get; set; }
        public string Phone { get; set; }
        public int Age { get; set; }
        public string AvatarPath { get; set; }
        public string About { get; set; }
    }
    public class UserRatedProfileModel : UserProfileModel
    {
        public int Rating { get; set; }
    }
}
