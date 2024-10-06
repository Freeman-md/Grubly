using System;
using System.ComponentModel.DataAnnotations;
using Grubly.Interfaces.Repositories;
using Grubly.Interfaces.Services;
using Grubly.Models;

namespace Grubly.Services;

public class RecipeService : IRecipeService
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IIngredientRepository _ingredientRepository;
    private readonly ICategoryRepository _categoryRepository;

    public RecipeService(IRecipeRepository recipeRepository, IIngredientRepository ingredientRepository, ICategoryRepository categoryRepository)
    {
        _ingredientRepository = ingredientRepository;
        _categoryRepository = categoryRepository;
        _recipeRepository = recipeRepository;
    }

    public async Task<Recipe> CreateRecipe(Recipe recipe)
    {
        if (recipe == null)
        {
            throw new ArgumentNullException(nameof(recipe));
        }

        ValidateRecipe(recipe);

        await EnsureAllRelatedIngredientsAndCategoriesExist(recipe);

        return await _recipeRepository.Create(recipe);
    }

    public async Task DeleteRecipe(int id)
    {
        Recipe? existingRecipe = await _recipeRepository.GetOne(id);

        if (existingRecipe == null)
        {
            throw new KeyNotFoundException($"Recipe with ID: {id} not found.");
        }

        await _recipeRepository.Delete(id);
    }

    public Task<IReadOnlyList<Recipe>> GetAllRecipes()
    {
        return _recipeRepository.GetAll();
    }

    public async Task<Recipe?> GetRecipe(int id)
    {
        return await _recipeRepository.GetOne(id);
    }

    public async Task<Recipe?> GetRecipe(string title)
    {
        return await _recipeRepository.GetOne(title);
    }

    public async Task<Recipe?> GetRecipeWithAllDetails(int id)
    {
        return await _recipeRepository.GetOneWithAllDetails(id);
    }

    public async Task<Recipe?> GetRecipeWithAllDetails(string title)
    {
        return await _recipeRepository.GetOneWithAllDetails(title);
    }

    public async Task<Recipe> UpdateRecipe(Recipe recipe, int id)
    {
        if (recipe == null)
        {
            throw new ArgumentNullException(nameof(recipe));
        }

        ValidateRecipe(recipe);

        await EnsureAllRelatedIngredientsAndCategoriesExist(recipe);

        return await _recipeRepository.Update(recipe, id);
    }

    private void ValidateRecipe(Recipe recipe)
    {
        // Validate Title
        if (string.IsNullOrWhiteSpace(recipe.Title))
        {
            throw new ValidationException("Recipe title cannot be null or empty.");
        }

        if (recipe.Title.Length > 50)
        {
            throw new ValidationException("Recipe title cannot exceed 50 characters.");
        }

        // Validate Description
        if (string.IsNullOrWhiteSpace(recipe.Description))
        {
            throw new ValidationException("Recipe description cannot be null or empty.");
        }

        if (recipe.Description.Length > 50)
        {
            throw new ValidationException("Recipe description cannot exceed 50 characters.");
        }

        // Validate CuisineType
        if (!Enum.IsDefined(typeof(CuisineType), recipe.CuisineType))
        {
            throw new ValidationException("Invalid Cuisine Type.");
        }

        // Validate DifficultyLevel
        if (!Enum.IsDefined(typeof(DifficultyLevel), recipe.DifficultyLevel))
        {
            throw new ValidationException("Invalid Difficulty Level.");
        }

        // Optionally validate ImageUrl if required
        if (!string.IsNullOrEmpty(recipe.ImageUrl) && recipe.ImageUrl.Length > 200)
        {
            throw new ValidationException("Image URL cannot exceed 200 characters.");
        }

        // Validate Instructions (if provided)
        if (!string.IsNullOrEmpty(recipe.Instructions) && recipe.Instructions.Length > 500)
        {
            throw new ValidationException("Recipe instructions cannot exceed 500 characters.");
        }
    }

    private async Task EnsureAllRelatedIngredientsAndCategoriesExist(Recipe recipe)
    {
        foreach (Ingredient ingredient in recipe.Ingredients)
        {
            Ingredient? existingIngredient = await _ingredientRepository.GetOne(ingredient.ID);

            if (existingIngredient == null)
            {
                throw new KeyNotFoundException(nameof(ingredient));
            }
        }

        foreach (Category category in recipe.Categories)
        {
            Category? existingIngredient = await _categoryRepository.GetOne(category.ID);

            if (existingIngredient == null)
            {
                throw new KeyNotFoundException(nameof(category));
            }
        }
    }
}
