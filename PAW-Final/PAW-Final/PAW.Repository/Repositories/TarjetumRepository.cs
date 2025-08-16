using Microsoft.EntityFrameworkCore;
using PAW.Data.MSSql;
using PAW.Models.Entities;
using PAW.Repository.Interfaces;

namespace PAW.Repository.Repositories
{
    public class TarjetumRepository : RepositoryBase<Tarjetum>, ITarjetumRepository
    {
        public TarjetumRepository(ProyectDbContext context) : base(context) { }

        public async Task<List<Tarjetum>> GetByListaIdAsync(int listaId)
        {
            return await _dbSet
          .Where(t => t.ListaId == listaId)
        .Include(t => t.Lista) // <- importante
        .Include(t => t.Comentarios) // opcional
        .ToListAsync();
        }

        public async Task<List<Tarjetum>> GetByUsuarioAsignadoIdAsync(int usuarioId)
        {
            return await _dbSet
                .Where(t => t.UsuarioAsignadoId == usuarioId)
                .ToListAsync();
        }
    }
}
