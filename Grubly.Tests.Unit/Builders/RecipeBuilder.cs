using System;
using Grubly.Models;

namespace Grubly.Tests.Unit.Builders;

public class RecipeBuilder
{
    private Recipe _recipe;

    public RecipeBuilder()
    {
        // Default recipe initialization
        _recipe = new Recipe
        {
            Title = Guid.NewGuid().ToString(),
            Description = "A default recipe description.",
            Instructions = "1. Default step 1\n2. Default step 2",
            CuisineType = CuisineType.Italian,
            DifficultyLevel = DifficultyLevel.Medium,
            ImageUrl = "https://example.com/default_recipe.jpg",
            Ingredients = new List<Ingredient>(),
            Categories = new List<Category>()
        };
    }

    public RecipeBuilder WithTitle(string title)
    {
        _recipe.Title = title;
        return this;
    }

    public RecipeBuilder WithIngredients(params Ingredient[] ingredients)
    {
        _recipe.Ingredients = ingredients.ToList();
        return this;
    }

    public RecipeBuilder WithCategories(params Category[] categories)
    {
        _recipe.Categories = categories.ToList();
        return this;
    }

    public Recipe Build()
    {
        return _recipe;
    }

    // Method to generate multiple recipes for testing
    public static List<Recipe> BuildMany(int count)
    {
        var recipes = new List<Recipe>();
        for (int i = 0; i < count; i++)
        {
            recipes.Add(new RecipeBuilder()
                .WithTitle($"Recipe {i + 1}")
                .WithIngredients(new Ingredient { Name = "Ingredient 1" }, new Ingredient { Name = "Ingredient 2" })
                .WithCategories(new Category { Name = "Category 1" })
                .Build());
        }
        return recipes;
    }
}

