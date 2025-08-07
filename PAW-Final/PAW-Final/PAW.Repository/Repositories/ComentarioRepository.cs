using Microsoft.EntityFrameworkCore;
using PAW.Data.MSSql;
using PAW.Models.Entities;
using PAW.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace PAW.Repository.Repositories
{
    public class ComentarioRepository : RepositoryBase<Comentario>, IComentarioRepository
    {
        public ComentarioRepository(ProyectDbContext context) : base(context) { }

        public async Task<List<Comentario>> GetByTarjetaIdAsync(int tarjetaId)
        {
            return await _dbSet
                .Where(c => c.TarjetaId == tarjetaId)
                .Include(c => c.Usuario)
                .ToListAsync();
        }

        public async Task<List<Comentario>> GetByUsuarioIdAsync(int usuarioId)
        {
            return await _dbSet
                .Where(c => c.UsuarioId == usuarioId)
                .Include(c => c.Tarjeta)
                .ToListAsync();
        }
    }
}

