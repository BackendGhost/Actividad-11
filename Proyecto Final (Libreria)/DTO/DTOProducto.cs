using Proyecto_Final__Libreria_.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_Final__Libreria_.DTO
{
    public class DTOProducto
    {
        public int id_Categoria { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public string Estado { get; set; }
    }
}
