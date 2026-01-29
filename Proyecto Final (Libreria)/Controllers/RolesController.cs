using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_Final__Libreria_.Data;
using Proyecto_Final__Libreria_.DTO;
using Proyecto_Final__Libreria_.Models;

namespace Proyecto_Final__Libreria_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("Cors")]
    public class RolesController : ControllerBase
    {
        public readonly DBconexion dBconexion;
        public RolesController(DBconexion dBconexion)
        {
            this.dBconexion = dBconexion;
        }
        //Listar Datos
        [HttpGet("Listar")]
        public async Task<ActionResult<Rol>> Get()
        {
            var rol = await dBconexion.Roles.ToListAsync();
            return Ok(rol);
        }
        // Listar Roles activas
        [HttpGet("ListarActivos")]
        public async Task<ActionResult<IEnumerable<Rol>>> GetActivo()
        {
            var RolesActivos = await dBconexion.Roles
                .Where(c => c.Estado == "Activo")
                .ToListAsync();

            return Ok(RolesActivos);
        }
        //Agregar Datos
        [HttpPost("Crear")]
        public async Task<ActionResult> Post([FromBody] DTORol rolDtos)
        {
            if (rolDtos == null)
            {
                return BadRequest("El objeto esta vacio");
            }
            var rolExistente = await dBconexion.Roles
            .FirstOrDefaultAsync(u => u.Descripcion == rolDtos.Descripcion);

            //Validar si rol ya existe
            if (rolExistente != null)
            {
                return BadRequest("Ya existe este Rol");
            }

            try
            {
                var rol = new Rol()
                {
                    Descripcion = rolDtos.Descripcion,
                    Estado = "Activo"
                };
                dBconexion.Add(rol);
                await dBconexion.SaveChangesAsync();
                return Ok("Se inserto correctamente");
            }
            catch (Exception)
            {
                return BadRequest("No se pudo insertar");
            }
        }
        //Actualizar Datos
        [HttpPut("{id}")]
        public async Task<ActionResult> PutRol(int id, Rol rol)
        {
            if (id != rol.Id)
            {
                return BadRequest("El id no existe");
            }

            var roles = await dBconexion.Roles.FindAsync(id);
            if (roles == null)
            {
                return NotFound("No se encontro el rol");
            }
            //Modificar uno por uno de acuerdo al dato

            /*if (rol.Descripcion.HasValue)
            {
                roles.Descripcion = rol.Descripcion.Value;
            }*/

            roles.Descripcion = rol.Descripcion;


            try
            {
                await dBconexion.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "Error no se pudo actualizar");
            }
            return Ok("Se modifico correctamente");
        }

        // Cambiar estado (Activo/Inactivo)
        [HttpDelete("CambiarEstado/{id}")]
        public async Task<ActionResult> CambiarEstadoRol(int id)
        {
            var rol = await dBconexion.Roles.FindAsync(id);
            if (rol == null)
            {
                return NotFound("No se encontro el Rol");
            }

            // Cambiar el estado
            rol.Estado = rol.Estado == "Activo" ? "Inactivo" : "Activo";

            try
            {
                await dBconexion.SaveChangesAsync();
                return Ok(new
                {
                    Message = $"El Rol esta {rol.Estado}",
                    Descripcion = rol.Descripcion,
                    NuevoEstado = rol.Estado
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "Error al cambiar el estado");
            }
        }
    }
}
