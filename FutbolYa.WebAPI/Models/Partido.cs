using FutbolYa.WebAPI.Models;

public class Partido
{
    public int Id { get; set; }
    public string Ubicacion { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }

    public int OrganizadorId { get; set; }
    public Usuario Organizador { get; set; }

    public ICollection<PartidoUsuario> Jugadores { get; set; } = new List<PartidoUsuario>();
}
