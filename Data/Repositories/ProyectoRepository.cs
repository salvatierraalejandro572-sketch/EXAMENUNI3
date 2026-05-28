using Microsoft.EntityFrameworkCore;
using EXAMENUNI3.Models;

namespace EXAMENUNI3.Data.Repositories;

public class ProyectoRepository : IProyectoRepository
{
    private readonly ApplicationDbContext _context;

    public ProyectoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Proyecto>> GetAllAsync()
    {
        return await _context.Proyectos
            .Include(p => p.Tareas)
            .OrderBy(p => p.FechaInicio)
            .ToListAsync();
    }

    public async Task<List<Proyecto>> GetAllByUserAsync(string userId)
    {
        return await _context.Proyectos
            .Where(p => p.UserId == userId)
            .Include(p => p.Tareas)
            .OrderBy(p => p.FechaInicio)
            .ToListAsync();
    }

    public async Task<Proyecto?> GetByIdAsync(int id)
    {
        return await _context.Proyectos.FindAsync(id);
    }

    public async Task<Proyecto?> GetByIdWithTareasAsync(int id)
    {
        return await _context.Proyectos
            .Include(p => p.Tareas)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task AddAsync(Proyecto proyecto)
    {
        await _context.Proyectos.AddAsync(proyecto);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Proyecto proyecto)
    {
        _context.Proyectos.Update(proyecto);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var proyecto = await _context.Proyectos.FindAsync(id);
        if (proyecto != null)
        {
            _context.Proyectos.Remove(proyecto);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Proyectos.AnyAsync(p => p.Id == id);
    }
}
