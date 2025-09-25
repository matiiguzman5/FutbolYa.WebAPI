namespace FutbolYa.WebAPI.Models
{
    public class Calificacion
    {
        public int Id { get; set; }

        public int PartidoId { get; set; }
        public Partido Partido { get; set; }

        public int EvaluadorId { get; set; }
        public Usuario Evaluador { get; set; }

        public int EvaluadoId { get; set; }
        public Usuario Evaluado { get; set; }

        public int Puntaje { get; set; }  // 1 a 5
        public string Comentario { get; set; } = string.Empty;

        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}
