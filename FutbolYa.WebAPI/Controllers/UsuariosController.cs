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
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public UsuariosController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: api/Usuarios/yo
        [HttpGet("yo")]
        public async Task<IActionResult> ObtenerPerfil()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var usuario = await _context.Usuarios
                .Where(u => u.Id == int.Parse(userId))
                .Select(u => new
                {
                    u.Id,
                    u.Nombre,
                    u.Correo,
                    u.Rol,
                    u.FotoPerfil
                })
                .FirstOrDefaultAsync();

            if (usuario == null)
                return NotFound("Usuario no encontrado.");

            return Ok(usuario);
        }


        // PUT: api/Usuarios/{id}  (ADMIN)
        [HttpPut("{id}")]
        [Authorize(Roles = "administrador")]
        public async Task<IActionResult> EditarUsuario(int id, [FromBody] Usuario body)
        {
            var u = await _context.Usuarios.FindAsync(id);
            if (u == null) return NotFound("Usuario no encontrado.");

            // Validar correo duplicado si viene uno nuevo
            if (!string.IsNullOrWhiteSpace(body?.Correo))
            {
                var existe = await _context.Usuarios.AnyAsync(x => x.Correo == body.Correo && x.Id != id);
                if (existe) return BadRequest("Ya existe un usuario con ese correo.");
                u.Correo = body.Correo;
            }

            // Campos que sí editar
            if (!string.IsNullOrWhiteSpace(body?.Nombre)) u.Nombre = body.Nombre;
            if (!string.IsNullOrWhiteSpace(body?.Telefono)) u.Telefono = body.Telefono;
            if (!string.IsNullOrWhiteSpace(body?.Rol)) u.Rol = body.Rol;
            if (!string.IsNullOrWhiteSpace(body?.Contraseña)) u.Contraseña = body.Contraseña; 

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Usuario actualizado" });
        }



        // POST: api/Usuarios/subir-foto
        [HttpPost("subir-foto")]
        public async Task<IActionResult> SubirFoto([FromForm] IFormFile archivo)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var usuario = await _context.Usuarios.FindAsync(userId);

            if (archivo == null || archivo.Length == 0)
                return BadRequest("No se envió ninguna imagen.");

            var carpetaPerfiles = Path.Combine(_env.WebRootPath, "perfiles");

            if (!Directory.Exists(carpetaPerfiles))
                Directory.CreateDirectory(carpetaPerfiles);

            var nombreArchivo = $"perfil_{userId}_{Guid.NewGuid()}{Path.GetExtension(archivo.FileName)}";
            var rutaCompleta = Path.Combine(carpetaPerfiles, nombreArchivo);

            using (var stream = new FileStream(rutaCompleta, FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }

            usuario.FotoPerfil = $"/perfiles/{nombreArchivo}";
            await _context.SaveChangesAsync();

            return Ok(new { ruta = usuario.FotoPerfil });
        }

        [HttpGet]
        [Authorize(Roles = "administrador")]
        public async Task<IActionResult> ObtenerTodos()
        {
            var usuarios = await _context.Usuarios
                .Select(u => new {
                    u.Id,
                    u.Nombre,
                    u.Correo,
                    u.Rol
                })
                .ToListAsync();

            return Ok(usuarios);
        }

        [HttpPost]
        [Authorize(Roles = "administrador")]
        public async Task<IActionResult> CrearUsuario([FromBody] Usuario dto)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Correo == dto.Correo))
                return BadRequest("Ya existe un usuario con ese correo.");

            var nuevo = new Usuario
            {
                Nombre = dto.Nombre,
                Correo = dto.Correo,
                Contraseña = dto.Contraseña,
                Rol = dto.Rol ?? "jugador"
            };

            _context.Usuarios.Add(nuevo);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Usuario creado", nuevo.Id });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "administrador")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound("Usuario no encontrado.");

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return Ok("Usuario eliminado.");
        }

        [AllowAnonymous]
        [HttpGet("establecimientos")]
        public async Task<IActionResult> ListarEstablecimientos()
        {
            var estabs = await _context.Usuarios
                .Where(u => u.Rol == "establecimiento")
                .Select(u => new {
                    u.Id,
                    u.Nombre,
                    u.Correo,
                    u.Telefono,
                    Canchas = _context.Canchas
                               .Where(c => c.UsuarioEstablecimientoId == u.Id)
                               .Select(c => new { c.Id, c.Nombre, c.Tipo })
                               .ToList()
                })
                .ToListAsync();

            return Ok(estabs);
        }


    }

}
