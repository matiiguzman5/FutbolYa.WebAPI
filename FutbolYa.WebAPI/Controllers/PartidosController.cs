using System.Security.Claims;
using FutbolYa.WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FutbolYa.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PartidosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PartidosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/partidos
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var partidos = await _context.Partidos
                .Include(p => p.Jugadores)
                .ToListAsync();
            return Ok(partidos);
        }

        [Authorize(Roles = "establecimiento")]
        [Authorize(Roles = "administrador")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CrearPartido([FromBody] PartidoDTO dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized("Token inválido");

            var partido = new Partido
            {
                
                Ubicacion = dto.Ubicacion,
                Fecha = dto.Fecha,
                OrganizadorId = userId,
                Jugadores = new List<Usuario>()
            };

            _context.Partidos.Add(partido);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Partido creado correctamente", partido.Id });
        }


        // GET: api/partidos/buscar
        [HttpGet("buscar")]
        public async Task<IActionResult> Buscar([FromQuery] string? ubicacion, [FromQuery] DateTime? fecha)
        {
            var query = _context.Partidos.AsQueryable();

            if (!string.IsNullOrEmpty(ubicacion))
                query = query.Where(p => p.Ubicacion.Contains(ubicacion));

            if (fecha.HasValue)
                query = query.Where(p => p.Fecha.Date == fecha.Value.Date);

            var resultados = await query.Include(p => p.Jugadores).ToListAsync();

            return Ok(resultados);
        }

        // POST: api/partidos/{id}/inscribirse
        [HttpPost("{id}/inscribirse")]
        public async Task<IActionResult> Inscribirse(int id, [FromBody] int usuarioId)
        {
            var partido = await _context.Partidos
                .Include(p => p.Jugadores)
                .FirstOrDefaultAsync(p => p.Id == id);

            var usuario = await _context.Usuarios.FindAsync(usuarioId);

            if (partido == null || usuario == null)
                return NotFound("Partido o usuario no encontrado");

            if (partido.Jugadores.Any(j => j.Id == usuarioId))
                return BadRequest("El usuario ya está inscrito en este partido");

            partido.Jugadores.Add(usuario);
            await _context.SaveChangesAsync();

            return Ok("Usuario inscrito correctamente");
        }
    }

    public class PartidoDTO
    {
        public string Ubicacion { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }

    }

}
