using Microsoft.AspNetCore.Mvc;
using PAW.Business.Interfaces;
using PAW.Models.Entities;

namespace PAW.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ListaController : ControllerBase
    {
        private readonly IListumBusiness _listumBusiness;

        public ListaController(IListumBusiness listumBusiness)
        {
            _listumBusiness = listumBusiness;
        }

        // GET: api/lista
        [HttpGet]
        public async Task<ActionResult<List<Listum>>> GetListas()
        {
            var listas = await _listumBusiness.ObtenerTodos();
            return Ok(listas);
        }

        // GET: api/lista/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Listum>> GetLista(int id)
        {
            var lista = await _listumBusiness.ObtenerPorId(id);
            if (lista == null)
                return NotFound();

            return Ok(lista);
        }

        // GET: api/lista/tablero/3
        [HttpGet("tablero/{tableroId}")]
        public async Task<ActionResult<List<Listum>>> GetListasPorTablero(int tableroId)
        {
            var listas = await _listumBusiness.ObtenerPorTablero(tableroId);
            return Ok(listas);
        }

        // POST: api/lista
        [HttpPost]
        public async Task<ActionResult> CrearLista([FromBody] Listum nuevaLista)
        {
            await _listumBusiness.Crear(nuevaLista);
            return CreatedAtAction(nameof(GetLista), new { id = nuevaLista.Id }, nuevaLista);
        }

       
      
        

            // DELETE: api/lista/5
            [HttpDelete("{id}")]
        public async Task<ActionResult> EliminarLista(int id)
        {
            var existente = await _listumBusiness.ObtenerPorId(id);
            if (existente == null)
                return NotFound();

            await _listumBusiness.Eliminar(id);
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> ActualizarLista(int id, [FromBody] Listum listaActualizada)
        {
            if (listaActualizada == null || string.IsNullOrWhiteSpace(listaActualizada.Titulo))
                return BadRequest("El título de la lista es obligatorio.");

            try
            {
  
                var existente = await _listumBusiness.ObtenerPorId(id);
                if (existente == null)
                    return NotFound($"No se encontró la lista con ID {id}.");

       
                existente.Titulo = listaActualizada.Titulo.Trim();

          
                await _listumBusiness.Actualizar(existente);

                return NoContent();
            }
            catch (Exception ex)
            {
     
                return StatusCode(500, $"Error inesperado: {ex.Message}\n{ex.StackTrace}");
            }
        }
}

}

