using PAW.Models.Entities;

namespace PAW.Business.Interfaces
{
    public interface ITarjetumBusiness
    {
        Task<List<Tarjetum>> ObtenerTodos();
        Task<Tarjetum?> ObtenerPorId(int id);
        
        Task<List<Tarjetum>> ObtenerPorUsuarioAsignado(int usuarioId);
        Task Crear(Tarjetum tarjeta);
        Task Actualizar(Tarjetum tarjeta);
        Task Eliminar(int id);

        Task<List<Tarjetum>> ObtenerPorLista(int listaId);

    }
}

