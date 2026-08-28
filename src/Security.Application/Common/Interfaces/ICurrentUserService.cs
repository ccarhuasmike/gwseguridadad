namespace Security.Application.Common.Interfaces;

/// <summary>
/// Exposes the identity of the user performing the current request, used to
/// stamp UsuarioRegistro/UsuarioModifica audit columns and for error logging.
/// Decoupled from the concrete authentication mechanism so it can be plugged
/// in later (JWT, cookies, etc.) without touching Application/Infrastructure code.
/// </summary>
public interface ICurrentUserService
{
    int? UserId { get; }

    string? UserName { get; }
}
