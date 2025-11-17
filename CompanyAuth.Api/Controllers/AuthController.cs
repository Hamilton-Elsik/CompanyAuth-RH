using CompanyAuth.Application.DTOs;
using CompanyAuth.Application.Interfaces.Repositories;
using CompanyAuth.Application.Interfaces.Services;
using CompanyAuth.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace CompanyAuth.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AuthorizationController : ControllerBase
{
    private readonly IAuthorizationRepository _authorizations;
    private readonly IRoleRepository _roles;
    private readonly IPermissionRepository _permissions;
    private readonly IAuthService _authService;

    public AuthorizationController(
        IAuthorizationRepository authorizations,
        IRoleRepository roles,
        IPermissionRepository permissions,
        IAuthService authService)
    {
        _authorizations = authorizations;
        _roles = roles;
        _permissions = permissions;
        _authService = authService;
    }

    // POST: /api/authorization/roles/{roleId}/permissions/{permissionId}
    [HttpPost("roles/{roleId:int}/permissions/{permissionId:int}")]
    public async Task<IActionResult> AssignPermission(int roleId, int permissionId)
    {
        var role = await _roles.GetByIdAsync(roleId);
        if (role is null) return NotFound(new { message = "Rol no encontrado" });

        var permissions = await _permissions.GetAllAsync();
        var permission = permissions.FirstOrDefault(p => p.Id == permissionId);
        if (permission is null) return NotFound(new { message = "Permiso no encontrado" });

        var existing = (await _authorizations.GetByRoleIdAsync(roleId))
                        .FirstOrDefault(a => a.PermissionId == permissionId);

        if (existing != null)
            return BadRequest(new { message = "El rol ya tiene este permiso asignado" });

        var authorization = new Authorization
        {
            RoleId = roleId,
            PermissionId = permissionId,
            DateAuth = DateTime.UtcNow
        };

        await _authorizations.AddAsync(authorization);
        await _authorizations.SaveChangesAsync();

        return Ok(new { message = "Permiso asignado correctamente" });
    }

    // DELETE: /api/authorization/roles/{roleId}/permissions/{permissionId}
    [HttpDelete("roles/{roleId:int}/permissions/{permissionId:int}")]
    public async Task<IActionResult> RevokePermission(int roleId, int permissionId)
    {
        var existing = (await _authorizations.GetByRoleIdAsync(roleId))
                        .FirstOrDefault(a => a.PermissionId == permissionId);

        if (existing is null)
            return NotFound(new { message = "El rol no tiene este permiso asignado" });

        await _authorizations.RemoveAsync(existing);
        await _authorizations.SaveChangesAsync();

        return Ok(new { message = "Permiso revocado correctamente" });
    }


    /// <summary>
    /// Inicia sesión y devuelve un JWT.
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.ValidateCredentialsAsync(request);

        if (result is null)
            return Unauthorized(new { message = "Credenciales inválidas" });

        return Ok(result);
    }
}