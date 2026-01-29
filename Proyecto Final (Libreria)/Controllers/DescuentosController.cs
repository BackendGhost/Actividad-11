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
    public class DescuentosController : ControllerBase
    {
        public readonly DBconexion dBconexion;
        public DescuentosController(DBconexion dBconexion)
        {
            this.dBconexion = dBconexion;
        }
        //Listar Datos
        [HttpGet("Listar")]
        public async Task<ActionResult<Descuento>> Get()
        {
            var descuento = await dBconexion.Descuentos.ToListAsync();
            return Ok(descuento);
        }
        // Listar descuentos activas
        [HttpGet("ListarActivos")]
        public async Task<ActionResult<IEnumerable<Descuento>>> GetActivo()
        {
            var DescuentosActivos = await dBconexion.Descuentos
                .Where(c => c.Estado == "Activo")
                .ToListAsync();

            return Ok(DescuentosActivos);
        }
        //Agregar Datos
        [HttpPost("Crear")]
        public async Task<ActionResult> Post([FromBody] DTODescuento descuentoDtos)
        {
            if (descuentoDtos == null)
            {
                return BadRequest("El objeto esta vacio");
            }

            //Validar Descuento
            var DescuentosExistente = await dBconexion.Descuentos
            .FirstOrDefaultAsync(u => u.Porcentaje == descuentoDtos.Porcentaje);

            if (DescuentosExistente != null)
            {
                return BadRequest("Ya existe este Descuento");
            }

            try
            {
                var descuento = new Descuento()
                {
                    Porcentaje = descuentoDtos.Porcentaje,
                    Estado = "Activo"
                };
                dBconexion.Add(descuento);
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
        public async Task<ActionResult> PutDescuento(int id, Descuento descuento)
        {
            if (id != descuento.Id)
            {
                return BadRequest("El id no existe");
            }
            var descuentos = await dBconexion.Descuentos.FindAsync(id);
            if (descuentos == null)
            {
                return NotFound("No se encontro el descuento");
            }

            //Modificar uno por uno de acuerdo al dato

            if (descuento.Porcentaje.HasValue)
            {
                descuentos.Porcentaje = descuento.Porcentaje.Value;
            }


            /*if (descuento.Estado.HasValue)
            {
                descuentos.Estado = descuento.Estado.Value;
            }*/

            descuentos.Estado = descuento.Estado;
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
        public async Task<ActionResult> CambiarEstadoDescuento(int id)
        {
            var descuento = await dBconexion.Descuentos.FindAsync(id);
            if (descuento == null)
            {
                return NotFound("No se encontro el Descuento");
            }

            // Cambiar el estado
            descuento.Estado = descuento.Estado == "Activo" ? "Inactivo" : "Activo";

            try
            {
                await dBconexion.SaveChangesAsync();
                return Ok(new
                {
                    Message = $"El Descuento esta {descuento.Estado}",
                    Porcentaje = descuento.Porcentaje,
                    NuevoEstado = descuento.Estado
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "Error al cambiar el estado");
            }
        }
    }
}
