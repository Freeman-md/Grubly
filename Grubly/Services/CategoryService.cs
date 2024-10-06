using System;
using System.ComponentModel.DataAnnotations;
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

    public async Task<Category> CreateCategory(Category category)
    {
        if (category == null)
        {
            throw new ArgumentNullException(nameof(category), "Ingredient cannot be null.");
        }

        ValidateCategory(category);

        await EnsureAllRelatedRecipesExist(category);

        return await _categoryRepository.Create(category);
    }

    public async Task DeleteCategory(int id)
    {
        Category? existingCategory = await _categoryRepository.GetOne(id);

        if (existingCategory == null)
        {
            throw new KeyNotFoundException($"Category with ID: {id} not found.");
        }

        await _categoryRepository.Delete(id);
    }

    public async Task<IReadOnlyList<Category>> GetAllCategories()
    {
        return await _categoryRepository.GetAll();
    }

    public async Task<Category?> GetCategory(int id)
    {
        return await _categoryRepository.GetOne(id);
    }

    public async Task<Category?> GetCategory(string name)
    {
        return await _categoryRepository.GetOne(name);
    }

    public async Task<Category?> GetCategoryWithAllDetails(int id)
    {
        return await _categoryRepository.GetOneWithAllDetails(id);
    }

    public async Task<Category?> GetCategoryWithAllDetails(string name)
    {
        return await _categoryRepository.GetOneWithAllDetails(name);
    }

    public async Task<Category> UpdateCategory(Category category, int id)
    {
        if (category == null)
        {
            throw new ArgumentNullException(nameof(category), "Category cannot be null.");
        }

        ValidateCategory(category);

        Category? existingCategory = await _categoryRepository.GetOne(id);

        if (existingCategory == null)
        {
            throw new KeyNotFoundException($"Category with ID: {id} not found.");
        }

        await EnsureAllRelatedRecipesExist(category);

        return await _categoryRepository.Update(category, id);
    }

    private void ValidateCategory(Category category)
    {
        if (string.IsNullOrWhiteSpace(category.Name))
        {
            throw new ValidationException("Category name cannot be null, empty, or whitespace.");
        }

        if (category.Name.Length > 50)
        {
            throw new ValidationException("Category name cannot exceed 50 characters.");
        }
    }

    private async Task EnsureAllRelatedRecipesExist(Category category)
    {
        foreach (Recipe recipe in category.Recipes)
        {
            Recipe? existingRecipe = await _recipeRepository.GetOne(recipe.ID);

            if (existingRecipe == null)
            {
                throw new KeyNotFoundException(nameof(recipe));
            }
        }
    }
}
