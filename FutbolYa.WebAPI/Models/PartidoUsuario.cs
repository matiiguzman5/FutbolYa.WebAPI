namespace FutbolYa.WebAPI.Models
{
    public class PartidoUsuario
    {
        public int PartidosId { get; set; }
        public Partido Partido { get; set; }

        public int JugadoresId { get; set; }
        public Usuario Jugador { get; set; }
    }
}
