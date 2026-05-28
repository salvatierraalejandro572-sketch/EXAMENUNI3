using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EXAMENUNI3.Models;

namespace EXAMENUNI3.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
{
    public DbSet<Proyecto> Proyectos { get; set; }
    public DbSet<Tarea> Tareas { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Proyecto>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(p => p.Descripcion).HasMaxLength(500);
        });

        builder.Entity<Tarea>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Titulo).IsRequired().HasMaxLength(150);
            entity.Property(t => t.Estado).HasMaxLength(50).HasDefaultValue("Pendiente");
            entity.HasOne(t => t.Proyecto)
                  .WithMany(p => p.Tareas)
                  .HasForeignKey(t => t.ProyectoId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
