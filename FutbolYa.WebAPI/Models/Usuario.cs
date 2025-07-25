using System.Text.Json.Serialization;

namespace FutbolYa.WebAPI.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Contraseña { get; set; }
        public string Rol { get; set; } = "usuario";
        public string? Telefono { get; set; }
        public string? Posicion { get; set; }
        public string? FotoPerfil { get; set; }

        public List<Calificacion> Calificaciones { get; set; } = new();

        [JsonIgnore]
        public ICollection<Partido> Partidos { get; set; } = new List<Partido>();
    }
}
