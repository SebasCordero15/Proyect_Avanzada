using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using PAW.Models.Entities;

namespace PAW.Mvc.Services
{
    public interface IAuthService
    {
        Task<Usuario> RegistrarAsync(string nombre, string correo, string password);
        Task<Usuario?> ValidarCredencialesAsync(string correo, string password);
        Task<string> CrearSesionAsync(Usuario usuario, HttpContext http, int dias = 7);
        Task<Usuario?> ObtenerUsuarioActualAsync(HttpContext http);
        Task CerrarSesionActualAsync(HttpContext http);
    }
}
