using System;
using Grubly.Data;
using Grubly.Interfaces.Repositories;
using Grubly.Models;
using Microsoft.EntityFrameworkCore;

namespace Grubly.Repositories;

public class RecipeRepository : IRecipeRepository
{
    private readonly GrublyContext _grublycontext;

    public RecipeRepository(GrublyContext grublyContext)
    {
        _grublycontext = grublyContext;
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

        await _grublycontext.Recipes.AddAsync(newRecipe);
        await _grublycontext.SaveChangesAsync();

        return newRecipe;
    }

    public async Task Delete(int id)
    {
        Recipe? existingRecipe = await GetOne(id);

        if (existingRecipe == null)
        {
            throw new KeyNotFoundException($"Recipe with ID: {id} not found.");
        }

        existingRecipe.Categories.Clear();
        existingRecipe.Ingredients.Clear();

        _grublycontext.Recipes.Remove(existingRecipe);
        await _grublycontext.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<Recipe>> GetAll()
    {
        return await _grublycontext.Recipes.ToListAsync();
    }

    public async Task<Recipe?> GetOne(int id)
    {
        return await _grublycontext.Recipes.FindAsync(id);
    }

    public async Task<Recipe?> GetOne(string title)
    {
        return await _grublycontext.Recipes.FirstOrDefaultAsync(recipe => recipe.Title == title);
    }

    public async Task<Recipe?> GetOneWithAllDetails(int id)
    {
        return await _grublycontext.Recipes
                    .Include(recipe => recipe.Categories)
                    .Include(recipe => recipe.Ingredients)
                    .FirstOrDefaultAsync(recipe => recipe.ID == id);
    }

    public async Task<Recipe?> GetOneWithAllDetails(string title)
    {
        return await _grublycontext.Recipes
                    .Include(recipe => recipe.Categories)
                    .Include(recipe => recipe.Ingredients)
                    .FirstOrDefaultAsync(recipe => recipe.Title == title);
    }

    public async Task<Recipe> Update(Recipe recipe, int id)
    {
        Recipe? existingRecipe = await GetOneWithAllDetails(id);

        if (existingRecipe == null)
        {
            throw new KeyNotFoundException($"Recipe with ID: {id} not found.");
        }

        if (recipe == null)
        {
            throw new ArgumentNullException(nameof(recipe));
        }

        // Update the fields
        existingRecipe.Title = recipe.Title;
        existingRecipe.Description = recipe.Description;
        existingRecipe.Instructions = recipe.Instructions;
        existingRecipe.CuisineType = recipe.CuisineType;
        existingRecipe.DifficultyLevel = recipe.DifficultyLevel;
        existingRecipe.ImageUrl = recipe.ImageUrl;

        // Handle relationships for Categories and Ingredients
        UpdateCategoriesAndIngredients(existingRecipe, recipe);

        // Save changes
        await _grublycontext.SaveChangesAsync();

        return existingRecipe;
    }

    private void UpdateCategoriesAndIngredients(Recipe targetRecipe, Recipe sourceRecipe)
    {
        targetRecipe.Ingredients.Clear(); // Clear existing ingredients
        foreach (var ingredient in sourceRecipe.Ingredients)
        {
            // Attach the ingredient as unchanged to avoid creating duplicates if it already exists
            _grublycontext.Entry(ingredient).State = EntityState.Unchanged;
            targetRecipe.Ingredients.Add(ingredient);
        }

        targetRecipe.Categories.Clear(); // Clear existing categories
        foreach (var category in sourceRecipe.Categories)
        {
            // Attach the category as unchanged to avoid creating duplicates if it already exists
            _grublycontext.Entry(category).State = EntityState.Unchanged;
            targetRecipe.Categories.Add(category);
        }
    }
}
