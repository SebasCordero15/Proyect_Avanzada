using PAW.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PAW.Repository.Interfaces
{
    public interface IComentarioRepository : IRepositoryBase<Comentario>
    {
        Task<List<Comentario>> GetByTarjetaIdAsync(int tarjetaId);
        Task<List<Comentario>> GetByUsuarioIdAsync(int usuarioId);
    }
}

