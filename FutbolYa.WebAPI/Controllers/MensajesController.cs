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
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var reservaExiste = await _context.Reservas.AnyAsync(r => r.Id == dto.ReservaId);
            var usuarioExiste = await _context.Usuarios.AnyAsync(u => u.Id == int.Parse(userId));

            if (!reservaExiste || !usuarioExiste)
                return BadRequest("Reserva o usuario inválido.");

            var mensaje = new Mensaje
            {
                ReservaId = dto.ReservaId,
                UsuarioId = int.Parse(userId),
                Contenido = dto.Contenido,
                Fecha = DateTime.Now
            };

            _context.Mensajes.Add(mensaje);
            await _context.SaveChangesAsync();

            // devolvemos el mensaje con usuario incluido
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
