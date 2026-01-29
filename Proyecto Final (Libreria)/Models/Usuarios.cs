using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace Proyecto_Final__Libreria_.Models
{
    public class Usuarios
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "El Nombre es requerido")]
        [StringLength(100, MinimumLength = 10)]
        public string? Nombre { get; set; }
        [Required(ErrorMessage = "El CI es requerido")]
        [StringLength(15, MinimumLength = 8)]
        public string? CI { get; set; }
        [Required(ErrorMessage = "El Telefono es requerido")]
        [StringLength(8, MinimumLength = 8)]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Solo números permitidos")]
        public string? Telefono { get; set; }
        [Required(ErrorMessage = "El Estado es requerido")]
        [StringLength(8, MinimumLength = 6)] //Activo o Inactivo
        public string? Estado { get; set; } 
    }
}
