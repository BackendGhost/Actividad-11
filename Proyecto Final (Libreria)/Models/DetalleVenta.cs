using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_Final__Libreria_.Models
{
    public class DetalleVenta
    {
        [Key]
        public int Id { get; set; }
        public int? id_Venta { get; set; }
        [ForeignKey(nameof(id_Venta))]
        public Venta? Venta { get; set; }
        public int? id_Producto { get; set; }
        [ForeignKey(nameof(id_Producto))]
        public Producto? Producto { get; set; }
        public int? id_Descuento { get; set; }
        [ForeignKey(nameof(id_Descuento))]
        public Descuento? Descuento { get; set; }
        public int? id_Promocion { get; set; }
        [ForeignKey(nameof(id_Promocion))]
        public Promocion? Promocion { get; set; }
        [Required(ErrorMessage = "La Cantidad es requerida")]

        public int? Cantidad { get; set; }
        [Required(ErrorMessage = "El Subtotal es requerido")]

        public decimal? Subtotal { get; set; }
        [Required(ErrorMessage = "El Estado es requerido")]
        [StringLength(10, MinimumLength = 5)] 
        public string? Estado { get; set; }
    }
}
