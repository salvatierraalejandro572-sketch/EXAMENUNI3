using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EXAMENUNI3.Data.Repositories;
using EXAMENUNI3.Models;

namespace EXAMENUNI3.Controllers;

public class ProyectosController : Controller
{
    private readonly IProyectoRepository _proyectoRepository;

    public ProyectosController(IProyectoRepository proyectoRepository)
    {
        _proyectoRepository = proyectoRepository;
    }

    public async Task<IActionResult> Index()
    {
        var proyectos = await _proyectoRepository.GetAllAsync();
        return View(proyectos);
    }

    public async Task<IActionResult> Details(int id)
    {
        var proyecto = await _proyectoRepository.GetByIdWithTareasAsync(id);
        if (proyecto == null) return NotFound();
        return View(proyecto);
    }

    [Authorize]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Create(Proyecto proyecto)
    {
        if (ModelState.IsValid)
        {
            await _proyectoRepository.AddAsync(proyecto);
            return RedirectToAction(nameof(Index));
        }
        return View(proyecto);
    }

    [Authorize]
    public async Task<IActionResult> Edit(int id)
    {
        var proyecto = await _proyectoRepository.GetByIdAsync(id);
        if (proyecto == null) return NotFound();
        return View(proyecto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Edit(int id, Proyecto proyecto)
    {
        if (id != proyecto.Id) return NotFound();
        if (ModelState.IsValid)
        {
            await _proyectoRepository.UpdateAsync(proyecto);
            return RedirectToAction(nameof(Index));
        }
        return View(proyecto);
    }

    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var proyecto = await _proyectoRepository.GetByIdWithTareasAsync(id);
        if (proyecto == null) return NotFound();
        return View(proyecto);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _proyectoRepository.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> GetProyectosJson()
    {
        var proyectos = await _proyectoRepository.GetAllAsync();
        return Json(proyectos);
    }
}
