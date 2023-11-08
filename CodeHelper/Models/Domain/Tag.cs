namespace CodeHelper.Models.Domain
{
    public class Tag
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string DisplayName { get; set; }
        public int QuestionId { get; set; }
        public Question Question { get; set; }
    }
}
