using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FutbolYa.WebAPI.Models
{
    public class DatosTarjeta
    {
        public int Id { get; set; }

        [Required]
        public int ReservaId { get; set; }

        [JsonIgnore]
        public Reserva Reserva { get; set; } = null!;

        [Required]
        [MaxLength(128)]
        public string HashToken { get; set; } = null!;

        [Required]
        [MaxLength(128)]
        public string HashNumero { get; set; } = null!;

        [MaxLength(4)]
        public string? Ultimos4 { get; set; }

        [MaxLength(128)]
        public string? HashCvv { get; set; }

        [MaxLength(100)]
        public string? NombreTitular { get; set; }

        [MaxLength(7)]
        public string? FechaExpiracion { get; set; }

        public DateTime FechaRegistroUtc { get; set; } = DateTime.UtcNow;
    }
}
