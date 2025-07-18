using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FutbolYa.WebAPI.Models
{
    public class Reserva
    {
        public int Id { get; set; }

        [Required]
        public int CanchaId { get; set; }
        public Cancha Cancha { get; set; }

        [Required]
        public DateTime FechaHora { get; set; } // Fecha + hora de inicio de la reserva

        [Required]
        public int DuracionMinutos { get; set; } // 60 minutos máximo

        [Required]
        public string ClienteNombre { get; set; }

        [Required]
        public string ClienteTelefono { get; set; }

        public ICollection<ReservaUsuario> Jugadores { get; set; } = new List<ReservaUsuario>();

        public string? ClienteEmail { get; set; }
        public bool EsFrecuente { get; set; }

        public string EstadoPago { get; set; } // Pagado / Pendiente / Seña
        public string? Observaciones { get; set; }

        public int UsuarioEstablecimientoId { get; set; }
        [JsonIgnore]
        public Usuario UsuarioEstablecimiento { get; set; }
    }
}
