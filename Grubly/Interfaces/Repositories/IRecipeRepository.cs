using System;
using Grubly.Models;

namespace Grubly.Interfaces.Repositories;

public interface IRecipeRepository
{
    public Task<IReadOnlyList<Recipe>> GetAll();
    public Task<Recipe> GetById(int id);
    public Task<Recipe> Create(Recipe recipe);
    public Task<Recipe> Update(Recipe recipe, int id);
    public Task Delete(int id);
}
