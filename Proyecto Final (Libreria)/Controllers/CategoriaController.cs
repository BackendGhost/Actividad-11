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
    public class CategoriaController : ControllerBase
    {
        public readonly DBconexion dBconexion;
        public CategoriaController(DBconexion dBconexion)
        {
            this.dBconexion = dBconexion;
        }
        //Listar Datos
        [HttpGet("Listar")]
        public async Task<ActionResult<Categoria>> Get()
        {
            var categoria = await dBconexion.Categorias.ToListAsync();
            return Ok(categoria);
        }
        // Listar Categoria activas
        [HttpGet("ListarActivos")]
        public async Task<ActionResult<IEnumerable<Categoria>>> GetActivo()
        {
            var CategoriaActivas = await dBconexion.Categorias
                .Where(c => c.Estado == "Activo")
                .ToListAsync();

            return Ok(CategoriaActivas);
        }
        //Agregar Datos
        [HttpPost("Crear")]
        public async Task<ActionResult> Post([FromBody] DTOCategoria categoriaDtos)
        {
            if (categoriaDtos == null)
            {
                return BadRequest("El objeto esta vacio");
            }

            //Validar Descuento
            var CategoriaExistente = await dBconexion.Categorias
            .FirstOrDefaultAsync(u => u.Descripcion == categoriaDtos.Descripcion);

            if (CategoriaExistente != null)
            {
                return BadRequest("Ya existe este Categoria");
            }

            try
            {
                var categoria = new Categoria()
                {
                    Descripcion = categoriaDtos.Descripcion,
                    Estado = "Activo"
                };
                dBconexion.Add(categoria);
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
        public async Task<ActionResult> PutCategoria(int id, Categoria categoria)
        {
            if (id != categoria.Id)
            {
                return BadRequest("El id no existe");
            }
            var categorias = await dBconexion.Categorias.FindAsync(id);
            if (categorias == null)
            {
                return NotFound("No se encontro la categoria");
            }


            //Modificar uno por uno de acuerdo al dato

            /*if (categoria.Descripcion.HasValue)
            {
                categorias.Descripcion = categoria.Descripcion.Value;
            }*/

            categorias.Descripcion = categoria.Descripcion;

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
        public async Task<ActionResult> CambiarEstadoCategoria(int id)
        {
            var categoria = await dBconexion.Categorias.FindAsync(id);
            if (categoria == null)
            {
                return NotFound("No se encontro la Categoria");
            }

            // Cambiar el estado
            categoria.Estado = categoria.Estado == "Activo" ? "Inactivo" : "Activo";

            try
            {
                await dBconexion.SaveChangesAsync();
                return Ok(new
                {
                    Message = $"La categoria esta {categoria.Estado}",
                    Descripcion = categoria.Descripcion,
                    NuevoEstado = categoria.Estado
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "Error al cambiar el estado");
            }
        }
    }
}
