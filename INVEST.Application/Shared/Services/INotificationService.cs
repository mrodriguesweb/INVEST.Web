namespace INVEST.Application.Shared.Services
{
    public interface INotificationService
    {
        Task SendAsync(string title, string message, CancellationToken ct);
    }
}