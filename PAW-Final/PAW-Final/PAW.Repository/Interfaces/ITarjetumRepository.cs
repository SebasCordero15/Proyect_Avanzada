using PAW.Models.Entities;

namespace PAW.Repository.Interfaces
{
    public interface ITarjetumRepository : IRepositoryBase<Tarjetum>
    {
        Task<List<Tarjetum>> GetByListaIdAsync(int listaId);
        Task<List<Tarjetum>> GetByUsuarioAsignadoIdAsync(int usuarioId);
    }
}

