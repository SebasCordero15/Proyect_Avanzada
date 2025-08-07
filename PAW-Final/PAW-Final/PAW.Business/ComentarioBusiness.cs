using PAW.Business.Interfaces;
using PAW.Models.Entities;
using PAW.Repository.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PAW.Business
{
    public class ComentarioService : IComentarioBusiness
    {
        private readonly IComentarioRepository _comentarioRepository;

        public ComentarioService(IComentarioRepository comentarioRepository)
        {
            _comentarioRepository = comentarioRepository;
        }

        public async Task<List<Comentario>> ObtenerTodos()
        {
            return await _comentarioRepository.GetAllAsync();
        }

        public async Task<Comentario?> ObtenerPorId(int id)
        {
            return await _comentarioRepository.GetByIdAsync(id);
        }

        public async Task<List<Comentario>> ObtenerPorTarjeta(int tarjetaId)
        {
            return await _comentarioRepository.GetByTarjetaIdAsync(tarjetaId);
        }

        public async Task<List<Comentario>> ObtenerPorUsuario(int usuarioId)
        {
            return await _comentarioRepository.GetByUsuarioIdAsync(usuarioId);
        }

        public async Task Crear(Comentario comentario)
        {
            await _comentarioRepository.AddAsync(comentario);
        }

        public async Task Actualizar(Comentario comentario)
        {
            await _comentarioRepository.UpdateAsync(comentario);
        }

        public async Task Eliminar(int id)
        {
            await _comentarioRepository.DeleteAsync(id);
        }
    }
}

