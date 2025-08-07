using PAW.Business.Interfaces;
using PAW.Models.Entities;
using PAW.Repository.Interfaces;

namespace PAW.Business
{
    public class ListumBusiness : IListumBusiness
    {
        private readonly IListumRepository _listumRepository;

        public ListumBusiness(IListumRepository listumRepository)
        {
            _listumRepository = listumRepository;
        }

        public async Task<List<Listum>> ObtenerTodos()
        {
            return await _listumRepository.GetAllAsync();
        }

        public async Task<Listum?> ObtenerPorId(int id)
        {
            return await _listumRepository.GetByIdAsync(id);
        }

        public async Task<List<Listum>> ObtenerPorTablero(int tableroId)
        {
            return await _listumRepository.GetByTableroIdAsync(tableroId);
        }

        public async Task Crear(Listum lista)
        {
            await _listumRepository.AddAsync(lista);
        }

        public async Task Actualizar(Listum lista)
        {
            await _listumRepository.UpdateAsync(lista);
        }

        public async Task Eliminar(int id)
        {
            await _listumRepository.DeleteAsync(id);
        }
    }
}

