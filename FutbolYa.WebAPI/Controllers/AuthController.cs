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
                return BadRequest("Ya existe un usuario con ese correo.");

            var usuario = new Usuario
            {
                Nombre = dto.Nombre,
                Correo = dto.Correo,
                Contraseña = dto.Contraseña,
                Rol = "jugador" //rol por defecto
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Usuario registrado correctamente", usuario.Id });
        }



        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Correo == login.Correo && u.Contraseña == login.Contraseña);

            if (usuario == null)
                return Unauthorized("Correo o contraseña incorrectos.");

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
            public string Nombre { get; set; }
            public string Correo { get; set; }
            public string Contraseña { get; set; }
            
        }

        public class LoginDTO
        {
            public string Correo { get; set; }
            public string Contraseña { get; set; }
        }
    }
}
