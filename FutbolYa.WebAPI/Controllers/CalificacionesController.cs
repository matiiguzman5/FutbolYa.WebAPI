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

            var partido = await _context.Partidos
                .Include(p => p.Jugadores)
                .FirstOrDefaultAsync(p => p.Id == dto.PartidoId);

            if (partido == null)
                return NotFound("Partido no encontrado");

            // Validar que ambos jugaron ese partido
            var jugoEvaluador = partido.Jugadores.Any(j => j.JugadoresId == userId);
            var jugoEvaluado = partido.Jugadores.Any(j => j.JugadoresId == dto.EvaluadoId);

            if (!jugoEvaluador || !jugoEvaluado)
                return BadRequest("Solo podés calificar a jugadores que participaron en tu mismo partido.");

            // Validar que no exista ya una calificación de este evaluador al mismo evaluado en este partido
            var yaCalifico = await _context.Calificaciones
                .AnyAsync(c => c.PartidoId == dto.PartidoId
                            && c.EvaluadorId == userId
                            && c.EvaluadoId == dto.EvaluadoId);

            if (yaCalifico)
                return BadRequest("Ya calificaste a este jugador en este partido.");

            // Crear la calificación
            var calificacion = new Calificacion
            {
                PartidoId = dto.PartidoId,
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
                c.PartidoId,
                c.Puntaje,
                c.Comentario,
                c.Fecha,
                Evaluador = new { c.Evaluador.Id, c.Evaluador.Nombre }
            });

            return Ok(resultado);
        }
        // GET: api/calificaciones/partido/1
        [HttpGet("partido/{id}")]
        public async Task<IActionResult> VerPorPartido(int id)
        {
            var calificaciones = await _context.Calificaciones
                .Where(c => c.PartidoId == id)
                .Include(c => c.Evaluador)
                .Include(c => c.Evaluado)
                .OrderByDescending(c => c.Fecha)
                .ToListAsync();

            if (!calificaciones.Any())
                return Ok(new { mensaje = "No hay calificaciones para este partido." });

            var resultado = calificaciones.Select(c => new
            {
                c.PartidoId,
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
                .Include(c => c.Partido)
                .OrderByDescending(c => c.Fecha)
                .ToListAsync();

            var resultado = calificaciones.Select(c => new
            {
                PartidoId = c.PartidoId,
                PartidoInfo = $"{c.Partido.Ubicacion} - {c.Partido.Fecha:dd/MM/yyyy HH:mm}",
                c.Puntaje,
                c.Comentario,
                c.Fecha,
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
