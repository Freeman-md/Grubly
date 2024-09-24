using System;
using Grubly.Models;

namespace Grubly.Interfaces.Services;

public interface ICategoryService
{
public Task<IReadOnlyList<Category>> GetAllCategories();
    public Task<Category> GetCategoryById(int id);
    public Task<Category> CreateCategory(Category category);
    public Task<Category> UpdateCategory(Category category, int id);
    public void DeleteCategory(int id);
}
