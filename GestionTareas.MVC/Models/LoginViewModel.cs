using System.ComponentModel.DataAnnotations;

namespace GestionTareas.MVC.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El campo usuario es obligatorio.")]
        public string NombreUsuario { get; set; }

        [Required(ErrorMessage = "El campo contraseña es obligatorio.")]
        public string ContrasenaHash { get; set; }
    }
}