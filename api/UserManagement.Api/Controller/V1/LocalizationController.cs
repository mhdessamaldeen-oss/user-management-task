using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using UserManagement.Application.Abstractions;

namespace UserManagement.Api.Controllers;

[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/localization")]
public class LocalizationController : ControllerBase
{
    private readonly ILocalizationService _loc;

    public LocalizationController(ILocalizationService loc)
    {
        _loc = loc;
    }

    // GET /api/v1/localization/en
    // GET /api/v1/localization/ar
    [HttpGet("{lang}"), MapToApiVersion(1.0)]
    [ProducesResponseType(typeof(Dictionary<string, string>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IDictionary<string, string>>> Get(
        string lang,
        CancellationToken ct)
    {
        var dict = await _loc.GetStringsAsync(lang, ct);
        return Ok(dict);
    }
}
