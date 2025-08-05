using System;
using System.ComponentModel.DataAnnotations;

namespace PAW.Models.ViewModels
{
    public class ComentarioViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El contenido es obligatorio.")]
        [Display(Name = "Comentario")]
        public string Contenido { get; set; }

        [Display(Name = "Fecha del Comentario")]
        public DateTime? Fecha { get; set; }

        [Required(ErrorMessage = "La tarjeta es obligatoria.")]
        [Display(Name = "Tarjeta")]
        public int TarjetaId { get; set; }

        [Required(ErrorMessage = "El usuario es obligatorio.")]
        [Display(Name = "Usuario")]
        public int UsuarioId { get; set; }

        // Puedes agregar propiedades de navegación si se requieren en la vista:
        [Display(Name = "Nombre del Usuario")]
        public string? NombreUsuario { get; set; }

        [Display(Name = "Título de la Tarjeta")]
        public string? TituloTarjeta { get; set; }
    }
}



