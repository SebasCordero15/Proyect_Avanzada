using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PAW.Models.Entities
{
    public class Board
    {
        public int BoardId { get; set; }
        public string Name { get; set; } = string.Empty;

        // Relation with User
        public int UserId { get; set; }
        public User User { get; set; }

        // One board can have many lists
        public ICollection<List> Lists { get; set; } = new List<List>();
    }
}