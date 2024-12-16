namespace FrontToBackMvc.Models
{
    public class Comment:BaseEntity
    {
        public int Id { get; set; }
        public string? Author { get; set; }
        public string? Content { get; set; }
        public int ProductId { get; set; }
        public string? UserId { get; set; }
        public Product? Product { get; set; }
        public User? User { get; set; }
        public string? UserEmail { get; set; } = null;


    }
}
