using Microsoft.AspNetCore.SignalR;
using FutbolYa.WebAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FutbolYa.WebAPI.Controllers
{
    public class ChatHub : Hub
    {
        private readonly AppDbContext _context;

        public ChatHub(AppDbContext context)
        {
            _context = context;
        }

        // 🔹 Enviar mensaje (con validación de inscripción)
        public async Task EnviarMensaje(string reservaId, string usuarioNombre, string contenido)
        {
            try
            {
                // ✅ 1. Validar token y usuario logueado
                var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                {
                    Console.WriteLine("⚠️ Usuario no autenticado.");
                    return;
                }

                // ✅ 2. Verificar si el usuario está inscripto en la reserva
                int idReserva = int.Parse(reservaId);

                bool estaInscripto = await _context.Reservas
                    .Include(r => r.Jugadores)
                    .AnyAsync(r => r.Id == idReserva && r.Jugadores.Any(j => j.UsuarioId == userId));

                if (!estaInscripto)
                {
                    Console.WriteLine($"🚫 Usuario {userId} intentó enviar mensaje sin estar inscripto.");
                    return; // no guarda ni envía
                }

                // ✅ 3. Obtener usuario (por nombre o ID)
                var usuario = await _context.Usuarios.FindAsync(userId);
                if (usuario == null)
                {
                    Console.WriteLine($"❌ Usuario con ID {userId} no encontrado.");
                    return;
                }

                // ✅ 4. Crear y guardar el mensaje en la base de datos
                var mensaje = new Mensaje
                {
                    ReservaId = idReserva,
                    UsuarioId = userId,
                    Contenido = contenido,
                    Fecha = DateTime.Now
                };

                _context.Mensajes.Add(mensaje);
                await _context.SaveChangesAsync();

                // ✅ 5. Emitir mensaje a todos los usuarios del grupo (SignalR)
                await Clients.Group(reservaId)
                    .SendAsync("RecibirMensaje", usuario.Nombre, contenido, mensaje.Fecha);

                Console.WriteLine($"💬 [{reservaId}] {usuario.Nombre}: {contenido}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en EnviarMensaje: {ex.Message}");
            }
        }

        // 🔹 Cuando alguien entra al chat
        public async Task UnirseAReserva(string reservaId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, reservaId);
            Console.WriteLine($"✅ Usuario {Context.ConnectionId} se unió al grupo {reservaId}");
        }

        // 🔹 Cuando alguien sale del chat
        public async Task SalirDeReserva(string reservaId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, reservaId);
            Console.WriteLine($"👋 Usuario {Context.ConnectionId} salió del grupo {reservaId}");
        }
    }
}
