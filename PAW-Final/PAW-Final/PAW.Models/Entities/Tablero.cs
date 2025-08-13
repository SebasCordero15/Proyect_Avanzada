using System;
using System.Collections.Generic;

namespace PAW.Models.Entities;

public partial class Tablero
{
    public int Id { get; set; }

    public string Titulo { get; set; } = null!;

    public DateTime? FechaCreacion { get; set; }

    public int UsuarioId { get; set; }

    public virtual ICollection<Listum> Lista { get; set; } = new List<Listum>();

    public virtual Usuario? Usuario { get; set; } 

}
