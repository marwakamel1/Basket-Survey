namespace basketSurvey.Abstractions
{
    public record Error(string code , string description)
    {
        public static readonly Error None = new(string.Empty,string.Empty);
    }
}
