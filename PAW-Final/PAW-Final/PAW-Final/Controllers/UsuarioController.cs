using Microsoft.AspNetCore.Mvc;
using PAW.Business.Interfaces;
using PAW.Models.Entities;

namespace PAW.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioBusiness _usuarioBusiness;

        public UsuarioController(IUsuarioBusiness usuarioBusiness)
        {
            _usuarioBusiness = usuarioBusiness;
        }

        // GET: api/usuario
        [HttpGet]
        public async Task<ActionResult<List<Usuario>>> GetUsuarios()
        {
            var usuarios = await _usuarioBusiness.ObtenerTodos();
            return Ok(usuarios);
        }

        // GET: api/usuario/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            var usuario = await _usuarioBusiness.ObtenerPorId(id);
            if (usuario == null)
                return NotFound();

            return Ok(usuario);
        }

        // GET: api/usuario/correo/algo@correo.com
        [HttpGet("correo/{correo}")]
        public async Task<ActionResult<Usuario>> GetUsuarioPorCorreo(string correo)
        {
            var usuario = await _usuarioBusiness.ObtenerPorCorreo(correo);
            if (usuario == null)
                return NotFound();

            return Ok(usuario);
        }

        // POST: api/usuario
        [HttpPost]
        public async Task<ActionResult> CrearUsuario([FromBody] Usuario nuevoUsuario)
        {
            await _usuarioBusiness.Crear(nuevoUsuario);
            return CreatedAtAction(nameof(GetUsuario), new { id = nuevoUsuario.Id }, nuevoUsuario);
        }

        // PUT: api/usuario/5
        [HttpPut("{id}")]
        public async Task<ActionResult> ActualizarUsuario(int id, [FromBody] Usuario usuarioActualizado)
        {
            if (id != usuarioActualizado.Id)
                return BadRequest("El ID del usuario no coincide.");

            var usuarioExistente = await _usuarioBusiness.ObtenerPorId(id);
            if (usuarioExistente == null)
                return NotFound();

            await _usuarioBusiness.Actualizar(usuarioActualizado);
            return NoContent();
        }

        // DELETE: api/usuario/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> EliminarUsuario(int id)
        {
            var usuarioExistente = await _usuarioBusiness.ObtenerPorId(id);
            if (usuarioExistente == null)
                return NotFound();

            await _usuarioBusiness.Eliminar(id);
            return NoContent();
        }
    }
}
