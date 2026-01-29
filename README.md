# Actividad-11 — Librería Web y ERP (Backend)

Resumen
-------
Proyecto integrador: backend Web API para una "Librería Web y ERP" — gestión de usuarios y roles, inventario (categorías y productos), compras/ventas, promociones y control de stock. Desarrollado en .NET (Web API) con Entity Framework Core y SQL Server; orientado a prácticas ágiles (Scrum).

Estado
------
- Backend: Proyecto .NET Core / ASP.NET Core Web API (carpeta: `Proyecto Final (Libreria)`).
- Documentación de API: Swagger (habilitado en desarrollo).
- Persistencia: Entity Framework Core con migraciones en `Migrations/`.
- Frontend: (mencionado en documentación previa como React) — no incluido en este repositorio.

Características principales
---------------------------
- Modelos de dominio con validaciones (Data Annotations): Usuarios, Roles, Categorías, Productos, Compra, Venta, Promociones, Descuentos.
- DTOs para separar contrato de API y entidades internas.
- Endpoints REST para gestión de usuarios y roles (ejemplos en `Controllers/UsuariosController.cs` y `Controllers/RolesController.cs`).
- Soporte para Swagger y CORS.
- Migraciones EF Core para creación/actualización del esquema en SQL Server.

Estructura del repositorio
--------------------------
- README.md — (este archivo)
- Proyecto Final (Libreria)/
  - Program.cs — configuración de la app (Swagger, CORS, MapControllers).
  - Controllers/
    - UsuariosController.cs
    - RolesController.cs
    - (otros controladores si aplican)
  - Models/
    - Usuarios.cs, Rol.cs, Categoria.cs, Producto.cs, Compra.cs, Venta.cs, Promocion.cs, Descuento.cs, etc.
  - DTO/
    - DTOUsuarios.cs, DTORol.cs, DTOProducto.cs
  - Migrations/ — migrations de EF Core
  - appsettings.json (configuración de conexión) — (si existe)
- Otros archivos de configuración del proyecto (.csproj, etc.)

Requisitos
----------
- .NET SDK (versión compatible con el proyecto; p. ej. .NET 6/7 — verificar .csproj)
- SQL Server (o SQL Server Express / Docker) para la base de datos
- dotnet-ef (opcional, para administración de migraciones)
- (Opcional) Cliente HTTP o Swagger UI para probar endpoints

Configuración y ejecución local
-------------------------------
1. Clonar el repositorio:
   ```bash
   git clone https://github.com/BackendGhost/Actividad-11.git
   cd Actividad-11/Proyecto\ Final\ \(Libreria\)/
   ```

2. Configurar la cadena de conexión en `appsettings.json` (o variables de entorno):
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=TU_SERVIDOR;Database=LibreriaDb;User Id=usuario;Password=contraseña;"
   }
   ```

3. Restaurar dependencias y compilar:
   ```bash
   dotnet restore
   dotnet build
   ```

4. Aplicar migraciones y actualizar la base de datos:
   - Si tienes dotnet-ef instalado:
     ```bash
     dotnet tool install --global dotnet-ef   # si no está instalado
     dotnet ef database update
     ```
   - Alternativamente, la primera ejecución con el context configurado puede crear la base si está programado.

5. Ejecutar la API:
   ```bash
   dotnet run
   ```
   - En entorno de desarrollo Swagger UI estará disponible en `https://localhost:5xxx/swagger` (puerto según configuración).

Endpoints (ejemplos encontrados)
-------------------------------
- POST /api/Usuarios/Crear — crear usuario. Validaciones: nombre, CI, teléfono, estado.
- POST /api/Roles/Crear — crear rol. Validaciones: descripción, existencia previa.
- (Otros endpoints esperados para Productos, Categorías, Compras, Ventas, Promociones)

Modelos y DTOs (resumen)
------------------------
- Models: Usuarios, Rol, Categoria, Producto, Compra, Venta, Promocion, Descuento, Usuario_Rol, etc.
- DTOs: DTOUsuarios, DTORol, DTOProducto — usados para recibir datos en controllers y proteger las entidades del dominio.

Buenas prácticas y notas
------------------------
- Validaciones usando Data Annotations en las entidades: aprovecharlas para mostrar mensajes claros al cliente.
- Control de estado (Activo / Inactivo) estandarizado en modelos.
- Separar responsabilidades: seguir expandiendo DTOs y servicios (servicio de aplicación) para pruebas unitarias.
- Añadir pruebas unitarias e integración para endpoints y capa de datos.
- Documentar los endpoints con descripciones y ejemplos en Swagger.

Contribuir
----------
1. Abrir un Issue describiendo la propuesta o bug.
2. Crear una rama con prefijo `feature/` o `fix/`.
3. Hacer PR describiendo los cambios y referencias a Issues.
4. Revisiones y merge hacia la rama principal tras aprobación.

Contacto y equipo
-----------------
Equipo original mencionado en la documentación:
- Miguel Portillo
- Jose Mendivil
- Alex Maizares
- Juan Cáceres
- Jhon Serrano
- Bernardo Ortiz

Licencia
--------
- Añade aquí la licencia del proyecto (p. ej. MIT) o los términos que correspondan.

Notas finales
-------------
Este README es una propuesta basada en la inspección del código existente. Revisa y ajusta versión de .NET, detalles de configuración y endpoints según los archivos de proyecto (.csproj, appsettings.json) y los controladores que quieras exponer públicamente.
