using GestionTareas.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

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

        // Listar todas las tareas + filtros y reportes
        public async Task<IActionResult> Index(string estado, string prioridad, DateTime? fechaVencimiento, int? proyectoId, int? usuarioId)
        {
            var cliente = _httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("token");
            cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var url = _apiUrl + "tareas";
            var respuesta = await cliente.GetAsync(url);
            var tareas = respuesta.IsSuccessStatusCode
                ? JsonConvert.DeserializeObject<List<Tarea>>(await respuesta.Content.ReadAsStringAsync())
                : new List<Tarea>();

            // Filtros locales
            if (!string.IsNullOrWhiteSpace(estado))
                tareas = tareas.Where(t => t.Estado == estado).ToList();
            if (!string.IsNullOrWhiteSpace(prioridad))
                tareas = tareas.Where(t => t.Prioridad == prioridad).ToList();
            if (fechaVencimiento.HasValue)
                tareas = tareas.Where(t => t.FechaVencimiento.HasValue && t.FechaVencimiento.Value.Date == fechaVencimiento.Value.Date).ToList();
            if (proyectoId.HasValue)
                tareas = tareas.Where(t => t.ProyectoId == proyectoId).ToList();
            if (usuarioId.HasValue)
                tareas = tareas.Where(t => t.UsuarioAsignadoId == usuarioId).ToList();

            // Para desplegar filtros en el frontend
            ViewBag.Estado = estado;
            ViewBag.Prioridad = prioridad;
            ViewBag.FechaVencimiento = fechaVencimiento?.ToString("yyyy-MM-dd");
            ViewBag.ProyectoId = proyectoId;
            ViewBag.UsuarioId = usuarioId;

            return View(tareas);
        }

        public async Task<IActionResult> Create()
        {
            await CargarProyectosYUsuarios();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Tarea tarea)
        {
            var cliente = _httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("token");
            cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var respuesta = await cliente.PostAsJsonAsync(_apiUrl + "tareas", tarea);
            if (respuesta.IsSuccessStatusCode)
                return RedirectToAction("Index");
            await CargarProyectosYUsuarios();
            return View(tarea);
        }

        public async Task<IActionResult> Edit(int id)
        {
            await CargarProyectosYUsuarios();
            var cliente = _httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("token");
            cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var respuesta = await cliente.GetAsync(_apiUrl + "tareas/" + id);
            if (!respuesta.IsSuccessStatusCode) return RedirectToAction("Index");
            var tarea = JsonConvert.DeserializeObject<Tarea>(await respuesta.Content.ReadAsStringAsync());
            return View(tarea);
        }

        [HttpPost]
        public async Task<IActionResult> Edi(Tarea tarea)
        {
            var cliente = _httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("token");
            cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var respuesta = await cliente.PutAsJsonAsync(_apiUrl + "tareas/" + tarea.Id, tarea);
            if (respuesta.IsSuccessStatusCode) return RedirectToAction("Index");
            await CargarProyectosYUsuarios();
            return View(tarea);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var cliente = _httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("token");
            cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            await cliente.DeleteAsync(_apiUrl + "tareas/" + id);
            return RedirectToAction("Index");
        }

        // Helper para combos de proyectos y usuarios
        private async Task CargarProyectosYUsuarios()
        {
            var cliente = _httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("token");
            cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Proyectos
            var respProy = await cliente.GetAsync(_apiUrl + "proyectos");
            var proyectos = respProy.IsSuccessStatusCode
                ? JsonConvert.DeserializeObject<List<Proyecto>>(await respProy.Content.ReadAsStringAsync())
                : new List<Proyecto>();
            ViewBag.Proyectos = proyectos;

            // Usuarios
            var respUsu = await cliente.GetAsync(_apiUrl + "usuarios");
            var usuarios = respUsu.IsSuccessStatusCode
                ? JsonConvert.DeserializeObject<List<Usuario>>(await respUsu.Content.ReadAsStringAsync())
                : new List<Usuario>();
            ViewBag.Usuarios = usuarios;
        }
    }
}