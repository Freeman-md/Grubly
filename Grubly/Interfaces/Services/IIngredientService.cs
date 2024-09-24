using System;
using Grubly.Models;

namespace Grubly.Interfaces.Services;

public interface IIngredientService
{
    public Task<IReadOnlyList<Ingredient>> GetAllIngredients();
    public Task<Ingredient> GetIngredientById(int id);
    public Task<Ingredient> CreateIngredient(Ingredient ingredient);
    public Task<Ingredient> UpdateIngredient(Ingredient ingredient, int id);
    public void DeleteIngredient(int id);
}
