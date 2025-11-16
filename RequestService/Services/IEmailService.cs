namespace RequestService.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
        Task SendEmailToManyAsync(IEnumerable<string> recipients, string subject, string body);
    }
}
