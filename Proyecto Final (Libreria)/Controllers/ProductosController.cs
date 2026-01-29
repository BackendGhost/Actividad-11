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
    public class ProductosController : ControllerBase
    {
        public readonly DBconexion dBconexion;
        public ProductosController(DBconexion dBconexion)
        {
            this.dBconexion = dBconexion;
        }
        //Listar Datos
        [HttpGet("Listar")]
        public async Task<ActionResult<Producto>> Get()
        {
            var producto = await dBconexion.Productos.ToListAsync();
            return Ok(producto);
        }
        // Listar Productos activas
        [HttpGet("ListarActivos")]
        public async Task<ActionResult<IEnumerable<Producto>>> GetActivo()
        {
            var ProductoActivas = await dBconexion.Productos
                .Where(c => c.Estado == "Activo")
                .ToListAsync();

            return Ok(ProductoActivas);
        }

        //ListarCategoria
        [HttpGet("ListarCategoria/{idCategoria}")]
        public async Task<ActionResult<IEnumerable<Producto>>> GetCategoria(int idCategoria)
        {
            return await dBconexion.Productos
                .Where(t => t.id_Categoria == idCategoria)
                .Include(t => t.Categoria)
                .ToListAsync();
        }
        //Agregar Datos
        [HttpPost("Crear")]
        public async Task<ActionResult> Post([FromBody] DTOProducto productoDtos)
        {
            if (productoDtos == null)
            {
                return BadRequest("El objeto esta vacio");
            }
            var idCategoria = await dBconexion.Categorias.FindAsync(productoDtos.id_Categoria);
            if (idCategoria == null)
            {
                return BadRequest("Categoria no existe");
            }

            //Validar Descuento
            var NombreExistente = await dBconexion.Productos
            .FirstOrDefaultAsync(u => u.Nombre == productoDtos.Nombre);

            if (NombreExistente != null)
            {
                return BadRequest("Ya existe este Nombre del Producto");
            }

            try
            {
                var producto = new Producto()
                {
                    id_Categoria = productoDtos.id_Categoria,
                    Nombre = productoDtos.Nombre,
                    Descripcion = productoDtos.Descripcion,
                    Cantidad = productoDtos.Cantidad,
                    Precio = productoDtos.Precio,
                    Estado = "Activo"
                };
                dBconexion.Add(producto);
                await dBconexion.SaveChangesAsync();
                return Ok("Se inserto correctamente");
            }
            catch (Exception)
            {
                return BadRequest("No se pudo insertar");
            }

        }
        //Listar Productos Bajos
        [HttpGet("ListarBajoStock")]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductosBajoStock()
        {
            var productosBajoStock = await dBconexion.Productos
                .Where(p => p.Cantidad.HasValue && p.Cantidad.Value < 10)
                .Include(p => p.Categoria) // Opcional: incluir información de categoría
                .OrderBy(p => p.Cantidad) // Ordenar por cantidad (de menor a mayor)
                .ToListAsync();

            return Ok(productosBajoStock);
        }
        //Actualizar Datos
        [HttpPut("{id}")]
        public async Task<ActionResult> PutProducto(int id, Producto producto)
        {
            if (id != producto.Id)
            {
                return BadRequest("El id no existe");
            }

            var productos = await dBconexion.Productos.FindAsync(id);
            if (productos == null)
            {
                return NotFound("No se encontro el Producto");
            }

            
            //Modificar uno por uno de acuerdo al dato
            if (producto.id_Categoria.HasValue)
            {
                var idProductoValido = await dBconexion.Categorias.AnyAsync(t => t.Id == producto.id_Categoria.Value);
                if (idProductoValido)
                {
                    productos.id_Categoria = producto.id_Categoria.Value;
                }
                else
                {
                    return BadRequest("El id la Categoria no existe");
                }
            }

            /*if (producto.Nombre.HasValue)
            {
                productos.Nombre = producto.Nombre.Value;
            }*/
            productos.Nombre = producto.Nombre;


            /*if (producto.Descripcion.HasValue)
            {
                productos.Descripcion = producto.Descripcion.Value;
            }*/
            productos.Descripcion = producto.Descripcion;


            if (producto.Cantidad.HasValue)
            {
                productos.Cantidad = producto.Cantidad.Value;
            }


            if (producto.Precio.HasValue)
            {
                productos.Precio = producto.Precio.Value;
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
        public async Task<ActionResult> CambiarEstadoProducto(int id)
        {
            var producto = await dBconexion.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound("No se encontro el Producto");
            }

            // Cambiar el estado
            producto.Estado = producto.Estado == "Activo" ? "Inactivo" : "Activo";

            try
            {
                await dBconexion.SaveChangesAsync();
                return Ok(new
                {
                    Message = $"El Producto esta {producto.Estado}",
                    Nombre = producto.Nombre,
                    NuevoEstado = producto.Estado
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "Error al cambiar el estado");
            }
        }
    }
}
