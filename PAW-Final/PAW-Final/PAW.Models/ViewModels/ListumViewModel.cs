using System.ComponentModel.DataAnnotations;

namespace PAW.Models.ViewModels
{
    public class ListumViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio.")]
        [Display(Name = "Título de la lista")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "El orden es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El orden debe ser mayor que cero.")]
        [Display(Name = "Orden")]
        public int Orden { get; set; }

        [Required(ErrorMessage = "Debe asignar un tablero.")]
        [Display(Name = "ID del tablero")]
        public int TableroId { get; set; }

        // Propiedad auxiliar
        [Display(Name = "Título del tablero")]
        public string? TituloTablero { get; set; }
    }
}

