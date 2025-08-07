using Microsoft.AspNetCore.Mvc;
using PAW.Business.Interfaces;
using PAW.Models.Entities;

namespace PAW.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComentarioController : ControllerBase
    {
        private readonly IComentarioBusiness _comentarioBusiness;

        public ComentarioController(IComentarioBusiness comentarioBusiness)
        {
            _comentarioBusiness = comentarioBusiness;
        }

        // GET: api/comentario
        [HttpGet]
        public async Task<ActionResult<List<Comentario>>> GetComentarios()
        {
            var comentarios = await _comentarioBusiness.ObtenerTodos();
            return Ok(comentarios);
        }

        // GET: api/comentario/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Comentario>> GetComentario(int id)
        {
            var comentario = await _comentarioBusiness.ObtenerPorId(id);
            if (comentario == null)
                return NotFound();

            return Ok(comentario);
        }

        // GET: api/comentario/tarjeta/3
        [HttpGet("tarjeta/{tarjetaId}")]
        public async Task<ActionResult<List<Comentario>>> GetComentariosPorTarjeta(int tarjetaId)
        {
            var comentarios = await _comentarioBusiness.ObtenerPorTarjeta(tarjetaId);
            return Ok(comentarios);
        }

        // POST: api/comentario
        [HttpPost]
        public async Task<ActionResult> CrearComentario([FromBody] Comentario nuevoComentario)
        {
            await _comentarioBusiness.Crear(nuevoComentario);
            return CreatedAtAction(nameof(GetComentario), new { id = nuevoComentario.Id }, nuevoComentario);
        }

        // PUT: api/comentario/5
        [HttpPut("{id}")]
        public async Task<ActionResult> ActualizarComentario(int id, [FromBody] Comentario comentarioActualizado)
        {
            if (id != comentarioActualizado.Id)
                return BadRequest("El ID del comentario no coincide.");

            var existente = await _comentarioBusiness.ObtenerPorId(id);
            if (existente == null)
                return NotFound();

            await _comentarioBusiness.Actualizar(comentarioActualizado);
            return NoContent();
        }

        // DELETE: api/comentario/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> EliminarComentario(int id)
        {
            var existente = await _comentarioBusiness.ObtenerPorId(id);
            if (existente == null)
                return NotFound();

            await _comentarioBusiness.Eliminar(id);
            return NoContent();
        }
    }
}

