using PAW.Models.Entities;

namespace PAW.Business.Interfaces
{
    public interface IListumBusiness
    {
        Task<List<Listum>> ObtenerTodos();
        Task<Listum?> ObtenerPorId(int id);
        Task<List<Listum>> ObtenerPorTablero(int tableroId);
        Task Crear(Listum lista);
        Task Actualizar(Listum lista);
        Task Eliminar(int id);
    }
}

