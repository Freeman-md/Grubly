using System;
using Grubly.Interfaces.Repositories;
using Grubly.Interfaces.Services;
using Grubly.Models;

namespace Grubly.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    private readonly IRecipeRepository _recipeRepository;

    public CategoryService(ICategoryRepository categoryRepository, IRecipeRepository recipeRepository)
    {
        _categoryRepository = categoryRepository;
        _recipeRepository = recipeRepository;
    }

    public Task<Category> CreateCategory(Category category)
    {
        throw new NotImplementedException();
    }

    public Task DeleteCategory(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<Category>> GetAllCategories()
    {
        throw new NotImplementedException();
    }

    public Task<Category?> GetCategory(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Category?> GetCategory(string name)
    {
        throw new NotImplementedException();
    }

    public Task<Category?> GetCategoryWithAllDetails(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Category?> GetCategoryWithAllDetails(string name)
    {
        throw new NotImplementedException();
    }

    public Task<Category> UpdateCategory(Category category, int id)
    {
        throw new NotImplementedException();
    }
}
