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

        // =======================
        // DTOs (privados al controlador)
        // =======================
        private class TableroDto
        {
            public int Id { get; set; }
            public string Titulo { get; set; } = null!;
            public DateTime FechaCreacion { get; set; }
            public int UsuarioId { get; set; }
        }

        private class ListumDto
        {
            public int Id { get; set; }
            public string Titulo { get; set; } = null!;
            public int Orden { get; set; }
            public int TableroId { get; set; }
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

        private class ComentarioDto
        {
            public int Id { get; set; }
            public string Contenido { get; set; } = null!;
            public DateTime FechaCreacion { get; set; }
            public int TarjetaId { get; set; }
            public int UsuarioId { get; set; }
        }

        // Utilidad para detectar si la petición viene desde fetch/AJAX
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
                // 1) Obtener tableros
                var tablerosResponse = await client.GetAsync("api/Tablero");
                if (!tablerosResponse.IsSuccessStatusCode)
                {
                    TempData["Error"] = $"Error al obtener tableros: {tablerosResponse.StatusCode}";
                    return View(new List<TableroViewModel>());
                }

                var tablerosDto = await tablerosResponse.Content.ReadFromJsonAsync<List<TableroDto>>() ?? new();

                var tablerosVm = tablerosDto.Select(t => new TableroViewModel
                {
                    Id = t.Id,
                    Titulo = t.Titulo,
                    FechaCreacion = t.FechaCreacion,
                    UsuarioId = t.UsuarioId,
                    NombreUsuario = null,
                    Lista = new List<ListumViewModel>()
                }).ToList();

                // 2) Obtener listas por tablero
                foreach (var tablero in tablerosVm)
                {
                    var listasResponse = await client.GetAsync($"api/Lista/tablero/{tablero.Id}");
                    if (!listasResponse.IsSuccessStatusCode) continue;

                    var listasDto = await listasResponse.Content.ReadFromJsonAsync<List<ListumDto>>() ?? new();

                    tablero.Lista = listasDto.Select(l => new ListumViewModel
                    {
                        Id = l.Id,
                        Titulo = l.Titulo,
                        Orden = l.Orden,
                        TableroId = l.TableroId,
                        ListaTarjetas = new List<TarjetumViewModel>()
                    }).OrderBy(l => l.Orden).ToList();

                    // 3) Obtener tarjetas de cada lista
                    foreach (var lista in tablero.Lista)
                    {
                        var tarjetasResponse = await client.GetAsync($"api/Tarjeta/lista/{lista.Id}");
                        if (!tarjetasResponse.IsSuccessStatusCode) continue;

                        var tarjetasDto = await tarjetasResponse.Content.ReadFromJsonAsync<List<TarjetumDto>>() ?? new();

                        lista.ListaTarjetas = tarjetasDto.Select(t => new TarjetumViewModel
                        {
                            Id = t.Id,
                            Titulo = t.Titulo,
                            Descripcion = t.Descripcion,
                            FechaCreacion = t.FechaCreacion,
                            FechaVencimiento = t.FechaVencimiento,
                            ListaId = t.ListaId,
                            UsuarioAsignadoId = t.UsuarioAsignadoId
                        }).ToList();
                    }
                }

                return View(tablerosVm);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"No se pudo contactar el API: {ex.Message}";
                return View(new List<TableroViewModel>());
            }
        }

        // =======================
        // TABLEROS - Details (Abrir un tablero)
        // =======================
        // GET: Board/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var client = _http.CreateClient("api");

            // Tablero
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
                Lista = new List<ListumViewModel>()
            };

            // Listas
            var lRes = await client.GetAsync($"api/Lista/tablero/{vm.Id}");
            if (lRes.IsSuccessStatusCode)
            {
                var lDtos = await lRes.Content.ReadFromJsonAsync<List<ListumDto>>() ?? new();
                vm.Lista = lDtos.OrderBy(l => l.Orden).Select(l => new ListumViewModel
                {
                    Id = l.Id,
                    Titulo = l.Titulo,
                    Orden = l.Orden,
                    TableroId = l.TableroId,
                    ListaTarjetas = new List<TarjetumViewModel>()
                }).ToList();
            }

            // Tarjetas por lista
            foreach (var lista in vm.Lista)
            {
                var cRes = await client.GetAsync($"api/Tarjeta/lista/{lista.Id}");
                if (!cRes.IsSuccessStatusCode) continue;

                var cDtos = await cRes.Content.ReadFromJsonAsync<List<TarjetumDto>>() ?? new();
                lista.ListaTarjetas = cDtos.Select(c => new TarjetumViewModel
                {
                    Id = c.Id,
                    Titulo = c.Titulo,
                    Descripcion = c.Descripcion,
                    FechaCreacion = c.FechaCreacion,
                    FechaVencimiento = c.FechaVencimiento,
                    ListaId = c.ListaId,
                    UsuarioAsignadoId = c.UsuarioAsignadoId
                }).ToList();
            }

            return View(vm);
        }

        // =======================
        // LISTAS
        // =======================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateList(int tableroId, string titulo)
        {
            if (string.IsNullOrWhiteSpace(titulo))
            {
                TempData["Error"] = "El título de la lista es obligatorio.";
                return RedirectToAction(nameof(Index));
            }

            var client = _http.CreateClient("api");

            // Calcular orden = último + 1
            var listasResp = await client.GetAsync($"api/Lista/tablero/{tableroId}");
            int nuevoOrden = 1;
            if (listasResp.IsSuccessStatusCode)
            {
                var existentes = await listasResp.Content.ReadFromJsonAsync<List<ListumDto>>() ?? new();
                nuevoOrden = (existentes.Any() ? existentes.Max(l => l.Orden) : 0) + 1;
            }

            var createPayload = new ListumDto
            {
                Titulo = titulo.Trim(),
                Orden = nuevoOrden,
                TableroId = tableroId
            };

            var res = await client.PostAsJsonAsync("api/Lista", createPayload);
            if (!res.IsSuccessStatusCode)
                TempData["Error"] = "No se pudo crear la lista.";

            // Volver a la página previa si venía de Details
            if (Request.Headers["Referer"].ToString().Contains("/Board/Details/", StringComparison.OrdinalIgnoreCase))
                return Redirect(Request.Headers["Referer"].ToString());

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteList(int id)
        {
            var client = _http.CreateClient("api");
            var res = await client.DeleteAsync($"api/Lista/{id}");
            if (!res.IsSuccessStatusCode)
                TempData["Error"] = "No se pudo eliminar la lista.";

            if (Request.Headers["Referer"].ToString().Contains("/Board/Details/", StringComparison.OrdinalIgnoreCase))
                return Redirect(Request.Headers["Referer"].ToString());

            return RedirectToAction(nameof(Index));
        }

        // =======================
        // TARJETAS
        // =======================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCard(int listaId, string titulo, string? descripcion, DateTime? fechaVencimiento)
        {
            if (string.IsNullOrWhiteSpace(titulo))
            {
                TempData["Error"] = "El título de la tarjeta es obligatorio.";
                return RedirectToAction(nameof(Index));
            }

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
            if (!res.IsSuccessStatusCode)
                TempData["Error"] = "No se pudo crear la tarjeta.";

            if (Request.Headers["Referer"].ToString().Contains("/Board/Details/", StringComparison.OrdinalIgnoreCase))
                return Redirect(Request.Headers["Referer"].ToString());

            return RedirectToAction(nameof(Index));
        }

        // Editar tarjeta (para modal/JS)
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

            return Ok(); // JS actualiza la UI sin recargar
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

            // Si viene de fetch (AJAX) devolvemos 200/xxx, si viene de form normal redirigimos
            if (IsAjaxRequest()) return Ok();

            if (Request.Headers["Referer"].ToString().Contains("/Board/Details/", StringComparison.OrdinalIgnoreCase))
                return Redirect(Request.Headers["Referer"].ToString());

            return RedirectToAction(nameof(Index));
        }

        // =======================
        // COMENTARIOS (uso vía fetch)
        // =======================
        [HttpGet]
        public async Task<IActionResult> GetComments(int tarjetaId)
        {
            var client = _http.CreateClient("api");
            var res = await client.GetAsync($"api/Comentario/tarjeta/{tarjetaId}");
            if (!res.IsSuccessStatusCode) return Json(Array.Empty<ComentarioDto>());

            var data = await res.Content.ReadFromJsonAsync<List<ComentarioDto>>() ?? new();
            return Json(data.OrderByDescending(c => c.FechaCreacion));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateComment(int tarjetaId, string contenido)
        {
            if (string.IsNullOrWhiteSpace(contenido))
                return BadRequest("El contenido es obligatorio.");

            var client = _http.CreateClient("api");
            var payload = new ComentarioDto { TarjetaId = tarjetaId, Contenido = contenido.Trim() };

            var res = await client.PostAsJsonAsync("api/Comentario", payload);
            if (!res.IsSuccessStatusCode)
                return StatusCode((int)res.StatusCode);

            return Ok(); // fetch refresca la lista de comentarios
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var client = _http.CreateClient("api");
            var res = await client.DeleteAsync($"api/Comentario/{id}");
            if (!res.IsSuccessStatusCode)
                return StatusCode((int)res.StatusCode);

            return Ok(); // fetch vuelve a cargar comentarios
        }
    }
}
