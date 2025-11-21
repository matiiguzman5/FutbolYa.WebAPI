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
                .Include(p => p.Organizador) 
                .Include(p => p.Jugadores)
                    .ThenInclude(pu => pu.Jugador) 
                .ToListAsync();

            var resultado = partidos.Select(p => new
            {
                p.Id,
                p.Ubicacion,
                p.Fecha,
                Organizador = new
                {
                    p.Organizador.Id,
                    p.Organizador.Nombre
                },
                Jugadores = p.Jugadores.Select(j => new
                {
                    j.JugadoresId,
                    j.Jugador.Nombre,
                    j.Jugador.Posicion
                })
            });

            return Ok(resultado);
        }

        [HttpPost]
        [Authorize(Roles = "establecimiento,administrador")]
        public async Task<IActionResult> CrearPartido([FromBody] PartidoDTO dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var partido = new Partido
            {
                Ubicacion = dto.Ubicacion,
                Fecha = dto.Fecha,
                OrganizadorId = userId,
                Jugadores = new List<PartidoUsuario>
        {
            new PartidoUsuario { JugadoresId = userId } // organizador se inscribe
        }
            };

            _context.Partidos.Add(partido);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Partido creado", partido.Id });
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

            if (partido == null)
                return NotFound("Partido no encontrado");

            // Validar que no esté ya inscripto
            if (partido.Jugadores.Any(j => j.JugadoresId == usuarioId))
                return BadRequest("El usuario ya está inscrito en este partido");

            // Crear relación en tabla intermedia
            var partidoUsuario = new PartidoUsuario
            {
                PartidosId = id,
                JugadoresId = usuarioId
            };

            _context.PartidoUsuarios.Add(partidoUsuario);
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
