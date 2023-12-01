using CodeHelper.Models;
using CodeHelper.Models.Domain;

namespace CodeHelper.ViewModels
{
    public class QuestionsViewModel
    {
        public ICollection<Question> Questions { get; set; }
        public ICollection<int> SelectedTags { get; set; }
        public ICollection<Tag> Tags { get; set; }
        public Pagination Pagination { get; set; }
        public bool NoAnswers { get; set; }
        public bool NoAcceptedAnswer { get; set; }
        public SortFilters Sort { get; set; }
    }
}
