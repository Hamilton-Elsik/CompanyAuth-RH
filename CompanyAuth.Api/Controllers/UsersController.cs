using CompanyAuth.Application.DTOs.Users;
using CompanyAuth.Application.Interfaces.Repositories;
using CompanyAuth.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CompanyAuth.Api.Controllers
{
    /// <summary>
    /// Controlador para la gestión de usuarios del sistema.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Todos requieren estar autenticados
    [Produces("application/json")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _users;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserRepository users, ILogger<UsersController> logger)
        {
            _users = users;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene la lista de usuarios.
        /// </summary>
        /// <remarks>
        /// Requiere el permiso <c>ViewUsers</c> o el rol <c>Admin</c>.
        /// </remarks>
        /// <response code="200">Lista de usuarios devuelta correctamente.</response>
        /// <response code="401">Usuario no autenticado.</response>
        /// <response code="403">Usuario sin permisos para ver usuarios.</response>
        [HttpGet]
        [Authorize(Policy = "ViewUsers")]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
        {
            try
            {
                var users = await _users.GetAllAsync();

                var result = users.Select(u => new UserDto(
                    u.Id,
                    u.FirstName,
                    u.LastName,
                    u.Email,
                    u.Role.Name
                ));

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de usuarios.");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "Ocurrió un error al obtener la lista de usuarios."
                });
            }
        }

        /// <summary>
        /// Obtiene un usuario por su identificador.
        /// </summary>
        /// <param name="id">Identificador del usuario.</param>
        /// <response code="200">Usuario encontrado.</response>
        /// <response code="404">Usuario no encontrado.</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(UserDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<UserDto>> GetById(int id)
        {
            try
            {
                var user = await _users.GetByIdAsync(id);
                if (user is null)
                    return NotFound(new { message = "Usuario no encontrado." });

                var dto = new UserDto(
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    user.Role.Name
                );

                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el usuario con Id {UserId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "Ocurrió un error al obtener el usuario."
                });
            }
        }

        /// <summary>
        /// Crea un nuevo usuario.
        /// </summary>
        /// <param name="request">Datos del usuario a crear.</param>
        /// <remarks>
        /// Solo usuarios con rol <c>Admin</c> pueden crear usuarios.
        /// </remarks>
        /// <response code="201">Usuario creado correctamente.</response>
        /// <response code="400">Datos de entrada no válidos.</response>
        /// <response code="401">Usuario no autenticado.</response>
        /// <response code="403">Usuario sin permisos para crear usuarios.</response>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(UserDto), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        public async Task<ActionResult<UserDto>> Create([FromBody] RegisterUserRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Podrías validar aquí si ya existe un usuario con ese email
                var existing = await _users.GetByEmailAsync(request.Email);
                if (existing is not null)
                {
                    return BadRequest(new { message = "Ya existe un usuario con ese correo electrónico." });
                }

                var user = new User
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    RoleId = request.RoleId,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
                };

                await _users.AddAsync(user);
                await _users.SaveChangesAsync();

                var created = await _users.GetByIdAsync(user.Id);

                var dto = new UserDto(
                    created!.Id,
                    created.FirstName,
                    created.LastName,
                    created.Email,
                    created.Role.Name
                );

                return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear un usuario.");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "Ocurrió un error al crear el usuario."
                });
            }
        }

        /// <summary>
        /// Actualiza los datos de un usuario existente.
        /// </summary>
        /// <param name="id">Identificador del usuario.</param>
        /// <param name="request">Datos actualizados del usuario.</param>
        /// <response code="204">Usuario actualizado correctamente.</response>
        /// <response code="400">Datos de entrada no válidos.</response>
        /// <response code="404">Usuario no encontrado.</response>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] RegisterUserRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _users.GetByIdAsync(id);
                if (user is null)
                    return NotFound(new { message = "Usuario no encontrado." });

                user.FirstName = request.FirstName;
                user.LastName = request.LastName;
                user.Email = request.Email;
                user.RoleId = request.RoleId;

                if (!string.IsNullOrWhiteSpace(request.Password))
                {
                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
                }

                await _users.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el usuario con Id {UserId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "Ocurrió un error al actualizar el usuario."
                });
            }
        }

        /// <summary>
        /// Elimina un usuario por su identificador.
        /// </summary>
        /// <param name="id">Identificador del usuario.</param>
        /// <response code="204">Usuario eliminado correctamente.</response>
        /// <response code="404">Usuario no encontrado.</response>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var user = await _users.GetByIdAsync(id);
                if (user is null)
                    return NotFound(new { message = "Usuario no encontrado." });

                await _users.RemoveAsync(user);
                await _users.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el usuario con Id {UserId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "Ocurrió un error al eliminar el usuario."
                });
            }
        }
    }
}
