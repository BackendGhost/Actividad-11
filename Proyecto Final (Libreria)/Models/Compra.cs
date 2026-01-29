using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_Final__Libreria_.Models
{
    public class Compra
    {
        [Key]
        public int Id { get; set; }
        public int? id_UsuarioRol {  get; set; }
        [ForeignKey(nameof(id_UsuarioRol))]
        public Usuario_Rol? UsuarioRol { get; set; }
        public int? id_Producto { get; set; }
        [ForeignKey(nameof(id_Producto))]
        public Producto? Producto { get; set; }
        [Required(ErrorMessage = "La Descripcion es requerida")]
        [StringLength(500, MinimumLength = 10)]
        public string? Descripcion { get; set; }
        [Required(ErrorMessage = "La Cantidad es requerida")]

        public int? Cantidad { get; set; }
        [Required(ErrorMessage = "El Total es requerido")]

        public decimal? Total { get; set; }
        [Required(ErrorMessage = "El Estado es requerido")]
        [StringLength(8, MinimumLength = 6)] //Activo o Inactivo
        public string? Estado { get; set; }
    }
}
