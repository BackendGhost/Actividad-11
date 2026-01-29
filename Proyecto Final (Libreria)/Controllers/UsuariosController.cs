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
    public class UsuariosController : ControllerBase
    {
        public readonly DBconexion dBconexion;
        public UsuariosController(DBconexion dBconexion)
        {
            this.dBconexion = dBconexion;
        }
        //Listar Datos
        [HttpGet("Listar")]
        public async Task<ActionResult<Usuarios>> Get()
        {
            var usuario = await dBconexion.Usuarioss.ToListAsync();
            return Ok(usuario);
        }
        // Listar usuarios activas
        [HttpGet("ListarActivos")]
        public async Task<ActionResult<IEnumerable<Usuarios>>> GetActivo()
        {
            var usuariosActivos = await dBconexion.Usuarioss
                .Where(c => c.Estado == "Activo")
                .ToListAsync();

            return Ok(usuariosActivos);
        }

        [HttpPost("Crear")]
        public async Task<ActionResult<Usuarios>> Post([FromBody] DTOUsuarios usuarioDtos)
        {
            if (usuarioDtos == null)
            {
                return BadRequest("El objeto esta vacio");
            }

            //Validar Nombre Completo
            var NombreExistente = await dBconexion.Usuarioss
            .FirstOrDefaultAsync(u => u.Nombre == usuarioDtos.Nombre);

            if (NombreExistente != null)
            {
                return BadRequest("Ya existe este Nombre Completo");
            }

            //Validar CI
            var CIExistente = await dBconexion.Usuarioss
            .FirstOrDefaultAsync(u => u.CI == usuarioDtos.CI);

            if (CIExistente != null)
            {
                return BadRequest("Ya existe este CI");
            }

            //Validar Telefono
            var TelefonoExistente = await dBconexion.Usuarioss
            .FirstOrDefaultAsync(u => u.Telefono == usuarioDtos.Telefono);

            if (TelefonoExistente != null)
            {
                return BadRequest("Ya existe este Telefono");
            }

            try
            {
                var usuario = new Usuarios()
                {
                    Nombre = usuarioDtos.Nombre,
                    CI = usuarioDtos.CI,
                    Telefono = usuarioDtos.Telefono,
                    Estado = "Activo"
                };

                dBconexion.Add(usuario);

                // Al guardar, la base de datos asigna el ID y Entity Framework actualiza el objeto 'usuario'
                await dBconexion.SaveChangesAsync();

                // CORRECCIÓN: Devolvemos el objeto usuario (que ya tiene el ID) en lugar de texto
                return Ok(usuario);
            }
            catch (Exception ex)
            {
                // Es útil devolver el mensaje de la excepción para saber qué pasó
                return BadRequest("No se pudo insertar: " + ex.Message);
            }
        }
        //Actualizar Datos
        [HttpPut("{id}")]
        public async Task<ActionResult> PutUsuario(int id, Usuarios usuario)
        {
            if (id != usuario.Id)
            {
                return BadRequest("El id no existe");
            }
            var usuarios = await dBconexion.Usuarioss.FindAsync(id);
            if (usuarios == null)
            {
                return NotFound("No se encontro el usuario");
            }

            
            //Modificar uno por uno de acuerdo al dato

            /*if (usuario.Nombre.HasValue)
            {
                usuarios.Nombre = usuario.Nombre.Value;
            }*/

            usuarios.Nombre = usuario.Nombre;

            /*if (usuario.CI.HasValue)
            {
                usuarios.CI = usuario.CI.Value;
            }*/

            usuarios.CI = usuario.CI;

            /*if (usuario.Telefono.HasValue)
            {
                usuarios.Telefono = usuario.Telefono.Value;
            }*/

            usuarios.Telefono = usuario.Telefono;

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
        public async Task<ActionResult> CambiarEstadoUsuario(int id)
        {
            var usuario = await dBconexion.Usuarioss.FindAsync(id);
            if (usuario == null)
            {
                return NotFound("No se encontro el usuario");
            }

            // Cambiar el estado
            usuario.Estado = usuario.Estado == "Activo" ? "Inactivo" : "Activo";

            try
            {
                await dBconexion.SaveChangesAsync();
                return Ok(new
                {
                    Message = $"El Usuario esta {usuario.Estado}",
                    Nombre = usuario.Nombre,
                    NuevoEstado = usuario.Estado
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "Error al cambiar el estado");
            }
        }
    }
}
