using System.ComponentModel.DataAnnotations;

namespace CodeHelper.ViewModels
{
    public class SignUpViewModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(RepeatPassword))]
        [MinLength(5)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        [MinLength(5)]
        public string RepeatPassword { get; set; }
    }
}
