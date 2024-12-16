using FrontToBackMvc.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FrontToBackMvc.DataAccess
{
    public class UniqloDbContext : IdentityDbContext<User>
    {
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ProductRating> ProductRatings { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public UniqloDbContext(DbContextOptions opt) : base(opt) { }
    }

}
