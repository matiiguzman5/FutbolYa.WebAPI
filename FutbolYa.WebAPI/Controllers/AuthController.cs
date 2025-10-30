using System.Text.Json.Serialization;
using FutbolYa.WebAPI.Helpers;
using FutbolYa.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FutbolYa.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("registro")]
        public async Task<IActionResult> Registrar([FromBody] RegisterDTO dto)
        {
            var existe = await _context.Usuarios.AnyAsync(u => u.Correo == dto.Correo);
            if (existe)
            {
                return BadRequest("Ya existe un usuario con ese correo.");
            }

            if (string.IsNullOrWhiteSpace(dto.Contrasena))
            {
                return BadRequest("La contrase\u00F1a es obligatoria.");
            }

            var usuario = new Usuario
            {
                Nombre = dto.Nombre,
                Correo = dto.Correo,
                Contrasena = dto.Contrasena,
                Rol = "jugador"
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Usuario registrado correctamente", usuario.Id });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            if (string.IsNullOrWhiteSpace(login.Contrasena))
            {
                return BadRequest("La contrase\u00F1a es obligatoria.");
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Correo == login.Correo && u.Contrasena == login.Contrasena);

            if (usuario == null)
            {
                return Unauthorized("Correo o contrase\u00F1a incorrectos.");
            }

            var key = _configuration["Jwt:Key"];
            var token = JwtHelper.GenerarToken(usuario.Id.ToString(), usuario.Rol, key);

            return Ok(new
            {
                token,
                usuario = new { usuario.Id, usuario.Nombre, usuario.Rol }
            });
        }

        public class RegisterDTO
        {
            public string Nombre { get; set; } = string.Empty;
            public string Correo { get; set; } = string.Empty;

            [JsonPropertyName("contrasena")]
            public string Contrasena { get; set; } = string.Empty;

            [JsonPropertyName("contrase\u00F1a")]
            public string? ContrasenaConTilde
            {
                set
                {
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        Contrasena = value;
                    }
                }
            }
        }

        public class LoginDTO
        {
            public string Correo { get; set; } = string.Empty;

            [JsonPropertyName("contrasena")]
            public string Contrasena { get; set; } = string.Empty;

            [JsonPropertyName("contrase\u00F1a")]
            public string? ContrasenaConTilde
            {
                set
                {
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        Contrasena = value;
                    }
                }
            }
        }
    }
}
