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
    public class PromocionesController : ControllerBase
    {
        public readonly DBconexion dBconexion;
        public PromocionesController(DBconexion dBconexion)
        {
            this.dBconexion = dBconexion;
        }
        //Listar Datos
        [HttpGet("Listar")]
        public async Task<ActionResult<Promocion>> Get()
        {
            var promocion = await dBconexion.Promociones.ToListAsync();
            return Ok(promocion);
        }
        // Listar Promociones activas
        [HttpGet("ListarActivos")]
        public async Task<ActionResult<IEnumerable<Promocion>>> GetActivo()
        {
            var PromocionesActivas = await dBconexion.Promociones
                .Where(c => c.Estado == "Activo")
                .ToListAsync();

            return Ok(PromocionesActivas);
        }
        //Agregar Datos
        [HttpPost("Crear")]
        public async Task<ActionResult> Post([FromBody] DTOPromocion promocionDtos)
        {
            if (promocionDtos == null)
            {
                return BadRequest("El objeto esta vacio");
            }

            //Validar Descripcion
            var DescripicionExistente = await dBconexion.Promociones
            .FirstOrDefaultAsync(u => u.Descripcion == promocionDtos.Descripcion);

            if (DescripicionExistente != null)
            {
                return BadRequest("Ya existe esta Promocion");
            }
            try
            {
                var promocion = new Promocion()
                {
                    Descripcion = promocionDtos.Descripcion,
                    Estado = "Activo"
                };
                dBconexion.Add(promocion);
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
        public async Task<ActionResult> PutPromocion(int id, Promocion promocion)
        {
            if (id != promocion.Id)
            {
                return BadRequest("El id no existe");
            }
            var promociones = await dBconexion.Promociones.FindAsync(id);
            if (promociones == null)
            {
                return NotFound("No se encontro la promocion");
            }

            //Modificar uno por uno de acuerdo al dato

            /*if (promocion.Descripcion.HasValue)
            {
                promociones.Descripcion = promocion.Descripcion.Value;
            }*/

            promociones.Descripcion = promocion.Descripcion;


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
        public async Task<ActionResult> CambiarEstadoPromocion(int id)
        {
            var promocion = await dBconexion.Promociones.FindAsync(id);
            if (promocion == null)
            {
                return NotFound("No se encontro la Promocion");
            }

            // Cambiar el estado
            promocion.Estado = promocion.Estado == "Activo" ? "Inactivo" : "Activo";

            try
            {
                await dBconexion.SaveChangesAsync();
                return Ok(new
                {
                    Message = $"La Promocion esta {promocion.Estado}",
                    Descripcion = promocion.Descripcion,
                    NuevoEstado = promocion.Estado
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "Error al cambiar el estado");
            }
        }
    }
}
