namespace UserManagement.Application.Abstractions;

public interface IAuthService
{
    Task<(string Token, DateTime ExpiresAt)?> LoginAsync(
        string username,
        string password,
        CancellationToken ct = default);
}
