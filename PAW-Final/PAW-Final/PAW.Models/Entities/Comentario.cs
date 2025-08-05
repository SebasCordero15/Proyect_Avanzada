using System;
using System.Collections.Generic;

namespace PAW.Models.Entities;

public partial class Comentario
{
    public int Id { get; set; }

    public string? Contenido { get; set; }

    public DateTime? Fecha { get; set; }

    public int TarjetaId { get; set; }

    public int UsuarioId { get; set; }

    public virtual Tarjetum Tarjeta { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;
}
