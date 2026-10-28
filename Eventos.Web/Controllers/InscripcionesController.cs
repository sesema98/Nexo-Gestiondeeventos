using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;

namespace Eventos.Web.Controllers;

public class InscripcionesController(InscripcionRepositorio repositorio, AsistenteRepositorio asistentes) : Controller
{
    public async Task<IActionResult> Index() => View(await repositorio.ListarAsync());

    public async Task<IActionResult> Create()
    {
        await CargarListasAsync();
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Inscripcion inscripcion)
    {
        ModelState.Remove(nameof(Inscripcion.Evento));
        ModelState.Remove(nameof(Inscripcion.Asistente));
        var evento = await repositorio.ObtenerEventoAsync(inscripcion.EventoId);
        if (evento is null) ModelState.AddModelError(nameof(Inscripcion.EventoId), "Seleccione un evento activo.");
        else
        {
            if (evento.Fecha < DateTime.Now)
                ModelState.AddModelError(nameof(Inscripcion.EventoId), "No se puede inscribir en un evento que ya pasó.");
            if (await repositorio.YaInscritoAsync(inscripcion.EventoId, inscripcion.AsistenteId))
                ModelState.AddModelError(string.Empty, "El asistente ya tiene una inscripción confirmada en este evento.");
            if (await repositorio.ContarConfirmadasAsync(inscripcion.EventoId) >= evento.Aforo)
                ModelState.AddModelError(nameof(Inscripcion.EventoId), "El aforo del evento está completo.");
        }
        if (!ModelState.IsValid)
        {
            await CargarListasAsync();
            return View(inscripcion);
        }
        await repositorio.CrearAsync(inscripcion);
        TempData["Mensaje"] = "Inscripción confirmada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancelar(int id)
    {
        if (!await repositorio.CancelarAsync(id)) return NotFound();
        TempData["Mensaje"] = "Inscripción cancelada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    private async Task CargarListasAsync()
    {
        ViewBag.Eventos = await repositorio.EventosActivosAsync();
        ViewBag.Asistentes = await asistentes.ActivosParaListaAsync();
    }
}
