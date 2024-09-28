using System;
using System.ComponentModel.DataAnnotations;
using Grubly.Models;
using Grubly.Tests.Unit.Builders;

namespace Grubly.Tests.Unit.Repository;

public partial class RecipeRepositoryTests
{
    [Theory]
    [InlineData("Updated Title", "Updated Description", "Updated Instructions", "https://example.com/updated_image.jpg", CuisineType.Mexican, DifficultyLevel.Hard)]
    [InlineData("Updated Title 2", "Another Updated Description", null, null, CuisineType.Chinese, DifficultyLevel.Medium)]
    public async Task UpdateRecipe_ValidEntity_UpdatesRecipeInDatabase(string title, string description, string? instructions, string? imageUrl, CuisineType cuisineType, DifficultyLevel difficultyLevel)
    {
        #region Arrange
        Recipe savedRecipe = await _recipeRepository.Create(new RecipeBuilder().Build());

        savedRecipe.Title = title;
        savedRecipe.Description = description;
        savedRecipe.Instructions = instructions;
        savedRecipe.ImageUrl = imageUrl;
        savedRecipe.CuisineType = cuisineType;
        savedRecipe.DifficultyLevel = difficultyLevel;
        #endregion

        #region Act
        Recipe updatedRecipe = await _recipeRepository.Update(savedRecipe, savedRecipe.ID);
        #endregion

        #region Assert
        Recipe? retrievedRecipe = await _recipeRepository.GetOne(updatedRecipe.ID);

        Assert.NotNull(retrievedRecipe);

        Assert.Equal(title, retrievedRecipe!.Title);
        Assert.Equal(description, retrievedRecipe.Description);
        Assert.Equal(instructions, retrievedRecipe.Instructions);
        Assert.Equal(imageUrl, retrievedRecipe.ImageUrl);
        Assert.Equal(cuisineType, retrievedRecipe.CuisineType);
        Assert.Equal(difficultyLevel, retrievedRecipe.DifficultyLevel);
        #endregion
    }

    [Theory]
    [InlineData(null, "Valid Description", CuisineType.Italian, DifficultyLevel.Easy)]
    [InlineData("", "Valid Description", CuisineType.Italian, DifficultyLevel.Easy)]
    [InlineData("Valid Title", "", CuisineType.Italian, DifficultyLevel.Easy)]
    [InlineData("Valid Title", "Valid Description", CuisineType.Italian, DifficultyLevel.Easy)]
    public async Task UpdateRecipe_InvalidInputs_ThrowsValidationException(string title, string description, CuisineType type, DifficultyLevel difficultyLevel)
    {
        #region Arrange
        Recipe savedRecipe = await _recipeRepository.Create(new RecipeBuilder().Build());
        savedRecipe.Title = title;
        savedRecipe.Description = description;
        savedRecipe.CuisineType = type;
        savedRecipe.DifficultyLevel = difficultyLevel;
        #endregion

        #region Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _recipeRepository.Update(savedRecipe, savedRecipe.ID));
        #endregion
    }

    [Fact]
    public async Task UpdateRecipe_InvalidId_ThrowsNotFoundException()
    {
        #region Arrange
        const int RANDOM_ID = 82923;
        Recipe savedRecipe = await _recipeRepository.Create(new RecipeBuilder().Build());
        #endregion

        #region Assert
        await Assert.ThrowsAsync<Exception>(async () => await _recipeRepository.Update(savedRecipe, RANDOM_ID));
        #endregion
    }

    [Fact]
    public async Task UpdateRecipe_DatabaseIntegrity_MaintainsRelationships() {
        #region Arrange
    // Create and save ingredients and categories
    Ingredient ingredient1 = new Ingredient { Name = "Tomato", Description = "Fresh red tomatoes" };
    Ingredient ingredient2 = new Ingredient { Name = "Garlic", Description = "Fresh garlic cloves" };
    Category category1 = new Category { Name = "Breakfast" };
    Category category2 = new Category { Name = "Lunch" };

    _dbContext.Ingredients.AddRange(ingredient1, ingredient2);
    _dbContext.Categories.AddRange(category1, category2);
    await _dbContext.SaveChangesAsync();

    // Create and save the original recipe with relationships
    Recipe originalRecipe = new RecipeBuilder()
        .WithTitle("Original Recipe")
        .WithIngredients(ingredient1, ingredient2)
        .WithCategories(category1)
        .Build();

    Recipe savedRecipe = await _recipeRepository.Create(originalRecipe);

    // Update the recipe title, but keep the same relationships (ingredients and categories)
    savedRecipe.Title = "Updated Recipe Title";
    #endregion

    #region Act
    await _recipeRepository.Update(savedRecipe, savedRecipe.ID);

    // Retrieve the updated recipe
    Recipe? updatedRecipe = await _recipeRepository.GetOneWithAllDetails(savedRecipe.ID);
    #endregion

    #region Assert
    Assert.NotNull(updatedRecipe);
    Assert.Equal("Updated Recipe Title", updatedRecipe!.Title);

    // Check that the relationships are still intact (ingredients and categories)
    Assert.Equal(2, updatedRecipe.Ingredients!.Count);
    Assert.Contains(updatedRecipe.Ingredients, i => i.Name == "Tomato");
    Assert.Contains(updatedRecipe.Ingredients, i => i.Name == "Garlic");

    Assert.Single(updatedRecipe.Categories!);
    Assert.Contains(updatedRecipe.Categories!, c => c.Name == "Breakfast");

    // Assert that the category relationship has not changed (category1 should still be linked)
    #endregion
    }

}
