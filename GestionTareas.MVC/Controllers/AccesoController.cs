using GestionTareas.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace GestionTareas.MVC.Controllers
{
    public class AccesoController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiUrl;

        public AccesoController(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _apiUrl = config["ApiUrl"];
        }

        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(Usuario usuario)
        {
            var cliente = _httpClientFactory.CreateClient();
            var contenido = new StringContent(JsonConvert.SerializeObject(usuario), Encoding.UTF8, "application/json");
            var respuesta = await cliente.PostAsync(_apiUrl + "auth/login", contenido);

            if (!respuesta.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Usuario o contraseña incorrectos");
                return View(usuario);
            }

            var json = await respuesta.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<dynamic>(json);
            HttpContext.Session.SetString("token", (string)obj.token);
            return RedirectToAction("Index", "Proyectos");
        }

        public IActionResult Registro() => View();

        [HttpPost]
        public async Task<IActionResult> Register(Usuario usuario)
        {
            var cliente = _httpClientFactory.CreateClient();
            var contenido = new StringContent(JsonConvert.SerializeObject(usuario), Encoding.UTF8, "application/json");
            var respuesta = await cliente.PostAsync(_apiUrl + "auth/registro", contenido);

            if (!respuesta.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Error al registrar usuario");
                return View(usuario);
            }
            return RedirectToAction("Login");
        }
    }
}