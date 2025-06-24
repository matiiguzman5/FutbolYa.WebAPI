using FutbolYa.WebAPI.Models;

public class Rendimientos
{
    public int Id { get; set; }

    public int PartidoId { get; set; }
    public Partido Partido { get; set; }  

    public int EvaluadorId { get; set; }
    public Usuario Evaluador { get; set; }

    public int EvaluadoId { get; set; }
    public Usuario Evaluado { get; set; }

    public int Actitud { get; set; }
    public int Pase { get; set; }
    public int Defensa { get; set; }
    public int TrabajoEquipo { get; set; }
    public int Puntualidad { get; set; }

    public DateTime Fecha { get; set; } = DateTime.Now;
}
