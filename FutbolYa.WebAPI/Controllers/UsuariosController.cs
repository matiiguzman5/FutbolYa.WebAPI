using FutbolYa.WebAPI.Helpers;
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
        private readonly EmailService _emailService;
        private readonly IConfiguration _configuration;

        public UsuariosController(AppDbContext context, IWebHostEnvironment env, EmailService emailService, IConfiguration configuration)
        {
            _context = context;
            _env = env;
            _emailService = emailService;
            _configuration = configuration;
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
                    u.FotoPerfil,
                    u.Telefono,
                    u.Ubicacion,
                    u.Posicion
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
            if (!string.IsNullOrWhiteSpace(body?.Contrasena)) u.Contrasena = body.Contrasena;
            if (!string.IsNullOrWhiteSpace(body?.Ubicacion)) u.Ubicacion = body.Ubicacion;

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Usuario actualizado" });
        }

        // GET: api/usuarios/estadisticas
        [HttpGet("estadisticas")]
        public async Task<IActionResult> VerEstadisticas()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var partidosJugados = await _context.ReservaUsuarios
                 .CountAsync(ru => ru.UsuarioId == userId);

            var valoraciones = await _context.Calificaciones
                .Where(c => c.EvaluadoId == userId)
                .Select(c => c.Puntaje)
                .ToListAsync();

            double promedio = valoraciones.Any() ? valoraciones.Average() : 0;

            return Ok(new
            {
                PartidosJugados = partidosJugados,
                ValoracionPromedio = promedio
            });
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
                Contrasena = BCrypt.Net.BCrypt.HashPassword(dto.Contrasena),
                Rol = dto.Rol ?? "jugador",
                EmailConfirmado = false
            };

            nuevo.EmailConfirmToken = Guid.NewGuid().ToString("N");
            nuevo.EmailConfirmTokenExpira = DateTime.UtcNow.AddHours(24);

            _context.Usuarios.Add(nuevo);
            await _context.SaveChangesAsync();

            var backendBase = _configuration["BackendUrl"];
            if (string.IsNullOrEmpty(backendBase))
                return StatusCode(500, "BackendUrl no configurado en el servidor.");
            var urlConfirmacion = $"{backendBase}/api/auth/confirmar-email?token={nuevo.EmailConfirmToken}";

            var html = $@"
            <div style='font-family:Arial,sans-serif;max-width:600px;margin:auto;color:#333;'>
                <h2 style='color:#007BFF;'>Bienvenido a FutbolYa</h2>

                <p>Hola <strong>{nuevo.Nombre}</strong>, tu cuenta fue creada correctamente.</p>
                <p>Ya podés empezar a organizar tus partidos, sumarte a reservas y ¡jugar ya!.</p>

                <p>Para activarla, hacé clic en el siguiente botón:</p>

                <p>
                    <a href='{urlConfirmacion}'
                       style='display:inline-block;
                              padding:10px 20px;
                              background:#007BFF;
                              color:#fff;
                              border-radius:2rem;
                              text-decoration:none;
                              font-weight:bold;'>
                        Confirmar mi cuenta
                    </a>
                </p>

                <p>O copiá y pegá este enlace en tu navegador:</p>
                <p style='word-break:break-all;'>{urlConfirmacion}</p>

                <br/>
                <small style='color:#777;'>Este enlace expira en 24 horas.</small><br/>
                <small style='color:#777;'>Si no esperabas este correo, podés ignorarlo.</small>
            </div>";
            Console.WriteLine(">>> INTENTANDO ENVIAR EMAIL A: " + nuevo.Correo);


            await _emailService.EnviarAsync(
                nuevo.Correo,
                "Confirma tu cuenta en FutbolYa",
                html
            );

            return Ok(new { mensaje = "Usuario creado. Confirmación enviada.", nuevo.Id });
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

        [HttpPut("{id}/editar-ubicacion")]
        [Authorize(Roles = "administrador")]
        public async Task<IActionResult> EditarUbicacion(int id, [FromBody] string nuevaUbicacion)
        {
            var usuario = await _context.Usuarios.FindAsync(id); 
            if (usuario == null) return NotFound("Usuario no encontrado.");

            if (usuario.Rol != "establecimiento")
                return BadRequest("Solo los establecimientos tienen ubicación.");

            usuario.Ubicacion = nuevaUbicacion;
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Ubicación actualizada", usuario.Id, usuario.Nombre, usuario.Ubicacion });
        }


        [HttpPut("editar-perfil")]
        public async Task<IActionResult> EditarPerfil([FromBody] EditarPerfilDTO dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var usuario = await _context.Usuarios.FindAsync(userId);

            if (usuario == null)
                return NotFound("Usuario no encontrado.");

            if (!string.IsNullOrWhiteSpace(dto.Nombre))
                usuario.Nombre = dto.Nombre;

            if (!string.IsNullOrWhiteSpace(dto.Telefono))
                usuario.Telefono = dto.Telefono;

            if (!string.IsNullOrWhiteSpace(dto.Ubicacion))
                usuario.Ubicacion = dto.Ubicacion;

            if (!string.IsNullOrWhiteSpace(dto.Posicion))
                usuario.Posicion = dto.Posicion;

            if (!string.IsNullOrWhiteSpace(dto.Contrasena))
                usuario.Contrasena = BCrypt.Net.BCrypt.HashPassword(dto.Contrasena);

            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Perfil actualizado" });
        }




        [AllowAnonymous]
        [HttpGet("establecimientos")]
        public async Task<IActionResult> ListarEstablecimientos()
        {
            var estabs = await _context.Usuarios
                .Where(u => u.Rol.ToLower() == "establecimiento") // ✅ compara en minúsculas para evitar errores
                .Include(u => u.Canchas)
                .Select(u => new {
                    u.Id,
                    u.Nombre,
                    u.Correo,
                    u.Telefono,
                    u.Ubicacion,
                    u.FotoPerfil, // ✅ agregado
                    Canchas = u.Canchas.Select(c => new {
                        c.Id,
                        c.Nombre,
                        c.Tipo,
                        c.Superficie,
                        c.Estado,
                        c.Precio
                    }).ToList()
                })
                .ToListAsync();

            return Ok(estabs);
        }





    }

}

