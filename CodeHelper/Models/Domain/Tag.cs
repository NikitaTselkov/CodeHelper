﻿namespace CodeHelper.Models.Domain
{
    public class Tag
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public ICollection<Question> Questions { get; set; }
    }
}
