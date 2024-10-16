using System;
using System.ComponentModel.DataAnnotations;
using Grubly.Data;
using Grubly.Interfaces.Repositories;
using Grubly.Interfaces.Services;
using Grubly.Models;
using Grubly.Repositories;
using Grubly.Services;
using Grubly.Tests.Unit.Builders;
using Grubly.Tests.Unit.Fixtures;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow.ValueContentAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Grubly.Tests.Services;

public partial class CategoryServiceTests
{
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly Mock<IRecipeRepository> _mockRecipeRepository;
    private readonly CategoryService _categoryService;

    public CategoryServiceTests()
    {
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _mockRecipeRepository = new Mock<IRecipeRepository>();

        _categoryService = new CategoryService(_mockCategoryRepository.Object, _mockRecipeRepository.Object);
    }

    [Fact]
    public async Task CreateCategory_ValidInput_CreatesCategorySuccessfully()
    {
        #region Arrange
        _mockCategoryRepository.Setup(repo => repo.Create(It.IsAny<Category>()))
                                .ReturnsAsync((Category category) =>
                                {
                                    category.ID = 1;
                                    return category;
                                });
        #endregion

        #region Act
        Category savedCategory = await _categoryService.CreateCategory(new CategoryBuilder().Build());
        #endregion

        #region Assert
        _mockCategoryRepository.Verify(repo => repo.Create(It.IsAny<Category>()), Times.Once);
        Assert.NotNull(savedCategory);
        Assert.True(savedCategory.ID > 0);
        #endregion
    }

    [Fact]
    public async Task CreateCategory_NullInput_ThrowsArgumentNullException()
    {
        #region Act -> Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _categoryService.CreateCategory(null!));
        #endregion
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("A very long name that exceeds the maximum allowed length for the category name which is 50 chars.")]
    [InlineData(" ")]
    public async Task CreateCategory_InvalidInput_ThrowsValidationException(string name)
    {
        #region Arrange
        Category category = new CategoryBuilder()
                                .WithName(name)
                                .Build();
        #endregion

        #region Act -> Assert
        await Assert.ThrowsAsync<ValidationException>(() => _categoryService.CreateCategory(category));
        #endregion
    }

    [Fact]
    public async Task CreateCategory_WithNonExistingRecipes_ThrowsKeyNotFoundException()
    {
        #region Arrange
        List<Recipe> recipes = RecipeBuilder.BuildMany(2);
        Category category = new CategoryBuilder().WithRecipes(recipes.ToArray()).Build();

        _mockRecipeRepository.Setup(repo => repo.GetOne(It.IsAny<int>())).ReturnsAsync((Recipe)null); // simulate non-existing categories by returning null
        #endregion

        #region Act -> Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _categoryService.CreateCategory(category));

        _mockRecipeRepository.Verify(repo => repo.GetOne(It.IsAny<int>()), Times.Once);
        #endregion
    }
}
