using CodeHelper.Models.Domain;

namespace CodeHelper.ViewModels
{
    public class AskQuestionViewModel
    {
        public Question Question { get; set; }
        public ICollection<int> SelectedTags { get; set; }
    }
}
