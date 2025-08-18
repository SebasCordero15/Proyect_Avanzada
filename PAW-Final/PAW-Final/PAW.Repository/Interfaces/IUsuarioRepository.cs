using PAW.Models.Entities;

namespace PAW.Repository.Interfaces
{
    public interface IUsuarioRepository : IRepositoryBase<Usuario>
    {
        Task<Usuario?> GetByCorreoAsync(string correo);
        Task UpdateUsuarioAsync(Usuario entity);
    }
}
