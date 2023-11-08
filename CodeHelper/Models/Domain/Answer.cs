﻿namespace CodeHelper.Models.Domain
{
    public class Answer
    {
        public required int Id { get; set; }
        public required string Title { get; set; }
        public required string Content { get; set; }
        public int LikesCount { get; set; }
        public required DateTime PublisedDate { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
