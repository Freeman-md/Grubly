using System;
using Grubly.Data;
using Grubly.Interfaces.Repositories;
using Grubly.Models;
using Microsoft.EntityFrameworkCore;

namespace Grubly.Repositories;

public class RecipeRepository : IRecipeRepository
{
    private readonly GrublyContext _grublyContext;

    public RecipeRepository(GrublyContext grublyContext)
    {
        _grublyContext = grublyContext;
    }

    public async Task<Recipe> Create(Recipe recipe)
    {
        if (recipe == null)
        {
            throw new ArgumentNullException(nameof(recipe));
        }

        var newRecipe = new Recipe
        {
            Title = recipe.Title,
            Description = recipe.Description,
            Instructions = recipe.Instructions,
            CuisineType = recipe.CuisineType,
            DifficultyLevel = recipe.DifficultyLevel,
            ImageUrl = recipe.ImageUrl
        };

        UpdateCategoriesAndIngredients(newRecipe, recipe);

        await _grublyContext.Recipes.AddAsync(newRecipe);
        await _grublyContext.SaveChangesAsync();

        return newRecipe;
    }

    public async Task Delete(int id)
    {
        Recipe? existingRecipe = await GetOne(id);

        existingRecipe!.Categories.Clear();
        existingRecipe.Ingredients.Clear();

        _grublyContext.Recipes.Remove(existingRecipe);
        await _grublyContext.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<Recipe>> GetAll()
    {
        return await _grublyContext.Recipes.ToListAsync();
    }

    public async Task<Recipe?> GetOne(int id)
    {
        return await _grublyContext.Recipes.FindAsync(id);
    }

    public async Task<Recipe?> GetOne(string title)
    {
        return await _grublyContext.Recipes.FirstOrDefaultAsync(recipe => recipe.Title == title);
    }

    public async Task<Recipe?> GetOneWithAllDetails(int id)
    {
        return await _grublyContext.Recipes
                    .Include(recipe => recipe.Categories)
                    .Include(recipe => recipe.Ingredients)
                    .FirstOrDefaultAsync(recipe => recipe.ID == id);
    }

    public async Task<Recipe?> GetOneWithAllDetails(string title)
    {
        return await _grublyContext.Recipes
                    .Include(recipe => recipe.Categories)
                    .Include(recipe => recipe.Ingredients)
                    .FirstOrDefaultAsync(recipe => recipe.Title == title);
    }

    public async Task<Recipe> Update(Recipe recipe, int id)
    {
        Recipe? existingRecipe = await GetOneWithAllDetails(id);

        if (recipe == null)
        {
            throw new ArgumentNullException(nameof(recipe));
        }

        // Update the fields
        existingRecipe!.Title = recipe.Title;
        existingRecipe.Description = recipe.Description;
        existingRecipe.Instructions = recipe.Instructions;
        existingRecipe.CuisineType = recipe.CuisineType;
        existingRecipe.DifficultyLevel = recipe.DifficultyLevel;
        existingRecipe.ImageUrl = recipe.ImageUrl;

        // Handle relationships for Categories and Ingredients
        UpdateCategoriesAndIngredients(existingRecipe, recipe);

        // Save changes
        await _grublyContext.SaveChangesAsync();

        return existingRecipe;
    }

    private void UpdateCategoriesAndIngredients(Recipe targetRecipe, Recipe sourceRecipe)
    {
        targetRecipe.Ingredients.Clear(); // Clear existing ingredients
        foreach (var ingredient in sourceRecipe.Ingredients)
        {
            // Attach the ingredient as unchanged to avoid creating duplicates if it already exists
            _grublyContext.Entry(ingredient).State = EntityState.Unchanged;
            targetRecipe.Ingredients.Add(ingredient);
        }

        targetRecipe.Categories.Clear(); // Clear existing categories
        foreach (var category in sourceRecipe.Categories)
        {
            // Attach the category as unchanged to avoid creating duplicates if it already exists
            _grublyContext.Entry(category).State = EntityState.Unchanged;
            targetRecipe.Categories.Add(category);
        }
    }
}
