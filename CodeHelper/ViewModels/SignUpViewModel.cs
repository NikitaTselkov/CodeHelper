using System.ComponentModel.DataAnnotations;

namespace CodeHelper.ViewModels
{
    public class SignUpViewModel
    {
        [Required(ErrorMessage = "Имя не может быть пустым")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email не может быть пустым")]
        [EmailAddress(ErrorMessage = "Email имеет не допустимый формат")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Пароль не может быть пустым")]
        [DataType(DataType.Password)]
        [Compare(nameof(RepeatPassword), ErrorMessage = "Пароли не совпадают")]
        [MinLength(5, ErrorMessage = "Минимальная длина пароля 5 символов")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Пароль не может быть пустым")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Пароли не совпадают")]
        [MinLength(5, ErrorMessage = "Минимальная длина пароля 5 символов")]
        public string RepeatPassword { get; set; }
    }
}
