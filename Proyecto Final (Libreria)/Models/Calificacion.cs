using System.ComponentModel.DataAnnotations;

namespace Proyecto_Final__Libreria_.Models
{
    public class Calificacion
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "La Nota es requerido")]
        [StringLength(1, MinimumLength = 1)]
        [RegularExpression(@"^[1-5]+$", ErrorMessage = "Solo números permitidos del 1 al 5")]
        public string? Nota { get; set; }
        [Required(ErrorMessage = "El Estado es requerido")]
        [StringLength(8, MinimumLength = 6)] //Activo o Inactivo
        public string? Estado { get; set; } 
    }
}
