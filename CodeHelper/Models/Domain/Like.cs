namespace CodeHelper.Models.Domain
{
    public class Like
    {
        public int Id { get; set; }
        public int AnswerId { get; set; }

        public Like() { }

        public Like(int answerId)
        {
            AnswerId = answerId;
        }
    }
}
