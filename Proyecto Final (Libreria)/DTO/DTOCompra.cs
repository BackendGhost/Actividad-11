using Proyecto_Final__Libreria_.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_Final__Libreria_.DTO
{
    public class DTOCompra
    {
        public int id_UsuarioRol { get; set; }
        public int id_Producto { get; set; }
        public string Descripcion { get; set; }
        public int Cantidad { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; }
    }
}
