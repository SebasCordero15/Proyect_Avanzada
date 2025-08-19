using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PAW.Models.Entities;
using PAW.Models.ViewModels;
using System.Net.Http.Json;

namespace PAW.Mvc.Controllers
{
    [Authorize]
    public class TableroController : Controller
    {
        private readonly IHttpClientFactory _http;

        public TableroController(IHttpClientFactory http) => _http = http;

        // GET: Tablero
        public async Task<IActionResult> Index()
        {
            var client = _http.CreateClient("api");
            var tableros = await client.GetFromJsonAsync<List<TableroViewModel>>("api/tablero");
            return View(tableros ?? new List<TableroViewModel>());
        }

        // GET: Tablero/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var client = _http.CreateClient("api");
            var tablero = await client.GetFromJsonAsync<TableroViewModel>($"api/tablero/{id}");
            if (tablero == null) return NotFound();
            return View(tablero);
        }

        // GET: Tablero/Create
        public IActionResult Create()
        {
            return View(new TableroViewModel());
        }

        // POST: Tablero/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TableroViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            try
            {
                // Mapear VM → entidad
                var tablero = new Tablero
                {
                    Titulo = vm.Titulo,
                    FechaCreacion = DateTime.Now,
                    UsuarioId = vm.UsuarioId,
                    Lista = vm.Lista?.Select(l => new Listum { Titulo = l.Titulo }).ToList() ?? new List<Listum>()
                };

                var client = _http.CreateClient("api");
                var res = await client.PostAsJsonAsync("api/tablero", tablero);

                if (!res.IsSuccessStatusCode)
                {
                    var error = await res.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"No se pudo crear el tablero: {error}");
                    return View(vm);
                }

                // → Ir a Board/Index
                TempData["Success"] = "Tablero creado correctamente.";
                return RedirectToAction("Index", "Board");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Ocurrió un error: {ex.Message}");
                return View(vm);
            }
        }

        // GET: Tablero/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var client = _http.CreateClient("api");
            var tablero = await client.GetFromJsonAsync<TableroViewModel>($"api/tablero/{id}");
            if (tablero == null) return NotFound();
            return View(tablero);
        }

        // POST: Tablero/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TableroViewModel vm)
        {
            if (id != vm.Id) return BadRequest();
            if (!ModelState.IsValid) return View(vm);

            try
            {
                var updatedTablero = new Tablero
                {
                    Id = vm.Id,
                    Titulo = vm.Titulo
                };

                var client = _http.CreateClient("api");
                var res = await client.PutAsJsonAsync($"api/tablero/{id}", updatedTablero);

                if (!res.IsSuccessStatusCode)
                {
                    var error = await res.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"No se pudo actualizar el tablero: {error}");
                    return View(vm);
                }

                // Puedes dejarlo aquí o también enviar a Board/Index si prefieres
                TempData["Success"] = "Tablero actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Ocurrió un error: {ex.Message}");
                return View(vm);
            }
        }

        // ❌ Eliminado el GET: Tablero/Delete (ya no usamos partials ni modales)

        // ✅ POST directo: Tablero/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var client = _http.CreateClient("api");
            var res = await client.DeleteAsync($"api/Tablero/{id}");

            if (!res.IsSuccessStatusCode)
            {
                TempData["Error"] = "No se pudo eliminar el tablero.";
            }
            else
            {
                TempData["Success"] = "Tablero eliminado.";
            }

            // → Siempre llevar a Board/Index
            return RedirectToAction("Index", "Board");
        }
    }
}
