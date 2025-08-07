using PAW.Models.Entities;

namespace PAW.Repository.Interfaces
{
    public interface IListumRepository : IRepositoryBase<Listum>
    {
        Task<List<Listum>> GetByTableroIdAsync(int tableroId);
    }
}

