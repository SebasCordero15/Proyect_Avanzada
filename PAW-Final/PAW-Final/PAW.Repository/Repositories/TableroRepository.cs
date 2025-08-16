using Microsoft.EntityFrameworkCore;
using PAW.Data.MSSql;
using PAW.Models.Entities;
using PAW.Repository.Interfaces;

namespace PAW.Repository.Repositories
{
    public class TableroRepository : RepositoryBase<Tablero>, ITableroRepository
    {
        public TableroRepository(ProyectDbContext context) : base(context) { }

        public async Task<List<Tablero>> GetByUsuarioIdAsync(int usuarioId)
        {
            return await _dbSet
                .Where(t => t.UsuarioId == usuarioId)
                .Include(t => t.Lista)
                .ToListAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}

