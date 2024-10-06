using System;
using System.ComponentModel.DataAnnotations;
using Grubly.Models;
using Grubly.Tests.Unit.Builders;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;

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

    [Fact]
    public async Task UpdateCategory_ReplacesOldRecipesWithNewOnes()
    {
        var (categoryRepository, dbContext) = CreateScope();

        #region Arrange
        // Create initial recipes
        List<Recipe> initialRecipes = RecipeBuilder.BuildMany(2);

        // Create new recipes to replace the old ones
        List<Recipe> newRecipes = RecipeBuilder.BuildMany(2);

        dbContext.Recipes.AddRange(initialRecipes);
        dbContext.Recipes.AddRange(newRecipes);
        await dbContext.SaveChangesAsync();

        // Create and save the original category with initial recipes
        Category originalCategory = new CategoryBuilder()
            .WithName("Original Category")
            .WithRecipes(initialRecipes.ToArray())
            .Build();

        Category savedCategory = await categoryRepository.Create(originalCategory);

        // Detach the savedCategory to stop it from being tracked
        dbContext.Entry(savedCategory).State = EntityState.Detached;

        // Update the category name and provide new recipes
        savedCategory.Name = "Updated Category";
        savedCategory.Recipes = newRecipes; // New recipes
        #endregion

        #region Act
        await categoryRepository.Update(savedCategory, savedCategory.ID);

        // Retrieve the updated category with details (including recipes)
        Category? updatedCategory = await categoryRepository.GetOneWithAllDetails(savedCategory.ID);
        #endregion

        #region Assert
        Assert.NotNull(updatedCategory);
        Assert.Equal("Updated Category", updatedCategory!.Name);

        // Verify that only the new set of recipes is associated with the category
        Assert.Equal(newRecipes.Count, updatedCategory.Recipes!.Count);
        foreach (var recipe in newRecipes)
        {
            Assert.Contains(updatedCategory.Recipes, r => r.Title == recipe.Title);
        }

        // Ensure the old recipes are no longer associated
        foreach (var recipe in initialRecipes)
        {
            Assert.DoesNotContain(updatedCategory.Recipes, r => r.Title == recipe.Title);
        }
        #endregion
    }

    [Fact]
    public async Task UpdateCategory_AddsNewRecipesWhileKeepingOldOnes()
    {
        var (categoryRepository, dbContext) = CreateScope();

        #region Arrange
        // Create initial recipes
        List<Recipe> initialRecipes = RecipeBuilder.BuildMany(2);

        // Create new recipes to add
        List<Recipe> additionalRecipes = RecipeBuilder.BuildMany(2);

        dbContext.Recipes.AddRange(initialRecipes);
        dbContext.Recipes.AddRange(additionalRecipes);
        await dbContext.SaveChangesAsync();

        // Create and save the original category with initial recipes
        Category originalCategory = new CategoryBuilder()
            .WithName("Original Category")
            .WithRecipes(initialRecipes.ToArray())
            .Build();

        Category savedCategory = await categoryRepository.Create(originalCategory);

        // Detach the savedCategory to stop it from being tracked
        dbContext.Entry(savedCategory).State = EntityState.Detached;

        // Update the category name and add new recipes, while keeping the old ones
        savedCategory.Name = "Updated Category";
        savedCategory.Recipes.AddRange(additionalRecipes); // Add new recipes
        #endregion

        #region Act
        await categoryRepository.Update(savedCategory, savedCategory.ID);

        // Retrieve the updated category with details (including recipes)
        Category? updatedCategory = await categoryRepository.GetOneWithAllDetails(savedCategory.ID);
        #endregion

        #region Assert
        Assert.NotNull(updatedCategory);
        Assert.Equal("Updated Category", updatedCategory!.Name);

        // Verify that both the old and new recipes are associated with the category
        Assert.Equal(initialRecipes.Count + additionalRecipes.Count, updatedCategory.Recipes!.Count);
        foreach (var recipe in initialRecipes.Concat(additionalRecipes))
        {
            Assert.Contains(updatedCategory.Recipes, r => r.Title == recipe.Title);
        }
        #endregion
    }


}
