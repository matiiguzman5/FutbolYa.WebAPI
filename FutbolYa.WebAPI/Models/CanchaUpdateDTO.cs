namespace FutbolYa.WebAPI.Models
{
    public class CanchaUpdateDTO
    {
        public string? Nombre { get; set; }
        public string? Tipo { get; set; }              // "F5" | "F7" | "F11"
        public string? Superficie { get; set; }        // "Sintética" | "Pasto Real"
        public string? Estado { get; set; }            // "Activa" | "Mantenimiento" | "Fuera de servicio"
        public decimal? PrecioBaseHora { get; set; }
        public decimal? PrecioFinDeSemana { get; set; }

    }
}
