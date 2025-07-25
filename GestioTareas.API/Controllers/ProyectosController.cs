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
    public class ProyectosController : ControllerBase
    {
        private readonly string _cadenaConexion;

        public ProyectosController(IConfiguration config)
        {
            _cadenaConexion = config.GetConnectionString("DefaultConnection");
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using var conexion = new SqlConnection(_cadenaConexion);
            var proyectos = await conexion.QueryAsync<Proyecto>("SELECT * FROM Proyectos");
            return Ok(proyectos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            using var conexion = new SqlConnection(_cadenaConexion);
            var proyecto = await conexion.QueryFirstOrDefaultAsync<Proyecto>("SELECT * FROM Proyectos WHERE Id = @Id", new { Id = id });
            return proyecto == null ? NotFound() : Ok(proyecto);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Proyecto proyecto)
        {
            using var conexion = new SqlConnection(_cadenaConexion);
            var sql = "INSERT INTO Proyectos (Nombre, Descripcion) VALUES (@Nombre, @Descripcion)";
            await conexion.ExecuteAsync(sql, proyecto);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Proyecto proyecto)
        {
            proyecto.Id = id;
            using var conexion = new SqlConnection(_cadenaConexion);
            var sql = "UPDATE Proyectos SET Nombre = @Nombre, Descripcion = @Descripcion WHERE Id = @Id";
            await conexion.ExecuteAsync(sql, proyecto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            using var conexion = new SqlConnection(_cadenaConexion);
            await conexion.ExecuteAsync("DELETE FROM Proyectos WHERE Id = @Id", new { Id = id });
            return Ok();
        }
    }
}