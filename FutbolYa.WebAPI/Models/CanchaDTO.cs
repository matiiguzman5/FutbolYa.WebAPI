namespace FutbolYa.WebAPI.Models
{
    public class CanchaDTO
    {
        public string Nombre { get; set; }
        public string Tipo { get; set; } // F5, F7, F11
        public string Superficie { get; set; }
        public string Estado { get; set; }

        public decimal PrecioBaseHora { get; set; }
        public decimal PrecioNocturno { get; set; }
        public decimal PrecioFinDeSemana { get; set; }
        public decimal PrecioPremium { get; set; }

        public TimeSpan HorarioApertura { get; set; }
        public TimeSpan HorarioCierre { get; set; }

        public string? BloquesMantenimiento { get; set; }
        public string? DiasNoDisponibles { get; set; }

        public string? LogReparaciones { get; set; }
        public string? EstadoEquipamiento { get; set; }
        public string? NotasEspeciales { get; set; }
        public DateTime? ProximoMantenimiento { get; set; }
    }
}
