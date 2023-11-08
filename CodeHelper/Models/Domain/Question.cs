using System.ComponentModel.DataAnnotations.Schema;

namespace CodeHelper.Models.Domain
{
    public class Question
    {
        public required int Id { get; set; }
        public required string Title { get; set; }
        public required string Content { get; set; }
        public int ViewsCount { get; set; }
        public required DateTime PublisedDate { get; set; }
        public ICollection<Tag> Tags { get; set; }
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User Author { get; set; }
    }
}
