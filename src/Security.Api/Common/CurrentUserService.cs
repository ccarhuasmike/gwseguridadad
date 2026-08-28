using Security.Application.Common.Interfaces;

namespace Security.Api.Common;

/// <summary>
/// Default implementation of <see cref="ICurrentUserService"/> based on
/// HttpContext.User. Returns null when no authentication mechanism has been
/// wired up yet; the structure is ready for a future JWT/cookie provider to
/// populate ClaimTypes.NameIdentifier / ClaimTypes.Name.
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int? UserId
    {
        get
        {
            var value = _httpContextAccessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(value, out var id) ? id : null;
        }
    }

    public string? UserName => _httpContextAccessor.HttpContext?.User?.Identity?.Name;
}
