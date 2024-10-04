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
        var ingredientId = 1;
        List<Recipe> recipes = RecipeBuilder.BuildMany(2);
        Ingredient updatedIngredient = new IngredientBuilder()
                                    .WithId(ingredientId)
                                    .WithRecipes(recipes.ToArray())
                                    .Build();

        _mockRepository.Setup(repo => repo.Update(updatedIngredient, ingredientId))
                       .ReturnsAsync(updatedIngredient);
        #endregion

        #region Act
        var result = await _service.UpdateIngredient(updatedIngredient, ingredientId);

        #endregion

        #region Assert
        Assert.NotNull(result);
        Assert.Equal(updatedIngredient.Name, result.Name);
        Assert.Equal(updatedIngredient.Description, result.Description);
        _mockRepository.Verify(repo => repo.Update(updatedIngredient, ingredientId), Times.Once);
        #endregion
    }

    [Fact]
    public async Task UpdateIngredient_NullInput_ThrowsArgumentNullException()
    {
        #region Act -> Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.UpdateIngredient(null!, 1));
        #endregion
    }

    [Fact]
    public async Task UpdateIngredient_InvalidId_ThrowsKeyNotFoundException()
    {
        #region Arrange
        var invalidId = 999;
        var ingredientToUpdate = new Ingredient { ID = invalidId, Name = "Non-existent Ingredient" };

        _mockRepository.Setup(repo => repo.Update(ingredientToUpdate, invalidId))
                   .ThrowsAsync(new KeyNotFoundException());
        #endregion

        #region Act -> Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.UpdateIngredient(ingredientToUpdate, invalidId));
        #endregion
    }

    [Fact]
    public async Task UpdateIngredient_DuplicateName_ThrowsDbUpdateException()
    {
        #region Arrange
        var ingredientId = 2;
        var existingIngredientName = "Tomato and Garlic";

        var ingredientToUpdate = new Ingredient { ID = ingredientId, Name = existingIngredientName };

        _mockRepository.Setup(repo => repo.Update(ingredientToUpdate, ingredientId))
                       .ThrowsAsync(new DbUpdateException());
        #endregion

        #region Act -> Assert
        await Assert.ThrowsAsync<DbUpdateException>(() => _service.UpdateIngredient(ingredientToUpdate, ingredientId));
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
        await Assert.ThrowsAsync<ValidationException>(() => _service.UpdateIngredient(ingredient, ingredient.ID));
        #endregion
    }

}
