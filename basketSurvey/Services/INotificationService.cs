namespace basketSurvey.Services
{
    public interface INotificationService
    {
        Task NotifyNewPoll(int? pollId = null);
    }
}
