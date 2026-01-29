using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_Final__Libreria_.Models
{
    public class Usuario_Rol
    {
        [Key]
        public int Id { get; set; }
        public int? id_Usuario { get; set; }
        [ForeignKey(nameof(id_Usuario))]
        public Usuarios? Usuarios { get; set; }
        public int? id_Rol {  get; set; }
        [ForeignKey(nameof(id_Rol))]
        public Rol? Rol { get; set; }
        [Required(ErrorMessage = "El Usuario es requerido")]
        [StringLength(100,MinimumLength = 5)]
        public string? Usuario { get; set; }
        [Required(ErrorMessage = "El Password es requerido")]
        [StringLength(100, MinimumLength = 5)]
        public string? Password { get; set; }
        [Required(ErrorMessage = "El Estado es requerido")]
        [StringLength(8, MinimumLength = 6)] //Activo o Inactivo
        public string? Estado { get; set; }
    }
}
