using Microsoft.AspNetCore.Mvc;
using PAW.Business.Interfaces;
using PAW.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PAW.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TableroController : ControllerBase
    {
        private readonly ITableroBusiness _tableroBusiness;

        public TableroController(ITableroBusiness tableroBusiness)
        {
            _tableroBusiness = tableroBusiness;
        }

        // GET: api/Tablero
        [HttpGet]
        public async Task<ActionResult<List<Tablero>>> GetTodos()
        {
            var tableros = await _tableroBusiness.ObtenerTodos();
            return Ok(tableros);
        }

        // GET: api/Tablero/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Tablero>> GetPorId(int id)
        {
            var tablero = await _tableroBusiness.ObtenerPorId(id);
            if (tablero == null)
            {
                return NotFound();
            }
            return Ok(tablero);
        }

        // GET: api/Tablero/usuario/3
        [HttpGet("usuario/{usuarioId}")]
        public async Task<ActionResult<List<Tablero>>> GetPorUsuario(int usuarioId)
        {
            var tableros = await _tableroBusiness.ObtenerPorUsuario(usuarioId);
            return Ok(tableros);
        }

        // POST: api/Tablero
        [HttpPost]
        public async Task<ActionResult> Crear([FromBody] Tablero tablero)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _tableroBusiness.Crear(tablero);

            // Devuelve el recurso creado con su URI
            return CreatedAtAction(nameof(GetPorId), new { id = tablero.Id }, tablero);
        }

        // PUT: api/Tablero/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Actualizar(int id, [FromBody] Tablero tablero)
        {
            if (id != tablero.Id)
            {
                return BadRequest("El ID del tablero no coincide.");
            }

            var existente = await _tableroBusiness.ObtenerPorId(id);
            if (existente == null)
            {
                return NotFound();
            }

            await _tableroBusiness.Actualizar(tablero);
            return NoContent(); // 204 sin contenido
        }

        // DELETE: api/Tablero/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Eliminar(int id)
        {
            var existente = await _tableroBusiness.ObtenerPorId(id);
            if (existente == null)
            {
                return NotFound();
            }

            await _tableroBusiness.Eliminar(id);
            return NoContent();
        }
    }
}
