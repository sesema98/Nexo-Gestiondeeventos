using System.ComponentModel.DataAnnotations;

namespace Eventos.Web.Models;

public class Inscripcion
{
    public int Id { get; set; }

    [Display(Name = "Evento")]
    public int EventoId { get; set; }
    public Evento Evento { get; set; } = null!;

    [Display(Name = "Asistente")]
    public int AsistenteId { get; set; }
    public Asistente Asistente { get; set; } = null!;

    [Display(Name = "Fecha de inscripción")]
    public DateTime FechaInscripcion { get; set; }

    [Required, RegularExpression("^(Confirmada|Cancelada)$")]
    public string Estado { get; set; } = "Confirmada";
}
