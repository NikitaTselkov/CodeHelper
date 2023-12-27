using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

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

        public static string CalculateNumber(int? num)
        {
            if (num == null) return string.Empty;

            if (num >= 1000000)
            {
                return ((int)num / 1000000d).ToString("0.#") + "m";
            }
            else if (num >= 1000)
            {
                return ((int)num / 1000d).ToString("0.#") + "k";
            }
            else
            {
                return num?.ToString();
            }
        }

        public static string TitleToUrl(string title)
        {
            var reservedCharacters = "!*'();:@&=+$,./?%#[]";

            if (string.IsNullOrWhiteSpace(title))
                return string.Empty;

            foreach (var item in reservedCharacters)
            {
                title = title.Replace(item.ToString(), "");
            }

            if (title.Length > 55)
                title = string.Join("", title.Take(55));

            title = title.Replace(" ", "-");
            title = title.ToLower();

            return title;
        }

        public static string GetDisplayName(this Enum enumValue)
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<DisplayAttribute>()
                            .GetName();
        }

        public static T[] QuickSort<T, P>(this T[] array, Expression<Func<T, P>> property, int leftIndex, int rightIndex) where P : IComparable<P>
        {
            var i = leftIndex;
            var j = rightIndex;
            T pivot = array[leftIndex];

            while (i <= j)
            {
                var propI = (P)GetPropertyInfo(array[i], property)?.GetValue(array[i]);
                var propJ = (P)GetPropertyInfo(array[j], property)?.GetValue(array[j]);
                var propPivot = (P)GetPropertyInfo(pivot, property)?.GetValue(pivot);

                while (propI.CompareTo(propPivot) < 0)
                {
                    i++;
                    propI = (P)GetPropertyInfo(array[i], property)?.GetValue(array[i]);
                }

                while (propJ.CompareTo(propPivot) > 0)
                {
                    j--;
                    propJ = (P)GetPropertyInfo(array[j], property)?.GetValue(array[j]);
                }

                if (i <= j)
                {
                    T temp = array[i];
                    array[i] = array[j];
                    array[j] = temp;
                    i++;
                    j--;
                }
            }

            if (leftIndex < j)
                array.QuickSort(property, leftIndex, j);
            if (i < rightIndex)
                array.QuickSort(property, i, rightIndex);

            return array;
        }

        public static T[] QuickSortDescending<T, P>(this T[] array, Expression<Func<T, P>> property, int leftIndex, int rightIndex) where P : IComparable<P>
        {
            var i = leftIndex;
            var j = rightIndex;
            T pivot = array[leftIndex];

            while (i <= j)
            {
                var propI = (P)GetPropertyInfo(array[i], property)?.GetValue(array[i]);
                var propJ = (P)GetPropertyInfo(array[j], property)?.GetValue(array[j]);
                var propPivot = (P)GetPropertyInfo(pivot, property)?.GetValue(pivot);

                while (propI.CompareTo(propPivot) > 0)
                {
                    i++;
                    propI = (P)GetPropertyInfo(array[i], property)?.GetValue(array[i]);
                }

                while (propJ.CompareTo(propPivot) < 0)
                {
                    j--;
                    propJ = (P)GetPropertyInfo(array[j], property)?.GetValue(array[j]);
                }

                if (i <= j)
                {
                    T temp = array[i];
                    array[i] = array[j];
                    array[j] = temp;
                    i++;
                    j--;
                }
            }

            if (leftIndex < j)
                array.QuickSortDescending(property, leftIndex, j);
            if (i < rightIndex)
                array.QuickSortDescending(property, i, rightIndex);

            return array;
        }

        public static PropertyInfo GetPropertyInfo<TSource, TProperty>(TSource source, Expression<Func<TSource, TProperty>> propertyLambda)
        {
            if (propertyLambda.Body is not MemberExpression member)
            {
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    propertyLambda.ToString()));
            }

            if (member.Member is not PropertyInfo propInfo)
            {
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    propertyLambda.ToString()));
            }

            Type type = typeof(TSource);
            if (propInfo.ReflectedType != null && type != propInfo.ReflectedType && !type.IsSubclassOf(propInfo.ReflectedType))
            {
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a property that is not from type {1}.",
                    propertyLambda.ToString(),
                    type));
            }

            return propInfo;
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
                return $"{years} {(years == 1 ? "год" : "лет")} назад";
            }
            else if (months > 0)
            {
                return $"{months} {(months == 1 ? "месяц" : "месяцев")} назад";
            }
            else if (days > 0)
            {
                return $"{days} {(days == 1 ? "день" : "дней")} назад";
            }
            else
            {
                return "Сегодня";
            }
        }
    }
}
