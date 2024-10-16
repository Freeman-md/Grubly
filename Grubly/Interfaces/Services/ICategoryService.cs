using System;
using Grubly.Models;

namespace Grubly.Interfaces.Services;

public interface ICategoryService
{
public Task<IReadOnlyList<Category>> GetAllCategories();
    public Task<Category?> GetCategory(int id);
    public Task<Category?> GetCategory(string name);
    public Task<Category?> GetCategoryWithAllDetails(int id);
    public Task<Category?> GetCategoryWithAllDetails(string name);
    public Task<Category> CreateCategory(Category category);
    public Task<Category> UpdateCategory(Category category, int id);
    public Task DeleteCategory(int id);
}
