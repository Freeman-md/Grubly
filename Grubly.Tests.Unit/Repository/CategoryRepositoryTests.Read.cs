using System;
using Grubly.Models;
using Grubly.Tests.Unit.Builders;

namespace Grubly.Tests.Unit.Repository;

public partial class CategoryRepositoryTests
{
[Fact]
    public async Task GetCategoryById_ValidId_ReturnsCorrectCategory()
    {
        #region Arrange
        Category unSavedCategory = new CategoryBuilder().Build();
        Category savedCategory = await _categoryRepository.Create(unSavedCategory);
        #endregion

        #region Act
        Category? retrievedCategory = await _categoryRepository.GetOne(savedCategory.ID);
        #endregion

        #region Assert
        Assert.NotNull(retrievedCategory);
        Assert.Equal(savedCategory.Name, retrievedCategory.Name);
        #endregion
    }

    [Fact]
    public async Task GetCategoryByName_ValidName_ReturnsCorrectCategory()
    {
        #region Arrange
        Category unSavedCategory = new CategoryBuilder().Build();
        Category savedCategory = await _categoryRepository.Create(unSavedCategory);
        #endregion

        #region Act
        Category? retrievedCategory = await _categoryRepository.GetOne(savedCategory.Name);
        #endregion

        #region Assert
        Assert.NotNull(retrievedCategory);
        Assert.Equal(savedCategory.Name, retrievedCategory.Name);
        #endregion
    }

    [Fact]
    public async Task GetCategoryById_InvalidId_ReturnsNull()
    {
        #region Arrange
        Category category = new CategoryBuilder().Build();
        category.ID = 893; // random ID
        #endregion

        #region Act
        Category? nullCategory = await _categoryRepository.GetOne(category.ID);
        #endregion

        #region Assert
        Assert.Null(nullCategory);
        #endregion
    }

    [Fact]
    public async Task GetCategoryByName_InvalidName_ReturnsNull()
    {
        #region Arrange
        Category category = new CategoryBuilder().Build();
        #endregion

        #region Act
        Category? nullCategory = await _categoryRepository.GetOne(category.Name);
        #endregion

        #region Assert
        Assert.Null(nullCategory);
        #endregion
    }

    [Fact]
    public async Task GetAllCategories_ReturnsAllCategories() {
        #region Arrange
            Category[] unSavedCategories = {
                new CategoryBuilder().WithName($"Tomato").Build(),
                new CategoryBuilder().WithName($"Cheese").Build(),
                new CategoryBuilder().WithName($"Raisin").Build()
            };

            foreach (Category category in unSavedCategories) {
                await _categoryRepository.Create(category);
            }
        #endregion

        #region Act
            IReadOnlyList<Category> savedCategories = await _categoryRepository.GetAll();
        #endregion

        #region Assert
            Assert.NotNull(savedCategories);
            Assert.Equal(unSavedCategories.Length, savedCategories.Count);
            Assert.Contains(savedCategories, (category) => category.Name.Equals(unSavedCategories[0].Name));
        #endregion
    }

     [Fact]
    public async Task GetAllCategories_EmptyDatabase_ReturnsEmptyList() {
        #region Act
            IReadOnlyList<Category> noCategories = await _categoryRepository.GetAll();
        #endregion

        #region Assert
            Assert.NotNull(noCategories);
            Assert.Empty(noCategories);
        #endregion
    }
}
