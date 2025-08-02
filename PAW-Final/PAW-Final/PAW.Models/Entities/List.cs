using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PAW.Models.Entities
{
    public class List
    {
        public int ListId { get; set; }
        public string Name { get; set; } = string.Empty;

        // Relation with Board
        public int BoardId { get; set; }
        public Board Board { get; set; }

        // One list can have many cards
        public ICollection<Card> Cards { get; set; } = new List<Card>();
    }
}