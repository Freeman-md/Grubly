using System;
using System.ComponentModel.DataAnnotations;
using Grubly.Interfaces.Repositories;
using Grubly.Interfaces.Services;
using Grubly.Models;

namespace Grubly.Services;

public class IngredientService : IIngredientService
{
    private readonly IIngredientRepository _ingredientRepository;
    private readonly IRecipeRepository _recipeRepository;

    public IngredientService(IIngredientRepository ingredientRepository, IRecipeRepository recipeRepository)
    {
        _ingredientRepository = ingredientRepository;
        _recipeRepository = recipeRepository;
    }

    public async Task<Ingredient> CreateIngredient(Ingredient ingredient)
    {
        if (ingredient == null)
        {
            throw new ArgumentNullException(nameof(ingredient), "Ingredient cannot be null.");
        }

        ValidateIngredient(ingredient);

        await EnsureAllRelatedRecipesExist(ingredient);

        return await _ingredientRepository.Create(ingredient);
    }

    public async Task DeleteIngredient(int id)
    {
        Ingredient? existingIngredient = await _ingredientRepository.GetOne(id);

        if (existingIngredient == null)
        {
            throw new KeyNotFoundException($"Ingredient with ID: {id} not found.");
        }

        await _ingredientRepository.Delete(id);
    }

    public Task<IReadOnlyList<Ingredient>> GetAllIngredients()
    {
        return _ingredientRepository.GetAll();
    }

    public Task<Ingredient?> GetIngredient(int id)
    {
        return _ingredientRepository.GetOne(id);
    }

    public Task<Ingredient?> GetIngredient(string name)
    {
        return _ingredientRepository.GetOne(name);
    }

    public Task<Ingredient?> GetIngredientWithAllDetails(int id)
    {
        return _ingredientRepository.GetOneWithAllDetails(id);
    }

    public Task<Ingredient?> GetIngredientWithAllDetails(string name)
    {
        return _ingredientRepository.GetOneWithAllDetails(name);
    }

    public async Task<Ingredient> UpdateIngredient(Ingredient ingredient, int id)
    {
        if (ingredient == null)
        {
            throw new ArgumentNullException(nameof(ingredient), "Ingredient cannot be null.");
        }

        ValidateIngredient(ingredient);

        Ingredient? existingIngredient = await _ingredientRepository.GetOne(id);

        if (existingIngredient == null)
        {
            throw new KeyNotFoundException($"Ingredient with ID: {id} not found.");
        }

        await EnsureAllRelatedRecipesExist(ingredient);

        return await _ingredientRepository.Update(ingredient, id);
    }

    private void ValidateIngredient(Ingredient ingredient)
    {
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

    private async Task EnsureAllRelatedRecipesExist(Ingredient ingredient)
    {
        foreach (Recipe recipe in ingredient.Recipes)
        {
            Recipe? existingRecipe = await _recipeRepository.GetOne(recipe.ID);

            if (existingRecipe == null)
            {
                throw new KeyNotFoundException(nameof(recipe));
            }
        }
    }
}
