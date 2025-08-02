using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PAW.Models.Entities
{
    public class Card
    {
        public int CardId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Relation with List
        public int ListId { get; set; }
        public List List { get; set; }

        // One card can have many comments
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}