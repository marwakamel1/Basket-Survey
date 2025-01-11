﻿namespace basketSurvey.Contracts.Polls
{
    public record PollResponse
    (int Id,
        string Title,
        string Summary,
        bool IsPublished,
        DateOnly StartAt,
        DateOnly EndAt
    );
}
