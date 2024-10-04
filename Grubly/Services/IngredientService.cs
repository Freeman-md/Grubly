using System;
using Grubly.Interfaces.Repositories;
using Grubly.Interfaces.Services;
using Grubly.Models;

namespace Grubly.Services;

public class IngredientService : IIngredientService
{
    private readonly IIngredientRepository _ingredientRepository;

    public IngredientService(IIngredientRepository ingredientRepository)
    {
        _ingredientRepository = ingredientRepository;
    }

    public Task<Ingredient> CreateIngredient(Ingredient ingredient)
    {
        throw new NotImplementedException();
    }

    public Task DeleteIngredient(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<Ingredient>> GetAllIngredients()
    {
        throw new NotImplementedException();
    }

    public Task<Ingredient?> GetIngredient(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Ingredient?> GetIngredient(string name)
    {
        throw new NotImplementedException();
    }

    public Task<Ingredient> UpdateIngredient(Ingredient ingredient, int id)
    {
        throw new NotImplementedException();
    }
}
