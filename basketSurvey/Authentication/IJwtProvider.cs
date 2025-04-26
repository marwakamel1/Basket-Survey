namespace basketSurvey.Authentication
{
    public interface IJwtProvider
    {
        public (string token, int expiresIn) GenerateToken(ApplicationUser user, IEnumerable<string> roles, IEnumerable<string> permissions);
        public string? ValidateToken(string token);
    }
}
