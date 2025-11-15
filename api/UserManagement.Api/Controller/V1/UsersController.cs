using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Api.Helpers;
using UserManagement.Application.Abstractions;
using UserManagement.Application.Models;
using UserManagement.Application.Users;
using UserManagement.Domain.Enums;

namespace UserManagement.Api.Controllers;

[ApiController]
[ApiVersion(1.0)]
[Authorize]
[Route("api/v{version:apiVersion}/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _svc;
    private readonly IAuditService _audit;

    public UsersController(IUserService svc, IAuditService audit)
    {
        _svc = svc;
        _audit = audit;
    }

    // ==========================================================
    // DataTable Server-Side Endpoint
    // ==========================================================
    [HttpPost("dt"), MapToApiVersion(1.0)]
    public async Task<IActionResult> DataTableServer(
        [FromBody] DataTableRequest req,
        [FromQuery] UserRole? role,
        CancellationToken ct)
    {
        var query = new UserQuery(
            req.Search?.Value,
            role,
            req.Start / req.Length + 1,
            req.Length,
            req.GetSortColumn(),
            req.GetSortDir()
        );

        var actor = User.GetActor();
        var ip = HttpContext.GetRemoteIp();

        await _audit.LogAsync(
            AuditAction.DataTableQuery.ToString(),
            "User",
            $"Page={query.Page}",
            actor,
            ip,
            "User list loaded via DataTable",
            ct);

        var page = await _svc.ListAsync(query, ct);

        return Ok(new DataTableResponse<UserDto>
        {
            Draw = req.Draw,
            RecordsTotal = page.TotalCount,
            RecordsFiltered = page.TotalCount,
            Data = page.Items.ToList()
        });
    }

    // ==========================================================
    // List Users
    // ==========================================================
    [HttpGet, MapToApiVersion(1.0)]
    public async Task<ActionResult<PagedResult<UserDto>>> List(
        [FromQuery] string? search,
        [FromQuery] UserRole? role,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sort = null,
        [FromQuery] string? dir = "asc",
        CancellationToken ct = default)
    {
        var actor = User.GetActor();
        var ip = HttpContext.GetRemoteIp();

        await _audit.LogAsync(
            AuditAction.ViewList.ToString(),
            "User",
            $"Page={page}; Search={search}",
            actor,
            ip,
            "User list viewed",
            ct);

        var query = new UserQuery(search, role, page, pageSize, sort, dir);
        return Ok(await _svc.ListAsync(query, ct));
    }

    // ==========================================================
    // Get User By ID
    // ==========================================================
    [HttpGet("{id:int}"), MapToApiVersion(1.0)]
    [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.ReadOnlyUser)}")]
    public async Task<ActionResult<UserDto>> Get(int id, CancellationToken ct)
    {
        var actor = User.GetActor();
        var ip = HttpContext.GetRemoteIp();

        var dto = await _svc.GetAsync(id, ct);

        await _audit.LogAsync(
            AuditAction.View.ToString(),
            "User",
            $"Id={id}",
            actor,
            ip,
            "Viewed user details",
            ct);

        return dto is null ? NotFound() : Ok(dto);
    }

    // ==========================================================
    // Create User
    // ==========================================================
    [HttpPost, MapToApiVersion(1.0)]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<UserDto>> Create(
        [FromBody] CreateUserRequest req,
        CancellationToken ct)
    {
        var actor = User.GetActor();
        var ip = HttpContext.GetRemoteIp();

        var dto = await _svc.CreateAsync(req, actor, ip, ct);

        await _audit.LogChangeAsync(
            AuditAction.Insert.ToString(),
            "User",
            $"Id={dto.Id}",
            oldValues: null,
            newValues: dto,
            performedBy: actor,
            ip: ip,
            description: "User created",
            ct);

        return CreatedAtAction(nameof(Get), new { id = dto.Id, version = "1.0" }, dto);
    }

    // ==========================================================
    // Update User
    // ==========================================================
    [HttpPut("{id:int}"), MapToApiVersion(1.0)]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<UserDto>> Update(
        int id,
        [FromBody] UpdateUserRequest req,
        CancellationToken ct)
    {
        var actor = User.GetActor();
        var ip = HttpContext.GetRemoteIp();

        // Load old
        var oldUser = await _svc.GetAsync(id, ct);
        if (oldUser is null)
            return NotFound();

        var dto = await _svc.UpdateAsync(id, req, actor, ip, ct);

        await _audit.LogChangeAsync(
            AuditAction.Update.ToString(),
            "User",
            $"Id={id}",
            oldValues: oldUser,
            newValues: dto,
            performedBy: actor,
            ip: ip,
            description: "User updated",
            ct);

        return Ok(dto);
    }

    // ==========================================================
    // Soft Delete
    // ==========================================================
    [HttpDelete("{id:int}"), MapToApiVersion(1.0)]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> SoftDelete(int id, CancellationToken ct)
    {
        var actor = User.GetActor();
        var ip = HttpContext.GetRemoteIp();

        var old = await _svc.GetAsync(id, ct);

        var ok = await _svc.SoftDeleteAsync(id, actor, ip, ct);

        await _audit.LogChangeAsync(
            AuditAction.Delete.ToString(),
            "User",
            $"Id={id}",
            oldValues: old,
            newValues: null,
            performedBy: actor,
            ip: ip,
            description: "User soft-deleted",
            ct);

        return ok ? NoContent() : NotFound();
    }

    // ==========================================================
    // Get Own Profile
    // ==========================================================
    [HttpGet("profile"), MapToApiVersion(1.0)]
    public async Task<ActionResult<UserDto>> GetUserProfile(CancellationToken ct)
    {
        var id = User.GetUserId();
        if (id is null) return Forbid();

        return Ok(await _svc.GetAsync(id.Value, ct));
    }

    // ==========================================================
    // Update Own Profile
    // ==========================================================
    [HttpPut("profile"), MapToApiVersion(1.0)]
    public async Task<ActionResult<UserDto>> UpdateProfile(
        [FromBody] UpdateUserProfileRequest req,
        CancellationToken ct)
    {
        var id = User.GetUserId();
        if (id is null) return Forbid();

        var actor = User.GetActor();
        var ip = HttpContext.GetRemoteIp();

        var old = await _svc.GetAsync(id.Value, ct);

        var dto = await _svc.UpdateProfileAsync(id.Value, req, actor, ip, ct);

        await _audit.LogChangeAsync(
            AuditAction.UpdateProfile.ToString(),
            "User",
            $"Id={id.Value}",
            oldValues: old,
            newValues: dto,
            performedBy: actor,
            ip: ip,
            description: "Profile updated",
            ct);

        return dto is null ? NotFound() : Ok(dto);
    }
}
