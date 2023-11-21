using Newtonsoft.Json;

namespace CodeHelper.Core
{
    public static class Extensions
    {
        private static readonly JsonSerializerSettings _settings = new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        public static string ToJson<T>(this T value)
        {
            return JsonConvert.SerializeObject(value, Formatting.None, _settings);
        }

        public static T FromJson<T>(this string value)
        {
            return JsonConvert.DeserializeObject<T>(value, _settings);
        }

        public static string CalculateNumber(int num)
        {
            if (num >= 1000000)
            {
                return (num / 1000000d).ToString("0.#") + "m";
            }
            else if (num >= 1000)
            {
                return (num / 1000d).ToString("0.#") + "k";
            }
            else
            {
                return num.ToString();
            }
        }

        public static string CalculateTimeElapsed(DateTime inputDate, DateTime currentDate)
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
