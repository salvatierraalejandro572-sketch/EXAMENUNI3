using EXAMENUNI3.Models;

namespace EXAMENUNI3.Data.Repositories;

public interface IProyectoRepository
{
    Task<List<Proyecto>> GetAllAsync();
    Task<List<Proyecto>> GetAllByUserAsync(string userId);
    Task<Proyecto?> GetByIdAsync(int id);
    Task<Proyecto?> GetByIdWithTareasAsync(int id);
    Task AddAsync(Proyecto proyecto);
    Task UpdateAsync(Proyecto proyecto);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
