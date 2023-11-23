using CodeHelper.Models;
using CodeHelper.Models.Domain;

namespace CodeHelper.ViewModels
{
    public class QuestionsViewModel
    {
        public ICollection<Question> Questions { get; set; }
        public IList<Tag> AllTags { get; set; }
        public ICollection<string> SelectedTags { get; set; }
        public int PageCount { get; set; }
        public int CurrentPage { get; set; }
        public int StartPage { get; set; }
        public int EndPage { get; set; }
        public bool NoAnswers { get; set; }
        public bool NoAcceptedAnswer { get; set; }
        public SortFilters Sort { get; set; }
    }
}
