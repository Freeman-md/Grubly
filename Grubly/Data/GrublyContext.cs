using Grubly.Models;
using Microsoft.EntityFrameworkCore;

namespace Grubly.Data
{
    public class GrublyContext : DbContext {
        public GrublyContext(DbContextOptions<GrublyContext> options) : base(options) {

        }

        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Rating> Ratings { get; set; }
    }
}
