using Microsoft.AspNetCore.Mvc.Rendering;

namespace CodeHelper.Models.Domain
{
    public class Tag
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string DisplayName { get; set; }
        public ICollection<Question> Questions { get; set; }
    }
}
