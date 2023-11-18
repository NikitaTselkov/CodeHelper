using System.ComponentModel.DataAnnotations.Schema;

namespace CodeHelper.Models.Domain
{
    public class Answer
    {
        public int Id { get; set; }
        public required string Content { get; set; }
        public int LikesCount { get; set; }
        public bool IsAcceptedAnswer { get; set; }
        public required DateTime PublisedDate { get; set; }
        public User User { get; set; }
        public Question Question { get; set; }
    }
}
