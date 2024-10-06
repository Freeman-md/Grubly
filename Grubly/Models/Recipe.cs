﻿using System.ComponentModel.DataAnnotations;
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

        [StringLength(50)]
        public required string Title { get; set; }

        [StringLength(50)]
        public required string Description { get; set; }
        public string? Instructions { get; set; }
        public required CuisineType CuisineType { get; set; }
        public required DifficultyLevel DifficultyLevel { get; set; }
        public string? ImageUrl { get; set; }

        public ICollection<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
        public ICollection<Category> Categories { get; set; } = new List<Category>();
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    }
}
