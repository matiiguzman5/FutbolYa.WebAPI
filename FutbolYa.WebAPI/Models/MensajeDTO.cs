namespace FutbolYa.WebAPI.Models
{
    public class MensajeDTO
    {
        public int ReservaId { get; set; }   // para chat de reserva
        public int PartidoId { get; set; }   // para chat de partido
        public string Contenido { get; set; }
    }


}
