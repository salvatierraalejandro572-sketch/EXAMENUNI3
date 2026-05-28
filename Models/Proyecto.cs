using System.ComponentModel.DataAnnotations;

namespace EXAMENUNI3.Models;

public class Proyecto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [MaxLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Fecha de Inicio")]
    public DateTime FechaInicio { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Fecha de Finalización")]
    public DateTime? FechaFinalizacion { get; set; }

    public ICollection<Tarea> Tareas { get; set; } = new List<Tarea>();
}
