using PAW.Business.Interfaces;
using PAW.Models.Entities;
using PAW.Repository.Interfaces;

namespace PAW.Business
{
    public class UsuarioBusiness : IUsuarioBusiness
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioBusiness(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<List<Usuario>> ObtenerTodos()
        {
            return await _usuarioRepository.GetAllAsync();
        }

        public async Task<Usuario?> ObtenerPorId(int id)
        {
            return await _usuarioRepository.GetByIdAsync(id);
        }

        public async Task<Usuario?> ObtenerPorCorreo(string correo)
        {
            return await _usuarioRepository.GetByCorreoAsync(correo);
        }

        public async Task Crear(Usuario usuario)
        {
            await _usuarioRepository.AddAsync(usuario);
        }

        

       


        public async Task Eliminar(int id)
        {
            await _usuarioRepository.DeleteAsync(id);
        }

        public async Task Actualizar(Usuario usuario)
        {
            if (usuario == null) throw new ArgumentNullException(nameof(usuario));
            await _usuarioRepository.UpdateUsuarioAsync(usuario); // 👈 usar el del repo
        }
    }
}

