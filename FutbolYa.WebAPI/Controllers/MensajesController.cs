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

            var partidoExiste = await _context.Partidos.AnyAsync(p => p.Id == dto.PartidoId);
            var usuarioExiste = await _context.Usuarios.AnyAsync(u => u.Id == int.Parse(userId));

            if (!partidoExiste || !usuarioExiste)
                return BadRequest("Partido o usuario inválido.");

            var mensaje = new Mensaje
            {
                PartidoId = dto.PartidoId,
                UsuarioId = int.Parse(userId),
                Contenido = dto.Contenido,
                Fecha = DateTime.Now
            };

            _context.Mensajes.Add(mensaje);
            await _context.SaveChangesAsync();

            return Ok("Mensaje enviado.");
        }

        // GET: api/mensajes/partido/1
        [HttpGet("partido/{partidoId}")]
        public async Task<IActionResult> ObtenerMensajes(int partidoId)
        {
            var mensajes = await _context.Mensajes
                .Where(m => m.PartidoId == partidoId)
                .Include(m => m.Usuario)
                .OrderBy(m => m.Fecha)
                .ToListAsync();

            return Ok(mensajes);
        }
    }
}
