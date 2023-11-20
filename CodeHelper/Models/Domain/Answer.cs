namespace CodeHelper.Models.Domain
{
    public class Answer
    {
        public int Id { get; set; }
        public required string Content { get; set; }
        public int LikesCount { get; set; }
        public bool IsAcceptedAnswer { get; set; }
        public bool IsLikedAnswer { get; set; }
        public required DateTime PublisedDate { get; set; }
        public Question Question { get; set; }
        public User User { get; set; }
    }
}
