using System.Text.Json.Serialization;

namespace FutbolYa.WebAPI.Models
{
    public class Mensaje
    {
        public int Id { get; set; }
        public int PartidoId { get; set; }
        [JsonIgnore]
        public Partido Partido { get; set; }
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
        public string Contenido { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}
