using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace FutbolYa.WebAPI.Controllers
{
    public class ChatHub : Hub
    {
        public async Task EnviarMensaje(string reservaId, string usuario, string mensaje)
        {
            await Clients.Group(reservaId)
                .SendAsync("RecibirMensaje", usuario, mensaje, DateTime.Now);
        }

        public async Task UnirseAReserva(string reservaId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, reservaId);
        }

        public async Task SalirDeReserva(string reservaId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, reservaId);
        }
    }
}
