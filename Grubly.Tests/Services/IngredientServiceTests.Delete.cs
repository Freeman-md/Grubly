using System;
using Grubly.Interfaces.Repositories;
using Grubly.Repositories;
using Grubly.Services;
using Moq;

namespace Grubly.Tests.Services;

public partial class IngredientServiceTests
{
    [Fact]
    public async Task DeleteIngredient_CallsRepositoryDeleteMethodSuccessfully()
    {
        #region Arrange
        var ingredientId = 1;
        #endregion


        #region Act
        await _service.DeleteIngredient(ingredientId);
        #endregion

        #region Assert
        _mockRepository.Verify(repo => repo.Delete(ingredientId), Times.Once,
        "The Delete method should be called exactly once with the correct ingredient ID.");
        #endregion
    }

    [Fact]
    public async Task DeleteIngredient_RepositoryThrowsException_PropagatesException()
    {
        #region Arrange
        var ingredientId = 1;

        _mockRepository.Setup(repo => repo.Delete(ingredientId))
                      .ThrowsAsync(new KeyNotFoundException());
        #endregion

        #region Act -> Assert 
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeleteIngredient(ingredientId));
        #endregion
    }


}
