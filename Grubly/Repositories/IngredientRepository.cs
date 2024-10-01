using System;
using Grubly.Data;
using Grubly.Interfaces.Repositories;
using Grubly.Models;
using Microsoft.EntityFrameworkCore;

namespace Grubly.Repositories;

public class IngredientRepository : IIngredientRepository
{
    private readonly GrublyContext _grublyContext;

    public IngredientRepository(GrublyContext grublyContext)
    {
        _grublyContext = grublyContext;
    }

    public async Task<Ingredient> Create(Ingredient ingredient)
    {
        if (ingredient == null)
        {
            throw new ArgumentNullException(nameof(ingredient));
        }

        foreach (var recipe in ingredient.Recipes)
        {
            _grublyContext.Entry(recipe).State = EntityState.Unchanged;
        }

        await _grublyContext.Ingredients.AddAsync(ingredient);
        await _grublyContext.SaveChangesAsync();
        return ingredient;
    }

    public async Task Delete(int id)
    {
        Ingredient? ingredient = await this.GetOne(id);

        if (ingredient == null)
        {
            throw new KeyNotFoundException($"Ingredient with ID {id} not found.");
        }

        ingredient.Recipes.Clear();

        _grublyContext.Ingredients.Remove(ingredient);

        await _grublyContext.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<Ingredient>> GetAll()
    {
        return await _grublyContext.Ingredients.ToListAsync();
    }

    public async Task<Ingredient?> GetOne(int id)
    {
        return await _grublyContext.Ingredients.FindAsync(id);
    }

    public async Task<Ingredient?> GetOne(string name)
    {
        return await _grublyContext.Ingredients.FirstOrDefaultAsync(ingredient => ingredient.Name == name);
    }

    public async Task<Ingredient?> GetOneWithAllDetails(int id)
    {
        return await _grublyContext.Ingredients
            .Include(i => i.Recipes)
            .FirstOrDefaultAsync(i => i.ID == id);
    }

    public async Task<Ingredient?> GetOneWithAllDetails(string name)
    {
        return await _grublyContext.Ingredients
           .Include(i => i.Recipes)
           .FirstOrDefaultAsync(i => i.Name == name);
    }

    public async Task<Ingredient> Update(Ingredient ingredient, int id)
    {
        // Retrieve the existing ingredient
        Ingredient? existingIngredient = await this.GetOne(id);

        if (existingIngredient == null)
        {
            throw new KeyNotFoundException($"Ingredient with ID {id} not found.");
        }

        // Update basic fields
        existingIngredient.Name = ingredient.Name;
        existingIngredient.Description = ingredient.Description;

        foreach (var recipe in ingredient.Recipes)
        {
            _grublyContext.Entry(recipe).State = EntityState.Unchanged;
        }

        // Save changes to the database
        await _grublyContext.SaveChangesAsync();

        return existingIngredient;
    }

}
