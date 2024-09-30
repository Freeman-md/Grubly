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
        Ingredient ingredient = await this.GetOne(id);

        if (ingredient == null)
        {
            throw new KeyNotFoundException($"Ingredient with ID {id} not found.");
        }

        _grublyContext.Ingredients.Remove(ingredient);

        await _grublyContext.SaveChangesAsync();
    }

    public Task<IReadOnlyList<Ingredient>> GetAll()
    {
        throw new NotImplementedException();
    }

    public async Task<Ingredient?> GetOne(int id)
    {
        return await _grublyContext.Ingredients.FindAsync(id);
    }

    public async Task<Ingredient?> GetOne(string name)
    {
        return await _grublyContext.Ingredients.FirstOrDefaultAsync(ingredient => ingredient.Name == name);
    }

    public Task<Ingredient?> GetOneWithAllDetails(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Ingredient?> GetOneWithAllDetails(string name)
    {
        throw new NotImplementedException();
    }

    public Task<Ingredient> Update(Ingredient ingredient, int id)
    {
        throw new NotImplementedException();
    }
}
