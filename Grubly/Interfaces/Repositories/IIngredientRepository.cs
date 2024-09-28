using System;
using Grubly.Models;

namespace Grubly.Interfaces.Repositories;

public interface IIngredientRepository
{
    public Task<IReadOnlyList<Ingredient>> GetAll();
    public Task<Ingredient> GetOne(int id);
    public Task<Ingredient> GetOne(string name);
    public Task<Ingredient> Create(Ingredient ingredient);
    public Task<Ingredient> Update(Ingredient ingredient, int id);
    public Task Delete(int id);
}
