using GestionTareas.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace GestionTareas.MVC.Controllers
{
    public class ReportesController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiUrl;

        public ReportesController(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _apiUrl = config["ApiUrl"].TrimEnd('/');
        }

        // Reporte de filtrar y ordenar
        public async Task<IActionResult> FiltrarOrdenar(string estado, string prioridad, string orden)
        {
            var cliente = _httpClientFactory.CreateClient();
            var url = $"{_apiUrl}/tareas";
            var response = await cliente.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();
            var tareas = JsonConvert.DeserializeObject<List<Tarea>>(json);

            // Filtrado
            if (!string.IsNullOrEmpty(estado))
                tareas = tareas.Where(t => t.Estado == estado).ToList();

            if (!string.IsNullOrEmpty(prioridad))
                tareas = tareas.Where(t => t.Prioridad == prioridad).ToList();

            // Ordenamiento
            if (orden == "fecha")
                tareas = tareas.OrderBy(t => t.FechaVencimiento).ToList();
            else if (orden == "prioridad")
                tareas = tareas.OrderBy(t => t.Prioridad).ToList();

            return View(tareas);
        }

        // Reporte de agrupación
        public async Task<IActionResult> Agrupar(string tipo)
        {
            var cliente = _httpClientFactory.CreateClient();
            var url = $"{_apiUrl}/tareas";
            var response = await cliente.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();
            var tareas = JsonConvert.DeserializeObject<List<Tarea>>(json);

            IEnumerable<IGrouping<int, Tarea>> grupos = null;
            string agrupadoPor = "";

            if (tipo == "proyecto")
            {
                grupos = tareas.GroupBy(t => t.ProyectoId);
                agrupadoPor = "Proyecto";
            }
            else if (tipo == "usuario")
            {
                grupos = tareas.GroupBy(t => t.UsuarioAsignadoId);
                agrupadoPor = "Usuario Asignado";
            }

            ViewBag.AgrupadoPor = agrupadoPor;
            return View(grupos);
        }
    }
}