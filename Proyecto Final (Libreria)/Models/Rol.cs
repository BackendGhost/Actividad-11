using System.ComponentModel.DataAnnotations;

namespace Proyecto_Final__Libreria_.Models
{
    public class Rol
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "La Descripcion es requerida")]
        [StringLength(500, MinimumLength = 5)]
        public string? Descripcion { get; set; }
        [Required(ErrorMessage = "El Estado es requerido")]
        [StringLength(8, MinimumLength = 6)] //Activo o Inactivo
        public string? Estado { get; set; }
    }
}
