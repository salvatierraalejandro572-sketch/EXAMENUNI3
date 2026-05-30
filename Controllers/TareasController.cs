using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using EXAMENUNI3.Data.Repositories;
using EXAMENUNI3.Models;

namespace EXAMENUNI3.Controllers;

public class TareasController : Controller
{
    private readonly ITareaRepository _tareaRepository;
    private readonly IProyectoRepository _proyectoRepository;

    public TareasController(ITareaRepository tareaRepository, IProyectoRepository proyectoRepository)
    {
        _tareaRepository = tareaRepository;
        _proyectoRepository = proyectoRepository;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return View(new List<Tarea>());
        var tareas = await _tareaRepository.GetAllByUserAsync(userId);
        return View(tareas);
    }

    [Authorize]
    public async Task<IActionResult> Details(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var tarea = await _tareaRepository.GetByIdAsync(id);
        if (tarea == null || tarea.Proyecto?.UserId != userId) return NotFound();
        return View(tarea);
    }

    [Authorize]
    public async Task<IActionResult> Create(int? proyectoId)
    {
        await PopulateProyectosDropDownListAsync(proyectoId);
        return View(new Tarea { ProyectoId = proyectoId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Create(Tarea tarea)
    {
        if (ModelState.IsValid)
        {
            await _tareaRepository.AddAsync(tarea);
            TempData["Success"] = "Tarea creada correctamente";
            return RedirectToAction(nameof(Index));
        }
        await PopulateProyectosDropDownListAsync(tarea.ProyectoId);
        return View(tarea);
    }

    [Authorize]
    public async Task<IActionResult> Edit(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var tarea = await _tareaRepository.GetByIdAsync(id);
        if (tarea == null || tarea.Proyecto?.UserId != userId) return NotFound();
        await PopulateProyectosDropDownListAsync(tarea.ProyectoId);
        return View(tarea);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Edit(int id, Tarea tarea)
    {
        if (id != tarea.Id) return NotFound();
        if (ModelState.IsValid)
        {
            await _tareaRepository.UpdateAsync(tarea);
            TempData["Success"] = "Tarea actualizada correctamente";
            return RedirectToAction(nameof(Index));
        }
        await PopulateProyectosDropDownListAsync(tarea.ProyectoId);
        return View(tarea);
    }

    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var tarea = await _tareaRepository.GetByIdAsync(id);
        if (tarea == null || tarea.Proyecto?.UserId != userId) return NotFound();
        return View(tarea);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var tarea = await _tareaRepository.GetByIdAsync(id);
        if (tarea == null || tarea.Proyecto?.UserId != userId) return NotFound();
        await _tareaRepository.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateProyectosDropDownListAsync(object? selectedProyectoId = null)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var proyectos = userId != null
            ? await _proyectoRepository.GetAllByUserAsync(userId)
            : new List<Proyecto>();
        ViewBag.ProyectoId = new SelectList(proyectos, "Id", "Nombre", selectedProyectoId);
    }
}
