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
    public class ComprasController : ControllerBase
    {
        public readonly DBconexion dBconexion;
        public ComprasController(DBconexion dBconexion)
        {
            this.dBconexion = dBconexion;
        }
        //Listar Datos
        [HttpGet("Listar")]
        public async Task<ActionResult<Compra>> Get()
        {
            var compra = await dBconexion.Compras.ToListAsync();
            return Ok(compra);
        }

        // Listar Compras activas
        [HttpGet("ListarActivos")]
        public async Task<ActionResult<IEnumerable<Compra>>> GetActivo()
        {
            var ComprasActivas = await dBconexion.Compras
                .Where(c => c.Estado == "Activo")
                .ToListAsync();

            return Ok(ComprasActivas);
        }

        //ListarProducto
        [HttpGet("ListarProducto/{idProducto}")]
        public async Task<ActionResult<IEnumerable<Compra>>> GetProducto(int idProducto)
        {
            return await dBconexion.Compras
                .Where(t => t.id_Producto == idProducto)
                .Include(t => t.Producto)
                .ToListAsync();
        }
        //ListarUsuarioRol
        [HttpGet("ListarUsuarioRol/{idUsuarioRol}")]
        public async Task<ActionResult<IEnumerable<Compra>>> GetUsuarioRol(int idUsuarioRol)
        {
            return await dBconexion.Compras
                .Where(t => t.id_UsuarioRol == idUsuarioRol)
                .Include(t => t.UsuarioRol)
                .ToListAsync();
        }
        [HttpPost("Crear")]
        public async Task<ActionResult> Post([FromBody] DTOCompra compraDtos)
        {
            if (compraDtos == null)
            {
                return BadRequest("El objeto está vacío");
            }

            // Validar UsuarioRol
            var usuarioRol = await dBconexion.Usuario_Roles.FindAsync(compraDtos.id_UsuarioRol);
            if (usuarioRol == null)
            {
                return BadRequest("Usuario Rol no existe");
            }

            // Validar Producto
            var producto = await dBconexion.Productos.FindAsync(compraDtos.id_Producto);
            if (producto == null)
            {
                return BadRequest("Producto no existe");
            }

            // Validar que la cantidad sea positiva
            if (compraDtos.Cantidad <= 0)
            {
                return BadRequest("La cantidad debe ser mayor a cero");
            }

            using var transaction = await dBconexion.Database.BeginTransactionAsync();
            try
            {
                // 1. Actualizar stock del producto (aumentar cantidad)
                producto.Cantidad += compraDtos.Cantidad;
                dBconexion.Productos.Update(producto);

                // 2. Crear la compra
                var compra = new Compra()
                {
                    id_UsuarioRol = compraDtos.id_UsuarioRol,
                    id_Producto = compraDtos.id_Producto,
                    Descripcion = compraDtos.Descripcion,
                    Cantidad = compraDtos.Cantidad,
                    Total = compraDtos.Total,
                    Estado = "Activo"
                };

                dBconexion.Add(compra);

                // 3. Guardar todos los cambios
                await dBconexion.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new
                {
                    Message = "Compra registrada y stock actualizado correctamente",
                    StockActual = producto.Cantidad,
                    CompraId = compra.Id
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Error al procesar la compra: {ex.Message}");
            }
        }

        //Actualizar Datos
        [HttpPut("{id}")]
        public async Task<ActionResult> PutCompra(int id, Compra compra)
        {
            if (id != compra.Id)
            {
                return BadRequest("El id no existe");
            }
            var compras = await dBconexion.Compras.FindAsync(id);
            if (compras == null)
            {
                return NotFound("No se encontro el usuario");
            }
            //Modificar uno por uno de acuerdo al dato
            if (compra.id_UsuarioRol.HasValue)
            {
                var idUsuarioRolValido = await dBconexion.Usuario_Roles.AnyAsync(t => t.Id == compra.id_UsuarioRol.Value);
                if (idUsuarioRolValido)
                {
                    compras.id_UsuarioRol = compra.id_UsuarioRol.Value;
                }
                else
                {
                    return BadRequest("El id del Usuario Rol no existe");
                }
            }

            if (compra.id_Producto.HasValue)
            {
                var idProductoValido = await dBconexion.Productos.AnyAsync(t => t.Id == compra.id_Producto.Value);
                if (idProductoValido)
                {
                    compras.id_Producto = compra.id_Producto.Value;
                }
                else
                {
                    return BadRequest("El id del Producto no existe");
                }
            }

            /*if (compra.Descripcion.HasValue)
            {
                compras.Descripcion = compra.Descripcion.Value;
            }*/
            compras.Descripcion = compra.Descripcion;


            if (compra.Cantidad.HasValue)
            {
                compras.Cantidad = compra.Cantidad.Value;
            }


            if (compra.Total.HasValue)
            {
                compras.Total = compra.Total.Value;
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
        public async Task<ActionResult> CambiarEstadoCompra(int id)
        {
            var compra = await dBconexion.Compras.FindAsync(id);
            if (compra == null)
            {
                return NotFound("No se encontro la Compra");
            }

            // Cambiar el estado
            compra.Estado = compra.Estado == "Activo" ? "Inactivo" : "Activo";

            try
            {
                await dBconexion.SaveChangesAsync();
                return Ok(new
                {
                    Message = $"La Compra esta {compra.Estado}",
                    id = compra.Id,
                    NuevoEstado = compra.Estado
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "Error al cambiar el estado");
            }
        }
    }
}
