using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace PAW.Models.ViewModels
{
    public class LoginVm
    {
        [Required, EmailAddress]
        [Display(Name = "Correo")]
        public string Correo { get; set; }

        [Required, DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Clave { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
