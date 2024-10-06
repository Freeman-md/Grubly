using System;
using Grubly.Interfaces.Repositories;
using Grubly.Models;
using Grubly.Repositories;
using Grubly.Services;
using Grubly.Tests.Unit.Builders;
using Moq;

namespace Grubly.Tests.Services;

public partial class RecipeServiceTests
{
    [Fact]
    public async Task DeleteRecipe_CallsRepositoryDeleteMethodSuccessfully()
    {
        #region Arrange
        Recipe recipe = new RecipeBuilder().Build();

        _mockRecipeRepository.Setup(repo => repo.GetOne(It.IsAny<int>())).ReturnsAsync(recipe);
        #endregion


        #region Act
        await _recipeService.DeleteRecipe(recipe.ID);
        #endregion

        #region Assert
        _mockRecipeRepository.Verify(repo => repo.Delete(recipe.ID), Times.Once,
        "The Delete method should be called exactly once with the correct recipe ID.");
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
