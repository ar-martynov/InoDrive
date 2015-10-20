using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class UserProfile
    {
        [Key, ForeignKey("User")]
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PublicEmail { get; set; }
        public string Phone { get; set; }
        public int Age { get; set; }
        public string AvatarPath { get; set; }
        public string About { get; set; }
        public virtual User User { get; set; }

    }
}
