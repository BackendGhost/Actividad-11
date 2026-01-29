using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Proyecto_Final__Libreria_.Data;
using Proyecto_Final__Libreria_.DTO;
using Proyecto_Final__Libreria_.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Proyecto_Final__Libreria_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("Cors")]
    public class Usuarios_RolesController : ControllerBase
    {
        public readonly DBconexion dBconexion;
        private IConfiguration _configuration;
        public Usuarios_RolesController(DBconexion dBconexion, IConfiguration configuration)
        {
            this.dBconexion = dBconexion;
            _configuration = configuration;
        }


        //Login

        [HttpGet("Login")]
        public async Task<IActionResult> Login(string usuario, string password)
        {
            // Verificar credenciales incluyendo el estado activo
            var usuarioRol = await dBconexion.Usuario_Roles
                .Include(ur => ur.Usuarios) // Incluir datos del usuario
                .Include(ur => ur.Rol) // Incluir datos del rol
                .FirstOrDefaultAsync(ur =>
                    ur.Usuario == usuario &&
                    ur.Password == password &&
                    ur.Estado == "Activo");

            if (usuarioRol == null)
                return BadRequest(new { message = "Credenciales inválidas o usuario inactivo" });

            // Generar token con más información
            string jwtToken = GenerarToken(usuarioRol);

            // Devolver información relevante
            return Ok(new
            {
                token = jwtToken,
                id = usuarioRol.Id,
                idUsuario = usuarioRol.id_Usuario,
                usuario = usuarioRol.Usuario,
                rol = usuarioRol.Rol?.Descripcion
            });
        }

        private string GenerarToken(Usuario_Rol usuarioRol)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, usuarioRol.Usuario),
                new Claim(ClaimTypes.NameIdentifier, usuarioRol.id_Usuario.ToString()),
                new Claim(ClaimTypes.Role, usuarioRol.Rol?.Descripcion ?? "Usuario") 
            };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Key").Value));
                var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var securityToken = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(60),
                    signingCredentials: credenciales
            );

            return new JwtSecurityTokenHandler().WriteToken(securityToken);
        }




        //Listar Datos
        [HttpGet("Listar")]
        public async Task<ActionResult<Usuario_Rol>> Get()
        {
            var usuarioRol = await dBconexion.Usuario_Roles.ToListAsync();
            return Ok(usuarioRol);
        }
        // Listar Usuarios Roles activas
        [HttpGet("ListarActivos")]
        public async Task<ActionResult<IEnumerable<Usuario_Rol>>> GetActivo()
        {
            var UsuarioRolesActivas = await dBconexion.Usuario_Roles
                .Where(c => c.Estado == "Activo")
                .ToListAsync();

            return Ok(UsuarioRolesActivas);
        }
        //ListarUsuarios
        [HttpGet("ListarUsuarios/{idUsuario}")]
        public async Task<ActionResult<IEnumerable<Usuario_Rol>>> GetUsuario(int idUsuario)
        {
            return await dBconexion.Usuario_Roles
                .Where(t => t.id_Usuario == idUsuario)
                .Include(t => t.Usuarios)
                .ToListAsync();
        }
        //ListarRoles
        [HttpGet("ListarRoles/{idRol}")]
        public async Task<ActionResult<IEnumerable<Usuario_Rol>>> GetRol(int idRol)
        {
            return await dBconexion.Usuario_Roles
                .Where(t => t.id_Rol == idRol)
                .Include(t => t.Rol)
                .ToListAsync();
        }
        //Agregar Datos
        [HttpPost("Crear")]
        public async Task<ActionResult> Post([FromBody] DTOUsuario_Rol usuarioRolDtos)
        {
            if (usuarioRolDtos == null)
            {
                return BadRequest("El objeto esta vacio");
            }
            var idUsuario = await dBconexion.Usuarioss.FindAsync(usuarioRolDtos.id_Usuario);
            if (idUsuario == null)
            {
                return BadRequest("Usuario no existe");
            }

            var idRol = await dBconexion.Roles.FindAsync(usuarioRolDtos.id_Rol);
            if (idRol == null)
            {
                return BadRequest("Rol no existe");
            }
            //Validar si el usuario ya existe
            var usuarioExistente = await dBconexion.Usuario_Roles
            .FirstOrDefaultAsync(u => u.Usuario == usuarioRolDtos.Usuario);

            if (usuarioExistente != null)
            {
                return BadRequest("Ya existe este Usuario");
            }

            try
            {
                var usuarioRol = new Usuario_Rol()
                {
                    id_Usuario = usuarioRolDtos.id_Usuario,
                    id_Rol = usuarioRolDtos.id_Rol,
                    Usuario = usuarioRolDtos.Usuario,
                    Password = usuarioRolDtos.Password,
                    Estado = "Activo"
                };
                dBconexion.Add(usuarioRol);
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
        public async Task<ActionResult> PutUsuarioRol(int id, Usuario_Rol usuarioRol)
        {
            if (id != usuarioRol.Id)
            {
                return BadRequest("El id no existe");
            }
            var usuariosRoles = await dBconexion.Usuario_Roles.FindAsync(id);
            if (usuariosRoles == null)
            {
                return NotFound("No se encontro el usuario Rol");
            }
            //Modificar uno por uno de acuerdo al dato
            if (usuarioRol.id_Usuario.HasValue)
            {
                var idUsuarioValido = await dBconexion.Usuarioss.AnyAsync(t => t.Id == usuarioRol.id_Usuario.Value);
                if (idUsuarioValido)
                {
                    usuariosRoles.id_Usuario = usuarioRol.id_Usuario.Value;
                }
                else
                {
                    return BadRequest("El id del Usuario no existe");
                }
            }

            if (usuarioRol.id_Rol.HasValue)
            {
                var idRolValido = await dBconexion.Roles.AnyAsync(t => t.Id == usuarioRol.id_Rol.Value);
                if (idRolValido)
                {
                    usuariosRoles.id_Rol = usuarioRol.id_Rol.Value;
                }
                else
                {
                    return BadRequest("El id del Rol no existe");
                }
            }

            /*if (usuarioRol.Usuario.HasValue)
            {
                usuariosRoles.Usuario = usuarioRol.Usuario.Value;
            }*/

            usuariosRoles.Usuario = usuarioRol.Usuario;

            /*if (usuarioRol.Password.HasValue)
            {
                usuariosRoles.Password = usuarioRol.Password.Value;
            }*/

            usuariosRoles.Password = usuarioRol.Password;

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
        public async Task<ActionResult> CambiarEstadoUsuarioRol(int id)
        {
            var usuarioRol = await dBconexion.Usuario_Roles.FindAsync(id);
            if (usuarioRol == null)
            {
                return NotFound("No se encontro el Usuario Rol");
            }

            // Cambiar el estado
            usuarioRol.Estado = usuarioRol.Estado == "Activo" ? "Inactivo" : "Activo";

            try
            {
                await dBconexion.SaveChangesAsync();
                return Ok(new
                {
                    Message = $"El Usuario Rol esta {usuarioRol.Estado}",
                    usuario = usuarioRol.Usuario,
                    NuevoEstado = usuarioRol.Estado
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "Error al cambiar el estado");
            }
        }
    }
}
