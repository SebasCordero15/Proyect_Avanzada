using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PAW.Models.ViewModels;
using System.Net.Http.Json;

namespace PAW.Mvc.Controllers
{
    [Authorize]
    public class BoardController : Controller
    {
        private readonly IHttpClientFactory _http;

        public BoardController(IHttpClientFactory http) => _http = http;


        private class TableroDto
        {
            public int Id { get; set; }
            public string Titulo { get; set; } = null!;
            public DateTime FechaCreacion { get; set; }
            public int UsuarioId { get; set; }
            public List<ListumDto> Lista { get; set; } = new();
        }

        private class ListumDto
        {
            public int Id { get; set; }
            public string Titulo { get; set; } = null!;
            public int Orden { get; set; }
            public int TableroId { get; set; }
            public List<TarjetumDto> Tarjeta { get; set; } = new();
        }

        private class TarjetumDto
        {
            public int Id { get; set; }
            public string Titulo { get; set; } = null!;
            public string? Descripcion { get; set; }
            public DateTime? FechaCreacion { get; set; }
            public DateTime? FechaVencimiento { get; set; }
            public int ListaId { get; set; }
            public int? UsuarioAsignadoId { get; set; }
        }


        private bool IsAjaxRequest() =>
            Request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
            Request.Headers["Accept"].ToString().Contains("application/json", StringComparison.OrdinalIgnoreCase);

        // =======================
        // TABLEROS - Index
        // =======================
        public async Task<IActionResult> Index()
        {
            var client = _http.CreateClient("api");
            try
            {
                // Obtenemos tableros con listas y tarjetas en una sola llamada
                var tablerosDto = await client.GetFromJsonAsync<List<TableroDto>>("api/Tablero") ?? new List<TableroDto>();

                var tablerosVm = tablerosDto.Select(t => new TableroViewModel
                {
                    Id = t.Id,
                    Titulo = t.Titulo,
                    FechaCreacion = t.FechaCreacion,
                    UsuarioId = t.UsuarioId,
                    Lista = t.Lista.OrderBy(l => l.Orden).Select(l => new ListumViewModel
                    {
                        Id = l.Id,
                        Titulo = l.Titulo,
                        Orden = l.Orden,
                        TableroId = l.TableroId,
                        ListaTarjetas = l.Tarjeta.Select(c => new TarjetumViewModel
                        {
                            Id = c.Id,
                            Titulo = c.Titulo,
                            Descripcion = c.Descripcion,
                            FechaCreacion = c.FechaCreacion,
                            FechaVencimiento = c.FechaVencimiento,
                            ListaId = c.ListaId,
                            UsuarioAsignadoId = c.UsuarioAsignadoId
                        }).ToList()
                    }).ToList()
                }).ToList();

                return View(tablerosVm);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"No se pudo contactar el API: {ex.Message}";
                return View(new List<TableroViewModel>());
            }
        }

        // =======================
        // TABLEROS - Details
        // =======================
        public async Task<IActionResult> Details(int id)
        {
            var client = _http.CreateClient("api");

            var tRes = await client.GetAsync($"api/Tablero/{id}");
            if (!tRes.IsSuccessStatusCode)
            {
                TempData["Error"] = "No se pudo cargar el tablero.";
                return RedirectToAction(nameof(Index));
            }

            var tDto = await tRes.Content.ReadFromJsonAsync<TableroDto>();
            if (tDto == null) return NotFound();

            var vm = new TableroViewModel
            {
                Id = tDto.Id,
                Titulo = tDto.Titulo,
                FechaCreacion = tDto.FechaCreacion,
                UsuarioId = tDto.UsuarioId,
                Lista = tDto.Lista.OrderBy(l => l.Orden).Select(l => new ListumViewModel
                {
                    Id = l.Id,
                    Titulo = l.Titulo,
                    Orden = l.Orden,
                    TableroId = l.TableroId,
                    ListaTarjetas = l.Tarjeta.Select(c => new TarjetumViewModel
                    {
                        Id = c.Id,
                        Titulo = c.Titulo,
                        Descripcion = c.Descripcion,
                        FechaCreacion = c.FechaCreacion,
                        FechaVencimiento = c.FechaVencimiento,
                        ListaId = c.ListaId,
                        UsuarioAsignadoId = c.UsuarioAsignadoId
                    }).ToList()
                }).ToList()
            };

            return View(vm);
        }

        // =======================
        // TARJETAS
        // =======================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCard(int listaId, int tableroId, string titulo, string? descripcion, DateTime? fechaVencimiento)
        {
            if (string.IsNullOrWhiteSpace(titulo))
            {
                TempData["Error"] = "El título de la tarjeta es obligatorio.";
                return RedirectToAction("Index", "Board");
            }

            try
            {
                var client = _http.CreateClient("api");
                var payload = new TarjetumDto
                {
                    Titulo = titulo.Trim(),
                    Descripcion = string.IsNullOrWhiteSpace(descripcion) ? null : descripcion.Trim(),
                    FechaVencimiento = fechaVencimiento,
                    ListaId = listaId,
                    FechaCreacion = DateTime.UtcNow
                };

                var res = await client.PostAsJsonAsync("api/Tarjeta", payload);

                if (res.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Tarjeta creada correctamente.";
                }
                else
                {
                    var err = await res.Content.ReadAsStringAsync();
                    TempData["Error"] = $"No se pudo crear la tarjeta. ({(int)res.StatusCode}) {res.ReasonPhrase}. {err}";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
            }

            // 👉 Siempre redirige a Board/Index
            return RedirectToAction("Index", "Board");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCard(int id, string titulo, string? descripcion, DateTime? fechaVencimiento)
        {
            if (string.IsNullOrWhiteSpace(titulo))
                return BadRequest("El título es obligatorio.");

            var client = _http.CreateClient("api");
            var payload = new TarjetumDto
            {
                Id = id,
                Titulo = titulo.Trim(),
                Descripcion = string.IsNullOrWhiteSpace(descripcion) ? null : descripcion.Trim(),
                FechaVencimiento = fechaVencimiento
            };

            var res = await client.PutAsJsonAsync($"api/Tarjeta/{id}", payload);
            if (!res.IsSuccessStatusCode) return StatusCode((int)res.StatusCode);

            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCard(int id)
        {
            var client = _http.CreateClient("api");
            var res = await client.DeleteAsync($"api/Tarjeta/{id}");
            if (!res.IsSuccessStatusCode)
            {
                if (IsAjaxRequest()) return StatusCode((int)res.StatusCode);
                TempData["Error"] = "No se pudo eliminar la tarjeta.";
            }

            return IsAjaxRequest() ? Ok() : RedirectToAction(nameof(Index));
        }
    }
}
