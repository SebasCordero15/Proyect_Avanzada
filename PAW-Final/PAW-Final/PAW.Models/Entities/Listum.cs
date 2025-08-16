using System;
using System.Collections.Generic;

namespace PAW.Models.Entities;

public partial class Listum
{
    public int Id { get; set; }

    public string Titulo { get; set; } = null!;

    public int Orden { get; set; }

    public int TableroId { get; set; }

    public virtual Tablero? Tablero { get; set; }

    public virtual ICollection<Tarjetum> Tarjeta { get; set; } = new List<Tarjetum>();
}
