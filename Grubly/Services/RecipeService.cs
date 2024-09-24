using System;
using Grubly.Interfaces.Repositories;
using Grubly.Interfaces.Services;
using Grubly.Models;

namespace Grubly.Services;

public class RecipeService : IRecipeService
{
    private readonly IRecipeRepository _recipeRepository;

    public RecipeService(IRecipeRepository recipeRepository)
    {
        _recipeRepository = recipeRepository;
    }

    public Task<Recipe> CreateRecipe(Recipe recipe)
    {
        throw new NotImplementedException();
    }

    public void DeleteRecipe(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<Recipe>> GetAllRecipes()
    {
        throw new NotImplementedException();
    }

    public Task<Recipe> GetRecipeById(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Recipe> UpdateRecipe(Recipe recipe, int id)
    {
        throw new NotImplementedException();
    }
}
