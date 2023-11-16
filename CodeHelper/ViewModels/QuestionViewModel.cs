using CodeHelper.Models.Domain;

namespace CodeHelper.ViewModels
{
    public class QuestionViewModel
    {
        public Question Question { get; set; }
        public Answer Answer { get; set; }

        public string PublishedDate
        {
            get
            {
                if (Question?.PublisedDate != null)
                {
                    return CalculateTimeElapsed(Question.PublisedDate, DateTime.UtcNow.Date);
                }

                return string.Empty;
            }
        }

        private string CalculateTimeElapsed(DateTime inputDate, DateTime currentDate)
        {
            TimeSpan timeSpan = currentDate - inputDate;
            int years = currentDate.Year - inputDate.Year;
            int months = currentDate.Month - inputDate.Month;
            int days = currentDate.Day - inputDate.Day;

            if (months < 0 || (months == 0 && days < 0))
            {
                years--;
                months = (currentDate.Month + 12) - inputDate.Month;
            }

            if (days < 0)
            {
                int previousMonth = currentDate.Month - 1;
                if (previousMonth == 0) previousMonth = 12;
                int daysInPreviousMonth = DateTime.DaysInMonth(currentDate.Year, previousMonth);
                days = daysInPreviousMonth + days;
                months--;
            }

            if (years > 0)
            {
                return $"{years} {(years == 1 ? "year" : "years")} ago";
            }
            else if (months > 0)
            {
                return $"{months} {(months == 1 ? "month" : "months")} ago";
            }
            else if (days > 0)
            {
                return $"{days} {(days == 1 ? "day" : "days")} ago";
            }
            else
            {
                return "Today";
            }
        }
    }
}
