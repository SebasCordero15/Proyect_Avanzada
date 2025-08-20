using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using PAW.Models.Entities;

namespace PAW.Mvc.Controllers
{
    public class TarjetaController : Controller
    {
        private readonly IHttpClientFactory _http;

        public TarjetaController(IHttpClientFactory httpClientFactory)
        {
            _http = httpClientFactory;
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
                var client = _http.CreateClient("api");

                var nuevaTarjeta = new Tarjetum
                {
                    Titulo = titulo.Trim(),
                    Descripcion = string.IsNullOrWhiteSpace(descripcion) ? null : descripcion.Trim(),
                    FechaCreacion = DateTime.UtcNow,
                    FechaVencimiento = fechaVencimiento,
                    ListaId = listaId
                };

                var response = await client.PostAsJsonAsync("api/Tarjeta", nuevaTarjeta);

                if (response.IsSuccessStatusCode)
                    TempData["Success"] = "Tarjeta creada correctamente.";
                else
                {
                    var err = await response.Content.ReadAsStringAsync();
                    TempData["Error"] = $"No se pudo crear la tarjeta. ({(int)response.StatusCode}) {response.ReasonPhrase}. {err}";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
            }

       
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
                var client = _http.CreateClient("api");

           
                var get = await client.GetAsync($"api/Tarjeta/{id}");
                if (!get.IsSuccessStatusCode)
                {
                    TempData["Error"] = $"No se pudo obtener la tarjeta. ({(int)get.StatusCode}) {get.ReasonPhrase}";
                    return RedirectToAction("Index", "Board");
                }

                var tarjeta = await get.Content.ReadFromJsonAsync<Tarjetum>();
                if (tarjeta is null)
                {
                    TempData["Error"] = "La tarjeta no existe.";
                    return RedirectToAction("Index", "Board");
                }

                tarjeta.Titulo = titulo.Trim();
                tarjeta.Descripcion = string.IsNullOrWhiteSpace(descripcion) ? null : descripcion.Trim();
                tarjeta.FechaVencimiento = fechaVencimiento;

                var put = await client.PutAsJsonAsync($"api/Tarjeta/{id}", tarjeta);
                if (put.IsSuccessStatusCode)
                    TempData["Success"] = "Tarjeta actualizada correctamente.";
                else
                {
                    var err = await put.Content.ReadAsStringAsync();
                    TempData["Error"] = $"No se pudo actualizar la tarjeta. ({(int)put.StatusCode}) {put.ReasonPhrase}. {err}";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
            }

       
            return RedirectToAction("Index", "Board");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCard(int id)
        {
            try
            {
                var client = _http.CreateClient("api");
                var res = await client.DeleteAsync($"api/Tarjeta/{id}");

                if (res.IsSuccessStatusCode)
                    TempData["Success"] = "Tarjeta eliminada correctamente.";
                else
                {
                    var err = await res.Content.ReadAsStringAsync();
                    TempData["Error"] = $"No se pudo eliminar la tarjeta. ({(int)res.StatusCode}) {res.ReasonPhrase}. {err}";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
            }

            return RedirectToAction("Index", "Board");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MoveCard(int id, int nuevaListaId)
        {
            try
            {
                var client = _http.CreateClient("api");
                var res = await client.PostAsync($"api/Tarjeta/{id}/mover/{nuevaListaId}", content: null);

                if (res.IsSuccessStatusCode)
                    TempData["Success"] = "Tarjeta movida correctamente.";
                else
                {
                    var err = await res.Content.ReadAsStringAsync();
                    TempData["Error"] = $"No se pudo mover la tarjeta. ({(int)res.StatusCode}) {res.ReasonPhrase}. {err}";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
            }

 
            return RedirectToAction("Index", "Board");
        }
    }
}
