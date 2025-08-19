using System;
using System.Collections.Generic;

namespace PAW.Models.Entities;

public partial class Tarjetum
{
    public int Id { get; set; }

    public string Titulo { get; set; } = null!;

    public string? Descripcion { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public DateTime? FechaVencimiento { get; set; }

    public int ListaId { get; set; }

    public int? UsuarioAsignadoId { get; set; }

    

    public virtual Listum Lista { get; set; } = null!;

    public virtual Usuario? UsuarioAsignado { get; set; }

    
}
