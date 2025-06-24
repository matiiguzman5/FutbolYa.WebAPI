using System.Security.Claims;
using FutbolYa.WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FutbolYa.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RendimientosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RendimientosController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/rendimientos
        [HttpPost]
        public async Task<IActionResult> Evaluar([FromBody] RendimientoDTO dto)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            int evaluadorId = int.Parse(userId);

            if (evaluadorId == dto.EvaluadoId)
                return BadRequest("No podés evaluarte a vos mismo.");

            var evaluado = await _context.Usuarios.FindAsync(dto.EvaluadoId);
            var partido = await _context.Partidos.FindAsync(dto.PartidoId);

            if (evaluado == null || partido == null)
                return BadRequest("Evaluado o partido no válido.");

            var rendimiento = new Rendimientos
            {
                PartidoId = dto.PartidoId,
                EvaluadorId = evaluadorId,
                EvaluadoId = dto.EvaluadoId,
                Actitud = dto.Actitud,
                Pase = dto.Pase,
                Defensa = dto.Defensa,
                TrabajoEquipo = dto.TrabajoEquipo,
                Puntualidad = dto.Puntualidad,
                Fecha = DateTime.Now
            };

            _context.Rendimientos.Add(rendimiento);
            await _context.SaveChangesAsync();

            return Ok("Evaluación guardada correctamente.");
        }

        // GET: api/rendimientos/usuario/2
        [HttpGet("usuario/{id}")]
        public async Task<IActionResult> ObtenerPorUsuario(int id)
        {
            var evaluaciones = await _context.Rendimientos
                .Where(r => r.EvaluadoId == id)
                .ToListAsync();

            return Ok(evaluaciones);
        }

        [HttpGet("mis-evaluaciones")]
        public async Task<IActionResult> ObtenerEvaluacionesRecibidas()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            int evaluadoId = int.Parse(userId);

            var evaluaciones = await _context.Rendimientos
                .Where(r => r.EvaluadoId == evaluadoId)
                .Include(r => r.Evaluador)
                .OrderByDescending(r => r.Fecha)
                .ToListAsync();

            if (!evaluaciones.Any())
            {
                return Ok(new
                {
                    Valoracion = 0,
                    Evaluaciones = new List<object>()
                });
            }

            var valoracion = evaluaciones.Average(r =>
                (r.Actitud + r.Pase + r.Defensa + r.TrabajoEquipo + r.Puntualidad) / 5.0);

            var resultado = new
            {
                Valoracion = Math.Round(valoracion, 1),
                Evaluaciones = evaluaciones.Select(r => new
                {
                    r.PartidoId,
                    r.Actitud,
                    r.Pase,
                    r.Defensa,
                    r.TrabajoEquipo,
                    r.Puntualidad,
                    r.Fecha,
                    Evaluador = new
                    {
                        r.Evaluador.Id,
                        r.Evaluador.Nombre
                    }
                }).ToList()
            };

            return Ok(resultado);
        }


        // GET: api/jugadores/me
        [HttpGet("me")]
        public async Task<IActionResult> ObtenerJugadorActual()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized("Token inválido");

            var jugador = await _context.Usuarios
                .Where(u => u.Id == userId)
                .Select(u => new
                {
                    u.Id,
                    u.Nombre,
                    u.Correo,
                    u.Posicion,
                    u.FotoPerfil
                })
                .FirstOrDefaultAsync();

            if (jugador == null)
                return NotFound("Jugador no encontrado");

            // Calcular promedio de valoración (promedio de todas las métricas)
            var evaluaciones = await _context.Rendimientos
                .Where(r => r.EvaluadoId == userId)
                .ToListAsync();

            double valoracion = 0;
            if (evaluaciones.Any())
            {
                valoracion = evaluaciones.Average(r =>
                    (r.Actitud + r.Pase + r.Defensa + r.TrabajoEquipo + r.Puntualidad) / 5.0
                );
            }

            return Ok(new
            {
                jugador.Id,
                jugador.Nombre,
                jugador.Correo,
                jugador.Posicion,
                jugador.FotoPerfil,
                Valoracion = Math.Round(valoracion, 1)
            });
        }
    }


}

