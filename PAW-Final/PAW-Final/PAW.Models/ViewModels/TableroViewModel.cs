using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PAW.Models.ViewModels
{
    public class TableroViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio.")]
        [Display(Name = "Título del tablero")]
        public string Titulo { get; set; }

        [Display(Name = "Fecha de creación")]
        public DateTime? FechaCreacion { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un usuario.")]
        [Display(Name = "ID del usuario")]
        public int UsuarioId { get; set; }

      
        [Display(Name = "Nombre del usuario")]
        public string? NombreUsuario { get; set; }

   
        public List<ListumViewModel> Lista { get; set; } = new();
    }
}
