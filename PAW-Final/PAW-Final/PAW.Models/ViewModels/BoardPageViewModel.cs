using System.Collections.Generic;

namespace PAW.Models.ViewModels
{
    // Lista + sus tarjetas
    public class ListaConTarjetas
    {
        public ListumViewModel Lista { get; set; }
        public List<TarjetumViewModel> Tarjetas { get; set; } = new();
    }

    // ViewModel de la página del board
    public class BoardPageViewModel
    {
        public TableroViewModel Tablero { get; set; }
        public List<ListaConTarjetas> Columnas { get; set; } = new();

        // Opcional: para selects / asignaciones
        public List<UsuarioViewModel> Usuarios { get; set; } = new();
    }
}
