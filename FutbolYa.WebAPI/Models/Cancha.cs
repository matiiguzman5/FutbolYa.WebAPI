using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FutbolYa.WebAPI.Models
{
    public class Cancha
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Tipo { get; set; }
        public string Superficie { get; set; }
        public string Estado { get; set; }
        public decimal Precio{ get; set; }
        public int UsuarioEstablecimientoId { get; set; }

        [JsonIgnore]
        public Usuario UsuarioEstablecimiento { get; set; }
        
    }
}
