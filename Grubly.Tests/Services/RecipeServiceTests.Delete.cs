using System;
using Grubly.Interfaces.Repositories;
using Grubly.Models;
using Grubly.Repositories;
using Grubly.Services;
using Moq;

namespace Grubly.Tests.Services;

public partial class RecipeServiceTests
{
    [Fact]
    public async Task DeleteRecipe_CallsRepositoryDeleteMethodSuccessfully()
    {
        #region Arrange
        var ingredientId = 1;
        #endregion


        #region Act
        await _recipeService.DeleteRecipe(ingredientId);
        #endregion

        #region Assert
        _mockRecipeRepository.Verify(repo => repo.Delete(ingredientId), Times.Once,
        "The Delete method should be called exactly once with the correct ingredient ID.");
        #endregion
    }

    [Fact]
    public async Task DeleteRecipe_ThatDoesNotExist_ThrowsKeyNotFoundException()
    {
        #region Arrange
        _mockRecipeRepository.Setup(repo => repo.GetOne(It.IsAny<int>())).ReturnsAsync((Recipe)null);
        #endregion

        #region Act -> Assert 
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _recipeService.DeleteRecipe(2));

        _mockRecipeRepository.Verify(repo => repo.GetOne(It.IsAny<int>()), Times.Once);
        #endregion
    }


}
