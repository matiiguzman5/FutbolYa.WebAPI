namespace FutbolYa.WebAPI.Models
{
    public class CanchaDTO
    {
        public string Nombre { get; set; }
        public string Tipo { get; set; } // F5, F7, F11
        public string Superficie { get; set; }
        public string Estado { get; set; }
        public decimal PrecioBaseHora { get; set; }
        public decimal PrecioFinDeSemana { get; set; }

    }
}
