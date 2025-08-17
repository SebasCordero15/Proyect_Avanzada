using PAW.Business.Interfaces;
using PAW.Models.Entities;
using PAW.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PAW.Business
{
    public class TableroBusiness : ITableroBusiness
    {
        private readonly ITableroRepository _tableroRepository;

        public TableroBusiness(ITableroRepository tableroRepository)
        {
            _tableroRepository = tableroRepository;
        }

        // Obtener todos los tableros con listas y tarjetas
        public async Task<List<Tablero>> ObtenerTodos()
        {
            return await _tableroRepository.GetAllWithListsAndCardsAsync();
        }

        // Obtener un tablero específico con listas y tarjetas
        public async Task<Tablero?> ObtenerPorId(int id)
        {
            return await _tableroRepository.GetByIdWithListsAndCardsAsync(id);
        }

        // Obtener tableros de un usuario específico con listas y tarjetas
        public async Task<List<Tablero>> ObtenerPorUsuario(int usuarioId)
        {
            return await _tableroRepository.GetByUsuarioIdWithListsAndCardsAsync(usuarioId);
        }

        // Crear un tablero
        public async Task Crear(Tablero tablero)
        {
            await _tableroRepository.AddAsync(tablero);
        }

        // Actualizar solo el título de un tablero existente
        public async Task Actualizar(Tablero tablero)
        {
            var existing = await _tableroRepository.GetByIdAsync(tablero.Id);
            if (existing == null)
                throw new Exception("El tablero no existe");

            existing.Titulo = tablero.Titulo;
            await _tableroRepository.UpdateAsync(existing);
        }

        // Eliminar un tablero
        public async Task Eliminar(int id)
        {
            await _tableroRepository.DeleteAsync(id);
        }
    }
}


