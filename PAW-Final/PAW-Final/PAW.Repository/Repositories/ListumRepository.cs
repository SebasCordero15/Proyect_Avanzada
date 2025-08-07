using Microsoft.EntityFrameworkCore;
using PAW.Data.MSSql;
using PAW.Models.Entities;
using PAW.Repository.Interfaces;

namespace PAW.Repository.Repositories
{
    public class ListumRepository : RepositoryBase<Listum>, IListumRepository
    {
        public ListumRepository(ProyectDbContext context) : base(context) { }

        public async Task<List<Listum>> GetByTableroIdAsync(int tableroId)
        {
            return await _dbSet
                .Where(l => l.TableroId == tableroId)
                .Include(l => l.Tarjeta)
                .ToListAsync();
        }
    }
}
