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


        public async Task<List<Tablero>> ObtenerTodos()
        {
            return await _tableroRepository.GetAllWithListsAndCardsAsync();
        }


        public async Task<Tablero?> ObtenerPorId(int id)
        {
            return await _tableroRepository.GetByIdWithListsAndCardsAsync(id);
        }

   
        public async Task<List<Tablero>> ObtenerPorUsuario(int usuarioId)
        {
            return await _tableroRepository.GetByUsuarioIdWithListsAndCardsAsync(usuarioId);
        }


        public async Task Crear(Tablero tablero)
        {
            await _tableroRepository.AddAsync(tablero);
        }


        public async Task Actualizar(Tablero tablero)
        {
            var existing = await _tableroRepository.GetByIdAsync(tablero.Id);
            if (existing == null)
                throw new Exception("El tablero no existe");

            existing.Titulo = tablero.Titulo;
            await _tableroRepository.UpdateAsync(existing);
        }

    
        public async Task Eliminar(int id)
        {
            await _tableroRepository.DeleteAsync(id);
        }
    }
}


