namespace FutbolYa.WebAPI.DTOs
{
    public class ReservaDTO
    {
        public int CanchaId { get; set; }
        public DateTime FechaHora { get; set; }
        public string Observaciones { get; set; }

        // Solo visible para rol "establecimiento"
        public string? ClienteNombre { get; set; }
        public string? ClienteTelefono { get; set; }
        public string? ClienteEmail { get; set; }
        public string EstadoPago { get; set; } = "pendiente";
        public bool EsFrecuente { get; set; } = false;
    }
}
