using CodeHelper.Models.Domain;

namespace CodeHelper.Core
{
    public static class LinqExtentions
    {
        public static IQueryable<Question> TrimQuestionContent(this IQueryable<Question> source, int length)
        {
            return source.Select(s => new Question
            {
                Title = s.Title,
                Answers = s.Answers,
                Author = s.Author,
                HasAcceptedAnswer = s.HasAcceptedAnswer,
                HasAnswers = s.HasAnswers,
                Id = s.Id,
                PublisedDate = s.PublisedDate,
                Tags = s.Tags,
                UserId = s.UserId,
                ViewsCount = s.ViewsCount,
                Content = s.Content.Length <= length ? s.Content : string.Join("", s.Content.Take(length))
            });
        }
    }
}
