using System;
using Grubly.Models;

namespace Grubly.ViewModels;

public class RecipeIndexViewModel
{
    public IReadOnlyCollection<Recipe> Recipes { get; }
    public IReadOnlyCollection<Category> Categories { get; }
    public IReadOnlyCollection<Ingredient> Ingredients { get; }
    public IReadOnlyCollection<CuisineType> CuisineTypes { get; }
    public IReadOnlyCollection<DifficultyLevel> DifficultyLevels { get; }

    public RecipeIndexViewModel(
        IReadOnlyCollection<Recipe> recipes,
        IReadOnlyCollection<Category> categories,
        IReadOnlyCollection<Ingredient> ingredients,
        IReadOnlyCollection<CuisineType> cuisineTypes,
        IReadOnlyCollection<DifficultyLevel> difficultyLevels
        )
    {
        Recipes = recipes;
        Categories = categories;
        Ingredients = ingredients;
        CuisineTypes = cuisineTypes;
        DifficultyLevels = difficultyLevels;
    }

}
