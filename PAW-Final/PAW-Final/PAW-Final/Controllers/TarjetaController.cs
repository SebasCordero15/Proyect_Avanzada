using Microsoft.AspNetCore.Mvc;
using PAW.Business.Interfaces;
using PAW.Models.Entities;
using PAW.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PAW.Web.ApiControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TarjetaController : ControllerBase
    {
        private readonly ITarjetumBusiness _tarjetumBusiness;

        public TarjetaController(ITarjetumBusiness tarjetumBusiness)
        {
            _tarjetumBusiness = tarjetumBusiness;
        }

        // GET: api/tarjeta
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TarjetumViewModel>>> GetAll()
        {
            var tarjetas = await _tarjetumBusiness.ObtenerTodos();

            var result = tarjetas.Select(t => new TarjetumViewModel
            {
                Id = t.Id,
                Titulo = t.Titulo,
                Descripcion = t.Descripcion,
                FechaCreacion = t.FechaCreacion,
                FechaVencimiento = t.FechaVencimiento,
                ListaId = t.ListaId,
                UsuarioAsignadoId = t.UsuarioAsignadoId
            }).ToList();

            return Ok(result);
        }

        // GET: api/tarjeta/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TarjetumViewModel>> GetById(int id)
        {
            var tarjeta = await _tarjetumBusiness.ObtenerPorId(id);
            if (tarjeta == null)
                return NotFound();

            var model = new TarjetumViewModel
            {
                Id = tarjeta.Id,
                Titulo = tarjeta.Titulo,
                Descripcion = tarjeta.Descripcion,
                FechaCreacion = tarjeta.FechaCreacion,
                FechaVencimiento = tarjeta.FechaVencimiento,
                ListaId = tarjeta.ListaId,
                UsuarioAsignadoId = tarjeta.UsuarioAsignadoId
            };

            return Ok(model);
        }

        // POST: api/tarjeta
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] TarjetumViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var tarjeta = new Tarjetum
            {
                Titulo = model.Titulo,
                Descripcion = model.Descripcion,
                FechaCreacion = DateTime.Now,
                FechaVencimiento = model.FechaVencimiento,
                ListaId = model.ListaId,
                UsuarioAsignadoId = model.UsuarioAsignadoId
            };

            await _tarjetumBusiness.Crear(tarjeta);

            // Retornamos CreatedAt con la ruta para obtener la tarjeta creada
            return CreatedAtAction(nameof(GetById), new { id = tarjeta.Id }, model);
        }

        // PUT: api/tarjeta/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] TarjetumViewModel model)
        {
            if (id != model.Id)
                return BadRequest("El id en la URL y el modelo no coinciden.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _tarjetumBusiness.ObtenerPorId(id);
            if (existing == null)
                return NotFound();

            existing.Titulo = model.Titulo;
            existing.Descripcion = model.Descripcion;
            existing.FechaVencimiento = model.FechaVencimiento;
            existing.ListaId = model.ListaId;
            existing.UsuarioAsignadoId = model.UsuarioAsignadoId;

            await _tarjetumBusiness.Actualizar(existing);

            return NoContent();
        }

        // DELETE: api/tarjeta/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existing = await _tarjetumBusiness.ObtenerPorId(id);
            if (existing == null)
                return NotFound();

            await _tarjetumBusiness.Eliminar(id);
            return NoContent();
        }

        // GET: api/tarjeta/lista/5
        [HttpGet("lista/{listaId}")]
        public async Task<ActionResult<IEnumerable<TarjetumViewModel>>> GetByLista(int listaId)
        {
            var tarjetas = await _tarjetumBusiness.ObtenerPorLista(listaId);
            if (tarjetas == null || !tarjetas.Any())
                return Ok(new List<TarjetumViewModel>());

            var result = tarjetas.Select(t => new TarjetumViewModel
            {
                Id = t.Id,
                Titulo = t.Titulo,
                Descripcion = t.Descripcion,
                FechaCreacion = t.FechaCreacion,
                FechaVencimiento = t.FechaVencimiento,
                ListaId = t.ListaId,
                UsuarioAsignadoId = t.UsuarioAsignadoId
            }).ToList();

            return Ok(result);
        }

    }
}
