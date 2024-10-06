using System;
using Grubly.Models;

namespace Grubly.Interfaces.Services;

public interface IRecipeService
{
    public Task<IReadOnlyList<Recipe>> GetAllRecipes();
    public Task<Recipe?> GetRecipe(int id);
    public Task<Recipe?> GetRecipe(string name);
    public Task<Recipe?> GetRecipeWithAllDetails(int id);
    public Task<Recipe?> GetRecipeWithAllDetails(string name);
    public Task<Recipe> CreateRecipe(Recipe recipe);
    public Task<Recipe> UpdateRecipe(Recipe recipe, int id);
    public Task DeleteRecipe(int id);
}
