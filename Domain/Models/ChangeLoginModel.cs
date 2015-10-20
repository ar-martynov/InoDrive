using System;
using System.ComponentModel.DataAnnotations;


namespace Domain.Models
{
    public class ChangeLoginModel
    {
        [Required(ErrorMessage = "Старый E-mail не введен")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                            @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                            @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
                            ErrorMessage = "Введите корректный предыдущий E-mail.")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "E-mail")]
        public string OldEmail { get; set; }

        [Required(ErrorMessage = "Новый E-mail не введен")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                            @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                            @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
                            ErrorMessage = "Введите корректный новый E-mail.")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "E-mail")]
        public string NewEmail { get; set; }
    }
}
