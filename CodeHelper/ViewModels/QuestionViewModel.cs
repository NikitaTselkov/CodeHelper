using CodeHelper.Models.Domain;
using CodeHelper.Core;
using CodeHelper.Models;

namespace CodeHelper.ViewModels
{
    public class Item
    {
        public int Id { get; set; }
        public string Value { get; set; }
    }

    public class QuestionViewModel
    {
        public Question Question { get; set; }
        public Pagination Pagination { get; set; }
        public string AnswerContent { get; set; }
        public List<Item> AnswersContent { get; set; }
        public int AnswersCount { get; set; }

        public string PublishedDate
        {
            get
            {
                if (Question?.PublisedDate != null)
                {
                    return Extensions.CalculateTimeElapsed(Question.PublisedDate, DateTime.UtcNow.Date);
                }

                return string.Empty;
            }
        }
    }
}
