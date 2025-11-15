using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.Abstractions;

namespace UserManagement.Api.Controller.V1;

[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth)
    {
        _auth = auth;
    }

    public record LoginRequest(string Username, string Password);
    public record LoginResponse(string Token, DateTime ExpiresAt);

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var result = await _auth.LoginAsync(request.Username, request.Password, ct);

        if (result is null)
            return Unauthorized();

        // result is (string Token, DateTime ExpiresAt)?
        // use .Value or deconstruct
        var (token, expiresAt) = result.Value;

        return Ok(new LoginResponse(token, expiresAt));
    }

}
