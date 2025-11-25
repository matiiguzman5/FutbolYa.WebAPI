using FutbolYa.WebAPI.Helpers;
using FutbolYa.WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Text.Json.Serialization;

namespace FutbolYa.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;

        public AuthController(
            AppDbContext context,
            IConfiguration configuration,
            EmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
        }

        // ===============================
        //  REGISTRO
        // ===============================
        [HttpPost("registro")]
        public async Task<IActionResult> Registrar([FromBody] RegisterDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre) ||
                string.IsNullOrWhiteSpace(dto.Correo))
            {
                return BadRequest("Nombre y correo son obligatorios.");
            }

            if (string.IsNullOrWhiteSpace(dto.Contrasena))
            {
                return BadRequest("La contraseña es obligatoria.");
            }

            if (dto.Contrasena != dto.ConfirmarContrasena)
            {
                return BadRequest("Las contraseñas no coinciden.");
            }

            var existe = await _context.Usuarios
                .AnyAsync(u => u.Correo == dto.Correo);

            if (existe)
            {
                return BadRequest("Ya existe un usuario con ese correo.");
            }

            var requiereConfirmacion = _configuration.GetValue<bool>("Auth:RequireEmailConfirmation", true);
            var smtpEnabled = _configuration.GetValue<bool>("Smtp:Enabled", true);
            var enviarMailConfirmacion = requiereConfirmacion && smtpEnabled;

            var usuario = new Usuario
            {
                Nombre = dto.Nombre,
                Correo = dto.Correo,
                Contrasena = dto.Contrasena,
                Rol = "jugador",
                EmailConfirmado = !enviarMailConfirmacion
            };

            if (enviarMailConfirmacion)
            {
                usuario.EmailConfirmToken = Guid.NewGuid().ToString();
                usuario.EmailConfirmTokenExpira = DateTime.UtcNow.AddHours(24);
            }

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            if (enviarMailConfirmacion && usuario.EmailConfirmToken != null)
            {
                var appUrl = _configuration["AppUrl"] ?? "https://futbolya.com";
                var urlConfirmacion = $"{appUrl.TrimEnd('/')}/api/auth/confirmar-email?token={usuario.EmailConfirmToken}";

                var cuerpoHtml = $@"
                <div style='font-family:Arial,sans-serif;max-width:600px;margin:auto;color:#333;'>
                    <h2 style='color:#007BFF;'>Confirmá tu cuenta en FutbolYa</h2>

                    <p>Hola <strong>{usuario.Nombre}</strong>,</p>

                    <p>Gracias por registrarte en <b>FutbolYa</b>.</p>
                    <p>Tu cuenta está casi lista para que puedas buscar partidos, unirte a reservas y ¡jugar ya!.</p>

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
                            Confirmar cuenta
                        </a>
                    </p>

                    <p>O copiá y pegá el siguiente enlace:</p>
                    <p style='word-break:break-all;'>{urlConfirmacion}</p>

                    <br/>
                    <small style='color:#777;'>Si no creaste esta cuenta, ignorá este correo.</small><br/>
                    <small style='color:#777;'>Por seguridad, nunca compartas este enlace.</small>
                </div>";


                try
                {
                    await _emailService.EnviarAsync(
                        usuario.Correo,
                        "Confirma tu cuenta en FutbolYa",
                        cuerpoHtml
                    );
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR SMTP: " + ex.Message);
                    _context.Usuarios.Remove(usuario);
                    await _context.SaveChangesAsync();
                    return StatusCode(500, "Error enviando mail: " + ex.Message);
                }
            }

            var mensaje = enviarMailConfirmacion
                ? "Usuario registrado correctamente. Revisa tu correo para confirmar la cuenta."
                : "Usuario registrado correctamente. Tu cuenta ya esta activa.";

            return Ok(new
            {
                mensaje,
                usuario.Id,
                requiereConfirmacionEmail = enviarMailConfirmacion
            });
        }
        // ===============================
        //  CONFIRMAR EMAIL
        // ===============================
        [HttpGet("confirmar-email")]
        public async Task<IActionResult> ConfirmarEmail([FromQuery] string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return BadRequest("Token inválido.");
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.EmailConfirmToken == token);

            if (usuario == null ||
                usuario.EmailConfirmTokenExpira == null ||
                usuario.EmailConfirmTokenExpira < DateTime.UtcNow)
            {
                return BadRequest("Token inválido o expirado.");
            }

            usuario.EmailConfirmado = true;
            usuario.EmailConfirmToken = null;
            usuario.EmailConfirmTokenExpira = null;

            await _context.SaveChangesAsync();

            // 🔽 Redirigir al front
            var frontendUrl = _configuration["FrontendUrl"] ?? "http://localhost:3000";
            var redirectUrl = $"{frontendUrl.TrimEnd('/')}/login?confirmado=1";

            return Redirect(redirectUrl);
        }


        // ===============================
        //  LOGIN
        // ===============================
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            if (string.IsNullOrWhiteSpace(login.Contrasena))
            {
                return BadRequest("La contraseña es obligatoria.");
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u =>
                    u.Correo == login.Correo &&
                    u.Contrasena == login.Contrasena);

            if (usuario == null)
            {
                return Unauthorized("Correo o contraseña incorrectos.");
            }

            if (!usuario.EmailConfirmado)
            {
                return BadRequest("Debes confirmar tu correo antes de iniciar sesión.");
            }

            var key = _configuration["Jwt:Key"];
            var token = JwtHelper.GenerarToken(usuario.Id.ToString(), usuario.Rol, key);

            return Ok(new
            {
                token,
                usuario = new { usuario.Id, usuario.Nombre, usuario.Rol }
            });
        }

        // POST: api/auth/restablecer-password
        [HttpPost("restablecer-password")]
        [AllowAnonymous]
        public async Task<IActionResult> RestablecerPassword([FromBody] ResetPasswordDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Token))
                return BadRequest("Token inválido.");

            if (string.IsNullOrWhiteSpace(dto.NuevaContrasena))
                return BadRequest("La nueva contraseña es obligatoria.");

            if (dto.NuevaContrasena != dto.ConfirmarContrasena)
                return BadRequest("Las contraseñas no coinciden.");

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.ResetPasswordToken == dto.Token);

            if (usuario == null ||
                usuario.ResetPasswordTokenExpira == null ||
                usuario.ResetPasswordTokenExpira < DateTime.UtcNow)
            {
                return BadRequest("Token inválido o expirado.");
            }

            usuario.Contrasena = dto.NuevaContrasena;
            usuario.ResetPasswordToken = null;
            usuario.ResetPasswordTokenExpira = null;

            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Contraseña restablecida correctamente. Ya podés iniciar sesión." });
        }


        // POST: api/auth/olvide-password
        [HttpPost("olvide-password")]
        [AllowAnonymous]
        public async Task<IActionResult> OlvidePassword([FromBody] ForgotPasswordDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Correo))
                return BadRequest("El correo es obligatorio.");

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Correo == dto.Correo);

            // Para no revelar si existe o no, devolvemos OK igual
            if (usuario == null)
            {
                return Ok(new { mensaje = "Si el correo está registrado, te enviamos un mail para restablecer la contraseña." });
            }

            var token = Guid.NewGuid().ToString();
            usuario.ResetPasswordToken = token;
            usuario.ResetPasswordTokenExpira = DateTime.UtcNow.AddHours(1);
            await _context.SaveChangesAsync();

            var frontendUrl = _configuration["FrontendUrl"] ?? "http://localhost:3000";
            var urlReset = $"{frontendUrl.TrimEnd('/')}/restablecer-password?token={token}";

            var cuerpoHtml = $@"
            <div style='font-family:Arial,sans-serif;max-width:600px;margin:auto;color:#333;'>
                <h2 style='color:#007BFF;'>Restablecé tu contraseña</h2>

                <p>Hola <strong>{usuario.Nombre}</strong>,</p>

                <p>Recibimos una solicitud para restablecer tu contraseña en <b>FutbolYa</b>.</p>
                <p>Si fuiste vos, hacé clic en el siguiente botón:</p>

                <p>
                    <a href='{urlReset}'
                       style='display:inline-block;
                              padding:10px 20px;
                              background:#007BFF;
                              color:#fff;
                              border-radius:2rem;
                              text-decoration:none;
                              font-weight:bold;'>
                        Restablecer contraseña
                    </a>
                </p>

                <p>Si no fuiste vos, ignorá este mensaje y te recomendamos cambiar tu contraseña por seguridad.</p>
                <p>Como buena práctica, evitá usar la misma contraseña en otros sitios.</p>

                <br/>
                <small style='color:#777;'>Este enlace es válido por 30 minutos.</small>
            </div>";


            try
            {
                await _emailService.EnviarAsync(
                    usuario.Correo,
                    "Restablecé tu contraseña - FutbolYa",
                    cuerpoHtml
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR SMTP (reset): " + ex.Message);
                // igual devolvemos OK para no exponer info
            }

            return Ok(new
            {
                mensaje = "Si el correo está registrado, te enviamos un mail para restablecer la contraseña."
            });
        }


        // ===============================
        //  DTOs
        // ===============================
        public class RegisterDTO
        {
            public string Nombre { get; set; } = string.Empty;
            public string Correo { get; set; } = string.Empty;

            [JsonPropertyName("contrasena")]
            public string Contrasena { get; set; } = string.Empty;

            // permitir también "contraseña" desde el front
            [JsonPropertyName("contraseña")]
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

            [JsonPropertyName("confirmarContrasena")]
            public string ConfirmarContrasena { get; set; } = string.Empty;

            [JsonPropertyName("confirmarContraseña")]
            public string? ConfirmarContrasenaConTilde
            {
                set
                {
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        ConfirmarContrasena = value;
                    }
                }
            }
        }
        public class ForgotPasswordDTO
        {
            public string Correo { get; set; } = string.Empty;
        }

        public class ResetPasswordDTO
        {
            public string Token { get; set; } = string.Empty;
            public string NuevaContrasena { get; set; } = string.Empty;
            public string ConfirmarContrasena { get; set; } = string.Empty;
        }


        public class LoginDTO
        {
            public string Correo { get; set; } = string.Empty;

            [JsonPropertyName("contrasena")]
            public string Contrasena { get; set; } = string.Empty;

            [JsonPropertyName("contraseña")]
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
