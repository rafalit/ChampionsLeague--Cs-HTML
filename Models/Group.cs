using System.Collections.Generic;

namespace WebApplication1.Models
{
    public class Group
    {
        public string Name { get; set; }
        public List<Club> Teams { get; set; } = new List<Club>();
    }
}