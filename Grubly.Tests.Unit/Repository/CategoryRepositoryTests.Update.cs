using System;
using System.ComponentModel.DataAnnotations;
using Grubly.Models;
using Grubly.Tests.Unit.Builders;
using Microsoft.EntityFrameworkCore;

namespace Grubly.Tests.Unit.Repository;

public partial class CategoryRepositoryTests
{
    [Theory]
    [InlineData("Updated Name")]
    public async Task UpdateCategory_ValidEntity_UpdatesCategoryInDatabase(string name)
    {
        var (categoryRepository, dbContext) = CreateScope();
        
        #region Arrange
        Category savedCategory = await categoryRepository.Create(new CategoryBuilder().Build());

        dbContext.Entry(savedCategory).State = EntityState.Detached;

        savedCategory.Name = name;
        #endregion

        #region Act
        Category updatedCategory = await categoryRepository.Update(savedCategory, savedCategory.ID);
        #endregion

        #region Assert
        Category? retrievedCategory = await categoryRepository.GetOne(updatedCategory.ID);

        Assert.NotNull(retrievedCategory);
        Assert.True(updatedCategory.ID > 0, "The Category ID should be greater than 0 after saving to the database.");

        Assert.Equal(updatedCategory.Name, retrievedCategory!.Name);
        #endregion
    }

    [Theory]
    [InlineData(null)]
    public async Task UpdateCategory_InvalidInputs_ThrowsDbUpdateException(string name)
    {
        var (categoryRepository, dbContext) = CreateScope();
        
        #region Arrange
        Category savedCategory = await categoryRepository.Create(new CategoryBuilder().Build());

        dbContext.Entry(savedCategory).State = EntityState.Detached;

        savedCategory.Name = name;
        #endregion

        #region Act & Assert
        await Assert.ThrowsAsync<DbUpdateException>(() => categoryRepository.Update(savedCategory, savedCategory.ID));
        #endregion
    }

    [Fact]
    public async Task UpdateCategory_InvalidId_ThrowsKeyNotFoundException()
    {
        var (categoryRepository, dbContext) = CreateScope();
        
        #region Arrange
        const int RANDOM_ID = 82923;
        Category savedCategory = await categoryRepository.Create(new CategoryBuilder().Build());

        dbContext.Entry(savedCategory).State = EntityState.Detached;
        #endregion

        #region Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async () => await categoryRepository.Update(savedCategory, RANDOM_ID));
        #endregion
    }

    [Fact]
    public async Task UpdateCategory_DatabaseIntegrity_MaintainsRelationships()
    {
        var (categoryRepository, dbContext) = CreateScope();
        
        #region Arrange
        const int NUMBER_OF_CREATED_RECIPES = 3;
        const string UPDATED_NAME = "Updated Category Name";
        List<Recipe> recipes = RecipeBuilder.BuildMany(NUMBER_OF_CREATED_RECIPES);

        dbContext.Recipes.AddRange(recipes);
        await dbContext.SaveChangesAsync();

        Category originalCategory = new CategoryBuilder()
            .WithName("Original Category")
            .WithRecipes(recipes.ToArray())
            .Build();

        Category savedCategory = await categoryRepository.Create(originalCategory);

        dbContext.Entry(savedCategory).State = EntityState.Detached;

        savedCategory.Name = UPDATED_NAME;
        #endregion

        #region Act
        await categoryRepository.Update(savedCategory, savedCategory.ID);

        Category? updatedCategory = await categoryRepository.GetOneWithAllDetails(savedCategory.ID);
        #endregion

        #region Assert
        Assert.NotNull(updatedCategory);
        Assert.True(updatedCategory.ID > 0, "The Category ID should be greater than 0 after saving to the database.");
        Assert.Equal(UPDATED_NAME, updatedCategory!.Name);

        Assert.Equal(recipes.Count, updatedCategory.Recipes!.Count);
        #endregion
    }
}
