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

        public async Task<IActionResult> Index()
        {
            var client = _http.CreateClient("api");

            try
            {
                // 1️⃣ Obtener tableros
                var tablerosResponse = await client.GetAsync("api/Tablero");
                if (!tablerosResponse.IsSuccessStatusCode)
                {
                    TempData["Error"] = $"Error al obtener tableros: {tablerosResponse.StatusCode}";
                    return View(new List<TableroViewModel>());
                }

                var tablerosDto = await tablerosResponse.Content.ReadFromJsonAsync<List<TableroDto>>();

                var tablerosVm = tablerosDto!.Select(t => new TableroViewModel
                {
                    Id = t.Id,
                    Titulo = t.Titulo,
                    FechaCreacion = t.FechaCreacion,
                    UsuarioId = t.UsuarioId,
                    NombreUsuario = null,
                    Lista = new List<ListumViewModel>()
                }).ToList();

                // 2️⃣ Obtener listas por tablero
                foreach (var tablero in tablerosVm)
                {
                    var listasResponse = await client.GetAsync($"api/Lista/tablero/{tablero.Id}");
                    if (!listasResponse.IsSuccessStatusCode) continue;

                    var listasDto = await listasResponse.Content.ReadFromJsonAsync<List<ListumDto>>();
                    if (listasDto == null) continue;

                    tablero.Lista = listasDto.Select(l => new ListumViewModel
                    {
                        Id = l.Id,
                        Titulo = l.Titulo,
                        Orden = l.Orden,
                        TableroId = l.TableroId,
                        ListaTarjetas = new List<TarjetumViewModel>()
                    }).OrderBy(l => l.Orden).ToList();

                    // 3️⃣ Obtener tarjetas de cada lista mediante endpoint /lista/{listaId}
                    foreach (var lista in tablero.Lista)
                    {
                        var tarjetasResponse = await client.GetAsync($"api/Tarjeta/lista/{lista.Id}");
                        if (!tarjetasResponse.IsSuccessStatusCode) continue;

                        var tarjetasDto = await tarjetasResponse.Content.ReadFromJsonAsync<List<TarjetumDto>>();
                        if (tarjetasDto == null) continue;

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
    }
}
