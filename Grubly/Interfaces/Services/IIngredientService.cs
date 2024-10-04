using System;
using Grubly.Models;

namespace Grubly.Interfaces.Services;

public interface IIngredientService
{
    public Task<IReadOnlyList<Ingredient>> GetAllIngredients();
    public Task<Ingredient?> GetIngredient(int id);
    public Task<Ingredient?> GetIngredient(string name);
    public Task<Ingredient> CreateIngredient(Ingredient ingredient);
    public Task<Ingredient> UpdateIngredient(Ingredient ingredient, int id);
    public Task DeleteIngredient(int id);
}
