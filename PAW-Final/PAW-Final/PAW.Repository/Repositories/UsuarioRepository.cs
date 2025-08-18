// using Microsoft.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore;
using PAW.Data.MSSql;
using PAW.Models.Entities;
using PAW.Repository.Interfaces;
using PAW.Repository.Repositories;

public class UsuarioRepository : RepositoryBase<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(ProyectDbContext context) : base(context) { }

    public async Task<Usuario?> GetByCorreoAsync(string correo)
        => await _dbSet.FirstOrDefaultAsync(u => u.Correo == correo);

    // NUEVO: Update que evita “another instance with the same key is already being tracked”
    public async Task UpdateUsuarioAsync(Usuario entity)
    {
        // Detachar cualquier instancia local con el mismo Id
        var local = _dbSet.Local.FirstOrDefault(u => u.Id == entity.Id);
        if (local != null)
            _context.Entry(local).State = EntityState.Detached;

        // Adjuntar y marcar modificado
        _dbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;

        await _context.SaveChangesAsync();
    }
}
