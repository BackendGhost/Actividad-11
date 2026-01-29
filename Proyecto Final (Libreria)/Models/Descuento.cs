using System.ComponentModel.DataAnnotations;

namespace Proyecto_Final__Libreria_.Models
{
    public class Descuento
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "El Descuento es requerido")]
        public int? Porcentaje {  get; set; }
        [Required(ErrorMessage = "El Estado es requerido")]
        [StringLength(8, MinimumLength = 6)] //Activo o Inactivo
        public string? Estado { get; set; }

    }
}
