using Eventos.Web.Data;
using Eventos.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Eventos.Web.Repositorios;

public class InscripcionRepositorio(EventosContext context)
{
    public Task<List<Inscripcion>> ListarAsync() => context.Inscripciones.AsNoTracking()
        .Include(i => i.Evento).Include(i => i.Asistente)
        .OrderByDescending(i => i.FechaInscripcion).ToListAsync();

    public async Task<List<SelectListItem>> EventosActivosAsync() => await context.Eventos.AsNoTracking()
        .Where(e => e.Activo).OrderBy(e => e.Fecha)
        .Select(e => new SelectListItem($"{e.Titulo} - {e.Fecha:dd/MM/yyyy}", e.Id.ToString())).ToListAsync();

    public Task<Evento?> ObtenerEventoAsync(int eventoId) => context.Eventos.AsNoTracking()
        .FirstOrDefaultAsync(e => e.Id == eventoId && e.Activo);

    public Task<bool> YaInscritoAsync(int eventoId, int asistenteId) => context.Inscripciones.AsNoTracking()
        .AnyAsync(i => i.EventoId == eventoId && i.AsistenteId == asistenteId && i.Estado == "Confirmada");

    public Task<int> ContarConfirmadasAsync(int eventoId) => context.Inscripciones.AsNoTracking()
        .CountAsync(i => i.EventoId == eventoId && i.Estado == "Confirmada");

    public async Task CrearAsync(Inscripcion inscripcion)
    {
        inscripcion.FechaInscripcion = DateTime.Now;
        inscripcion.Estado = "Confirmada";
        context.Inscripciones.Add(inscripcion);
        await context.SaveChangesAsync();
    }

    public async Task<bool> CancelarAsync(int id)
    {
        var inscripcion = await context.Inscripciones.FirstOrDefaultAsync(i => i.Id == id && i.Estado == "Confirmada");
        if (inscripcion is null) return false;
        inscripcion.Estado = "Cancelada";
        await context.SaveChangesAsync();
        return true;
    }
}
