using System;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;


namespace Domain.Models
{
    public class UserRegistrationModel
    {
        [Required(ErrorMessage="Введите пароль")]
        [StringLength(100, ErrorMessage = "{0} должен быть длиннее {2} символов.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }
 
        [DataType(DataType.Password)]
        [Display(Name = "Подтвердите пароль")]
        [Compare("Password", ErrorMessage = "Введенные пароли не совпадают")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "E-mail не введен")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                            @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                            @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
                            ErrorMessage = "Введите корректный E-mail")]
        [DataType(DataType.EmailAddress)]
        [Display(Name="E-mail")]
        public string UserName {get; set;}

        [Required(ErrorMessage = "Введите ваше имя")]
        [Display(Name = "Ваше имя")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Введите вашу фамилию")]
        [Display(Name = "Ваша фамилия")]
        public string LastName { get; set; }
    }
}
