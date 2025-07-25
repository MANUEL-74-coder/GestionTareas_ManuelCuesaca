using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using System.Data.SqlClient;
using GestionTareas.API.Models;

namespace GestionTareas.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly string _cadenaConexion;

        public UsuariosController(IConfiguration config)
        {
            _cadenaConexion = config.GetConnectionString("DefaultConnection");
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using var conexion = new SqlConnection(_cadenaConexion);
            var usuarios = await conexion.QueryAsync<Usuario>("SELECT * FROM Usuarios");
            return Ok(usuarios);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            using var conexion = new SqlConnection(_cadenaConexion);
            var usuario = await conexion.QueryFirstOrDefaultAsync<Usuario>("SELECT * FROM Usuarios WHERE Id = @Id", new { Id = id });
            return usuario == null ? NotFound() : Ok(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Usuario usuario)
        {
            using var conexion = new SqlConnection(_cadenaConexion);
            var sql = "INSERT INTO Usuarios (NombreUsuario, Correo, ContrasenaHash) VALUES (@NombreUsuario, @Correo, @ContrasenaHash)";
            await conexion.ExecuteAsync(sql, usuario);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Usuario usuario)
        {
            usuario.Id = id;
            using var conexion = new SqlConnection(_cadenaConexion);
            var sql = "UPDATE Usuarios SET NombreUsuario = @NombreUsuario, Correo = @Correo, ContrasenaHash = @ContrasenaHash WHERE Id = @Id";
            await conexion.ExecuteAsync(sql, usuario);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            using var conexion = new SqlConnection(_cadenaConexion);
            await conexion.ExecuteAsync("DELETE FROM Usuarios WHERE Id = @Id", new { Id = id });
            return Ok();
        }
    }
}