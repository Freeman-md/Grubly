namespace Grubly.Models
{
    public enum CuisineType
    {
        Italian,
        Chinese,
        Mexican
    }

    public enum DifficultyLevel
    {
        Easy,
        Medium,
        Hard
    }
    public class Recipe
    {
        public int ID { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string Instructions { get; set; }
        public required CuisineType CuisineType { get; set; }
        public required DifficultyLevel DifficultyLevel { get; set; }
        public string? ImageUrl { get; set; }

        public ICollection<Ingredient>? Ingredients { get; set; }
        public ICollection<Category>? Categories { get; set; }
        public ICollection<Rating>? Ratings { get; set; }
    }
}
