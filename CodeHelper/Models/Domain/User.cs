using CodeHelper.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace CodeHelper.Models.Domain
{
    public class User : IdentityUser
    {
        public ICollection<Question> Questions { get; set; }
        public ICollection<Answer> Answers { get; set; }

        public User()
        {

        }

        public User(SignUpViewModel model)
        {
            UserName = model.UserName;
            Email = model.Email;
            Questions = new List<Question>();
            Answers = new List<Answer>();
        }
    }
}
