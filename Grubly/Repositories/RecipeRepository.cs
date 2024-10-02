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

        PersistCategoriesAndIngredients(recipe);

        await _grublycontext.Recipes.AddAsync(recipe);
        await _grublycontext.SaveChangesAsync();

        return recipe;
    }

    public async Task Delete(int id)
    {
        Recipe? exisitingRecipe = await GetOne(id);

        if (exisitingRecipe == null) {
            throw new KeyNotFoundException($"Recipe with ID: {id} not found.");
        }

        exisitingRecipe.Categories.Clear();
        exisitingRecipe.Ingredients.Clear();

        _grublycontext.Recipes.Remove(exisitingRecipe);
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

    public Task<Recipe> Update(Recipe recipe, int id)
    {
        throw new NotImplementedException();
    }

    private void PersistCategoriesAndIngredients(Recipe recipe)
    {
        foreach (Category category in recipe.Categories)
        {
            _grublycontext.Entry(category).State = EntityState.Unchanged;
        }

        foreach (Ingredient ingredient in recipe.Ingredients)
        {
            _grublycontext.Entry(ingredient).State = EntityState.Unchanged;
        }
    }
}
