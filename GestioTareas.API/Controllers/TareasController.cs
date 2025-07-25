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
    public class TareasController : ControllerBase
    {
        private readonly string _cadenaConexion;

        public TareasController(IConfiguration config)
        {
            _cadenaConexion = config.GetConnectionString("DefaultConnection");
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using var conexion = new SqlConnection(_cadenaConexion);
            var tareas = await conexion.QueryAsync<Tarea>("SELECT * FROM Tareas");
            return Ok(tareas);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            using var conexion = new SqlConnection(_cadenaConexion);
            var tarea = await conexion.QueryFirstOrDefaultAsync<Tarea>("SELECT * FROM Tareas WHERE Id = @Id", new { Id = id });
            return tarea == null ? NotFound() : Ok(tarea);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Tarea tarea)
        {
            using var conexion = new SqlConnection(_cadenaConexion);
            var sql = @"INSERT INTO Tareas (Titulo, Descripcion, Estado, Prioridad, FechaVencimiento, ProyectoId, UsuarioAsignadoId)
                        VALUES (@Titulo, @Descripcion, @Estado, @Prioridad, @FechaVencimiento, @ProyectoId, @UsuarioAsignadoId)";
            await conexion.ExecuteAsync(sql, tarea);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] Tarea tarea)
        {
            tarea.Id = id;
            using var conexion = new SqlConnection(_cadenaConexion);
            var sql = @"UPDATE Tareas SET Titulo = @Titulo, Descripcion = @Descripcion, Estado = @Estado, Prioridad = @Prioridad,
                        FechaVencimiento = @FechaVencimiento, ProyectoId = @ProyectoId, UsuarioAsignadoId = @UsuarioAsignadoId WHERE Id = @Id";
            await conexion.ExecuteAsync(sql, tarea);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            using var conexion = new SqlConnection(_cadenaConexion);
            await conexion.ExecuteAsync("DELETE FROM Tareas WHERE Id = @Id", new { Id = id });
            return Ok();
        }
    }
}