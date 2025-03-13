namespace basketSurvey.Errors
{
    public class QuestionErrors
    {
        public static readonly Error QuestionNotFound = new Error("QuestionNotFound", "Question Not Found", StatusCodes.Status404NotFound);

        public static readonly Error QuestionDuplicatedContent = new Error("QuestionDuplicatedContent", "Question Duplicated Content", StatusCodes.Status409Conflict);
    }
}
