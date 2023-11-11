using System.ComponentModel.DataAnnotations;

namespace CodeHelper.ViewModels
{
    public class ForgetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
