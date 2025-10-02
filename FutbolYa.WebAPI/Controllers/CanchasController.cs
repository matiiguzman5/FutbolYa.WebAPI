using FutbolYa.WebAPI.Models;
using FutbolYa.WebAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FutbolYa.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CanchasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CanchasController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/canchas
        [Authorize(Roles = "establecimiento, administrador")]
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CanchaDTO dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var cancha = new Cancha
            {
                Nombre = dto.Nombre,
                Tipo = dto.Tipo,
                Superficie = dto.Superficie,
                Estado = dto.Estado,
                PrecioBaseHora = dto.PrecioBaseHora,
                PrecioFinDeSemana = dto.PrecioFinDeSemana,
                UsuarioEstablecimientoId = userId
            };

            _context.Canchas.Add(cancha);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Cancha creada correctamente", cancha.Id });
        }

        // PUT: api/canchas/{id}
        [Authorize(Roles = "establecimiento")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Editar(int id, [FromBody] CanchaUpdateDTO dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var cancha = await _context.Canchas
                .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioEstablecimientoId == userId);

            if (cancha == null)
                return NotFound("Cancha no encontrada o no te pertenece.");


            if (dto.Nombre != null) cancha.Nombre = dto.Nombre;
            if (dto.Tipo != null) cancha.Tipo = dto.Tipo;
            if (dto.Superficie != null) cancha.Superficie = dto.Superficie;
            if (dto.Estado != null) cancha.Estado = dto.Estado;


            if (dto.PrecioBaseHora.HasValue) cancha.PrecioBaseHora = dto.PrecioBaseHora.Value;
            if (dto.PrecioFinDeSemana.HasValue) cancha.PrecioFinDeSemana = dto.PrecioFinDeSemana.Value;


            await _context.SaveChangesAsync();
            return Ok("Cancha actualizada correctamente");
        }

        // GET: api/canchas/mis-canchas
        [Authorize(Roles = "establecimiento")]
        [HttpGet("mis-canchas")]
        public async Task<IActionResult> ObtenerPropias()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var canchas = await _context.Canchas
                .Where(c => c.UsuarioEstablecimientoId == userId)
                .ToListAsync();

            return Ok(canchas);
        }

        [HttpGet("de/{establecimientoId}/disponibles")]
        public async Task<IActionResult> DisponiblesEnHorario(int establecimientoId, [FromQuery] DateTime fechaHora)
        {
            var canchas = await _context.Canchas
                .Where(c => c.UsuarioEstablecimientoId == establecimientoId)
                .ToListAsync();

            var libres = new List<object>();
            var fin = fechaHora.AddMinutes(60);

            foreach (var cancha in canchas)
            {
                bool ocupado = await _context.Reservas.AnyAsync(r =>
                    r.CanchaId == cancha.Id &&
                    r.FechaHora < fin &&
                    r.FechaHora.AddMinutes(r.DuracionMinutos) > fechaHora
                );

                if (!ocupado)
                {
                    libres.Add(new
                    {
                        cancha.Id,
                        cancha.Nombre,
                        cancha.Tipo,
                        cancha.Superficie,
                        PrecioBaseHora = cancha.PrecioBaseHora
                    });
                }
            }

            return Ok(libres);
        }



        // GET: api/canchas/disponibles  (listado simple para la app)
        [AllowAnonymous]
        [HttpGet("disponibles")]
        public async Task<IActionResult> ObtenerDisponibles()
        {
            var canchas = await _context.Canchas.ToListAsync();

            var resultado = canchas.Select(c => new
            {
                c.Id,
                c.Nombre,
                c.Tipo,
                c.Superficie,

                c.PrecioBaseHora,
            });

            return Ok(resultado);
        }

        // DELETE: api/canchas/{id}
        [Authorize(Roles = "establecimiento, administrador")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var cancha = await _context.Canchas.FirstOrDefaultAsync(c => c.Id == id && c.UsuarioEstablecimientoId == userId);
            if (cancha == null)
                return NotFound("Cancha no encontrada");

            _context.Canchas.Remove(cancha);
            await _context.SaveChangesAsync();

            return Ok("Cancha eliminada");
        }

        // GET: api/canchas/de/2?tipo=F7
        [AllowAnonymous]
        [HttpGet("de/{establecimientoId}")]
        public async Task<IActionResult> CanchasDeEstablecimiento(
            int establecimientoId, [FromQuery] string? tipo)
        {
            var query = _context.Canchas
                .Where(c => c.UsuarioEstablecimientoId == establecimientoId);

            if (!string.IsNullOrWhiteSpace(tipo))
                query = query.Where(c => c.Tipo == tipo);

            var canchas = await query
                .Select(c => new {
                    c.Id,
                    c.Nombre,
                    c.Tipo,
                    c.Superficie,

                    c.PrecioBaseHora,
                })
                .ToListAsync();

            return Ok(canchas);
        }

        // GET: api/canchas/de/2/tipos
        [AllowAnonymous]
        [HttpGet("de/{establecimientoId}/tipos")]
        public async Task<IActionResult> TiposDisponibles(int establecimientoId)
        {
            var tipos = await _context.Canchas
                .Where(c => c.UsuarioEstablecimientoId == establecimientoId)
                .Select(c => c.Tipo)
                .Distinct()
                .ToListAsync();

            return Ok(tipos);
        }
    }
}
