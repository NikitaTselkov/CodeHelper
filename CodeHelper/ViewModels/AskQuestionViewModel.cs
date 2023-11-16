using CodeHelper.Models.Domain;

namespace CodeHelper.ViewModels
{
    public class AskQuestionViewModel
    {
        public Question Question { get; set; }
        public IList<Tag> AllTags { get; set; }
        public ICollection<string> SelectedTags { get; set; }
    }
}
