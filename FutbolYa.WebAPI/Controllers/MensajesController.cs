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
    public class MensajesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MensajesController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/mensajes
        [HttpPost]
        public async Task<IActionResult> EnviarMensaje([FromBody] MensajeDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Contenido))
                return BadRequest("El contenido del mensaje no puede estar vacío.");

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized("Token inválido.");

            var reserva = await _context.Reservas
                .Include(r => r.Jugadores)
                .FirstOrDefaultAsync(r => r.Id == dto.ReservaId);

            if (reserva == null)
                return BadRequest("La reserva no existe.");

            // 🔐 Opcional: solo dejar chatear a quien está inscripto
            var estaInscripto = reserva.Jugadores.Any(j => j.UsuarioId == userId);
            if (!estaInscripto)
                return Forbid("No estás inscripto en esta reserva.");

            var mensaje = new Mensaje
            {
                ReservaId = dto.ReservaId,
                UsuarioId = userId,
                Contenido = dto.Contenido,
                Fecha = DateTime.Now
            };

            try
            {
                _context.Mensajes.Add(mensaje);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Acá podés loguear ex.InnerException?.Message para ver si vuelve a aparecer algo de FK
                Console.WriteLine("Error al guardar mensaje: " + ex.InnerException?.Message ?? ex.Message);
                return StatusCode(500, "Error al guardar el mensaje.");
            }

            var mensajeConUsuario = await _context.Mensajes
                .Include(m => m.Usuario)
                .FirstOrDefaultAsync(m => m.Id == mensaje.Id);

            return Ok(mensajeConUsuario);
        }

        // GET: api/mensajes/reserva/5
        [HttpGet("reserva/{reservaId}")]
        public async Task<IActionResult> ObtenerMensajes(int reservaId)
        {
            var mensajes = await _context.Mensajes
                .Where(m => m.ReservaId == reservaId)
                .Include(m => m.Usuario)
                .OrderBy(m => m.Fecha)
                .ToListAsync();

            return Ok(mensajes);
        }
    }
}
