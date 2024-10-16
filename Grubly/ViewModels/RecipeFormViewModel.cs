using System.ComponentModel.DataAnnotations;
using Grubly.Attributes;
using Grubly.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Grubly.ViewModels
{
    public class RecipeFormViewModel
    {
        public int? ID { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(50, ErrorMessage = "Title cannot exceed 50 characters.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters.")]
        public string Description { get; set; }

        [StringLength(1000, ErrorMessage = "Instructions cannot exceed 1000 characters.")]
        public string Instructions { get; set; }

        [Required(ErrorMessage = "Cuisine type is required.")]
        public CuisineType CuisineType { get; set; }

        [Required(ErrorMessage = "Difficulty level is required.")]
        public DifficultyLevel DifficultyLevel { get; set; }

        [Url(ErrorMessage = "Please enter a valid URL.")]
        public string? ImageUrl { get; set; }

        [ValidateNever]
        public List<Ingredient> AvailableIngredients { get; set; }
        [AtLeastOneSelected(ErrorMessage = "Please select at least one ingredient.")]
        public bool[] SelectedIngredients { get; set; }

        [ValidateNever]
        public List<Category> AvailableCategories { get; set; }
        [AtLeastOneSelected(ErrorMessage = "Please select at least one category.")]
        public bool[] SelectedCategories { get; set; }
    }
}