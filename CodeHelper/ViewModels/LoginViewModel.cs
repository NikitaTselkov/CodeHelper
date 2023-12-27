using System.ComponentModel.DataAnnotations;

namespace CodeHelper.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email не может быть пустым")]
        [EmailAddress(ErrorMessage = "Email имеет не допустимый формат")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Пароль не может быть пустым")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
