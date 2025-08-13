using System;
using System.Collections.Generic;

namespace PAW.Models.Entities;

public partial class Usuario
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string Clave { get; set; } = null!;

    public virtual ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();

    public virtual ICollection<Tablero> Tableros { get; set; } = new List<Tablero>();

    public virtual ICollection<Tarjetum> Tarjeta { get; set; } = new List<Tarjetum>();

    public virtual ICollection<Sesion> Sesiones { get; set; } = new List<Sesion>();
}
