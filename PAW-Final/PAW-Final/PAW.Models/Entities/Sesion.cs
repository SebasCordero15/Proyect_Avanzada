using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace PAW.Models.Entities
{
    public class Sesion
    {
        public int Id { get; set; }

        [Required] public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        [Required, MaxLength(200)]
        public string Token { get; set; } = "";

        public DateTime Creado { get; set; } = DateTime.UtcNow;
        public DateTime Expira { get; set; } = DateTime.UtcNow.AddDays(7);
        public bool Revocado { get; set; } = false;

        [MaxLength(45)] public string? Ip { get; set; }
        [MaxLength(256)] public string? UserAgent { get; set; }
    }
}

