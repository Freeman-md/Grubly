using System;
using System.ComponentModel.DataAnnotations;
using Grubly.Models;
using Grubly.Services;
using Grubly.Tests.Unit.Builders;
using Microsoft.EntityFrameworkCore;
using Moq;
using NuGet.Packaging;

namespace Grubly.Tests.Services;

public partial class CategoryServiceTests
{
    [Fact]
    public async Task UpdateCategory_ValidInput_UpdatesCategorySuccessfully()
    {

        #region Arrange
        Category updatedCategory = new CategoryBuilder()
                                    .WithId(1)
                                    .Build();

        _mockCategoryRepository.Setup(repo => repo.Update(updatedCategory, updatedCategory.ID))
                       .ReturnsAsync(updatedCategory);
        #endregion

        #region Act
        var result = await _categoryService.UpdateCategory(updatedCategory, updatedCategory.ID);

        #endregion

        #region Assert
        Assert.NotNull(result);
        Assert.Equal(updatedCategory.Name, result.Name);
        _mockCategoryRepository.Verify(repo => repo.Update(updatedCategory, updatedCategory.ID), Times.Once);
        #endregion
    }

    [Fact]
    public async Task UpdateCategory_NullInput_ThrowsArgumentNullException()
    {
        #region Act -> Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _categoryService.UpdateCategory(null!, 1));
        #endregion
    }

    [Fact]
    public async Task UpdateCategory_InvalidId_ThrowsKeyNotFoundException()
    {
        #region Arrange
        var invalidId = 999;
        var categoryToUpdate = new Category { ID = invalidId, Name = "Non-existent Category" };

        _mockCategoryRepository.Setup(repo => repo.Update(categoryToUpdate, invalidId))
                   .ThrowsAsync(new KeyNotFoundException());
        #endregion

        #region Act -> Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _categoryService.UpdateCategory(categoryToUpdate, invalidId));
        #endregion
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("A very long name that exceeds the maximum allowed length for the category name which is 50 chars.")]
    [InlineData(" ")]
    public async Task UpdateCategory_InvalidInput_ThrowsValidationException(string name)
    {
        #region Arrange
        Category category = new CategoryBuilder()
                                .WithName(name)
                                .Build();
        #endregion

        #region Act -> Assert
        await Assert.ThrowsAsync<ValidationException>(() => _categoryService.UpdateCategory(category, category.ID));
        #endregion
    }

    [Fact]
    public async Task UpdateCategory_WithNonExistingRecipes_ThrowsKeyNotFoundException()
    {
        #region Arrange
        List<Recipe> recipes = RecipeBuilder.BuildMany(2); // Building two sample recipes
        Category categoryToUpdate = new CategoryBuilder()
                                           .WithRecipes(recipes.ToArray()) // Attach the recipes
                                           .Build();

        // Simulate that one of the recipes doesn't exist by returning null
        _mockRecipeRepository.Setup(repo => repo.GetOne(It.IsAny<int>()))
                             .ReturnsAsync((Recipe)null);
        #endregion

        #region Act -> Assert
        // Assert that the service throws a KeyNotFoundException when trying to update an category with non-existing recipes
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _categoryService.UpdateCategory(categoryToUpdate, categoryToUpdate.ID));

        // Verify that the repository method to get the recipe is called once
        _mockRecipeRepository.Verify(repo => repo.GetOne(It.IsAny<int>()), Times.Once);
        #endregion
    }


}
