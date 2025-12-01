using System.Text.Json.Serialization;

namespace FutbolYa.WebAPI.Models
{
    public class Mensaje
    {
        public int Id { get; set; }

        // 🔹 Chat por reserva
        public int ReservaId { get; set; }
        public Reserva Reserva { get; set; }

        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        public string Contenido { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}
