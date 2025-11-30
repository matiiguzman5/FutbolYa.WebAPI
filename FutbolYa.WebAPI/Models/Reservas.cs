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
        public DateTime FechaHora { get; set; }

        [Required]
        public int DuracionMinutos { get; set; }

        [Required]
        public string ClienteNombre { get; set; }

        [Required]
        public string ClienteTelefono { get; set; }

        public string EstadoPago { get; set; } = "Pendiente";
        public string MetodoPago { get; set; }

        public bool SedeConfirmoTransferencia { get; set; }

        public ICollection<ReservaUsuario> Jugadores { get; set; } = new List<ReservaUsuario>();

        [JsonIgnore]
        public ICollection<DatosTarjeta> DatosTarjetas { get; set; } = new List<DatosTarjeta>();

        public string? ClienteEmail { get; set; }
        public bool EsFrecuente { get; set; }
        public DateTime? FechaPago { get; set; }

        public string? Observaciones { get; set; }

        public int UsuarioEstablecimientoId { get; set; }

        [JsonIgnore]
        public Usuario UsuarioEstablecimiento { get; set; }

        public ICollection<Calificacion> Calificaciones { get; set; } = new List<Calificacion>();
    }
}
