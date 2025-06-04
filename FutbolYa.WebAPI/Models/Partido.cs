using FutbolYa.WebAPI.Models;
using System.Text.Json.Serialization;

public class Partido
{
    public int Id { get; set; }
    public string Ubicacion { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }

    public int OrganizadorId { get; set; }

    [JsonIgnore] // 👈 Oculta Organizador del binding de entrada
    public Usuario? Organizador { get; set; }

    public ICollection<Usuario> Jugadores { get; set; } = new List<Usuario>();
}
