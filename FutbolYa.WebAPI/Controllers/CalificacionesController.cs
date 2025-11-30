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
    public class CalificacionesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CalificacionesController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/calificaciones
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CalificacionDTO dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (userId == dto.EvaluadoId)
                return BadRequest("No podés evaluarte a vos mismo.");

            // Buscar la reserva
            var reserva = await _context.Reservas
                .Include(r => r.Jugadores)
                .FirstOrDefaultAsync(r => r.Id == dto.ReservaId);

            if (reserva == null)
                return NotFound("Reserva no encontrada.");

            // Validar que ambos jugaron
            bool jugoEvaluador = reserva.Jugadores.Any(u => u.UsuarioId == userId);
            bool jugoEvaluado = reserva.Jugadores.Any(u => u.UsuarioId == dto.EvaluadoId);

            if (!jugoEvaluador || !jugoEvaluado)
                return BadRequest("Solo podés calificar a jugadores que participaron en la misma reserva.");

            // Validar que no exista ya una calificación repetida
            bool yaCalifico = await _context.Calificaciones
                .AnyAsync(c => c.ReservaId == dto.ReservaId
                            && c.EvaluadorId == userId
                            && c.EvaluadoId == dto.EvaluadoId);

            if (yaCalifico)
                return BadRequest("Ya calificaste a este jugador en esta reserva.");

            var calificacion = new Calificacion
            {
                ReservaId = dto.ReservaId,
                EvaluadorId = userId,
                EvaluadoId = dto.EvaluadoId,
                Puntaje = dto.Puntaje,
                Comentario = dto.Comentario,
                Fecha = DateTime.Now
            };

            _context.Calificaciones.Add(calificacion);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Calificación creada correctamente" });
        }

        // GET: api/calificaciones/usuario/5
        [HttpGet("usuario/{id}")]
        public async Task<IActionResult> VerPorUsuario(int id)
        {
            var calificaciones = await _context.Calificaciones
                .Where(c => c.EvaluadoId == id)
                .Include(c => c.Evaluador)
                .OrderByDescending(c => c.Fecha)
                .ToListAsync();

            var resultado = calificaciones.Select(c => new
            {
                c.ReservaId,
                c.Puntaje,
                c.Comentario,
                c.Fecha,
                Evaluador = new { c.Evaluador.Id, c.Evaluador.Nombre }
            });

            return Ok(resultado);
        }

        // GET: api/calificaciones/reserva/1
        [HttpGet("reserva/{id}")]
        public async Task<IActionResult> VerPorReserva(int id)
        {
            var calificaciones = await _context.Calificaciones
                .Where(c => c.ReservaId == id)
                .Include(c => c.Evaluador)
                .Include(c => c.Evaluado)
                .OrderByDescending(c => c.Fecha)
                .ToListAsync();

            if (!calificaciones.Any())
                return Ok(new { mensaje = "No hay calificaciones para esta reserva." });

            var resultado = calificaciones.Select(c => new
            {
                c.ReservaId,
                Evaluador = new { c.Evaluador.Id, c.Evaluador.Nombre },
                Evaluado = new { c.Evaluado.Id, c.Evaluado.Nombre },
                c.Puntaje,
                c.Comentario,
                c.Fecha
            });

            return Ok(resultado);
        }

        // GET: api/calificaciones/mias
        [HttpGet("mias")]
        public async Task<IActionResult> VerMisCalificaciones()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var calificaciones = await _context.Calificaciones
                .Where(c => c.EvaluadoId == userId)
                .Include(c => c.Evaluador)
                .Include(c => c.Reserva)
                    .ThenInclude(r => r.Cancha)
                .Include(c => c.Reserva)
                    .ThenInclude(r => r.UsuarioEstablecimiento)
                .OrderByDescending(c => c.Fecha)
                .ToListAsync();

            if (calificaciones.Count == 0)
                return Ok(new List<object>());

            var resultado = calificaciones.Select(c => new
            {
                c.Id,
                ReservaId = c.ReservaId,

                ReservaInfo = string.Join(" — ", new[]
                {
            c.Reserva.Cancha?.Nombre,
            c.Reserva.UsuarioEstablecimiento?.Ubicacion,
            c.Reserva.FechaHora.ToString("dd/MM/yyyy HH:mm")
        }.Where(s => !string.IsNullOrWhiteSpace(s))),

                c.Puntaje,
                c.Comentario,
                Fecha = c.Fecha.ToString("yyyy-MM-dd HH:mm"),

                Evaluador = new
                {
                    c.Evaluador.Id,
                    c.Evaluador.Nombre
                }
            });

            return Ok(resultado);
        }

    }
}
