using Microsoft.EntityFrameworkCore;
using EXAMENUNI3.Models;

namespace EXAMENUNI3.Data.Repositories;

public class TareaRepository : ITareaRepository
{
    private readonly ApplicationDbContext _context;

    public TareaRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Tarea>> GetAllAsync()
    {
        return await _context.Tareas
            .Include(t => t.Proyecto)
            .OrderBy(t => t.FechaVencimiento)
            .ToListAsync();
    }

    public async Task<List<Tarea>> GetAllByUserAsync(string userId)
    {
        return await _context.Tareas
            .Include(t => t.Proyecto)
            .Where(t => t.Proyecto!.UserId == userId)
            .OrderBy(t => t.FechaVencimiento)
            .ToListAsync();
    }

    public async Task<List<Tarea>> GetByProyectoIdAsync(int proyectoId)
    {
        return await _context.Tareas
            .Where(t => t.ProyectoId == proyectoId)
            .OrderBy(t => t.FechaVencimiento)
            .ToListAsync();
    }

    public async Task<Tarea?> GetByIdAsync(int id)
    {
        return await _context.Tareas
            .Include(t => t.Proyecto)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task AddAsync(Tarea tarea)
    {
        await _context.Tareas.AddAsync(tarea);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Tarea tarea)
    {
        _context.Tareas.Update(tarea);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var tarea = await _context.Tareas.FindAsync(id);
        if (tarea != null)
        {
            _context.Tareas.Remove(tarea);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Tareas.AnyAsync(t => t.Id == id);
    }
}
