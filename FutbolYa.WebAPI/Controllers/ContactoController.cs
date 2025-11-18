using FutbolYa.WebAPI.Helpers;
using FutbolYa.WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FutbolYa.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactoController : ControllerBase
    {
        private readonly EmailService _emailService;
        private readonly IConfiguration _config;

        public ContactoController(EmailService emailService, IConfiguration config)
        {
            _emailService = emailService;
            _config = config;
        }

        [HttpPost("establecimiento")]
        [AllowAnonymous]
        public async Task<IActionResult> ContactoEstablecimiento([FromBody] ContactoEstablecimientoDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre) || string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest("Nombre y correo son obligatorios.");

            var destino = _config["Contacto:EmailDestino"] ?? "soporte@futbolya.com.ar";

            var cuerpo = $@"
                <p>Nuevo establecimiento interesado en FutbolYa:</p>
                <p><b>Nombre:</b> {dto.Nombre}</p>
                <p><b>Email:</b> {dto.Email}</p>
                <p><b>Teléfono:</b> {dto.Telefono}</p>
                <p><b>Mensaje:</b> {dto.Mensaje}</p>
            ";

            await _emailService.EnviarAsync(destino, "Nuevo establecimiento interesado", cuerpo);

            return Ok("Mensaje enviado. Te vamos a contactar a la brevedad.");
        }
    }
}
