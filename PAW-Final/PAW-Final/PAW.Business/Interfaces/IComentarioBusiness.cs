using PAW.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PAW.Business.Interfaces
{
    public interface IComentarioBusiness
    {
        Task<List<Comentario>> ObtenerTodos();
        Task<Comentario?> ObtenerPorId(int id);
        Task<List<Comentario>> ObtenerPorTarjeta(int tarjetaId);
        Task<List<Comentario>> ObtenerPorUsuario(int usuarioId);
        Task Crear(Comentario comentario);
        Task Actualizar(Comentario comentario);
        Task Eliminar(int id);
    }
}

