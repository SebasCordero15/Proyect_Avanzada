using System;
using System.ComponentModel.DataAnnotations;

namespace PAW.Models.ViewModels
{
    public class TarjetumViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio.")]
        [Display(Name = "Título de la tarjeta")]
        public string Titulo { get; set; }

        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [Display(Name = "Fecha de creación")]
        public DateTime? FechaCreacion { get; set; }

        [Display(Name = "Fecha de vencimiento")]
        [DataType(DataType.DateTime)]
        public DateTime? FechaVencimiento { get; set; }

        [Required(ErrorMessage = "Debe asignar una lista.")]
        [Display(Name = "ID de la lista")]
        public int ListaId { get; set; }

        [Display(Name = "Usuario asignado")]
        public int? UsuarioAsignadoId { get; set; }


        [Display(Name = "Título de la lista")]
        public string? TituloLista { get; set; }

        [Display(Name = "Nombre del usuario asignado")]
        public string? NombreUsuarioAsignado { get; set; }

        
    }
}

