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

    public string Actitud { get; set; }
    public string Pase { get; set; }
    public string Defensa { get; set; }
    public string TrabajoEquipo { get; set; }
    public string Puntualidad { get; set; }

    public DateTime Fecha { get; set; } = DateTime.Now;
}
