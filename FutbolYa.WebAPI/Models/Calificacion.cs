namespace FutbolYa.WebAPI.Models
{
    public class Calificacion
    {
        public int Id { get; set; }
        public int Puntaje { get; set; }
        public string Comentario { get; set; }

        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
    }
}
