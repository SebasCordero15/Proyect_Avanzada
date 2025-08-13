using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            if (!ModelState.IsValid) return View(vm);

            var client = _http.CreateClient("api");
            var res = await client.PostAsJsonAsync("api/tablero", vm);

            if (!res.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "No se pudo crear el tablero.");
                return View(vm);
            }
            return RedirectToAction(nameof(Index));
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

            var client = _http.CreateClient("api");
            var res = await client.PutAsJsonAsync($"api/tablero/{id}", vm);

            if (!res.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "No se pudo actualizar el tablero.");
                return View(vm);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Tablero/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var client = _http.CreateClient("api");
            var tablero = await client.GetFromJsonAsync<TableroViewModel>($"api/tablero/{id}");
            if (tablero == null) return NotFound();
            return View(tablero);
        }

        // POST: Tablero/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = _http.CreateClient("api");
            var res = await client.DeleteAsync($"api/tablero/{id}");
            return RedirectToAction(nameof(Index));
        }
    }
}
