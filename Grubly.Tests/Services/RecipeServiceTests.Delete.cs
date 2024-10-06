using System;
using Grubly.Interfaces.Repositories;
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
    public async Task DeleteRecipe_RepositoryThrowsException_PropagatesException()
    {
        #region Arrange
        var ingredientId = 1;

        _mockRecipeRepository.Setup(repo => repo.Delete(ingredientId))
                      .ThrowsAsync(new KeyNotFoundException());
        #endregion

        #region Act -> Assert 
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _recipeService.DeleteRecipe(ingredientId));
        #endregion
    }


}
