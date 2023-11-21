using CodeHelper.Models.Domain;
using CodeHelper.Core;

namespace CodeHelper.ViewModels
{
    public class QuestionViewModel
    {
        public Question Question { get; set; }
        public string AnswerContent { get; set; }

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
