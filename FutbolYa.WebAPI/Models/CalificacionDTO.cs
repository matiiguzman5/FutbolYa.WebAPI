namespace FutbolYa.WebAPI.Models
{
    public class CalificacionDTO
    {
        public int ReservaId { get; set; }
        public int EvaluadoId { get; set; }
        public int Puntaje { get; set; }
        public string Comentario { get; set; } = string.Empty;
    }
}
