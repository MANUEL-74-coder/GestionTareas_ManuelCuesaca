using Dapper;
using System.Data.SqlClient;
using GestionTareas.API.Models;

namespace GestionTareas.API.Data
{
    public class ProyectoRepositorio
    {
        private readonly string _cadenaConexion;
        public ProyectoRepositorio(IConfiguration config)
        {
            _cadenaConexion = config.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<Proyecto>> ObtenerTodosAsync()
        {
            using var conexion = new SqlConnection(_cadenaConexion);
            return await conexion.QueryAsync<Proyecto>("SELECT * FROM Proyectos");
        }

        public async Task<Proyecto?> ObtenerPorIdAsync(int id)
        {
            using var conexion = new SqlConnection(_cadenaConexion);
            return await conexion.QueryFirstOrDefaultAsync<Proyecto>(
                "SELECT * FROM Proyectos WHERE Id = @Id", new { Id = id });
        }

        public async Task<int> CrearAsync(Proyecto proyecto)
        {
            using var conexion = new SqlConnection(_cadenaConexion);
            var sql = @"INSERT INTO Proyectos (Nombre, Descripcion)
                        VALUES (@Nombre, @Descripcion)";
            return await conexion.ExecuteAsync(sql, proyecto);
        }

        public async Task<int> ActualizarAsync(Proyecto proyecto)
        {
            using var conexion = new SqlConnection(_cadenaConexion);
            var sql = @"UPDATE Proyectos SET Nombre = @Nombre, Descripcion = @Descripcion WHERE Id = @Id";
            return await conexion.ExecuteAsync(sql, proyecto);
        }

        public async Task<int> EliminarAsync(int id)
        {
            using var conexion = new SqlConnection(_cadenaConexion);
            return await conexion.ExecuteAsync("DELETE FROM Proyectos WHERE Id = @Id", new { Id = id });
        }
    }
}
