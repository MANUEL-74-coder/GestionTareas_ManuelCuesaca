using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Dapper;
using System.Data.SqlClient;
using GestionTareas.API.Models;

namespace GestionTareas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly string _cadenaConexion;

        public AuthController(IConfiguration config)
        {
            _config = config;
            _cadenaConexion = config.GetConnectionString("DefaultConnection");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Usuario usuario)
        {
            using var conexion = new SqlConnection(_cadenaConexion);
            var user = await conexion.QueryFirstOrDefaultAsync<Usuario>(
                // Permite login por usuario o correo
                "SELECT * FROM Usuarios WHERE (NombreUsuario = @NombreUsuario OR Correo = @NombreUsuario) AND ContrasenaHash = @ContrasenaHash",
                new { usuario.NombreUsuario, usuario.ContrasenaHash });

            if (user == null)
                return Unauthorized();

            var claims = new[]
            {
        new Claim(ClaimTypes.Name, user.NombreUsuario),
        new Claim("UsuarioId", user.Id.ToString())
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds);

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Usuario usuario)
        {
            using var conexion = new SqlConnection(_cadenaConexion);
            var existe = await conexion.QueryFirstOrDefaultAsync<Usuario>(
                "SELECT * FROM Usuarios WHERE NombreUsuario = @NombreUsuario", usuario);

            if (existe != null)
                return BadRequest("El usuario ya existe");

            await conexion.ExecuteAsync(
                "INSERT INTO Usuarios (NombreUsuario, Correo, ContrasenaHash) VALUES (@NombreUsuario, @Correo, @ContrasenaHash)",
                usuario);

            return Ok();
        }
    }
}