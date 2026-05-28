using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EXAMENUNI3.Models;

public class Tarea
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El título es obligatorio")]
    [MaxLength(150, ErrorMessage = "El título no puede exceder 150 caracteres")]
    public string Titulo { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    [MaxLength(50)]
    public string Estado { get; set; } = "Pendiente";

    [DataType(DataType.Date)]
    [Display(Name = "Fecha de Vencimiento")]
    public DateTime? FechaVencimiento { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un proyecto")]
    [Display(Name = "Proyecto")]
    public int? ProyectoId { get; set; }

    [ForeignKey(nameof(ProyectoId))]
    public Proyecto? Proyecto { get; set; } 
}
