using GestionTareas.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace GestionTareas.MVC.Controllers
{
    public class TareasController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiUrl;

        public TareasController(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _apiUrl = config["ApiUrl"];
        }

        public async Task<IActionResult> Index(string estado, string prioridad, string buscar, int? proyectoId, int? usuarioAsignadoId, string ordenarPor)
        {
            var cliente = _httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("token");
            if (string.IsNullOrEmpty(token))
            {
                // Manejar caso de token no disponible (redirección a login, por ejemplo)
                return RedirectToAction("Login", "Account");
            }
            cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var query = new List<string>();
            if (!string.IsNullOrEmpty(estado)) query.Add($"estado={Uri.EscapeDataString(estado)}");
            if (!string.IsNullOrEmpty(prioridad)) query.Add($"erioridad={Uri.EscapeDataString(prioridad)}");
            if (!string.IsNullOrEmpty(buscar)) query.Add($"buscar={Uri.EscapeDataString(buscar)}");
            if (proyectoId != null) query.Add($"eroyectoId={proyectoId}");
            if (usuarioAsignadoId != null) query.Add($"usuarioAsignadoId={usuarioAsignadoId}");
            if (!string.IsNullOrEmpty(ordenarPor)) query.Add($"ordenarPor={Uri.EscapeDataString(ordenarPor)}");
            var url = _apiUrl + "tareas";
            if (query.Count > 0) url += "?" + string.Join("&", query);

            var respuesta = await cliente.GetAsync(url);
            var lista = new List<Tarea>();
            if (respuesta.IsSuccessStatusCode)
            {
                var json = await respuesta.Content.ReadAsStringAsync();
                lista = JsonConvert.DeserializeObject<List<Tarea>>(json) ?? new List<Tarea>();
            }
            ViewBag.Estado = estado;
            ViewBag.Prioridad = prioridad;
            ViewBag.Buscar = buscar;
            ViewBag.ProyectoId = proyectoId;
            ViewBag.UsuarioAsignadoId = usuarioAsignadoId;
            ViewBag.OrdenarPor = ordenarPor;
            return View(lista);
        }

        public IActionResult Crear() => View();

        [HttpPost]
        public async Task<IActionResult> Create(Tarea tarea)
        {
            var cliente = _httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("token");
            cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var respuesta = await cliente.PostAsJsonAsync(_apiUrl + "tareas", tarea);
            if (respuesta.IsSuccessStatusCode) return RedirectToAction("Index");
            return View(tarea);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var cliente = _httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("token");
            cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var respuesta = await cliente.GetAsync(_apiUrl + "tareas/" + id);
            if (!respuesta.IsSuccessStatusCode) return RedirectToAction("Index");
            var tarea = JsonConvert.DeserializeObject<Tarea>(await respuesta.Content.ReadAsStringAsync());
            return View(tarea);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Tarea tarea)
        {
            var cliente = _httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("token");
            cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var respuesta = await cliente.PutAsJsonAsync(_apiUrl + "tareas/" + tarea.Id, tarea);
            if (respuesta.IsSuccessStatusCode) return RedirectToAction("Index");
            return View(tarea);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var cliente = _httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("token");
            cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            await cliente.DeleteAsync(_apiUrl + "Tareas/" + id);
            return RedirectToAction("Index");
        }
    }
}