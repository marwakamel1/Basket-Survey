
using basketSurvey.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace basketSurvey.Services
{
    public class NotificationService(ApplicationDbContext context
        ,UserManager<ApplicationUser> userManager
        ,IHttpContextAccessor httpContextAccessor
        ,IEmailSender emailSender
        ) : INotificationService
    {
        private readonly ApplicationDbContext _context = context;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly IEmailSender _emailSender = emailSender;

        public async Task NotifyNewPoll(int? pollId = null)
        {
            IEnumerable<Poll> polls = [];

            if (pollId.HasValue)
            {
                var poll = await _context.Polls.SingleOrDefaultAsync(p => p.Id == pollId);

                polls = [poll!];
            }
            else
            {
                polls = await _context.Polls.Where(p => p.IsPublished && DateOnly.FromDateTime(DateTime.UtcNow) == p.StartAt ).ToListAsync();
            }

            var users = await _userManager.Users.ToListAsync();

            foreach(var poll in polls)
            {
                foreach(var user in users)
                {
                    var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

                    var emailBody = EmailBodyBuilder.GenerateEmailBody("PollNotification",
                        templateModel: new Dictionary<string, string>
                        {
                            { "{{name}}", user.FirstName },
                            {"{{pollTill}}", poll.Title},
                            {"{{endDate}}",poll.EndAt.ToString()},
                            {"{{url}}", $"{origin}/start/{poll.Id}" }
                        }
                    );

                    await _emailSender.SendEmailAsync(user.Email!, $"Survey Basket: New Poll - {poll.Title}", emailBody);
                }
            }



        }
    }
}
