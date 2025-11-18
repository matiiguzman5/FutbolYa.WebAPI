using FutbolYa.WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using FutbolYa.WebAPI.Helpers;
using System.Globalization;
using System.Linq;
using FutbolYa.WebAPI.DTOs;


namespace FutbolYa.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ReservasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReservasController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/reservas
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] ReservaDTO dto)
        {
            int duracion = 60;
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userRol = User.FindFirst(ClaimTypes.Role)?.Value;

            var usuario = await _context.Usuarios.FindAsync(userId);
            if (usuario == null)
                return Unauthorized("Usuario no encontrado");

            var cancha = await _context.Canchas.FindAsync(dto.CanchaId);
            if (cancha == null) return NotFound("Cancha no encontrada");

            if (dto.FechaHora < DateTime.Now)
                return BadRequest("No se puede crear una reserva en el pasado.");

            var fin = dto.FechaHora.AddMinutes(duracion);
            var conflicto = await _context.Reservas.AnyAsync(r =>
                r.CanchaId == dto.CanchaId &&
                dto.FechaHora < r.FechaHora.AddMinutes(r.DuracionMinutos) &&
                fin > r.FechaHora
            );
            if (conflicto) return BadRequest("Ya hay una reserva en ese horario");

            var reserva = new Reserva
            {
                CanchaId = dto.CanchaId,
                FechaHora = dto.FechaHora.ToLocalTime(),
                DuracionMinutos = duracion,
                ClienteNombre = userRol == "jugador" ? usuario.Nombre : dto.ClienteNombre,
                ClienteTelefono = userRol == "jugador" ? usuario.Telefono ?? "No informado" : dto.ClienteTelefono,
                ClienteEmail = userRol == "jugador" ? usuario.Correo : dto.ClienteEmail,
                EsFrecuente = false,
                Observaciones = dto.Observaciones,
                UsuarioEstablecimientoId = userRol == "establecimiento"
                    ? userId
                    : cancha.UsuarioEstablecimientoId,
                MetodoPago = dto.MetodoPago,
                EstadoPago = dto.EstadoPago,
                SedeConfirmoTransferencia = dto.SedeConfirmoTransferencia,
                FechaPago = dto.FechaPago
            };

            _context.Reservas.Add(reserva);
            await _context.SaveChangesAsync();

            _context.ReservaUsuarios.Add(new ReservaUsuario
            {
                ReservaId = reserva.Id,
                UsuarioId = userId
            });
            await _context.SaveChangesAsync();

            Console.WriteLine($"[CREAR] Reserva {reserva.Id} - Usuario {userId} - Cancha {dto.CanchaId} - {dto.FechaHora}");

            return Ok(new { mensaje = "Reserva creada", reserva.Id });
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelarReserva(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userRol = User.FindFirst(ClaimTypes.Role)?.Value;

            var reserva = await _context.Reservas
                .Include(r => r.Jugadores)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reserva == null)
                return NotFound("Reserva no encontrada.");

            
            var creador = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == reserva.ClienteEmail);
            if (creador == null || creador.Id != userId)
                return Forbid("No tenés permiso para cancelar esta reserva.");

            
            if (reserva.Jugadores.Any())
                return BadRequest("No se puede cancelar la reserva porque hay jugadores anotados.");

            
            _context.Reservas.Remove(reserva);
            await _context.SaveChangesAsync();

            return Ok("Reserva cancelada correctamente.");
        }

        // POST: api/reservas/cancha/{canchaId}
        // Crea una reserva de 60' pasando solo fecha/hora + observaciones.
        // La cancha va en la URL, y se autocompletan los datos del jugador.
        [HttpPost("cancha/{canchaId}")]
        public async Task<IActionResult> CrearEnCancha(int canchaId, [FromBody] ReservaNewDTO dto)
        {
            const int duracion = 60;

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var userRol = User.FindFirst(ClaimTypes.Role)?.Value;

            var usuario = await _context.Usuarios.FindAsync(userId);
            if (usuario == null)
                return Unauthorized("Usuario no encontrado");

            var cancha = await _context.Canchas.FindAsync(canchaId);
            if (cancha == null)
                return NotFound("Cancha no encontrada");

            // Validaciones básicas (mismo criterio que el POST general)
            if (dto.FechaHora < DateTime.Now)
                return BadRequest("No se puede crear una reserva en el pasado.");

           /* if (dto.FechaHora.TimeOfDay < cancha.HorarioApertura ||
                dto.FechaHora.AddMinutes(duracion).TimeOfDay > cancha.HorarioCierre)
                return BadRequest("La reserva está fuera del horario permitido de la cancha.");

            var diaReserva = dto.FechaHora.ToString("dddd", new CultureInfo("es-ES"));
            if (!string.IsNullOrEmpty(cancha.DiasNoDisponibles) && cancha.DiasNoDisponibles.Contains(diaReserva))
                return BadRequest($"La cancha no está disponible los días {diaReserva}."); */

            var fin = dto.FechaHora.AddMinutes(duracion);
            var conflicto = await _context.Reservas.AnyAsync(r =>
                r.CanchaId == canchaId &&
                dto.FechaHora < r.FechaHora.AddMinutes(r.DuracionMinutos) &&
                fin > r.FechaHora
            );
            if (conflicto)
                return BadRequest("Ya hay una reserva en ese horario");

            var reserva = new Reserva
            {
                CanchaId = canchaId,
                FechaHora = dto.FechaHora.ToLocalTime(),
                DuracionMinutos = duracion,
                ClienteNombre = usuario.Nombre,
                ClienteTelefono = usuario.Telefono ?? "No informado",
                ClienteEmail = usuario.Correo,
                EsFrecuente = false,
                EstadoPago = "pendiente",
                Observaciones = dto.Observaciones,
                UsuarioEstablecimientoId = cancha.UsuarioEstablecimientoId
            };

            _context.Reservas.Add(reserva);
            await _context.SaveChangesAsync();

            // Si es jugador, se anota automáticamente como participante
            if (userRol == "jugador")
            {
                _context.ReservaUsuarios.Add(new ReservaUsuario
                {
                    ReservaId = reserva.Id,
                    UsuarioId = userId
                });
                await _context.SaveChangesAsync();
            }

            return Ok(new { mensaje = "Reserva creada", reserva.Id });
        }



        // GET: api/reservas/activas
        [HttpGet("activas")]
        [Authorize(Roles = "jugador")]
        public async Task<IActionResult> VerReservasActivas()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var reservas = await _context.ReservaUsuarios
                .Include(ru => ru.Reserva)
                    .ThenInclude(r => r.Cancha)
                .Where(ru => ru.UsuarioId == userId && ru.Reserva.FechaHora > DateTime.Now)
                .Select(ru => new
                {
                    ru.Reserva.Id,
                    ru.Reserva.FechaHora,
                    ru.Reserva.DuracionMinutos,
                    Cancha = ru.Reserva.Cancha.Nombre,
                    ru.Reserva.Observaciones,
                    ru.Reserva.EstadoPago
                })
                .ToListAsync();

            return Ok(reservas);
        }


        [HttpGet("mis")]
        public async Task<IActionResult> VerMisReservas()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var userRol = User.FindFirst(ClaimTypes.Role)?.Value;

                if (userRol == "establecimiento")
                {
                    var reservas = await _context.Reservas
                        .Include(r => r.Cancha)
                        .Where(r => r.UsuarioEstablecimientoId == userId)
                        .ToListAsync();

                    var resultado = reservas.Select(r => new
                    {
                        r.Id,
                        r.CanchaId,
                        CanchaNombre = r.Cancha?.Nombre,
                        Fecha = r.FechaHora.ToString("yyyy-MM-ddTHH:mm:ss"),
                        r.DuracionMinutos,
                        r.ClienteNombre,
                        r.ClienteTelefono,
                        r.ClienteEmail,
                        r.EstadoPago,
                        r.Observaciones
                    });

                    return Ok(resultado);
                }

                if (userRol == "administrador")
                {
                    var reservas = await _context.Reservas
                        .Include(r => r.Cancha)
                        .ToListAsync();

                    var resultado = reservas.Select(r => new
                    {
                        r.Id,
                        r.CanchaId,
                        CanchaNombre = r.Cancha?.Nombre,
                        Fecha = r.FechaHora.ToString("yyyy-MM-ddTHH:mm:ss"),
                        r.DuracionMinutos,
                        r.ClienteNombre,
                        r.ClienteTelefono,
                        r.ClienteEmail,
                        r.EstadoPago,
                        r.Observaciones
                    });

                    return Ok(resultado);
                }

                // jugador
                var reservasJugador = await _context.ReservaUsuarios
                    .Where(ru => ru.UsuarioId == userId)
                    .Include(ru => ru.Reserva)
                        .ThenInclude(r => r.Cancha)
                    .Select(ru => ru.Reserva)
                    .ToListAsync();

                var resultadoJugador = reservasJugador.Select(r => new
                {
                    r.Id,
                    r.CanchaId,
                    CanchaNombre = r.Cancha?.Nombre,
                    Fecha = r.FechaHora.ToString("yyyy-MM-ddTHH:mm:ss"),
                    r.DuracionMinutos,
                    r.ClienteNombre,
                    r.EstadoPago,
                    r.Observaciones
                });

                return Ok(resultadoJugador);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] VerMisReservas: {ex.Message}");
                return StatusCode(500, "Error interno del servidor en VerMisReservas.");
            }
        }



        [HttpGet("cancha/{id}")]
        [Authorize(Roles = "establecimiento")]
        public async Task<IActionResult> VerPorCancha(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var cancha = await _context.Canchas
    .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioEstablecimientoId == userId);

            if (cancha == null)
            {
                return Unauthorized("No podés ver las reservas de una cancha que no es tuya o no existe");
            }



            var reservas = await _context.Reservas
                .Where(r => r.CanchaId == id)
                .Include(r => r.Cancha)
                .ToListAsync();

            var resultado = reservas.Select(r => new
            {
                r.Id,
                r.CanchaId,
                CanchaNombre = r.Cancha.Nombre,
                fechaHora = r.FechaHora.ToString("dd/MM/yyyy HH:mm"),
                r.DuracionMinutos,
                r.ClienteNombre,
                r.EstadoPago,
                r.Observaciones
            });

            return Ok(resultado);
        }


        [HttpGet("agenda")]
        [Authorize(Roles = "establecimiento")]
        public async Task<IActionResult> VerAgenda([FromQuery] DateTime? fecha, [FromQuery] bool semana = false, [FromQuery] int? canchaId = null)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var dia = fecha?.Date ?? DateTime.Today;

                var inicio = semana ? dia.StartOfWeek(DayOfWeek.Monday) : dia;
                var fin = semana ? inicio.AddDays(7) : dia.AddDays(1);

                var query = _context.Reservas
                    .Include(r => r.Cancha)
                    .Where(r => r.UsuarioEstablecimientoId == userId &&
                                r.FechaHora >= inicio &&
                                r.FechaHora < fin);

                if (canchaId.HasValue)
                    query = query.Where(r => r.CanchaId == canchaId.Value);

                var reservas = await query.OrderBy(r => r.FechaHora).ToListAsync();

                var resultado = reservas.Select(r => new
                {
                    id = r.Id,
                    title = $"{r.Cancha.Nombre} - {r.ClienteNombre} ({r.EstadoPago})",
                    start = r.FechaHora,
                    end = r.FechaHora.AddMinutes(r.DuracionMinutos)
                });

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] VerAgenda: {ex.Message}");
                return StatusCode(500, "Error interno en VerAgenda.");
            }
        }
        [Authorize]
        [HttpGet("{id}/esta-inscripto")]
        public async Task<IActionResult> EstaInscripto(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized("Token inválido");

            var reserva = await _context.Reservas
                .Include(r => r.Jugadores)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reserva == null)
                return NotFound("Reserva no encontrada.");

            // 👇 Como Jugadores es una colección de ReservaUsuario, usamos UsuarioId
            bool estaInscripto = reserva.Jugadores.Any(j => j.UsuarioId == userId);

            return Ok(estaInscripto);
        }





        // PUT: api/reservas/5
        [HttpPut("{id}")]
        [Authorize(Roles = "establecimiento")]
        public async Task<IActionResult> Editar(int id, [FromBody] ReservaDTO dto)
        {
            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva == null) return NotFound("Reserva no encontrada");

            reserva.EstadoPago = dto.EstadoPago;
            reserva.Observaciones = dto.Observaciones;
            await _context.SaveChangesAsync();

            return Ok("Reserva actualizada");
        }

        // POST: api/reservas/5/unirse
        [HttpPost("{id}/unirse")]
        [Authorize(Roles = "jugador")]
        public async Task<IActionResult> Unirse(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var reserva = await _context.Reservas
                .Include(r => r.Jugadores)
                .ThenInclude(j => j.Usuario)
                .Include(r => r.Cancha)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reserva == null)
                return NotFound("Reserva no encontrada");

            // Verifica si ya está unido
            if (reserva.Jugadores.Any(j => j.UsuarioId == userId))
                return BadRequest("Ya estás unido a esta reserva");

            // Verifica límite según tipo de cancha
            var tipo = reserva.Cancha.Tipo?.Trim().ToUpper();

            int capacidadMaxima = tipo switch
            {
                "F5" or "FUTBOL-5" => 10,
                "F7" or "FUTBOL-7" => 14,
                "F11" or "FUTBOL-11" => 22,
                _ => 10
            };


            if (reserva.Jugadores.Count >= capacidadMaxima)
                return BadRequest("La reserva ya alcanzó el máximo de jugadores");

            // Agrega jugador a la reserva
            var nuevoJugador = new ReservaUsuario
            {
                ReservaId = id,
                UsuarioId = userId
            };

            _context.ReservaUsuarios.Add(nuevoJugador);
            await _context.SaveChangesAsync();

            return Ok("Te uniste correctamente a la reserva");
        }

        // GET: api/reservas/5/jugadores
        [HttpGet("{id}/jugadores")]
        [Authorize] // cualquier usuario logueado
        public async Task<IActionResult> VerJugadores(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var rol = User.FindFirst(ClaimTypes.Role)?.Value;

            var reserva = await _context.Reservas
                .Include(r => r.Cancha)
                .Include(r => r.Jugadores)
                    .ThenInclude(ru => ru.Usuario)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reserva == null)
                return NotFound("Reserva no encontrada");

            // ¿Quién puede ver?
            bool esAdmin = rol == "administrador";
            bool esEstablecimientoDeLaReserva = rol == "establecimiento" && reserva.UsuarioEstablecimientoId == userId;
            bool esJugadorDeLaReserva = reserva.Jugadores.Any(j => j.UsuarioId == userId);

            if (!esAdmin && !esEstablecimientoDeLaReserva && !esJugadorDeLaReserva)
                return Forbid("No tenés permiso para ver los jugadores de esta reserva.");

            var creador = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Correo == reserva.ClienteEmail);

            var jugadores = reserva.Jugadores.Select(j => j.Usuario).ToList();

            if (creador != null && !jugadores.Any(j => j.Id == creador.Id))
                jugadores.Insert(0, creador);

            var tipo = reserva.Cancha.Tipo?.Trim().ToUpper();

            int capacidad = tipo switch
            {
                "F5" or "FUTBOL-5" => 10,
                "F7" or "FUTBOL-7" => 14,
                "F11" or "FUTBOL-11" => 22,
                _ => 10
            };

            var resultado = jugadores.Select(j => new
            {
                j.Id,
                j.Nombre,
                j.Correo,
                j.Posicion,
                j.FotoPerfil,
                EsCreador = j.Correo == reserva.ClienteEmail
            });

            return Ok(new
            {
                Capacidad = capacidad,
                Anotados = resultado.Count(),
                EspaciosDisponibles = capacidad - resultado.Count(),
                Jugadores = resultado
            });
        }



        [HttpGet("disponibles")]
        [AllowAnonymous]
        public async Task<IActionResult> VerReservasDisponibles()
        {
            try
            {
                var ahora = DateTime.Now;

                var reservas = await _context.Reservas
                    .Include(r => r.Cancha)
                    .Include(r => r.UsuarioEstablecimiento) // ✅ para traer la foto
                    .Include(r => r.Jugadores)
                    .Where(r => r.FechaHora > ahora)
                    .ToListAsync();

                int? userId = null;
                if (User.Identity.IsAuthenticated)
                {
                    userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }

                var resultado = reservas
                    .Select(r =>
                    {
                        var tipo = r.Cancha.Tipo?.Trim().ToUpper();
                        int capacidad = tipo switch
                        {
                            "F5" or "FUTBOL-5" => 10,
                            "F7" or "FUTBOL-7" => 14,
                            "F11" or "FUTBOL-11" => 22,
                            _ => 10
                        };

                        int jugadores = r.Jugadores.Count;
                        bool yaEstoyUnido = userId != null && r.Jugadores.Any(j => j.UsuarioId == userId);

                        return new
                        {
                            r.Id,
                            r.FechaHora,
                            r.CanchaId,
                            NombreCancha = r.Cancha?.Nombre,
                            Tipo = r.Cancha?.Tipo,
                            Superficie = r.Cancha?.Superficie,
                            Capacidad = capacidad,
                            Anotados = jugadores,
                            EspaciosDisponibles = capacidad - jugadores,
                            Observaciones = r.Observaciones,
                            EstadoPago = r.EstadoPago,
                            YaEstoyUnido = yaEstoyUnido,
                            FotoEstablecimiento = r.UsuarioEstablecimiento?.FotoPerfil // ✅ nueva línea
                        };
                    })
                    .Where(r => r.Anotados < r.Capacidad || r.YaEstoyUnido);

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] VerReservasDisponibles: {ex.Message}");
                return StatusCode(500, "Error interno en VerReservasDisponibles.");
            }
        }





        [HttpDelete("{reservaId}/salir")]
        public async Task<IActionResult> SalirDeReserva(int reservaId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var participacion = await _context.ReservaUsuarios
                .FirstOrDefaultAsync(ru => ru.ReservaId == reservaId && ru.UsuarioId == userId);

            if (participacion == null)
                return NotFound("No estás anotado en esta reserva.");

            _context.ReservaUsuarios.Remove(participacion);
            await _context.SaveChangesAsync();

            // Verificar si queda alguien más en la reserva
            var quedanJugadores = await _context.ReservaUsuarios
                .AnyAsync(ru => ru.ReservaId == reservaId);

            if (!quedanJugadores)
            {
                var reserva = await _context.Reservas.FindAsync(reservaId);
                if (reserva != null)
                {
                    _context.Reservas.Remove(reserva);
                    await _context.SaveChangesAsync();
                }
            }

            return Ok("Saliste de la reserva.");
        }

        [HttpGet("mias")]
        [Authorize(Roles = "establecimiento")]
        public async Task<IActionResult> GetMisCanchas()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var canchas = await _context.Canchas
                .Where(c => c.UsuarioEstablecimientoId == userId)
                .Select(c => new
                {
                    id = c.Id,
                    nombre = c.Nombre,
                    tipo = c.Tipo,
                })
                .ToListAsync();

            return Ok(canchas);
        }


        [HttpPut("pagar/{id}")]
        public async Task<IActionResult> ConfirmarPago(int id, [FromBody] ConfirmarPagoDTO? body = null)
        {
            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva == null) return NotFound();

            string? estadoPago = body?.EstadoPago;
            if (string.IsNullOrWhiteSpace(estadoPago))
            {
                var estadoQuery = Request?.Query["tipoPago"].ToString();
                if (!string.IsNullOrWhiteSpace(estadoQuery))
                {
                    estadoPago = estadoQuery;
                }
            }

            string? metodoPago = body?.MetodoPago;
            if (string.IsNullOrWhiteSpace(metodoPago))
            {
                var metodoQuery = Request?.Query["metodo"].ToString();
                if (!string.IsNullOrWhiteSpace(metodoQuery))
                {
                    metodoPago = metodoQuery;
                }
            }

            if (string.IsNullOrWhiteSpace(estadoPago) || string.IsNullOrWhiteSpace(metodoPago))
            {
                return BadRequest("Debe indicar el estado y el metodo de pago.");
            }

            reserva.EstadoPago = estadoPago;
            reserva.MetodoPago = metodoPago;
            reserva.FechaPago = body?.FechaPago ?? DateTime.Now;

            if (body?.SedeConfirmoTransferencia.HasValue == true)
            {
                reserva.SedeConfirmoTransferencia = body.SedeConfirmoTransferencia.Value;
            }

            if (body != null)
            {
                var numeroSanitizado = string.IsNullOrWhiteSpace(body.NumeroTarjeta)
                    ? string.Empty
                    : new string(body.NumeroTarjeta.Where(char.IsDigit).ToArray());

                var tokenNormalizado = body.Token?.Trim();
                var logEstado = body.EstadoPago ?? estadoPago;
                var logMetodo = body.MetodoPago ?? metodoPago;
                var logUltimos4 = string.Empty;
                if (!string.IsNullOrEmpty(numeroSanitizado))
                {
                    var startIndex = numeroSanitizado.Length >= 4 ? numeroSanitizado.Length - 4 : 0;
                    logUltimos4 = numeroSanitizado.Substring(startIndex);
                }

                // TODO: Quitar log temporal de confirmacion de pago.
                Console.WriteLine($"[PAGO] ConfirmarPago body -> Reserva {reserva.Id} | Estado: {logEstado ?? "(sin dato)"} | Metodo: {logMetodo ?? "(sin dato)"} | Token presente: {!string.IsNullOrWhiteSpace(tokenNormalizado)} | Digitos: {numeroSanitizado.Length} | Ultimos4: {logUltimos4}");

                var requierePersistencia = !string.IsNullOrWhiteSpace(numeroSanitizado)
                    || !string.IsNullOrWhiteSpace(tokenNormalizado)
                    || !string.IsNullOrWhiteSpace(body.CodigoSeguridad);

                if (requierePersistencia)
                {
                    var hashToken = HashingHelper.ComputeSha256(tokenNormalizado ?? string.Empty);
                    var hashNumero = HashingHelper.ComputeSha256(numeroSanitizado);

                    var existeRegistro = await _context.DatosTarjetas
                        .AnyAsync(dt => dt.ReservaId == reserva.Id && dt.HashToken == hashToken && dt.HashNumero == hashNumero);

                    if (!existeRegistro)
                    {
                        string? ultimos4 = null;
                        if (!string.IsNullOrEmpty(numeroSanitizado))
                        {
                            ultimos4 = numeroSanitizado.Length >= 4
                                ? numeroSanitizado.Substring(numeroSanitizado.Length - 4)
                                : numeroSanitizado;
                        }

                        var registro = new DatosTarjeta
                        {
                            ReservaId = reserva.Id,
                            HashToken = hashToken,
                            HashNumero = hashNumero,
                            Ultimos4 = ultimos4,
                            HashCvv = string.IsNullOrWhiteSpace(body.CodigoSeguridad)
                                ? null
                                : HashingHelper.ComputeSha256(body.CodigoSeguridad.Trim()),
                            NombreTitular = string.IsNullOrWhiteSpace(body.NombreTitular)
                                ? null
                                : body.NombreTitular.Trim(),
                            FechaExpiracion = string.IsNullOrWhiteSpace(body.FechaExpiracion)
                                ? null
                                : body.FechaExpiracion.Trim(),
                        };

                        _context.DatosTarjetas.Add(registro);
                    }
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new { mensaje = $"Reserva actualizada como {estadoPago} con {metodoPago}" });
        }
    }
}
