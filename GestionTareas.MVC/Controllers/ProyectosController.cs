using GestionTareas.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GestionTareas.MVC.Controllers
{
    public class ProyectosController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiUrl;

        public ProyectosController(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _apiUrl = config["ApiUrl"];
        }

        public async Task<IActionResult> Index()
        {
            var cliente = _httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("token");
            cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var respuesta = await cliente.GetAsync(_apiUrl + "proyectos");
            if (!respuesta.IsSuccessStatusCode) return View(new List<Proyecto>());
            var json = await respuesta.Content.ReadAsStringAsync();
            var lista = JsonConvert.DeserializeObject<List<Proyecto>>(json);
            return View(lista);
        }

        public IActionResult Crear() => View();

        [HttpPost]
        public async Task<IActionResult> Create(Proyecto proyecto)
        {
            var cliente = _httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("token");
            cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var respuesta = await cliente.PostAsJsonAsync(_apiUrl + "proyectos", proyecto);
            if (respuesta.IsSuccessStatusCode) return RedirectToAction("Index");
            return View(proyecto);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var cliente = _httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("token");
            cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var respuesta = await cliente.GetAsync(_apiUrl + "proyectos/" + id);
            if (!respuesta.IsSuccessStatusCode) return RedirectToAction("Index");
            var proyecto = JsonConvert.DeserializeObject<Proyecto>(await respuesta.Content.ReadAsStringAsync());
            return View(proyecto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Proyecto proyecto)
        {
            var cliente = _httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("token");
            cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var respuesta = await cliente.PutAsJsonAsync(_apiUrl + "proyectos/" + proyecto.Id, proyecto);
            if (respuesta.IsSuccessStatusCode) return RedirectToAction("Index");
            return View(proyecto);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var cliente = _httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("token");
            cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            await cliente.DeleteAsync(_apiUrl + "proyectos/" + id);
            return RedirectToAction("Index");
        }
    }
}