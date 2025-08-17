using PAW.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PAW.Repository.Interfaces
{
    public interface ITableroRepository : IRepositoryBase<Tablero>
    {
        Task<List<Tablero>> GetByUsuarioIdAsync(int usuarioId);

        Task<List<Tablero>> GetAllWithListsAndCardsAsync();
        Task<Tablero?> GetByIdWithListsAndCardsAsync(int id);
        Task<List<Tablero>> GetByUsuarioIdWithListsAndCardsAsync(int usuarioId);
    }
}

