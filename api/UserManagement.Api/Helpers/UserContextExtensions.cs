
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace UserManagement.Api.Helpers;

public static class UserContextExtensions
{
    /// <summary>
    /// Get current user id from claims (NameIdentifier or "sub").
    /// Returns null if not found or not an int.
    /// </summary>
    public static int? GetUserId(this ClaimsPrincipal user)
    {
        if (user == null) return null;

        var idClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                      ?? user.FindFirst("sub")?.Value;

        return int.TryParse(idClaim, out var id) ? id : null;
    }

    /// <summary>
    /// Get actor name (claims identity name), fallback to "system".
    /// </summary>
    public static string GetActor(this ClaimsPrincipal user)
    {
        if (user?.Identity?.Name is { Length: > 0 } name)
            return name;

        return "system";
    }

    /// <summary>
    /// Get remote IP as string from HttpContext.
    /// </summary>
    public static string? GetRemoteIp(this HttpContext httpContext)
    {
        return httpContext.Connection.RemoteIpAddress?.ToString();
    }
}
