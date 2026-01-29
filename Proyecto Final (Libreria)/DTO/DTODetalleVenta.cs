using Proyecto_Final__Libreria_.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_Final__Libreria_.DTO
{
    public class DTODetalleVenta
    {
        public int id_Venta { get; set; }
        public int id_Producto { get; set; }
        public int id_Descuento { get; set; }
        public int id_Promocion { get; set; }
        public int Cantidad { get; set; }
        public decimal Subtotal { get; set; }
        public string Estado { get; set; }
    }
}
