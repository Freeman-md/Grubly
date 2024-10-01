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
        if (category == null) {
            throw new ArgumentNullException(nameof(category));
        }

        foreach (Recipe recipe in category.Recipes) {
            _grublyContext.Entry(recipe).State = EntityState.Unchanged;
        }

        await _grublyContext.Categories.AddAsync(category);
        await _grublyContext.SaveChangesAsync();
        return category;
    }

    public Task Delete(int id)
    {
        throw new NotImplementedException();
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

    public Task<Category> Update(Category category, int id)
    {
        throw new NotImplementedException();
    }
}
