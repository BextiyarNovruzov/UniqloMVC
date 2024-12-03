using System.ComponentModel.DataAnnotations;

namespace FrontToBackMvc.Models
{
    public class Slider:BaseEntity
    {
        [MaxLength(60)]
        public string Title { get; set; } = null!;

        [MaxLength(60)]
        public string Subtitle { get; set; } = null!;
        public string ImageUrl { get; set; } =null!;
        public string? Link { get; set; }
        
         
    }
}
