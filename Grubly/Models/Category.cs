namespace Grubly.Models
{
    public class Category
    {
        public int ID { get; set; }
        public required string Name { get; set; }

        public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();

    }
}