using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

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

    [Index(nameof(Title), IsUnique = true)]
    public class Recipe
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(50, ErrorMessage = "Title cannot exceed 50 characters.")]
        public required string Title { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters.")]
        public required string Description { get; set; }

        [StringLength(1000, ErrorMessage = "Instructions cannot exceed 1000 characters.")]
        public string? Instructions { get; set; }

        [Required(ErrorMessage = "Cuisine type is required.")]
        public required CuisineType CuisineType { get; set; }
        
        [Required(ErrorMessage = "Difficulty level is required.")]
        public required DifficultyLevel DifficultyLevel { get; set; }
        
        [Url(ErrorMessage = "Please enter a valid URL.")]
        public string? ImageUrl { get; set; }

        public ICollection<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
        public ICollection<Category> Categories { get; set; } = new List<Category>();
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    }
}
