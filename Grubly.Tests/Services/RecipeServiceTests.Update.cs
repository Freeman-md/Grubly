using System;
using System.ComponentModel.DataAnnotations;
using Grubly.Models;
using Grubly.Services;
using Grubly.Tests.Unit.Builders;
using Microsoft.EntityFrameworkCore;
using Moq;
using NuGet.Packaging;

namespace Grubly.Tests.Services;

public partial class RecipeServiceTests
{
    [Fact]
    public async Task UpdateRecipe_ValidInput_UpdatesRecipeSuccessfully()
    {

        #region Arrange
        Recipe updatedRecipe = new RecipeBuilder()
                                    .WithId(1)
                                    .Build();

        _mockRecipeRepository.Setup(repo => repo.Update(updatedRecipe, updatedRecipe.ID))
                       .ReturnsAsync(updatedRecipe);
        #endregion

        #region Act
        var result = await _recipeService.UpdateRecipe(updatedRecipe, updatedRecipe.ID);

        #endregion

        #region Assert
        Assert.NotNull(result);
        Assert.Equal(updatedRecipe.Title, result.Title);
        Assert.Equal(updatedRecipe.Description, result.Description);

        _mockRecipeRepository.Verify(repo => repo.Update(updatedRecipe, updatedRecipe.ID), Times.Once);
        #endregion
    }

    [Fact]
    public async Task UpdateRecipe_NullInput_ThrowsArgumentNullException()
    {
        #region Act -> Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _recipeService.UpdateRecipe(null!, 1));
        #endregion
    }

    [Fact]
    public async Task UpdateRecipe_InvalidId_ThrowsKeyNotFoundException()
    {
        #region Arrange
        var invalidId = 999;
        var recipeToUpdate = new RecipeBuilder().WithId(invalidId).Build();

        _mockRecipeRepository.Setup(repo => repo.Update(recipeToUpdate, invalidId))
                   .ThrowsAsync(new KeyNotFoundException());
        #endregion

        #region Act -> Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _recipeService.UpdateRecipe(recipeToUpdate, invalidId));
        #endregion
    }

    [Theory]
    [InlineData(null, "Valid Description", CuisineType.Italian, DifficultyLevel.Easy, "Step 1", "https://example.com/image.jpg")] // Null Title
    [InlineData("", "Valid Description", CuisineType.Chinese, DifficultyLevel.Medium, "Step 1", "https://example.com/image.jpg")] // Empty Title
    [InlineData("A very long title that exceeds the maximum allowed length for the recipe title which is 50 chars.", "Valid Description", CuisineType.Mexican, DifficultyLevel.Hard, "Step 1", "https://example.com/image.jpg")] // Exceeding max length Title
    [InlineData(" ", "Valid Description", CuisineType.Italian, DifficultyLevel.Easy, "Step 1", "https://example.com/image.jpg")] // Whitespace Title
    [InlineData("Valid Title", "", CuisineType.Italian, DifficultyLevel.Easy, "Step 1", "https://example.com/image.jpg")] // Empty Description
    [InlineData("Valid Title", null, CuisineType.Italian, DifficultyLevel.Easy, "Step 1", "https://example.com/image.jpg")] // Null Description
    [InlineData("Valid Title", "A very long description that exceeds the maximum allowed length for the recipe description, which should trigger validation", CuisineType.Italian, DifficultyLevel.Easy, "Step 1", "https://example.com/image.jpg")] // Long Description
    [InlineData("Valid Title", "Valid Description", (CuisineType)(-1), DifficultyLevel.Easy, "Step 1", "https://example.com/image.jpg")] // Null CuisineType
    [InlineData("Valid Title", "Valid Description", CuisineType.Italian, (DifficultyLevel)(-1), "Step 1", "https://example.com/image.jpg")] // Null DifficultyLevel
    public async Task UpdateIngredient_InvalidInput_ThrowsValidationException(string title, string description, CuisineType cuisineType, DifficultyLevel difficultyLevel, string instructions, string imageUrl)
    {
        #region Arrange
        Recipe recipe = new RecipeBuilder()
                            .WithTitle(title)
                            .WithDescription(description)
                            .WithCuisineType(cuisineType)
                            .WithDifficultyLevel(difficultyLevel)
                            .WithInstructions(instructions)
                            .WithImageUrl(imageUrl)
                            .Build();
        #endregion

        #region Act -> Assert
        await Assert.ThrowsAsync<ValidationException>(() => _recipeService.UpdateRecipe(recipe, recipe.ID));
        #endregion
    }

    [Fact]
    public async Task UpdateRecipe_WithNonExistingIngredients_ThrowsKeyNotFoundException()
    {
        #region Arrange
        // Build the recipe with some ingredients
        List<Ingredient> ingredients = IngredientBuilder.BuildMany(2);
        Recipe recipeToUpdate = new RecipeBuilder()
                                    .WithIngredients(ingredients.ToArray())
                                    .WithId(1)
                                    .Build();

        // Simulate the GetOne call returning null (ingredient does not exist)
        _mockIngredientRepository.Setup(repo => repo.GetOne(It.IsAny<int>())).ReturnsAsync((Ingredient)null);
        #endregion

        #region Act -> Assert
        // Assert that the exception is thrown when updating with non-existing ingredient
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _recipeService.UpdateRecipe(recipeToUpdate, recipeToUpdate.ID));

        // Verify that GetOne was called once for the ingredient
        _mockIngredientRepository.Verify(repo => repo.GetOne(It.IsAny<int>()), Times.Once);
        #endregion
    }

    [Fact]
    public async Task UpdateRecipe_WithNonExistingCategories_ThrowsKeyNotFoundException()
    {
        #region Arrange
        // Build the recipe with some categories
        List<Category> categories = CategoryBuilder.BuildMany(2);
        Recipe recipeToUpdate = new RecipeBuilder()
                                    .WithCategories(categories.ToArray())
                                    .WithId(1)
                                    .Build();

        // Simulate the GetOne call returning null (category does not exist)
        _mockCategoryRepository.Setup(repo => repo.GetOne(It.IsAny<int>())).ReturnsAsync((Category)null);
        #endregion

        #region Act -> Assert
        // Assert that the exception is thrown when updating with non-existing category
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _recipeService.UpdateRecipe(recipeToUpdate, recipeToUpdate.ID));

        // Verify that GetOne was called once for the category
        _mockCategoryRepository.Verify(repo => repo.GetOne(It.IsAny<int>()), Times.Once);
        #endregion
    }


}
