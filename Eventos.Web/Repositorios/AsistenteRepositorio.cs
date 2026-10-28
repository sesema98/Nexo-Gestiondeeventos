using Eventos.Web.Data;
using Eventos.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Eventos.Web.Repositorios;

public class AsistenteRepositorio(EventosContext context)
{
    public Task<List<Asistente>> ListarAsync() => context.Asistentes.AsNoTracking()
        .Where(a => a.Activo).OrderBy(a => a.Nombres).ToListAsync();

    public Task<bool> ExisteDniAsync(string dni) => context.Asistentes.AnyAsync(a => a.Dni == dni);
    public Task<bool> ExisteCorreoAsync(string correo) => context.Asistentes.AnyAsync(a => a.Correo == correo);

    public async Task CrearAsync(Asistente asistente)
    {
        asistente.Activo = true;
        context.Asistentes.Add(asistente);
        await context.SaveChangesAsync();
    }

    public async Task<List<SelectListItem>> ActivosParaListaAsync() => await context.Asistentes
        .AsNoTracking().Where(a => a.Activo).OrderBy(a => a.Nombres)
        .Select(a => new SelectListItem($"{a.Nombres} ({a.Dni})", a.Id.ToString())).ToListAsync();
}
