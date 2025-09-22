using FutbolYa.WebAPI.Models;
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
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var cancha = new Cancha
            {
                Nombre = dto.Nombre,
                Tipo = dto.Tipo,
                Superficie = dto.Superficie,
                //Estado = dto.Estado,
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

      
        // PUT: api/canchas/5
        [Authorize(Roles = "establecimiento, administrador")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Editar(int id, [FromBody] CanchaDTO dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var cancha = await _context.Canchas.FirstOrDefaultAsync(c => c.Id == id && c.UsuarioEstablecimientoId == userId);
            if (cancha == null)
                return NotFound("Cancha no encontrada");

            cancha.Nombre = dto.Nombre;
            cancha.Tipo = dto.Tipo;
            cancha.Superficie = dto.Superficie;
            //cancha.Estado = dto.Estado;
            cancha.PrecioBaseHora = dto.PrecioBaseHora;
            cancha.PrecioNocturno = dto.PrecioNocturno;
            cancha.PrecioFinDeSemana = dto.PrecioFinDeSemana;
            cancha.PrecioPremium = dto.PrecioPremium;
            cancha.HorarioApertura = dto.HorarioApertura;
            cancha.HorarioCierre = dto.HorarioCierre;
            cancha.BloquesMantenimiento = dto.BloquesMantenimiento;
            cancha.DiasNoDisponibles = dto.DiasNoDisponibles;
            cancha.LogReparaciones = dto.LogReparaciones;
            cancha.EstadoEquipamiento = dto.EstadoEquipamiento;
            cancha.NotasEspeciales = dto.NotasEspeciales;
            cancha.ProximoMantenimiento = dto.ProximoMantenimiento;

            await _context.SaveChangesAsync();

            return Ok("Cancha actualizada correctamente");
        }


        [Authorize]
        [HttpGet("mis-canchas")]
        public async Task<IActionResult> ObtenerPropias()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var canchas = await _context.Canchas
                .Where(c => c.UsuarioEstablecimientoId == userId)
                .ToListAsync();

            return Ok(canchas);
        }

        // GET: api/canchas/disponibles
        [HttpGet("disponibles")]
        [AllowAnonymous] // o [Authorize] si querés que solo usuarios logueados puedan ver
        public async Task<IActionResult> ObtenerDisponibles()
        {
            var canchas = await _context.Canchas
                .Include(c => c.UsuarioEstablecimiento)
                .ToListAsync();

            var resultado = canchas.Select(c => new
            {
                c.Id,
                c.Nombre,
                c.Superficie
               // c.Estado
            });

            return Ok(resultado);
        }

        // DELETE: api/canchas/5
        [Authorize(Roles = "establecimiento, administrador")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var cancha = await _context.Canchas.FirstOrDefaultAsync(c => c.Id == id && c.UsuarioEstablecimientoId == userId);
            if (cancha == null)
                return NotFound("Cancha no encontrada");

            _context.Canchas.Remove(cancha);
            await _context.SaveChangesAsync();

            return Ok("Cancha eliminada");
        }

    }
}
