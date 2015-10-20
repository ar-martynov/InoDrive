using System;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;


namespace Domain.Models
{
    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "Введите старый пароль")]
        [StringLength(100, ErrorMessage = "{0} должен быть длиннее {2} символов.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Старый пароль")]
        public string OldPassword { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Введите новый пароль.")]
        [Display(Name = "Новый пароль")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Подтвердите новый пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Подтвердите новый пароль")]
        [Compare("NewPassword", ErrorMessage = "Новый пароль и его подтверждение не совпадают.")]
        public string NewPasswordConfirm { get; set; }
    }
}
