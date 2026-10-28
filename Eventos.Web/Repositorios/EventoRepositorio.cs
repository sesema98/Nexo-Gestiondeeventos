using Eventos.Web.Data;
using Eventos.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Eventos.Web.Repositorios;

public class EventoRepositorio(EventosContext context)
{
    public async Task<List<EventoListadoViewModel>> ListarAsync(int? categoriaId)
    {
        var eventos = await context.Eventos.AsNoTracking()
            .Include(e => e.Categoria)
            .Include(e => e.Inscripciones)
            .Where(e => e.Activo && (!categoriaId.HasValue || e.CategoriaId == categoriaId))
            .OrderBy(e => e.Fecha)
            .ToListAsync();

        return eventos.Select(e => new EventoListadoViewModel
            {
                Evento = e,
                InscritosConfirmados = e.Inscripciones.Count(i => i.Estado == "Confirmada")
            }).ToList();
    }

    public Task<Evento?> ObtenerAsync(int id, bool tracking = false)
    {
        var query = context.Eventos.Include(e => e.Categoria).AsQueryable();
        if (!tracking) query = query.AsNoTracking();
        return query.FirstOrDefaultAsync(e => e.Id == id && e.Activo);
    }

    public async Task<List<SelectListItem>> CategoriasActivasAsync() =>
        await context.Categorias.AsNoTracking().Where(c => c.Activo).OrderBy(c => c.Nombre)
            .Select(c => new SelectListItem(c.Nombre, c.Id.ToString())).ToListAsync();

    public async Task<List<SelectListItem>> FiltroCategoriasAsync()
    {
        var items = await CategoriasActivasAsync();
        items.Insert(0, new SelectListItem("Todas", ""));
        return items;
    }

    public async Task CrearAsync(Evento evento)
    {
        evento.Activo = true;
        context.Eventos.Add(evento);
        await context.SaveChangesAsync();
    }

    public async Task<bool> EditarAsync(Evento datos)
    {
        var evento = await context.Eventos.FirstOrDefaultAsync(e => e.Id == datos.Id && e.Activo);
        if (evento is null) return false;
        evento.Titulo = datos.Titulo;
        evento.Fecha = datos.Fecha;
        evento.Lugar = datos.Lugar;
        evento.Aforo = datos.Aforo;
        evento.Precio = datos.Precio;
        evento.CategoriaId = datos.CategoriaId;
        evento.Modalidad = datos.Modalidad;
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> EliminarLogicamenteAsync(int id)
    {
        var evento = await context.Eventos.FirstOrDefaultAsync(e => e.Id == id && e.Activo);
        if (evento is null) return false;
        evento.Activo = false;
        await context.SaveChangesAsync();
        return true;
    }
}
