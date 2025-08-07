using PAW.Models.Entities;

namespace PAW.Repository.Interfaces
{
    public interface ITableroRepository : IRepositoryBase<Tablero>
    {
        Task<List<Tablero>> GetByUsuarioIdAsync(int usuarioId);
    }
}

