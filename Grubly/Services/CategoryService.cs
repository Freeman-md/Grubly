using System;
using Grubly.Interfaces.Repositories;
using Grubly.Interfaces.Services;
using Grubly.Models;

namespace Grubly.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public Task<Category> CreateCategory(Category category)
    {
        throw new NotImplementedException();
    }

    public void DeleteCategory(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<Category>> GetAllCategories()
    {
        throw new NotImplementedException();
    }

    public Task<Category> GetCategoryById(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Category> UpdateCategory(Category category, int id)
    {
        throw new NotImplementedException();
    }
}
