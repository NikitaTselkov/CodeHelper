using CodeHelper.Models.Domain;

namespace CodeHelper.ViewModels
{
    public class QuestionsViewModel
    {
        public ICollection<Question> Questions { get; set; }
        public IList<Tag> Tags { get; set; }
    }
}
