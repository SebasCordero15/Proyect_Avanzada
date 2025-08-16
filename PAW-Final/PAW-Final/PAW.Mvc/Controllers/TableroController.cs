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
                // Mapear TableroViewModel → Tablero
                var tablero = new Tablero
                {
                    Titulo = vm.Titulo,
                    FechaCreacion = DateTime.Now, // asignar fecha actual
                    UsuarioId = vm.UsuarioId,
                    Lista = vm.Lista?.Select(l => new Listum
                    {
                        Titulo = l.Titulo
                        // asigna otras propiedades de Listum si existen
                    }).ToList() ?? new List<Listum>()
                };

                var client = _http.CreateClient("api");
                var res = await client.PostAsJsonAsync("api/tablero", tablero);

                if (!res.IsSuccessStatusCode)
                {
                    // Leer el mensaje de error real de la API
                    var error = await res.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"No se pudo crear el tablero: {error}");
                    return View(vm);
                }

                // Redirigir al índice de tableros si todo fue bien
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Capturar cualquier excepción inesperada
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
                // Solo enviamos el nombre actualizado
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

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Ocurrió un error: {ex.Message}");
                return View(vm);
            }
        }
        // GET: Tablero/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var client = _http.CreateClient("api");
            var tablero = await client.GetFromJsonAsync<TableroViewModel>($"api/tablero/{id}");
            if (tablero == null) return NotFound();

            // Retornamos partial view para el modal
            return PartialView("_DeleteTableroPartial", tablero);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBoard(int id)
        {
            var client = _http.CreateClient("api");

            // Llamada al API para eliminar el tablero
            var res = await client.DeleteAsync($"api/Tablero/{id}");

            if (!res.IsSuccessStatusCode)
            {
                // Si la petición es AJAX devolvemos el código de error
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return StatusCode((int)res.StatusCode);

                TempData["Error"] = "No se pudo eliminar el tablero.";
                return RedirectToAction(nameof(Index));
            }

            // Respuesta para AJAX
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Ok();

            // Redirigir si se usó form normal
            return RedirectToAction(nameof(Index));
        }


    }
}
