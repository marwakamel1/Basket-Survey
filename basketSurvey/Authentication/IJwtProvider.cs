namespace basketSurvey.Authentication
{
    public interface IJwtProvider
    {
        public (string token, int expiresIn) GenerateToken(ApplicationUser user);
        public string? ValidateToken(string token);
    }
}
