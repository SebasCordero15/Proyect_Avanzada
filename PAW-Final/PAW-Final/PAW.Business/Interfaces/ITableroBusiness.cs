using PAW.Models.Entities;

namespace PAW.Business.Interfaces
{
    public interface ITableroBusiness
    {
        Task<List<Tablero>> ObtenerTodos();
        Task<Tablero?> ObtenerPorId(int id);
        Task<List<Tablero>> ObtenerPorUsuario(int usuarioId);

        Task Crear(Tablero tablero);
        Task Actualizar(Tablero tablero);
        Task Eliminar(int id);
    }
}

