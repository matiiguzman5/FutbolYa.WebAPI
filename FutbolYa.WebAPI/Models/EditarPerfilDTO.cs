using System.Text.Json.Serialization;

namespace FutbolYa.WebAPI.Models
{
    public class EditarPerfilDTO
    {
        public string? Nombre { get; set; }
        public string? Telefono { get; set; }
        public string? Posicion { get; set; }
        public string? Ubicacion { get; set; }

        [JsonPropertyName("contrasena")]
        public string? Contrasena { get; set; }

        [JsonPropertyName("contrase\u00F1a")]
        public string? ContrasenaConTilde
        {
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    Contrasena = value;
                }
            }
        }
    }
}
