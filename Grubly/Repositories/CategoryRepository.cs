using System;
using Grubly.Data;
using Grubly.Interfaces.Repositories;
using Grubly.Models;
using Microsoft.EntityFrameworkCore;

namespace Grubly.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly GrublyContext _grublyContext;

    public CategoryRepository(GrublyContext grublyContext)
    {
        _grublyContext = grublyContext;
    }

    public async Task<Category> Create(Category category)
    {
        if (category == null)
        {
            throw new ArgumentNullException(nameof(category));
        }

        var newCategory = new Category {
            Name = category.Name,
        };

        UpdateRecipes(newCategory, category);

        await _grublyContext.Categories.AddAsync(newCategory);
        await _grublyContext.SaveChangesAsync();
        return newCategory;
    }

    public async Task Delete(int id)
    {
        Category? existingCategory = await GetOne(id);

        existingCategory!.Recipes.Clear();

        _grublyContext.Remove(existingCategory);
        await _grublyContext.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<Category>> GetAll()
    {
        return await _grublyContext.Categories.ToListAsync();
    }

    public async Task<Category?> GetOne(int id)
    {
        return await _grublyContext.Categories.FindAsync(id);
    }

    public async Task<Category?> GetOne(string name)
    {
        return await _grublyContext.Categories.FirstOrDefaultAsync(category => category.Name == name);
    }

    public async Task<Category?> GetOneWithAllDetails(int id)
    {
        return await _grublyContext.Categories
                            .Include(category => category.Recipes)
                            .FirstOrDefaultAsync(category => category.ID == id);
    }

    public async Task<Category?> GetOneWithAllDetails(string name)
    {
        return await _grublyContext.Categories
                            .Include(category => category.Recipes)
                            .FirstOrDefaultAsync(category => category.Name == name);
    }

    public async Task<Category> Update(Category category, int id)
    {
        Category? existingCategory = await GetOneWithAllDetails(id);

        if (category == null)
        {
            throw new ArgumentNullException(nameof(category));
        }

        existingCategory!.Name = category.Name;

        UpdateRecipes(existingCategory, category);

        await _grublyContext.SaveChangesAsync();

        return existingCategory;
    }

    private void UpdateRecipes(Category targetCategory, Category sourceCategory)
    {
        targetCategory.Recipes.Clear(); // Clear existing recipes
        foreach (var recipe in sourceCategory.Recipes)
        {
            // Attach the recipe as unchanged to avoid creating duplicates if it already exists
            _grublyContext.Entry(recipe).State = EntityState.Unchanged;
            targetCategory.Recipes.Add(recipe);
        }
    }
}
