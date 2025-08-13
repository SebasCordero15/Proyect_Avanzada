using PAW.Business.Interfaces;
using PAW.Models.Entities;
using PAW.Repository.Interfaces;

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
            return await _tableroRepository.GetAllAsync();
        }

        public async Task<Tablero?> ObtenerPorId(int id)
        {
            return await _tableroRepository.GetByIdAsync(id);
        }

        public async Task<List<Tablero>> ObtenerPorUsuario(int usuarioId)
        {
            return await _tableroRepository.GetByUsuarioIdAsync(usuarioId);
        }

        public async Task Crear(Tablero tablero)
        {
            await _tableroRepository.AddAsync(tablero);
        }

        public async Task Actualizar(Tablero tablero)
        {
            // Traer el tablero que ya existe
            var existing = await _tableroRepository.GetByIdAsync(tablero.Id);
            if (existing == null)
                throw new Exception("El tablero no existe");

            // Solo actualizar el título
            existing.Titulo = tablero.Titulo;

            // Guardar cambios usando el repositorio
            await _tableroRepository.UpdateAsync(existing);
        }

        public async Task Eliminar(int id)
        {
            await _tableroRepository.DeleteAsync(id);
        }
    }
}

