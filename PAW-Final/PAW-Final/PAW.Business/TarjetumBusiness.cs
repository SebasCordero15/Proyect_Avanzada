using PAW.Business.Interfaces;
using PAW.Models.Entities;
using PAW.Repository.Interfaces;

namespace PAW.Business
{
    public class TarjetumBusiness : ITarjetumBusiness
    {
        private readonly ITarjetumRepository _tarjetumRepository;

        public TarjetumBusiness(ITarjetumRepository tarjetumRepository)
        {
            _tarjetumRepository = tarjetumRepository;
        }

        public async Task<List<Tarjetum>> ObtenerTodos()
        {
            return await _tarjetumRepository.GetAllAsync();
        }

        public async Task<Tarjetum?> ObtenerPorId(int id)
        {
            return await _tarjetumRepository.GetByIdAsync(id);
        }

        public async Task<List<Tarjetum>> ObtenerPorLista(int listaId)
        {
            return await _tarjetumRepository.GetByListaIdAsync(listaId);
        }

        public async Task<List<Tarjetum>> ObtenerPorUsuarioAsignado(int usuarioId)
        {
            return await _tarjetumRepository.GetByUsuarioAsignadoIdAsync(usuarioId);
        }

        public async Task Crear(Tarjetum tarjeta)
        {
            await _tarjetumRepository.AddAsync(tarjeta);
        }

        public async Task Actualizar(Tarjetum tarjeta)
        {
            await _tarjetumRepository.UpdateAsync(tarjeta);
        }

        public async Task Eliminar(int id)
        {
            await _tarjetumRepository.DeleteAsync(id);
        }
    }
}

