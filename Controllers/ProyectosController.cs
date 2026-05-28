using System.Security.Claims;
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
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return View(new List<Proyecto>());
        var proyectos = await _proyectoRepository.GetAllByUserAsync(userId);
        return View(proyectos);
    }

    public async Task<IActionResult> Details(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var proyecto = await _proyectoRepository.GetByIdWithTareasAsync(id);
        if (proyecto == null || proyecto.UserId != userId) return NotFound();
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
        proyecto.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
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
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var proyecto = await _proyectoRepository.GetByIdAsync(id);
        if (proyecto == null || proyecto.UserId != userId) return NotFound();
        return View(proyecto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Edit(int id, Proyecto proyecto)
    {
        if (id != proyecto.Id) return NotFound();
        proyecto.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
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
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var proyecto = await _proyectoRepository.GetByIdWithTareasAsync(id);
        if (proyecto == null || proyecto.UserId != userId) return NotFound();
        return View(proyecto);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var proyecto = await _proyectoRepository.GetByIdAsync(id);
        if (proyecto == null || proyecto.UserId != userId) return NotFound();
        await _proyectoRepository.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetProyectosJson()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var proyectos = await _proyectoRepository.GetAllByUserAsync(userId);
        return Json(proyectos);
    }
}
