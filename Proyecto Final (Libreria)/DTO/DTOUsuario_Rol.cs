using Proyecto_Final__Libreria_.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_Final__Libreria_.DTO
{
    public class DTOUsuario_Rol
    {
        public int id_Usuario { get; set; }
        public int id_Rol { get; set; }
        [EmailAddress]
        public string Usuario { get; set; }        
        public string Password { get; set; }
        public string Estado { get; set; }
    }
}
