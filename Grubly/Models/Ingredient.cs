namespace Grubly.Models
{
    public class Ingredient
    {
        public int ID { get; set; }
        public required string Name { get; set; }

        public string? Description { get; set; }

        public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
    }
}