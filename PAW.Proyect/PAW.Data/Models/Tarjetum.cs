﻿using System;
using System.Collections.Generic;

namespace PAW.Data.Models;

public partial class Tarjetum
{
    public int Id { get; set; }

    public string Titulo { get; set; } = null!;

    public string? Descripcion { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public DateTime? FechaVencimiento { get; set; }

    public int ListaId { get; set; }

    public int? UsuarioAsignadoId { get; set; }

    public virtual ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();

    public virtual Listum Lista { get; set; } = null!;

    public virtual Usuario? UsuarioAsignado { get; set; }
}
