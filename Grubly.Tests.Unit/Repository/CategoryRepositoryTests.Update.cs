using System;
using System.ComponentModel.DataAnnotations;
using Grubly.Models;
using Grubly.Tests.Unit.Builders;

namespace Grubly.Tests.Unit.Repository;

public partial class CategoryRepositoryTests
{
    [Theory]
    [InlineData("Updated Name")]
    public async Task UpdateCategory_ValidEntity_UpdatesCategoryInDatabase(string name)
    {
        #region Arrange
        Category savedCategory = await _categoryRepository.Create(new CategoryBuilder().Build());

        savedCategory.Name = name;
        #endregion

        #region Act
        Category updatedCategory = await _categoryRepository.Update(savedCategory, savedCategory.ID);
        #endregion

        #region Assert
        Category? retrievedCategory = await _categoryRepository.GetOne(updatedCategory.ID);

        Assert.NotNull(retrievedCategory);
        Assert.True(updatedCategory.ID > 0, "The Category ID should be greater than 0 after saving to the database.");

        Assert.Equal(updatedCategory.Name, retrievedCategory!.Name);
        #endregion
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task UpdateCategory_InvalidInputs_ThrowsValidationException(string name)
    {
        #region Arrange
        Category savedCategory = await _categoryRepository.Create(new CategoryBuilder().Build());
        savedCategory.Name = name;
        #endregion

        #region Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _categoryRepository.Update(savedCategory, savedCategory.ID));
        #endregion
    }

    [Fact]
    public async Task UpdateCategory_InvalidId_ThrowsNotFoundException()
    {
        //TODO: Create a NotFoundException Class in main project and use here
        #region Arrange
        const int RANDOM_ID = 82923;
        Category savedCategory = await _categoryRepository.Create(new CategoryBuilder().Build());
        #endregion

        #region Assert
        await Assert.ThrowsAsync<Exception>(async () => await _categoryRepository.Update(savedCategory, RANDOM_ID));
        #endregion
    }

    [Fact]
    public async Task UpdateCategory_DatabaseIntegrity_MaintainsRelationships()
    {
        #region Arrange
        const int NUMBER_OF_CREATED_RECIPES = 3;
        const string UPDATED_NAME = "Updated Category Name";
        List<Recipe> recipes = RecipeBuilder.BuildMany(NUMBER_OF_CREATED_RECIPES);

        _dbContext.Recipes.AddRange(recipes);
        await _dbContext.SaveChangesAsync();

        Category originalCategory = new CategoryBuilder()
            .WithName("Original Category")
            .WithRecipes(recipes.ToArray())
            .Build();

        Category savedCategory = await _categoryRepository.Create(originalCategory);

        savedCategory.Name = UPDATED_NAME;
        #endregion

        #region Act
        await _categoryRepository.Update(savedCategory, savedCategory.ID);

        Category? updatedCategory = await _categoryRepository.GetOneWithAllDetails(savedCategory.ID);
        #endregion

        #region Assert
        Assert.NotNull(updatedCategory);
        Assert.True(updatedCategory.ID > 0, "The Category ID should be greater than 0 after saving to the database.");
        Assert.Equal(UPDATED_NAME, updatedCategory!.Name);

        Assert.Equal(recipes.Count, updatedCategory.Recipes!.Count);
        #endregion
    }
}
