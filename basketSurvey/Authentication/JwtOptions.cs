using System.ComponentModel.DataAnnotations;

namespace basketSurvey.Authentication
{
    public class JwtOptions
    {
        public static string SectionName = "Jwt";

        [Required]
        public string Issuer { get; set; }
        [Required]
        public string Audience { get; set; }
        [Required]
        public string Key { get; set; }
        [Range(1, int.MaxValue)]
        public int ExpiresIn { get; set; }
    }
}
