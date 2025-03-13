namespace basketSurvey.Abstractions
{
    public record Error(string code , string description , int? statusCode)
    {
        public static readonly Error None = new(string.Empty,string.Empty , null);
    }
}
