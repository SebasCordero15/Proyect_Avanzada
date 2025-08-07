using PAW.Models.Entities;

namespace PAW.Business.Interfaces
{
    public interface IUsuarioBusiness
    {
        Task<List<Usuario>> ObtenerTodos();
        Task<Usuario?> ObtenerPorId(int id);
        Task<Usuario?> ObtenerPorCorreo(string correo);
        Task Crear(Usuario usuario);
        Task Actualizar(Usuario usuario);
        Task Eliminar(int id);
    }
}

