using System;
using Grubly.Interfaces.Repositories;
using Grubly.Interfaces.Services;
using Grubly.Models;

namespace Grubly.Services;

public class RecipeService : IRecipeService
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IIngredientRepository _ingredientRepository;
    private readonly ICategoryRepository _categoryRepository;

    public RecipeService(IRecipeRepository recipeRepository, IIngredientRepository ingredientRepository, ICategoryRepository categoryRepository)
    {
        _ingredientRepository = ingredientRepository;
        _categoryRepository = categoryRepository;
        _recipeRepository = recipeRepository;
    }

    public Task<Recipe> CreateRecipe(Recipe recipe)
    {
        throw new NotImplementedException();
    }

    public Task DeleteRecipe(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<Recipe>> GetAllRecipes()
    {
        throw new NotImplementedException();
    }

    public Task<Recipe> GetRecipe(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Recipe> GetRecipe(string title)
    {
        throw new NotImplementedException();
    }

    public Task<Recipe> GetRecipeWithAllDetails(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Recipe> GetRecipeWithAllDetails(string title)
    {
        throw new NotImplementedException();
    }

    public Task<Recipe> UpdateRecipe(Recipe recipe, int id)
    {
        throw new NotImplementedException();
    }
}
