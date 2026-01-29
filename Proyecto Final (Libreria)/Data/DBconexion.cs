using Microsoft.EntityFrameworkCore;
using Proyecto_Final__Libreria_.Models;

namespace Proyecto_Final__Libreria_.Data
{
    public class DBconexion :DbContext
    { 
        public DBconexion(DbContextOptions<DBconexion> options) : base(options) { }
        public DbSet<Calificacion> Calificaciones { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<Descuento> Descuentos { get; set; }
        public DbSet<DetalleVenta> DetalleVentas { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Promocion> Promociones { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Usuario_Rol> Usuario_Roles { get; set; }
        public DbSet<Usuarios> Usuarioss { get; set; }
        public DbSet<Venta> Ventas { get; set; }
    }
}
