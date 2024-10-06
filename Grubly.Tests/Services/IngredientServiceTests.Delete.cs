using Grubly.Models;
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
        await _ingredientService.DeleteIngredient(ingredientId);
        #endregion

        #region Assert
        _mockIngredientRepository.Verify(repo => repo.Delete(ingredientId), Times.Once,
        "The Delete method should be called exactly once with the correct ingredient ID.");
        #endregion
    }

    [Fact]
    public async Task DeleteIngredient_ThatDoesNotExist_ThrowsKeyNotFoundException()
    {
        #region Arrange
        _mockIngredientRepository.Setup(repo => repo.GetOne(It.IsAny<int>())).ReturnsAsync((Ingredient)null);
        #endregion

        #region Act -> Assert 
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _ingredientService.DeleteIngredient(2));

        _mockIngredientRepository.Verify(repo => repo.GetOne(It.IsAny<int>()), Times.Once);
        #endregion
    }


}
