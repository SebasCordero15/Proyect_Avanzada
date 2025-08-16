using Microsoft.AspNetCore.Mvc;
using PAW.Models.ViewModels;
using PAW.Models.Entities;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace PAW.Mvc.Controllers
{
    public class ListaController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ListaController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> CreateList(int tableroId, string titulo)
        {
            if (string.IsNullOrWhiteSpace(titulo))
            {
                TempData["Error"] = "El título de la lista es obligatorio.";
                return RedirectToAction("Index", "Board");
            }

            try
            {
                var client = _httpClientFactory.CreateClient("api");

                // 1️⃣ Obtener listas actuales para calcular el orden
                var listasResponse = await client.GetAsync($"api/Lista/tablero/{tableroId}");
                int nuevoOrden = 1;

                if (listasResponse.IsSuccessStatusCode)
                {
                    var listas = await listasResponse.Content.ReadFromJsonAsync<List<ListumViewModel>>() ?? new();
                    if (listas.Any())
                        nuevoOrden = listas.Max(l => l.Orden) + 1;
                }

                // 2️⃣ Mapear a la entidad que espera la API
                var nuevaListaEntidad = new Listum
                {
                    Titulo = titulo.Trim(),
                    Orden = nuevoOrden,
                    TableroId = tableroId,
                    
                    Tablero = null
                };

                // 3️⃣ Enviar al API
                var response = await client.PostAsJsonAsync("api/Lista", nuevaListaEntidad);

                if (!response.IsSuccessStatusCode)
                {
                    // Leer el contenido de error devuelto por el API
                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["Error"] = $"No se pudo crear la lista. Status: {(int)response.StatusCode} - {response.ReasonPhrase}. Detalle: {errorContent}";
                }
                else
                {
                    TempData["Success"] = "Lista creada correctamente.";
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

            // 🔄 Redirigir al Index de Board
            return RedirectToAction("Index", "Board");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditList(int id, string titulo)
        {
            if (string.IsNullOrWhiteSpace(titulo))
            {
                TempData["Error"] = "El título de la lista es obligatorio.";
                return RedirectToAction("Index", "Board");
            }

            try
            {
                var client = _httpClientFactory.CreateClient("api");

                // Primero obtenemos la lista existente
                var getResponse = await client.GetAsync($"api/Lista/{id}");
                if (!getResponse.IsSuccessStatusCode)
                {
                    TempData["Error"] = $"No se pudo obtener la lista. Status: {(int)getResponse.StatusCode} - {getResponse.ReasonPhrase}";
                    return RedirectToAction("Index", "Board");
                }

                var listaExistente = await getResponse.Content.ReadFromJsonAsync<Listum>();
                if (listaExistente == null)
                {
                    TempData["Error"] = "La lista no existe.";
                    return RedirectToAction("Index", "Board");
                }

                // Cambiamos solo el título
                listaExistente.Titulo = titulo.Trim();

                // Enviamos la actualización completa
                var putResponse = await client.PutAsJsonAsync($"api/Lista/{id}", listaExistente);
                if (!putResponse.IsSuccessStatusCode)
                {
                    var errorContent = await putResponse.Content.ReadAsStringAsync();
                    TempData["Error"] = $"No se pudo actualizar la lista. Status: {(int)putResponse.StatusCode} - {putResponse.ReasonPhrase}. Detalle: {errorContent}";
                }
                else
                {
                    TempData["Success"] = "Lista actualizada correctamente.";
                }
            }
            catch (HttpRequestException httpEx)
            {
                TempData["Error"] = $"Error HTTP: {httpEx.Message}\n{httpEx.StackTrace}";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}\n{ex.StackTrace}";
            }

            return RedirectToAction("Index", "Board");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteList(int id)
        {
            var client = _httpClientFactory.CreateClient("api");
            var res = await client.DeleteAsync($"api/Lista/{id}");
            if (!res.IsSuccessStatusCode)
                TempData["Error"] = "No se pudo eliminar la lista.";

            // Redirige siempre al Index de Board
            return RedirectToAction("Index", "Board");
        }


    }
}
