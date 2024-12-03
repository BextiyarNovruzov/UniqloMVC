using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FrontToBackMvc.ViewModels.Sliders
{
    public class SliderUpdateVM
    {
        [MaxLength(32, ErrorMessage = "Başlıq 32 simvoldan çox ola bilməz")]
        [Required]
        public string Title { get; set; } = null!;
        [Required]
        public string Subtitle { get; set; } = null!;

        public string? Link { get; set; }
        [Required(ErrorMessage = "Fayl seçilməyib")]
       public string? ImageUrl { get; set; }
        public IFormFile? File { get; set; }


    }
}
