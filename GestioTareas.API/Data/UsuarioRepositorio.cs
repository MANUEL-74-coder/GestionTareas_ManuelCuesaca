using Dapper;
using System.Data.SqlClient;
using GestionTareas.API.Models;

namespace GestionTareas.API.Data
{
    public class UsuarioRepositorio
    {
        private readonly string _cadenaConexion;
        public UsuarioRepositorio(IConfiguration config)
        {
            _cadenaConexion = config.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<Usuario>> ObtenerTodosAsync()
        {
            using var conexion = new SqlConnection(_cadenaConexion);
            return await conexion.QueryAsync<Usuario>("SELECT * FROM Usuarios");
        }

        public async Task<Usuario?> ObtenerPorIdAsync(int id)
        {
            using var conexion = new SqlConnection(_cadenaConexion);
            return await conexion.QueryFirstOrDefaultAsync<Usuario>(
                "SELECT * FROM Usuarios WHERE Id = @Id", new { Id = id });
        }

        public async Task<int> CrearAsync(Usuario usuario)
        {
            using var conexion = new SqlConnection(_cadenaConexion);
            var sql = @"INSERT INTO Usuarios (NombreUsuario, Correo, ContrasenaHash)
                        VALUES (@NombreUsuario, @Correo, @ContrasenaHash)";
            return await conexion.ExecuteAsync(sql, usuario);
        }

        public async Task<int> ActualizarAsync(Usuario usuario)
        {
            using var conexion = new SqlConnection(_cadenaConexion);
            var sql = @"UPDATE Usuarios SET NombreUsuario = @NombreUsuario, Correo = @Correo, ContrasenaHash = @ContrasenaHash WHERE Id = @Id";
            return await conexion.ExecuteAsync(sql, usuario);
        }

        public async Task<int> EliminarAsync(int id)
        {
            using var conexion = new SqlConnection(_cadenaConexion);
            return await conexion.ExecuteAsync("DELETE FROM Usuarios WHERE Id = @Id", new { Id = id });
        }
    }
}