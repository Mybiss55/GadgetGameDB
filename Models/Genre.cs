using System.ComponentModel.DataAnnotations;

namespace GadgetIsLanding.Models
{
    public class Genre
    {
        public int Id { get; set; }
        [Required(), Display(Name="Genre Name")]
        public string Name { get; set; }
        [Required(), Display(Name="Genre Description")]
        public string? Description { get; set; }
        public List<Game>? Games { get; set; }
    }
}
