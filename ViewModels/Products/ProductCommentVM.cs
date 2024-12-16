namespace FrontToBackMvc.ViewModels.Products
{
    public class ProductCommentVM
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string? UserId { get; set; }
        public string? Author { get; set; }
        public string Content { get; set; }
        public string?UserEmail { get; set; }

    }
}
