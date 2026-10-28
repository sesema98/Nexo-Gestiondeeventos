using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;

namespace Eventos.Web.Controllers;

public class EventosController(EventoRepositorio repositorio) : Controller
{
    public async Task<IActionResult> Index(int? categoriaId)
    {
        ViewBag.Categorias = await repositorio.FiltroCategoriasAsync();
        ViewBag.CategoriaId = categoriaId;
        return View(await repositorio.ListarAsync(categoriaId));
    }

    public async Task<IActionResult> Details(int id)
    {
        var evento = await repositorio.ObtenerAsync(id);
        return evento is null ? NotFound() : View(evento);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Categorias = await repositorio.CategoriasActivasAsync();
        return View(new Evento { Fecha = DateTime.Today.AddDays(1), Modalidad = "Presencial" });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Evento evento)
    {
        ModelState.Remove(nameof(Evento.Categoria));
        ModelState.Remove(nameof(Evento.Inscripciones));
        if (!ModelState.IsValid)
        {
            ViewBag.Categorias = await repositorio.CategoriasActivasAsync();
            return View(evento);
        }
        await repositorio.CrearAsync(evento);
        TempData["Mensaje"] = "Evento registrado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var evento = await repositorio.ObtenerAsync(id);
        if (evento is null) return NotFound();
        ViewBag.Categorias = await repositorio.CategoriasActivasAsync();
        return View(evento);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Evento evento)
    {
        if (id != evento.Id) return NotFound();
        ModelState.Remove(nameof(Evento.Categoria));
        ModelState.Remove(nameof(Evento.Inscripciones));
        if (!ModelState.IsValid)
        {
            ViewBag.Categorias = await repositorio.CategoriasActivasAsync();
            return View(evento);
        }
        if (!await repositorio.EditarAsync(evento)) return NotFound();
        TempData["Mensaje"] = "Evento actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var evento = await repositorio.ObtenerAsync(id);
        return evento is null ? NotFound() : View(evento);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (!await repositorio.EliminarLogicamenteAsync(id)) return NotFound();
        TempData["Mensaje"] = "Evento desactivado correctamente.";
        return RedirectToAction(nameof(Index));
    }
}
