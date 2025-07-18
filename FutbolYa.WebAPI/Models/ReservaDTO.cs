namespace FutbolYa.WebAPI.Models
{
    public class ReservaDTO
    {
        public int CanchaId { get; set; }
        public DateTime FechaHora { get; set; }
        public int DuracionMinutos { get; set; }

        public string ClienteNombre { get; set; }
        public string ClienteTelefono { get; set; }
        public string? ClienteEmail { get; set; }
        public bool EsFrecuente { get; set; }

        public string EstadoPago { get; set; }
        public string? Observaciones { get; set; }
    }
}
