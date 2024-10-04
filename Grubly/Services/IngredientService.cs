using System;
using System.ComponentModel.DataAnnotations;
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
        ValidateIngredient(ingredient);

        return _ingredientRepository.Create(ingredient);
    }

    public Task DeleteIngredient(int id)
    {
        return _ingredientRepository.Delete(id);
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

    private void ValidateIngredient(Ingredient ingredient)
    {
        if (ingredient == null)
        {
            throw new ArgumentNullException(nameof(ingredient), "Ingredient cannot be null.");
        }

        if (string.IsNullOrWhiteSpace(ingredient.Name))
        {
            throw new ValidationException("Ingredient name cannot be null, empty, or whitespace.");
        }

        if (ingredient.Name.Length > 50)
        {
            throw new ValidationException("Ingredient name cannot exceed 50 characters.");
        }

        if (ingredient.Description != null && ingredient.Description.Length > 500) 
        {
            throw new ValidationException("Ingredient description cannot exceed 500 characters.");
        }
    }
}
