using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_Final__Libreria_.Models
{
    public class Producto
    {
        [Key]
        public int Id { get; set; }
        public int? id_Categoria { get; set; }
        [ForeignKey(nameof(id_Categoria))]
        public Categoria? Categoria { get; set; }
        public string? Nombre { get; set; }
        [Required(ErrorMessage = "La Descripcion es requerida")]
        [StringLength(500,MinimumLength = 20)]
        public string? Descripcion { get; set; }
        [Required(ErrorMessage = "La Cantidad es requerida")]

        public int? Cantidad { get; set; }
        [Required(ErrorMessage ="El Precio es requerido")]
        public decimal? Precio { get; set; }
        [Required(ErrorMessage = "El Estado es requerido")]
        [StringLength(8, MinimumLength = 6)] //Activo o Inactivo
        public string? Estado { get; set; }
    }
}
