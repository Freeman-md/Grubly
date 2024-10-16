using System;
using Grubly.Models;
using Grubly.Tests.Unit.Builders;
using Moq;
using NuGet.Packaging.Signing;

namespace Grubly.Tests.Services;

public partial class CategoryServiceTests
{
    [Fact]
    public async Task GetCategory_ById_ValidId_ReturnsCategory()
    {
        #region Arrange

        var category = new CategoryBuilder().WithId(1).Build();

        _mockCategoryRepository.Setup(repo => repo.GetOne(category.ID))
                      .ReturnsAsync(category);
        #endregion

        #region Act
        var result = await _categoryService.GetCategory(category.ID);
        #endregion

        #region Assert
        Assert.NotNull(result);
        Assert.Equal(category.ID, result.ID);
        _mockCategoryRepository.Verify(repo => repo.GetOne(category.ID), Times.Once);
        #endregion
    }

    [Fact]
    public async Task GetCategory_ById_ValidId_ReturnsCategoryWithDetails()
    {
        #region Arrange

        List<Recipe> recipes = RecipeBuilder.BuildMany(2);

        Category category = new CategoryBuilder().WithId(1).WithRecipes(recipes.ToArray()).Build();

        _mockCategoryRepository.Setup(repo => repo.GetOneWithAllDetails(category.ID))
                      .ReturnsAsync(category);
        #endregion

        #region Act
        var result = await _categoryService.GetCategoryWithAllDetails(category.ID);
        #endregion

        #region Assert
        Assert.NotNull(result);
        Assert.Equal(category.ID, result.ID);
        _mockCategoryRepository.Verify(repo => repo.GetOneWithAllDetails(category.ID), Times.Once);
        #endregion
    }

    [Fact]
    public async Task GetCategory_ById_InvalidId_ReturnsNull()
    {
        #region Arrange
        var invalidId = 99;
        _mockCategoryRepository.Setup(repo => repo.GetOne(invalidId))
                                 .ReturnsAsync((Category)null);
        #endregion

        #region Act
        var result = await _categoryService.GetCategory(invalidId);
        #endregion

        #region Assert
        Assert.Null(result);
        _mockCategoryRepository.Verify(repo => repo.GetOne(invalidId), Times.Once);
        #endregion
    }

    [Fact]
    public async Task GetCategoryByName_ValidName_ReturnsCategory()
    {
        #region Arrange
        var category = new CategoryBuilder().Build();

        _mockCategoryRepository.Setup(repo => repo.GetOneWithAllDetails(category.Name))
                            .ReturnsAsync((category));
        #endregion

        #region Act
        var result = await _categoryService.GetCategoryWithAllDetails(category.Name);
        #endregion

        #region Assert
        Assert.NotNull(result);
        Assert.Equal(category.Name, result.Name);
        _mockCategoryRepository.Verify(repo => repo.GetOneWithAllDetails(category.Name), Times.Once);
        #endregion
    }

    [Fact]
    public async Task GetCategoryByName_ValidName_ReturnsCategoryWithDetails()
    {
        #region Arrange
        List<Recipe> recipes = RecipeBuilder.BuildMany(2);

        Category category = new CategoryBuilder().WithRecipes(recipes.ToArray()).Build();

        _mockCategoryRepository.Setup(repo => repo.GetOneWithAllDetails(category.Name))
                            .ReturnsAsync((category));
        #endregion

        #region Act
        var result = await _categoryService.GetCategoryWithAllDetails(category.Name);
        #endregion

        #region Assert
        Assert.NotNull(result);
        Assert.Equal(category.Name, result.Name);
        _mockCategoryRepository.Verify(repo => repo.GetOneWithAllDetails(category.Name), Times.Once);
        #endregion
    }

    [Fact]
    public async Task GetCategoryByName_InvalidName_ReturnsNull()
    {
        #region Arrange
        string invalidName = "Tomato & Garlic";

        _mockCategoryRepository.Setup(repo => repo.GetOne(invalidName))
                            .ReturnsAsync((Category)null);
        #endregion

        #region Act
        var result = await _categoryService.GetCategory(invalidName);
        #endregion

        #region Assert
        Assert.Null(result);
        _mockCategoryRepository.Verify(repo => repo.GetOne(invalidName), Times.Once);
        #endregion
    }

    [Fact]
    public async Task GetAllCategories_ReturnsListOfCategories() {
        #region Arrange
            List<Category> categories = CategoryBuilder.BuildMany(4);

            _mockCategoryRepository.Setup(repo => repo.GetAll())
                            .ReturnsAsync(categories);         
        #endregion

        #region Act
            IReadOnlyCollection<Category> retrievedCategories = await _categoryService.GetAllCategories();
        #endregion

        #region Assert
            Assert.NotNull(retrievedCategories);
            Assert.Equal(categories.Count, retrievedCategories.Count);

            _mockCategoryRepository.Verify(repo => repo.GetAll(), Times.Once);
        #endregion
    }

    [Fact]
    public async Task GetAllCategories_EmptyDatabase_ReturnsEmptyList() {
        #region Arrange
            List<Category> categories = new List<Category>();

            _mockCategoryRepository.Setup(repo => repo.GetAll())
                            .ReturnsAsync(categories);         
        #endregion

        #region Act
            IReadOnlyCollection<Category> retrievedCategories = await _categoryService.GetAllCategories();
        #endregion

        #region Assert
            Assert.NotNull(retrievedCategories);
            Assert.Empty(retrievedCategories);

            _mockCategoryRepository.Verify(repo => repo.GetAll(), Times.Once);
        #endregion
    }


}
