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
    public class CalificacionesController : ControllerBase
    {
        public readonly DBconexion dBconexion;
        public CalificacionesController(DBconexion dBconexion)
        {
            this.dBconexion = dBconexion;
        }
        //Listar Datos
        [HttpGet("Listar")]
        public async Task<ActionResult<Calificacion>> Get()
        {
            var calificacion = await dBconexion.Calificaciones.ToListAsync();
            return Ok(calificacion);
        }

        // Listar calificaciones activas
        [HttpGet("ListarActivos")]
        public async Task<ActionResult<IEnumerable<Calificacion>>> GetActivo()
        {
            var calificacionesActivas = await dBconexion.Calificaciones
                .Where(c => c.Estado == "Activo")
                .ToListAsync();

            return Ok(calificacionesActivas);
        }

        //Agregar Datos
        [HttpPost("Crear")]
        public async Task<ActionResult> Post([FromBody] DTOCalificacion calificacionDtos)
        {
            if (calificacionDtos == null)
            {
                return BadRequest("El objeto esta vacio");
            }
            //Validar Nombre Completo
            var NotaExistente = await dBconexion.Calificaciones
            .FirstOrDefaultAsync(u => u.Nota == calificacionDtos.Nota);

            if (NotaExistente != null)
            {
                return BadRequest("Ya existe esta Nota");
            }

            try
            {
                var calificacion = new Calificacion()
                {
                    Nota = calificacionDtos.Nota,
                    Estado = "Activo"
                };
                dBconexion.Add(calificacion);
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
        public async Task<ActionResult> PutCalificacion(int id, Calificacion calificacion)
        {
            if (id != calificacion.Id)
            {
                return BadRequest("El id no existe");
            }
            var calificaciones = await dBconexion.Calificaciones.FindAsync(id);
            if (calificaciones == null)
            {
                return NotFound("No se encontro la calificacion");
            }

            //Modificar uno por uno de acuerdo al dato

            /*if (calificacion.Nota.HasValue)
            {
                calificaciones.Nota = calificacion.Nota.Value;
            }*/
            calificaciones.Nota = calificacion.Nota;

            
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
        public async Task<ActionResult> CambiarEstadoCalificacion(int id)
        {
            var calificacion = await dBconexion.Calificaciones.FindAsync(id);
            if (calificacion == null)
            {
                return NotFound("No se encontro la Calificacion");
            }

            // Cambiar el estado
            calificacion.Estado = calificacion.Estado == "Activo" ? "Inactivo" : "Activo";

            try
            {
                await dBconexion.SaveChangesAsync();
                return Ok(new
                {
                    Message = $"La Calificacion esta {calificacion.Estado}",
                    nota = calificacion.Nota,
                    NuevoEstado = calificacion.Estado
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "Error al cambiar el estado");
            }
        }
    }
}
