using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_Final__Libreria_.Models
{
    public class Venta
    {
        [Key]
        public int Id { get; set; }
        public int? id_UsuarioRol {  get; set; }
        [ForeignKey(nameof(id_UsuarioRol))]
        public Usuario_Rol? Usuario_Rol { get; set; }
        public int? id_Calificacion { get; set; }
        [ForeignKey(nameof(id_Calificacion))]
        public Calificacion? Calificacion { get; set; }
        public int? id_Usuario {  get; set; }
        [ForeignKey(nameof(id_Usuario))]
        public Usuarios? Usuarios { get; set; }

        [Required(ErrorMessage = "El campo Fecha es requerido")]
        [DataType(DataType.Date)]
        public DateTime? Fecha { get; set; }
        [Required(ErrorMessage = "El Total es requerido")]
        public decimal? Total { get; set; }
        [Required(ErrorMessage = "El Estado es requerido")]
        [StringLength(20, MinimumLength = 6)]
        public string? Estado { get; set; }
    }
}
