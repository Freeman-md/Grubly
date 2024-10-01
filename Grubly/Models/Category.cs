using Microsoft.EntityFrameworkCore;

namespace Grubly.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class Category
    {
        public int ID { get; set; }
        public required string Name { get; set; }

        public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();

    }
}