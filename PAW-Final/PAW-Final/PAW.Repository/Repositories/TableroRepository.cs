using Microsoft.EntityFrameworkCore;
using PAW.Data.MSSql;
using PAW.Models.Entities;
using PAW.Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PAW.Repository.Repositories
{
    public class TableroRepository : RepositoryBase<Tablero>, ITableroRepository
    {
        public TableroRepository(ProyectDbContext context) : base(context) { }

        // Tableros normales por usuario (solo listas, no tarjetas)
        public async Task<List<Tablero>> GetByUsuarioIdAsync(int usuarioId)
        {
            return await _dbSet
                .Where(t => t.UsuarioId == usuarioId)
                .Include(t => t.Lista)
                .ToListAsync();
        }

        // Tableros con listas y tarjetas
        public async Task<List<Tablero>> GetAllWithListsAndCardsAsync()
        {
            return await _dbSet
                .Include(t => t.Lista)
                    .ThenInclude(l => l.Tarjeta)
                .ToListAsync();
        }

        public async Task<Tablero?> GetByIdWithListsAndCardsAsync(int id)
        {
            return await _dbSet
                .Include(t => t.Lista)
                    .ThenInclude(l => l.Tarjeta)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<List<Tablero>> GetByUsuarioIdWithListsAndCardsAsync(int usuarioId)
        {
            return await _dbSet
                .Where(t => t.UsuarioId == usuarioId)
                .Include(t => t.Lista)
                    .ThenInclude(l => l.Tarjeta)
                .ToListAsync();
        }

        // Eliminar tablero
        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}


