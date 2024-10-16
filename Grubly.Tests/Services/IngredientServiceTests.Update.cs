using System;
using System.ComponentModel.DataAnnotations;
using Grubly.Models;
using Grubly.Services;
using Grubly.Tests.Unit.Builders;
using Microsoft.EntityFrameworkCore;
using Moq;
using NuGet.Packaging;

namespace Grubly.Tests.Services;

public partial class IngredientServiceTests
{
    [Fact]
    public async Task UpdateIngredient_ValidInput_UpdatesIngredientSuccessfully()
    {

        #region Arrange
        Ingredient updatedIngredient = new IngredientBuilder()
                                    .WithId(1)
                                    .Build();

        _mockIngredientRepository.Setup(repo => repo.GetOne(It.IsAny<int>()))
                       .ReturnsAsync(updatedIngredient);
        _mockIngredientRepository.Setup(repo => repo.Update(updatedIngredient, updatedIngredient.ID))
                       .ReturnsAsync(updatedIngredient);
        #endregion

        #region Act
        var result = await _ingredientService.UpdateIngredient(updatedIngredient, updatedIngredient.ID);

        #endregion

        #region Assert
        Assert.NotNull(result);
        Assert.Equal(updatedIngredient.Name, result.Name);
        Assert.Equal(updatedIngredient.Description, result.Description);
        _mockIngredientRepository.Verify(repo => repo.Update(updatedIngredient, updatedIngredient.ID), Times.Once);
        #endregion
    }

    [Fact]
    public async Task UpdateIngredient_NullInput_ThrowsArgumentNullException()
    {
        #region Act -> Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _ingredientService.UpdateIngredient(null!, 1));
        #endregion
    }

    [Fact]
    public async Task UpdateIngredient_InvalidId_ThrowsKeyNotFoundException()
    {
        #region Arrange
        var invalidId = 999;
        var ingredientToUpdate = new Ingredient { ID = invalidId, Name = "Non-existent Ingredient" };

        _mockIngredientRepository.Setup(repo => repo.Update(ingredientToUpdate, invalidId))
                   .ThrowsAsync(new KeyNotFoundException());
        #endregion

        #region Act -> Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _ingredientService.UpdateIngredient(ingredientToUpdate, invalidId));
        #endregion
    }

    [Theory]
    [InlineData(null, "Valid Description")]
    [InlineData("", "Valid Description")]
    [InlineData("A very long name that exceeds the maximum allowed length for the ingredient name which is 50 chars.", "Valid Description")]
    [InlineData(" ", "Valid Description")]
    [InlineData("", " ")]
    public async Task UpdateIngredient_InvalidInput_ThrowsValidationException(string name, string description)
    {
        #region Arrange
        Ingredient ingredient = new IngredientBuilder()
                                .WithName(name)
                                .WithDescription(description)
                                .Build();
        #endregion

        #region Act -> Assert
        await Assert.ThrowsAsync<ValidationException>(() => _ingredientService.UpdateIngredient(ingredient, ingredient.ID));
        #endregion
    }

    [Fact]
    public async Task UpdateIngredient_WithNonExistingRecipes_ThrowsKeyNotFoundException()
    {
        #region Arrange
        List<Recipe> recipes = RecipeBuilder.BuildMany(2); // Building two sample recipes
        Ingredient ingredientToUpdate = new IngredientBuilder()
                                           .WithRecipes(recipes.ToArray()) // Attach the recipes
                                           .Build();

        _mockIngredientRepository.Setup(repo => repo.GetOne(It.IsAny<int>()))
                                .ReturnsAsync(ingredientToUpdate);

        // Simulate that one of the recipes doesn't exist by returning null
        _mockRecipeRepository.Setup(repo => repo.GetOne(It.IsAny<int>()))
                             .ReturnsAsync((Recipe)null);
        #endregion

        #region Act -> Assert
        // Assert that the service throws a KeyNotFoundException when trying to update an ingredient with non-existing recipes
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _ingredientService.UpdateIngredient(ingredientToUpdate, ingredientToUpdate.ID));

        // Verify that the repository method to get the recipe is called once
        _mockRecipeRepository.Verify(repo => repo.GetOne(It.IsAny<int>()), Times.Once);
        #endregion
    }

    [Fact]
    public async Task UpdateIngredient_ThatDoesNotExist_ThrowsKeyNotFoundException()
    {
        #region Arrange
        _mockIngredientRepository.Setup(repo => repo.GetOne(It.IsAny<int>())).ReturnsAsync((Ingredient)null);
        #endregion

        #region Act -> Assert 
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _ingredientService.UpdateIngredient(new IngredientBuilder().Build(), 2));

        _mockIngredientRepository.Verify(repo => repo.GetOne(It.IsAny<int>()), Times.Once);
        #endregion
    }


}
