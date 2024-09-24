using System;
using Grubly.Models;

namespace Grubly.Interfaces.Repositories;

public interface IIngredientRepository
{
    public Task<IReadOnlyList<Ingredient>> GetAll();
    public Task<Ingredient> GetById(int id);
    public Task<Ingredient> Create(Ingredient ingredient);
    public Task<Ingredient> Update(Ingredient ingredient, int id);
    public Task Delete(int id);
}
