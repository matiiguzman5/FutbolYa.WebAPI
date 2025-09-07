namespace FutbolYa.WebAPI.Models
{
    public class CanchaUpdateDTO
    {
        public string? Nombre { get; set; }
        public string? Tipo { get; set; }              // "F5" | "F7" | "F11"
        public string? Superficie { get; set; }        // "Sintética" | "Pasto Real"
        public string? Estado { get; set; }            // "Activa" | "Mantenimiento" | "Fuera de servicio"

        public decimal? PrecioBaseHora { get; set; }
        public decimal? PrecioNocturno { get; set; }
        public decimal? PrecioFinDeSemana { get; set; }
        public decimal? PrecioPremium { get; set; }

        public TimeSpan? HorarioApertura { get; set; } // formato JSON: "06:00:00"
        public TimeSpan? HorarioCierre { get; set; }  // formato JSON: "23:00:00"

        public string? BloquesMantenimiento { get; set; } // ej: "13:00-14:00"
        public string? DiasNoDisponibles { get; set; }     // ej: "Lunes,Domingo"
        public string? LogReparaciones { get; set; }
        public string? EstadoEquipamiento { get; set; }    // "OK", "Revisar iluminación", etc.
        public string? NotasEspeciales { get; set; }
        public DateTime? ProximoMantenimiento { get; set; } // "2025-08-01T00:00:00"
    }
}
