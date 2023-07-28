using System.ComponentModel.DataAnnotations;

namespace GadgetIsLanding.Models
{
    public class Game
    {
        public int Id { get; set; }
        [Required(), Display(Name="Genre")]
        public int GenreId { get; set; }
        [Required(), Display(Name="Game Name")]
        public string Name { get; set; }
        [Display(Name="Game Description")]
        public string Description { get; set; }
        [Required, Display(Name = "Price in $")]
        public decimal Price { get; set; }
        [Required(), Display(Name = "Developer")]
        public string Developer { get; set; }
        [Display(Name="Image")]
        public string? Image { get; set; }
        public Genre? Genre { get; set; }

    }
}