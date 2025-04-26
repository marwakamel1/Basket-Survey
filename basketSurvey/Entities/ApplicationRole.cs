using Microsoft.AspNetCore.Identity;

namespace basketSurvey.Entities
{
    public class ApplicationRole : IdentityRole
    {
        public bool IsDefault { get; set; }

        public bool IsDeleted { get; set; }

    }
}
