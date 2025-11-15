namespace UserManagement.Application.Abstractions;

public interface ILocalizationService
{
    Task<IDictionary<string, string>> GetStringsAsync(
        string lang,
        CancellationToken ct = default);
}
