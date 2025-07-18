using System.Text.Json.Serialization;

namespace FutbolYa.WebAPI.Models
{
    public class ReservaUsuario
    {
        public int ReservaId { get; set; }
        [JsonIgnore]
        public Reserva Reserva { get; set; }

        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
    }
}
