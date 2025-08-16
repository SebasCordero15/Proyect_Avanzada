using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using PAW.Models.Entities;

namespace PAW.Mvc.Controllers
{
    public class TarjetaController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public TarjetaController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCard(int listaId, string titulo, string? descripcion, DateTime? fechaVencimiento)
        {
            if (string.IsNullOrWhiteSpace(titulo))
            {
                TempData["Error"] = "El título de la tarjeta es obligatorio.";
                return RedirectToAction("Index", "Board");
            }

            try
            {
                var client = _httpClientFactory.CreateClient("api");

                var nuevaTarjeta = new Tarjetum
                {
                    Titulo = titulo.Trim(),
                    Descripcion = string.IsNullOrWhiteSpace(descripcion) ? null : descripcion.Trim(),
                    FechaCreacion = DateTime.UtcNow,
                    FechaVencimiento = fechaVencimiento,
                    ListaId = listaId
                };

                var response = await client.PostAsJsonAsync("api/Tarjeta", nuevaTarjeta);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["Error"] = $"No se pudo crear la tarjeta. Status: {(int)response.StatusCode} - {response.ReasonPhrase}. Detalle: {errorContent}";
                }
                else
                {
                    TempData["Success"] = "Tarjeta creada correctamente.";
                }
            }
            catch (HttpRequestException httpEx)
            {
                TempData["Error"] = $"Error HTTP: {httpEx.Message}";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
            }

            // Si venía desde /Board/Details/{id}, regresa ahí; si no, al Index
            var referer = Request.Headers["Referer"].ToString();
            if (referer.Contains("/Board/Details/", StringComparison.OrdinalIgnoreCase))
                return Redirect(referer);

            return RedirectToAction("Index", "Board");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCard(int id, string titulo, string? descripcion, DateTime? fechaVencimiento)
        {
            if (string.IsNullOrWhiteSpace(titulo))
            {
                TempData["Error"] = "El título de la tarjeta es obligatorio.";
                return RedirectToAction("Index", "Board");
            }

            try
            {
                var client = _httpClientFactory.CreateClient("api");

                // Obtener tarjeta actual
                var getResponse = await client.GetAsync($"api/Tarjeta/{id}");
                if (!getResponse.IsSuccessStatusCode)
                {
                    TempData["Error"] = $"No se pudo obtener la tarjeta. Status: {(int)getResponse.StatusCode} - {getResponse.ReasonPhrase}";
                    return RedirectToAction("Index", "Board");
                }

                var tarjetaExistente = await getResponse.Content.ReadFromJsonAsync<Tarjetum>();
                if (tarjetaExistente == null)
                {
                    TempData["Error"] = "La tarjeta no existe.";
                    return RedirectToAction("Index", "Board");
                }

                // Actualizar campos editables
                tarjetaExistente.Titulo = titulo.Trim();
                tarjetaExistente.Descripcion = string.IsNullOrWhiteSpace(descripcion) ? null : descripcion.Trim();
                tarjetaExistente.FechaVencimiento = fechaVencimiento;
                // Nota: si quieres permitir mover de lista aquí, ajusta ListaId,
                // pero normalmente se usa un endpoint específico (Move).

                var putResponse = await client.PutAsJsonAsync($"api/Tarjeta/{id}", tarjetaExistente);
                if (!putResponse.IsSuccessStatusCode)
                {
                    var errorContent = await putResponse.Content.ReadAsStringAsync();
                    TempData["Error"] = $"No se pudo actualizar la tarjeta. Status: {(int)putResponse.StatusCode} - {putResponse.ReasonPhrase}. Detalle: {errorContent}";
                }
                else
                {
                    TempData["Success"] = "Tarjeta actualizada correctamente.";
                }
            }
            catch (HttpRequestException httpEx)
            {
                TempData["Error"] = $"Error HTTP: {httpEx.Message}";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
            }

            var referer = Request.Headers["Referer"].ToString();
            if (referer.Contains("/Board/Details/", StringComparison.OrdinalIgnoreCase))
                return Redirect(referer);

            return RedirectToAction("Index", "Board");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCard(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("api");
                var res = await client.DeleteAsync($"api/Tarjeta/{id}");

                if (!res.IsSuccessStatusCode)
                {
                    var errorContent = await res.Content.ReadAsStringAsync();
                    TempData["Error"] = $"No se pudo eliminar la tarjeta. Status: {(int)res.StatusCode} - {res.ReasonPhrase}. Detalle: {errorContent}";
                }
                else
                {
                    TempData["Success"] = "Tarjeta eliminada correctamente.";
                }
            }
            catch (HttpRequestException httpEx)
            {
                TempData["Error"] = $"Error HTTP: {httpEx.Message}";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
            }

            var referer = Request.Headers["Referer"].ToString();
            if (referer.Contains("/Board/Details/", StringComparison.OrdinalIgnoreCase))
                return Redirect(referer);

            return RedirectToAction("Index", "Board");
        }

        // Mover tarjeta entre listas (drag & drop u acción puntual)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MoveCard(int id, int nuevaListaId)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("api");
                var res = await client.PostAsync($"api/Tarjeta/{id}/mover/{nuevaListaId}", content: null);

                if (!res.IsSuccessStatusCode)
                {
                    var errorContent = await res.Content.ReadAsStringAsync();
                    TempData["Error"] = $"No se pudo mover la tarjeta. Status: {(int)res.StatusCode} - {res.ReasonPhrase}. Detalle: {errorContent}";
                }
                else
                {
                    TempData["Success"] = "Tarjeta movida correctamente.";
                }
            }
            catch (HttpRequestException httpEx)
            {
                TempData["Error"] = $"Error HTTP: {httpEx.Message}";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
            }

            var referer = Request.Headers["Referer"].ToString();
            if (referer.Contains("/Board/Details/", StringComparison.OrdinalIgnoreCase))
                return Redirect(referer);

            return RedirectToAction("Index", "Board");
        }
    }
}
