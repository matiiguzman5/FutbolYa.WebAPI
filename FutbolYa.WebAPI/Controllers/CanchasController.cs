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
                PrecioNocturno = dto.PrecioNocturno,
                PrecioFinDeSemana = dto.PrecioFinDeSemana,
                PrecioPremium = dto.PrecioPremium,
                HorarioApertura = dto.HorarioApertura,
                HorarioCierre = dto.HorarioCierre,
                BloquesMantenimiento = dto.BloquesMantenimiento,
                DiasNoDisponibles = dto.DiasNoDisponibles,
                LogReparaciones = dto.LogReparaciones,
                EstadoEquipamiento = dto.EstadoEquipamiento,
                NotasEspeciales = dto.NotasEspeciales,
                ProximoMantenimiento = dto.ProximoMantenimiento,
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

            // Patch parcial: aplico sólo lo que vino
            if (dto.Nombre != null) cancha.Nombre = dto.Nombre;
            if (dto.Tipo != null) cancha.Tipo = dto.Tipo;
            if (dto.Superficie != null) cancha.Superficie = dto.Superficie;
            if (dto.Estado != null) cancha.Estado = dto.Estado;

            if (dto.PrecioBaseHora.HasValue) cancha.PrecioBaseHora = dto.PrecioBaseHora.Value;
            if (dto.PrecioNocturno.HasValue) cancha.PrecioNocturno = dto.PrecioNocturno.Value;
            if (dto.PrecioFinDeSemana.HasValue) cancha.PrecioFinDeSemana = dto.PrecioFinDeSemana.Value;
            if (dto.PrecioPremium.HasValue) cancha.PrecioPremium = dto.PrecioPremium.Value;

            if (dto.HorarioApertura.HasValue) cancha.HorarioApertura = dto.HorarioApertura.Value;
            if (dto.HorarioCierre.HasValue)  cancha.HorarioCierre  = dto.HorarioCierre.Value;

            if (dto.BloquesMantenimiento != null) cancha.BloquesMantenimiento = dto.BloquesMantenimiento;
            if (dto.DiasNoDisponibles != null)    cancha.DiasNoDisponibles = dto.DiasNoDisponibles;
            if (dto.LogReparaciones != null)      cancha.LogReparaciones = dto.LogReparaciones;
            if (dto.EstadoEquipamiento != null)   cancha.EstadoEquipamiento = dto.EstadoEquipamiento;
            if (dto.NotasEspeciales != null)      cancha.NotasEspeciales = dto.NotasEspeciales;
            if (dto.ProximoMantenimiento.HasValue) cancha.ProximoMantenimiento = dto.ProximoMantenimiento.Value;

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
                c.Estado,
                c.PrecioBaseHora,
                c.HorarioApertura,
                c.HorarioCierre
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
                    c.Estado,
                    c.PrecioBaseHora,
                    c.HorarioApertura,
                    c.HorarioCierre
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
