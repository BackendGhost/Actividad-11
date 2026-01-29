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
    public class DetalleVentasController : ControllerBase
    {
        public readonly DBconexion dBconexion;
        public DetalleVentasController(DBconexion dBconexion)
        {
            this.dBconexion = dBconexion;
        }
        //Listar Datos
        [HttpGet("Listar")]
        public async Task<ActionResult<DetalleVenta>> Get()
        {
            var detalleVenta = await dBconexion.DetalleVentas.ToListAsync();
            return Ok(detalleVenta);
        }
        // Listar DetalleVentas activas
        [HttpGet("ListarActivos")]
        public async Task<ActionResult<IEnumerable<DetalleVenta>>> GetActivo()
        {
            var DetalleVentaActivas = await dBconexion.DetalleVentas
                .Where(c => c.Estado == "Activo")
                .ToListAsync();

            return Ok(DetalleVentaActivas);
        }
        //ListarVenta
        [HttpGet("ListarVenta/{idVenta}")]
        public async Task<ActionResult<IEnumerable<DetalleVenta>>> GetVenta(int idVenta)
        {
            return await dBconexion.DetalleVentas
                .Where(t => t.id_Venta == idVenta)
                .Include(t => t.Venta)
                .Include(t => t.Producto)                // 1. Incluimos el Producto
                    .ThenInclude(p => p.Categoria)       // 2. Y dentro del Producto, incluimos su Categoría
                .ToListAsync();
        }
        //ListarProducto
        [HttpGet("ListarProducto/{idProducto}")]
        public async Task<ActionResult<IEnumerable<DetalleVenta>>> GetProducto(int idProducto)
        {
            return await dBconexion.DetalleVentas
                .Where(t => t.id_Producto == idProducto)
                .Include(t => t.Producto
                )
                .ToListAsync();
        }
        //ListarDescuento
        [HttpGet("ListarDescuento/{idDescuento}")]
        public async Task<ActionResult<IEnumerable<DetalleVenta>>> GetDescuento(int idDescuento)
        {
            return await dBconexion.DetalleVentas
                .Where(t => t.id_Descuento == idDescuento)
                .Include(t => t.Descuento)
                .ToListAsync();
        }
        //ListarPromocion
        [HttpGet("ListarPromocion/{idPromocion}")]
        public async Task<ActionResult<IEnumerable<DetalleVenta>>> GetPromocion(int idPromocion)
        {
            return await dBconexion.DetalleVentas
                .Where(t => t.id_Promocion == idPromocion)
                .Include(t => t.Promocion)
                .ToListAsync();
        }
        //Productos Mas Vendidos
        [HttpGet("ProductosMasVendidos")]
        public async Task<ActionResult<IEnumerable<object>>> GetProductosMasVendidos()
        {
            var productosMasVendidos = await dBconexion.DetalleVentas
                .Include(dv => dv.Producto) // Incluir información del producto
                .Where(dv => dv.Estado == "Activo") // Solo ventas activas si aplica
                .GroupBy(dv => new { dv.id_Producto, dv.Producto.Nombre }) // Agrupar por producto
                .Select(g => new
                {
                    IdProducto = g.Key.id_Producto,
                    NombreProducto = g.Key.Nombre,
                    TotalVendido = g.Sum(dv => dv.Cantidad), // Sumar cantidades vendidas
                    TotalIngresos = g.Sum(dv => dv.Subtotal) // Sumar subtotales
                })
                .OrderByDescending(p => p.TotalVendido) // Ordenar por más vendido primero
                .Take(10) // Limitar a los 10 más vendidos
                .ToListAsync();

            return Ok(productosMasVendidos);
        }

        //Agregar Datos
        [HttpPost("Crear")]
        public async Task<ActionResult> Post([FromBody] DTODetalleVenta detalleVentaDtos)
        {
            if (detalleVentaDtos == null)
            {
                return BadRequest("El objeto esta vacio");
            }

            // Validar existencia de venta
            var venta = await dBconexion.Ventas.FindAsync(detalleVentaDtos.id_Venta);
            if (venta == null)
            {
                return BadRequest("Venta no existe");
            }

            // Validar existencia y stock de producto
            var producto = await dBconexion.Productos.FindAsync(detalleVentaDtos.id_Producto);
            if (producto == null)
            {
                return BadRequest("Producto no existe");
            }

            // Verificar que haya suficiente stock
            if (producto.Cantidad < detalleVentaDtos.Cantidad)
            {
                return BadRequest($"No hay suficiente stock. Disponible: {producto.Cantidad}, Solicitado: {detalleVentaDtos.Cantidad}");
            }

            // Validar descuento (opcional si puede ser null)
            if (detalleVentaDtos.id_Descuento != 0) // Asumiendo que 0 significa sin descuento
            {
                var descuento = await dBconexion.Descuentos.FindAsync(detalleVentaDtos.id_Descuento);
                if (descuento == null)
                {
                    return BadRequest("Descuento no existe");
                }
            }

            // Validar promoción (opcional si puede ser null)
            if (detalleVentaDtos.id_Promocion != 0) // Asumiendo que 0 significa sin promoción
            {
                var promocion = await dBconexion.Promociones.FindAsync(detalleVentaDtos.id_Promocion);
                if (promocion == null)
                {
                    return BadRequest("Promoción no existe");
                }
            }

            using var transaction = await dBconexion.Database.BeginTransactionAsync();
            try
            {
                // 1. Actualizar stock del producto
                producto.Cantidad -= detalleVentaDtos.Cantidad;
                dBconexion.Productos.Update(producto);

                // 2. Crear el detalle de venta
                var detalleVenta = new DetalleVenta()
                {
                    id_Venta = detalleVentaDtos.id_Venta,
                    id_Producto = detalleVentaDtos.id_Producto,
                    id_Descuento = detalleVentaDtos.id_Descuento != 0 ? detalleVentaDtos.id_Descuento : null,
                    id_Promocion = detalleVentaDtos.id_Promocion != 0 ? detalleVentaDtos.id_Promocion : null,
                    Cantidad = detalleVentaDtos.Cantidad,
                    Subtotal = detalleVentaDtos.Subtotal,
                    Estado = "Activo"
                };

                dBconexion.DetalleVentas.Add(detalleVenta);

                // 3. Guardar todos los cambios
                await dBconexion.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new
                {
                    Message = "Detalle de venta creado y stock actualizado correctamente",
                    StockRestante = producto.Cantidad
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Error al procesar la venta: {ex.Message}");
            }
        }



        //Actualizar Datos
        [HttpPut("{id}")]
        public async Task<ActionResult> PutDetalleVenta(int id, DetalleVenta detalleVenta)
        {
            if (id != detalleVenta.Id)
            {
                return BadRequest("El id no existe");
            }
            var detalleVentas = await dBconexion.DetalleVentas.FindAsync(id);
            if (detalleVentas == null)
            {
                return NotFound("No se encontro el usuario");
            }
            //Modificar uno por uno de acuerdo al dato
            if (detalleVenta.id_Venta.HasValue)
            {
                var idVentaRolValido = await dBconexion.Ventas.AnyAsync(t => t.Id == detalleVenta.id_Venta.Value);
                if (idVentaRolValido)
                {
                    detalleVentas.id_Venta = detalleVenta.id_Venta.Value;
                }
                else
                {
                    return BadRequest("El id del Detalle Venta Rol no existe");
                }
            }

            if (detalleVenta.id_Producto.HasValue)
            {
                var idProductoValido = await dBconexion.Productos.AnyAsync(t => t.Id == detalleVenta.id_Producto.Value);
                if (idProductoValido)
                {
                    detalleVentas.id_Producto = detalleVenta.id_Producto.Value;
                }
                else
                {
                    return BadRequest("El id del Producto no existe");
                }
            }

            if (detalleVenta.id_Descuento.HasValue)
            {
                var idDescuentoValido = await dBconexion.Descuentos.AnyAsync(t => t.Id == detalleVenta.id_Descuento.Value);
                if (idDescuentoValido)
                {
                    detalleVentas.id_Descuento = detalleVenta.id_Descuento.Value;
                }
                else
                {
                    return BadRequest("El id del Descuento no existe");
                }
            }

            if (detalleVenta.id_Promocion.HasValue)
            {
                var idPromocionValido = await dBconexion.Promociones.AnyAsync(t => t.Id == detalleVenta.id_Promocion.Value);
                if (idPromocionValido)
                {
                    detalleVentas.id_Promocion = detalleVenta.id_Promocion.Value;
                }
                else
                {
                    return BadRequest("el id de la Promocion no existe");
                }
            }


            if (detalleVenta.Cantidad.HasValue)
            {
                detalleVentas.Cantidad = detalleVenta.Cantidad.Value;
            }


            if (detalleVenta.Subtotal.HasValue)
            {
                detalleVentas.Subtotal = detalleVenta.Subtotal.Value;
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
        public async Task<ActionResult> CambiarEstadoDetalleVenta(int id)
        {
            var detalleVenta = await dBconexion.DetalleVentas.FindAsync(id);
            if (detalleVenta == null)
            {
                return NotFound("No se encontro la Categoria");
            }

            // Cambiar el estado
            detalleVenta.Estado = detalleVenta.Estado == "Activo" ? "Inactivo" : "Activo";

            try
            {
                await dBconexion.SaveChangesAsync();
                return Ok(new
                {
                    Message = $"El detalle venta esta {detalleVenta.Estado}",
                    id = detalleVenta.Id,
                    NuevoEstado = detalleVenta.Estado
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "Error al cambiar el estado");
            }
        }
    }
}
