using System.ComponentModel.DataAnnotations;
namespace Domain.Models
{
    public class ConfirmChangeLoginModel
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public string Email { get; set; }
    }
}
