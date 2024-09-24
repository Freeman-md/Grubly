using System;
using Grubly.Models;

namespace Grubly.Interfaces.Services;

public interface IRecipeService
{
 public Task<IReadOnlyList<Recipe>> GetAllRecipes();
    public Task<Recipe> GetRecipeById(int id);
    public Task<Recipe> CreateRecipe(Recipe recipe);
    public Task<Recipe> UpdateRecipe(Recipe recipe, int id);
    public void DeleteRecipe(int id);
}
