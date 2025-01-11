namespace basketSurvey.Errors
{

    public static class PollErrors
    {
        public static readonly Error PollNotFound = new Error("PollNotFound", "Poll Not Found");

        public static readonly Error PollDuplicatedTitle = new Error("PollDuplicatedTitle", "Poll Duplicated Title");
    }
}
