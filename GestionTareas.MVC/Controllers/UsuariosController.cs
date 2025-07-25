using GestionTareas.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace GestionTareas.MVC.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiUrl;

        public UsuariosController(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _apiUrl = config["ApiUrl"];
        }

        public async Task<IActionResult> Index()
        {
            var cliente = _httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("token");
            cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var respuesta = await cliente.GetAsync(_apiUrl + "usuarios");
            var usuarios = respuesta.IsSuccessStatusCode
                ? JsonConvert.DeserializeObject<List<Usuario>>(await respuesta.Content.ReadAsStringAsync())
                : new List<Usuario>();
            return View(usuarios);
        }

        public IActionResult Crear() => View();

        [HttpPost]
        public async Task<IActionResult> Crear(Usuario usuario)
        {
            var cliente = _httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("token");
            cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var respuesta = await cliente.PostAsJsonAsync(_apiUrl + "usuarios", usuario);
            if (respuesta.IsSuccessStatusCode) return RedirectToAction("Index");
            return View(usuario);
        }

        public async Task<IActionResult> Editar(int id)
        {
            var cliente = _httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("token");
            cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var respuesta = await cliente.GetAsync(_apiUrl + "usuarios/" + id);
            if (!respuesta.IsSuccessStatusCode) return RedirectToAction("Index");
            var usuario = JsonConvert.DeserializeObject<Usuario>(await respuesta.Content.ReadAsStringAsync());
            return View(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Usuario usuario)
        {
            var cliente = _httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("token");
            cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var respuesta = await cliente.PutAsJsonAsync(_apiUrl + "usuarios/" + usuario.Id, usuario);
            if (respuesta.IsSuccessStatusCode) return RedirectToAction("Index");
            return View(usuario);
        }

        public async Task<IActionResult> Eliminar(int id)
        {
            var cliente = _httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("token");
            cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            await cliente.DeleteAsync(_apiUrl + "usuarios/" + id);
            return RedirectToAction("Index");
        }
    }
}