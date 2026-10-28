namespace Eventos.Web.Models;

public class EventoListadoViewModel
{
    public Evento Evento { get; set; } = null!;
    public int InscritosConfirmados { get; set; }
    public int CuposDisponibles => Evento.Aforo - InscritosConfirmados;
}
