using System;
using System.Text.Json.Serialization;

namespace FutbolYa.WebAPI.Models
{
    public class ConfirmarPagoDTO
    {
        [JsonPropertyName("estadoPago")]
        public string? EstadoPago { get; set; }

        [JsonPropertyName("metodoPago")]
        public string? MetodoPago { get; set; }

        [JsonPropertyName("tipoPago")]
        public string? TipoPagoAlias
        {
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    EstadoPago = value;
                }
            }
        }

        [JsonPropertyName("metodo")]
        public string? MetodoAlias
        {
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    MetodoPago = value;
                }
            }
        }

        [JsonPropertyName("fechaPago")]
        public DateTime? FechaPago { get; set; }

        [JsonPropertyName("token")]
        public string? Token { get; set; }

        [JsonPropertyName("tokenPago")]
        public string? TokenAlias
        {
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    Token = value;
                }
            }
        }

        [JsonPropertyName("numeroTarjeta")]
        public string? NumeroTarjeta { get; set; }

        [JsonPropertyName("numero")]
        public string? NumeroTarjetaAlias
        {
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    NumeroTarjeta = value;
                }
            }
        }

        [JsonPropertyName("nombreTitular")]
        public string? NombreTitular { get; set; }

        [JsonPropertyName("fechaExpiracion")]
        public string? FechaExpiracion { get; set; }

        [JsonPropertyName("codigoSeguridad")]
        public string? CodigoSeguridad { get; set; }

        [JsonPropertyName("cvv")]
        public string? CodigoSeguridadAlias
        {
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    CodigoSeguridad = value;
                }
            }
        }

        [JsonPropertyName("sedeConfirmoTransferencia")]
        public bool? SedeConfirmoTransferencia { get; set; }
    }
}
