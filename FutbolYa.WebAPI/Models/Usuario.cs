using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FutbolYa.WebAPI.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }

        [Column("Contrase\u00F1a")]
        public string Contrasena { get; set; }

        public string Rol { get; set; } = "usuario";
        public string? Telefono { get; set; }
        public string? Posicion { get; set; }
        public string? FotoPerfil { get; set; }
        public string? Ubicacion { get; set; }

        [JsonIgnore]
        public ICollection<Partido> Partidos { get; set; } = new List<Partido>();
        public ICollection<Cancha> Canchas { get; set; } = new List<Cancha>();
    }
}
