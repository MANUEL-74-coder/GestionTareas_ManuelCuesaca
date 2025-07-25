using Dapper;
using System.Data.SqlClient;
using GestionTareas.API.Models;

namespace GestionTareas.API.Data
{
    public class TareaRepositorio
    {
        private readonly string _cadenaConexion;
        public TareaRepositorio(IConfiguration config)
        {
            _cadenaConexion = config.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<Tarea>> ObtenerTodosAsync()
        {
            using var conexion = new SqlConnection(_cadenaConexion);
            return await conexion.QueryAsync<Tarea>("SELECT * FROM Tareas");
        }

        public async Task<Tarea?> ObtenerPorIdAsync(int id)
        {
            using var conexion = new SqlConnection(_cadenaConexion);
            return await conexion.QueryFirstOrDefaultAsync<Tarea>(
                "SELECT * FROM Tareas WHERE Id = @Id", new { Id = id });
        }

        public async Task<int> CrearAsync(Tarea tarea)
        {
            using var conexion = new SqlConnection(_cadenaConexion);
            var sql = @"INSERT INTO Tareas (Titulo, Descripcion, Estado, Prioridad, FechaVencimiento, ProyectoId, UsuarioAsignadoId)
                        VALUES (@Titulo, @Descripcion, @Estado, @Prioridad, @FechaVencimiento, @ProyectoId, @UsuarioAsignadoId)";
            return await conexion.ExecuteAsync(sql, tarea);
        }

        public async Task<int> ActualizarAsync(Tarea tarea)
        {
            using var conexion = new SqlConnection(_cadenaConexion);
            var sql = @"UPDATE Tareas SET Titulo = @Titulo, Descripcion = @Descripcion, Estado = @Estado, Prioridad = @Prioridad,
                        FechaVencimiento = @FechaVencimiento, ProyectoId = @ProyectoId, UsuarioAsignadoId = @UsuarioAsignadoId WHERE Id = @Id";
            return await conexion.ExecuteAsync(sql, tarea);
        }

        public async Task<int> EliminarAsync(int id)
        {
            using var conexion = new SqlConnection(_cadenaConexion);
            return await conexion.ExecuteAsync("DELETE FROM Tareas WHERE Id = @Id", new { Id = id });
        }
    }
}
