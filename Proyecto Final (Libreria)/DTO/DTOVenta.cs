using Proyecto_Final__Libreria_.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_Final__Libreria_.DTO
{
    public class DTOVenta
    {
        public int id_UsuarioRol { get; set; }
        public int id_Calificacion { get; set; }
        public int id_Usuario { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; }
    }
}
