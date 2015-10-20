using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class PasswordRestoreModel
    {
        [Required(ErrorMessage = "E-mail не введен")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                            @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                            @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
                            ErrorMessage = "Введите корректный E-mail")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "UserName")]
        public string UserName { get; set; }

        public string Password { get; set; }
        public string Code { get; set; }
    }
}
