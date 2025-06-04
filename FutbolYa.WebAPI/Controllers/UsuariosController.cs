using FutbolYa.WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FutbolYa.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Usuarios/yo
        [HttpGet("yo")]
        public async Task<IActionResult> ObtenerPerfil()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var usuario = await _context.Usuarios
                .Where(u => u.Id == int.Parse(userId))
                .Select(u => new
                {
                    u.Id,
                    u.Nombre,
                    u.Correo,
                    u.Rol
                })
                .FirstOrDefaultAsync();

            if (usuario == null)
                return NotFound("Usuario no encontrado.");

            return Ok(usuario);
        }
    }
}
