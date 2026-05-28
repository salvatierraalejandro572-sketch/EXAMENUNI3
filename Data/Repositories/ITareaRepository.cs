using EXAMENUNI3.Models;

namespace EXAMENUNI3.Data.Repositories;

public interface ITareaRepository
{
    Task<List<Tarea>> GetAllAsync();
    Task<List<Tarea>> GetAllByUserAsync(string userId);
    Task<List<Tarea>> GetByProyectoIdAsync(int proyectoId);
    Task<Tarea?> GetByIdAsync(int id);
    Task AddAsync(Tarea tarea);
    Task UpdateAsync(Tarea tarea);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
