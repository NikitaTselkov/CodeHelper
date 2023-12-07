using System.ComponentModel.DataAnnotations;

namespace CodeHelper.ViewModels
{
    public class ResetPasswordViewModel
    {
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
