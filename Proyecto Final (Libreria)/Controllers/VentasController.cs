using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_Final__Libreria_.Data;
using Proyecto_Final__Libreria_.DTO;
using Proyecto_Final__Libreria_.Models;
using System.Globalization;

namespace Proyecto_Final__Libreria_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("Cors")]
    public class VentasController : ControllerBase
    {
        public readonly DBconexion dBconexion;
        public VentasController(DBconexion dBconexion)
        {
            this.dBconexion = dBconexion;
        }

        //Listar Datos
        [HttpGet("Listar")]
        public async Task<ActionResult<Venta>> Get()
        {
            var venta = await dBconexion.Ventas.ToListAsync();
            return Ok(venta);
        }
        //Listar Ventas Activas
        [HttpGet("ListarActivos")]
        public async Task<ActionResult<IEnumerable<Venta>>> GetActivo()
        {
            var VentaActivas = await dBconexion.Ventas
                .Where(c => c.Estado == "Activo")
                .ToListAsync();

            return Ok(VentaActivas);
        }
        //ListarCalificacion
        [HttpGet("ListarCalificacion/{idCalificacion}")]
        public async Task<ActionResult<IEnumerable<Venta>>> GetCalificacion(int idCalificacion)
        {
            return await dBconexion.Ventas
                .Where(t => t.id_Calificacion == idCalificacion)
                .Include(t => t.Calificacion)
                .ToListAsync();
        }
        //ListarUsuario_Roles
        [HttpGet("ListarUsuarioRol/{idUsuarioRol}")]
        public async Task<ActionResult<IEnumerable<Venta>>> GetUsuarioRol(int idUsuarioRol)
        {
            return await dBconexion.Ventas
                .Where(t => t.id_UsuarioRol == idUsuarioRol)
                .Include(t => t.Usuario_Rol)
                .ToListAsync();
        }
        // Listar ventas de un mes y año
        [HttpGet("VentasPorMes/{anio}/{mes}")]
        public async Task<ActionResult<object>> GetVentasPorMes(int anio, int mes)
        {
            try
            {
                var primerDiaDelMes = new DateTime(anio, mes, 1);
                var ultimoDiaDelMes = primerDiaDelMes.AddMonths(1).AddDays(-1);

                // Obtener las ventas del mes
                var ventasDelMes = await dBconexion.Ventas
                    .Where(v => v.Fecha >= primerDiaDelMes && v.Fecha <= ultimoDiaDelMes)
                    .Include(v => v.Usuario_Rol)
                    .Include(v => v.Calificacion)
                    .ToListAsync();

                // Calcular el total de ganancias
                var totalGanancias = ventasDelMes
                    .Where(v => v.Total.HasValue)
                    .Sum(v => v.Total.Value);

                // Crear objeto de respuesta que incluye tanto las ventas como el total
                var resultado = new
                {
                    Ventas = ventasDelMes,
                    TotalGanancias = totalGanancias,
                    CantidadVentas = ventasDelMes.Count,
                    Mes = mes,
                    Anio = anio,
                    NombreMes = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(mes)
                };

                return Ok(resultado);
            }
            catch (ArgumentOutOfRangeException)
            {
                return BadRequest("Mes o año inválido");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener las ventas: {ex.Message}");
            }
        }
        //Listar toda la informacion de usuarios y usuario rol
        [HttpGet("ListarVentasCompletas")]
        public async Task<ActionResult<IEnumerable<object>>> GetVentasCompletas()
        {
            var ventasCompletas = await dBconexion.Ventas
                .Include(v => v.Usuario_Rol) // Incluir Usuario_Rol
                .ThenInclude(ur => ur.Rol) // Incluir Rol desde Usuario_Rol
                .Include(v => v.Usuarios) // Incluir Usuario directamente
                .Include(v => v.Calificacion) // Incluir Calificación si es necesario
                .Where(v => v.Estado == "Activo") // Filtro opcional para solo ventas activas
                .OrderByDescending(v => v.Fecha) // Ordenar por fecha más reciente primero
                .Select(v => new
                {
                    // Información de la Venta
                    VentaId = v.Id,
                    Fecha = v.Fecha,
                    Total = v.Total,
                    EstadoVenta = v.Estado,
                    CalificacionId = v.id_Calificacion,
                    Calificacion = v.Calificacion != null ? v.Calificacion.Nota : null, // Asumiendo que Calificacion tiene Nombre

                    // Información del Usuario (directo)
                    UsuarioId = v.id_Usuario,
                    UsuarioNombre = v.Usuarios != null ? v.Usuarios.Nombre : null, // Asumiendo que Usuarios tiene Nombre

                    // Información de Usuario_Rol
                    UsuarioRolId = v.id_UsuarioRol,
                    UsuarioRolNombre = v.Usuario_Rol != null ? v.Usuario_Rol.Usuarios.Nombre : null,
                    EstadoUsuarioRol = v.Usuario_Rol != null ? v.Usuario_Rol.Estado : null,

                    // Información del Rol (a través de Usuario_Rol)
                    RolId = v.Usuario_Rol != null ? v.Usuario_Rol.id_Rol : null,
                    RolNombre = v.Usuario_Rol != null && v.Usuario_Rol.Rol != null ? v.Usuario_Rol.Rol.Descripcion : null
                })
                .ToListAsync();

            return Ok(ventasCompletas);
        }

        //Agregar Datos
        [HttpPost("Crear")]
        public async Task<ActionResult> Post([FromBody] DTOVenta ventaDtos)
        {
            if (ventaDtos == null)
            {
                return BadRequest("El objeto esta vacio");
            }

            var idUsuarioRol = await dBconexion.Usuario_Roles.FindAsync(ventaDtos.id_UsuarioRol);
            if (idUsuarioRol == null)
            {
                return BadRequest("Usuario rol no existe");
            }

            var idCalificacion = await dBconexion.Calificaciones.FindAsync(ventaDtos.id_Calificacion);
            if (idCalificacion == null)
            {
                return BadRequest("Calificacion no existe");
            }

            var idUsuario = await dBconexion.Usuarioss.FindAsync(ventaDtos.id_Usuario);
            if (idUsuario == null)
            {
                return BadRequest("Usuario no existe");
            }
            try
            {
                var venta = new Venta()
                {
                    id_UsuarioRol = ventaDtos.id_UsuarioRol,
                    id_Calificacion = ventaDtos.id_Calificacion,
                    id_Usuario = ventaDtos.id_Usuario,
                    Fecha = ventaDtos.Fecha,
                    Total = ventaDtos.Total,
                    Estado = "Activo"
                };
                dBconexion.Add(venta);
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
        public async Task<ActionResult> PutVenta(int id, Venta venta)
        {
            if (id != venta.Id)
            {
                return BadRequest("El id no existe");
            }
            var ventas = await dBconexion.Ventas.FindAsync(id);
            if (ventas == null)
            {
                return NotFound("No se encontro la venta");
            }
            //Modificar uno por uno de acuerdo al dato
            if (venta.id_UsuarioRol.HasValue)
            {
                var idUsuarioRolValido = await dBconexion.Usuario_Roles.AnyAsync(t => t.Id == venta.id_UsuarioRol.Value);
                if (idUsuarioRolValido)
                {
                    ventas.id_UsuarioRol = venta.id_UsuarioRol.Value;
                }
                else
                {
                    return BadRequest("El id del Usuario Rol no existe");
                }
            }

            if (venta.id_Calificacion.HasValue)
            {
                var idCalificacionValido = await dBconexion.Calificaciones.AnyAsync(t => t.Id == venta.id_Calificacion.Value);
                if (idCalificacionValido)
                {
                    ventas.id_Calificacion = venta.id_Calificacion.Value;
                }
                else
                {
                    return BadRequest("El id de la Calificacion no existe");
                }
            }

            if (venta.id_Usuario.HasValue)
            {
                var idUsuarioValido = await dBconexion.Usuarioss.AnyAsync(t => t.Id == venta.id_Usuario.Value);
                if (idUsuarioValido) 
                {
                    ventas.id_Calificacion = venta.id_Usuario.Value;
                }
                else
                {
                    return BadRequest("El id del Usuario no existe");
                }
            }

            if (venta.Fecha.HasValue)
            {
                ventas.Fecha = venta.Fecha.Value;
            }

            if (venta.Total.HasValue)
            {
                ventas.Total = venta.Total.Value;
            }

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
        public async Task<ActionResult> CambiarEstadoVenta(int id)
        {
            var venta = await dBconexion.Ventas.FindAsync(id);
            if (venta == null)
            {
                return NotFound("No se encontro la Venta");
            }

            // Cambiar el estado
            venta.Estado = venta.Estado == "Activo" ? "Inactivo" : "Activo";

            try
            {
                await dBconexion.SaveChangesAsync();
                return Ok(new
                {
                    Message = $"La Venta esta {venta.Estado}",
                    id = venta.Id,
                    NuevoEstado = venta.Estado
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "Error al cambiar el estado");
            }
        }
    }
}
