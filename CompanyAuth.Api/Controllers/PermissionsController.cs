using CompanyAuth.Application.DTOs.Permissions;
using CompanyAuth.Application.Interfaces.Repositories;
using CompanyAuth.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CompanyAuth.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")] // Solo admin puede gestionar permisos
public class PermissionsController : ControllerBase
{
    private readonly IPermissionRepository _permissions;

    public PermissionsController(IPermissionRepository permissions)
    {
        _permissions = permissions;
    }

    /// <summary>
    /// Obtiene todos los permisos del sistema.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PermissionDto>>> GetAll()
    {
        var permissions = await _permissions.GetAllAsync();

        var dto = permissions.Select(p => new PermissionDto(
            p.Id,
            p.Name,
            p.Description,
            p.Module
        ));

        return Ok(dto);
    }

    /// <summary>
    /// Obtiene un permiso por ID.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<PermissionDto>> GetById(int id)
    {
        var permission = await _permissions.GetByIdAsync(id);

        if (permission is null)
            return NotFound(new { message = "Permiso no encontrado" });

        var dto = new PermissionDto(
            permission.Id,
            permission.Name,
            permission.Description,
            permission.Module
        );

        return Ok(dto);
    }

    /// <summary>
    /// Crea un nuevo permiso.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<PermissionDto>> Create([FromBody] CreatePermissionRequest request)
    {
        var permission = new Permission
        {
            Name = request.Name,
            Description = request.Description,
            Module = request.Module
        };

        await _permissions.AddAsync(permission);
        await _permissions.SaveChangesAsync();

        var dto = new PermissionDto(permission.Id, permission.Name, permission.Description, permission.Module);

        return CreatedAtAction(nameof(GetById), new { id = permission.Id }, dto);
    }

    /// <summary>
    /// Actualiza un permiso existente.
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePermissionRequest model)
    {
        var permission = await _permissions.GetByIdAsync(id);

        if (permission is null)
            return NotFound(new { message = "Permiso no encontrado" });

        permission.Name = model.Name;
        permission.Description = model.Description;
        permission.Module = model.Module;

        await _permissions.SaveChangesAsync();

        return NoContent();
    }
     
    /// <summary>
    /// Elimina un permiso por ID.
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var permissions = await _permissions.GetAllAsync();
        var permission = permissions.FirstOrDefault(p => p.Id == id);

        if (permission is null)
            return NotFound(new { message = "Permiso no encontrado" });

        await _permissions.RemoveAsync(permission);
        await _permissions.SaveChangesAsync();

        return Ok(new { message = "Permiso eliminado correctamente" });
    }
}