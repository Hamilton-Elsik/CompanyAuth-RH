using CompanyAuth.Application.DTOs.Roles;
using CompanyAuth.Application.Interfaces.Repositories;
using CompanyAuth.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CompanyAuth.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class RolesController : ControllerBase
{
    private readonly IRoleRepository _roles;

    public RolesController(IRoleRepository roles)
    {
        _roles = roles;
    }

    /// <summary>
    /// Obtiene todos los roles del sistema.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoleDto>>> GetAll()
    {
        var roles = await _roles.GetAllAsync();

        var result = roles.Select(r => new RoleDto(
            r.Id,
            r.Name,
            r.Description
        ));

        return Ok(result);
    }

    /// <summary>
    /// Obtiene un role por ID.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<RoleDto>> GetById(int id)
    {
        var role = await _roles.GetByIdAsync(id);
        if (role is null)
            return NotFound(new { message = "Rol no encontrado" });

        var dto = new RoleDto(
            role.Id,
            role.Name,
            role.Description
        );

        return Ok(dto);
    }

    /// <summary>
    /// Crea un nuevo role.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<RoleDto>> Create([FromBody] CreateRoleRequest request)
    {
        var role = new Role
        {
            Name = request.Name,
            Description = request.Description
        };

        await _roles.AddAsync(role);
        await _roles.SaveChangesAsync();

        var dto = new RoleDto(role.Id, role.Name, role.Description);

        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }
}