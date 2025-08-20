using System.Collections.Generic;

namespace PAW.Models.ViewModels
{
  
    public class ListaConTarjetas
    {
        public ListumViewModel Lista { get; set; }
        public List<TarjetumViewModel> Tarjetas { get; set; } = new();
    }

  
    public class BoardPageViewModel
    {
        public TableroViewModel Tablero { get; set; }
        public List<ListaConTarjetas> Columnas { get; set; } = new();

       
        public List<UsuarioViewModel> Usuarios { get; set; } = new();
    }
}
