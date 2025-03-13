﻿namespace basketSurvey.Entities
{
    public sealed class Poll : AuditableEntity
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;

        public bool IsPublished { get; set; }

        public DateOnly StartAt { get; set; }

        public DateOnly EndAt { get; set; }

        public ICollection<Question> Questions { get; set; } = [];
        public ICollection<Vote> Votes { get; set; } = [];
    }
}
